using ClosedXML.Excel;
using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;
using Session.Domain.Entities;
using Session.Domain.Interfaces;

namespace Session.Application.Services;

/// <summary>
/// Parses the two Excel files:
/// 1. SE_CapstoneProject_FA25_Review.xlsx - Projects sheet + Student sheet for group/student data
/// 2. LecturerBookingReviewSlots_RegistrationForm.xlsx - Review sheet for lecturer availability
/// </summary>
public class ExcelImportService : IExcelImportService
{
    private readonly IUnitOfWork _uow;
    private readonly ILecturerNameMapper _mapper;

    public ExcelImportService(IUnitOfWork uow, ILecturerNameMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ImportResultDto> ImportGroupsAndStudentsAsync(
        Guid campaignId, Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var errors = new List<string>();
        int successCount = 0;

        var campaign = await _uow.Campaigns.GetByIdAsync(campaignId, ct)
            ?? throw ErrorHelper.NotFound($"Campaign {campaignId} not found.");

        using var workbook = new XLWorkbook(fileStream);

        // --- Parse Projects sheet ---
        var projectsSheet = workbook.Worksheet("Projects")
            ?? throw ErrorHelper.BadRequest("Sheet 'Projects' not found in workbook.");

        var groupData = new Dictionary<string, GroupProjectInfo>(StringComparer.OrdinalIgnoreCase);

        // Smart header detection: scan rows to find the actual header row
        // (skips merged/title rows like "PROJECTS INFORMATION")
        var headerRow = FindHeaderRow(projectsSheet, "Mã nhóm", "GroupCode", "Group Code");
        if (headerRow == null)
            throw ErrorHelper.BadRequest("Cannot find header row containing 'Mã nhóm' or 'GroupCode' in Projects sheet.");

        int headerRowNum = headerRow.RowNumber();

        int colGroupCode = FindColumn(headerRow, "Mã nhóm", "GroupCode", "Group Code");
        int colProjectCode = FindColumn(headerRow, "Mã đề tài", "ProjectCode", "Project Code");
        int colProjectNameEn = FindColumn(headerRow, "Tên đề tài Tiếng Anh", "ProjectNameEn", "ProjectName",
            "Tên đề tài Tiếng Anh/ Tiếng Nhật");
        int colProjectNameVn = FindColumn(headerRow, "Tên đề tài Tiếng Việt", "ProjectNameVn");
        int colMentor = FindColumn(headerRow, "GVHD", "Mentor", "MentorName");

        var lastRow = projectsSheet.LastRowUsed()?.RowNumber() ?? headerRowNum;

        // Skip sub-header rows: find the first data row after header
        // Data rows start after header; skip any additional sub-header rows
        // by checking if the "Mã nhóm" cell looks like a sub-header (e.g., empty or non-group-code)
        for (int r = headerRowNum + 1; r <= lastRow; r++)
        {
            var row = projectsSheet.Row(r);
            var groupCode = GetCellString(row, colGroupCode);
            if (string.IsNullOrWhiteSpace(groupCode)) continue;

            // Skip sub-header rows (e.g., row containing only text like "Code", "Count", etc.)
            // A valid group code should not be a single generic word
            if (groupCode.Length <= 3 && !groupCode.StartsWith("G", StringComparison.OrdinalIgnoreCase)) continue;

            var mentor = GetCellString(row, colMentor);
            groupData[groupCode] = new GroupProjectInfo
            {
                GroupCode = groupCode,
                ProjectCode = GetCellString(row, colProjectCode),
                ProjectNameEn = GetCellString(row, colProjectNameEn),
                ProjectNameVn = GetCellString(row, colProjectNameVn),
                MentorName = mentor
            };
        }

        // --- Parse Student sheet ---
        var studentSheet = workbook.Worksheet("Student")
            ?? throw ErrorHelper.BadRequest("Sheet 'Student' not found in workbook.");

        // Smart header detection for Student sheet
        var studentHeaderRow = FindHeaderRow(studentSheet, "MSSV");
        if (studentHeaderRow == null)
            throw ErrorHelper.BadRequest("Cannot find header row containing 'MSSV' in Student sheet.");

        int studentHeaderRowNum = studentHeaderRow.RowNumber();

        int colMssv = FindColumn(studentHeaderRow, "MSSV");
        int colStudentName = FindColumn(studentHeaderRow, "Họ và tên", "FullName", "StudentName");
        int colStudentGroup = FindColumn(studentHeaderRow, "Mã nhóm", "GroupCode");
        int colDepartment = FindColumn(studentHeaderRow, "Ngành", "Department", "Khoa");

        var studentLastRow = studentSheet.LastRowUsed()?.RowNumber() ?? studentHeaderRowNum;
        var studentsByGroup = new Dictionary<string, List<StudentMemberDto>>(StringComparer.OrdinalIgnoreCase);

        // Track the last seen group code for fill-down behavior
        // (Excel files often only fill group code on the first row of each group)
        string lastGroupCode = string.Empty;

        for (int r = studentHeaderRowNum + 1; r <= studentLastRow; r++)
        {
            var row = studentSheet.Row(r);
            var mssv = GetCellString(row, colMssv);
            if (string.IsNullOrWhiteSpace(mssv)) continue;

            var studentGroup = GetCellString(row, colStudentGroup);

            // Fill-down: if group code is empty, use the last non-empty group code
            if (!string.IsNullOrWhiteSpace(studentGroup))
                lastGroupCode = studentGroup;
            else
                studentGroup = lastGroupCode;

            // Still empty after fill-down — skip
            if (string.IsNullOrWhiteSpace(studentGroup)) continue;

            if (!studentsByGroup.ContainsKey(studentGroup))
                studentsByGroup[studentGroup] = new List<StudentMemberDto>();

            studentsByGroup[studentGroup].Add(new StudentMemberDto(
                mssv,
                GetCellString(row, colStudentName),
                GetCellString(row, colDepartment)));
        }

        // --- Resolve all mentor names to IDs ---
        var allMentorNames = groupData.Values
            .Where(g => !string.IsNullOrWhiteSpace(g.MentorName))
            .SelectMany(g => g.MentorName.Split(new[] { '/', ',' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(n => n.Trim())
            .Distinct()
            .ToList();

        var resolvedMentors = await _mapper.ResolveBatchAsync(allMentorNames, ct);

        // --- Persist groups ---
        int totalRows = groupData.Count;
        foreach (var (groupCode, info) in groupData)
        {
            if (string.IsNullOrWhiteSpace(info.ProjectCode))
            {
                errors.Add($"Row '{groupCode}': ProjectCode is empty. Skipped.");
                continue;
            }

            var mentorNames = info.MentorName.Split(new[] { '/', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                             .Select(n => n.Trim()).ToList();

            if (mentorNames.Count == 0 || !resolvedMentors.TryGetValue(mentorNames[0], out var mentorId))
            {
                errors.Add($"Row '{groupCode}': Cannot resolve primary mentor '{info.MentorName}'. Skipped.");
                continue;
            }

            var additionalSupervisors = new List<Guid>();
            for (int i = 1; i < mentorNames.Count; i++)
            {
                if (resolvedMentors.TryGetValue(mentorNames[i], out var supId))
                {
                    additionalSupervisors.Add(supId);
                }
                else
                {
                    errors.Add($"Row '{groupCode}': Cannot resolve additional supervisor '{mentorNames[i]}'. Ignoring this supervisor.");
                }
            }

            var existingGroup = await _uow.CapstoneGroups.GetByGroupCodeAsync(groupCode, ct);
            if (existingGroup != null)
            {
                errors.Add($"Row '{groupCode}': Group already exists. Skipped.");
                continue;
            }

            var group = CapstoneGroup.Create(
                campaignId,
                groupCode,
                info.ProjectCode,
                info.ProjectNameEn,
                info.ProjectNameVn,
                mentorId,
                additionalSupervisors);

            if (studentsByGroup.TryGetValue(groupCode, out var members))
            {
                foreach (var m in members)
                {
                    group.Members.Add(CapstoneGroupMember.Create(
                        group.Id, m.StudentMssv, m.StudentName, m.Department));
                }
            }

            await _uow.CapstoneGroups.AddAsync(group, ct);
            successCount++;
        }

        await _uow.SaveChangesAsync(ct);

        return new ImportResultDto(totalRows, successCount, totalRows - successCount, errors);
    }

    public async Task<ImportResultDto> ImportAvailabilityAsync(
        Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var errors = new List<string>();
        int successCount = 0;

        using var workbook = new XLWorkbook(fileStream);

        var sheet = workbook.Worksheet("Review")
            ?? throw ErrorHelper.BadRequest("Sheet 'Review' not found in workbook.");

        // Smart header detection for Review sheet
        var headerRow = FindHeaderRow(sheet, "Full Name", "Name", "Họ và tên");
        if (headerRow == null)
            throw ErrorHelper.BadRequest("Could not find header row containing 'Full Name' in Review sheet.");

        int headerRowNum = headerRow.RowNumber();

        int colName = FindColumn(headerRow, "Full Name", "Name", "Họ và tên");
        int colMon = FindColumn(headerRow, "Mon", "Monday", "Mon (20/4)");
        int colTue = FindColumn(headerRow, "Tue", "Tuesday", "Tue(21/4)");
        int colWed = FindColumn(headerRow, "Wed", "Wednesday", "Wed (22/4)");
        int colThu = FindColumn(headerRow, "Thu", "Thursday", "Thu(23/4)");
        int colFri = FindColumn(headerRow, "Fri", "Friday", "Fri (24/4)");
        int colSat = FindColumn(headerRow, "Sat", "Saturday");

        if (colName < 0)
            throw ErrorHelper.BadRequest("Could not find 'Full Name' column in Review sheet.");

        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? headerRowNum;

        var allNames = new HashSet<string>();
        for (int r = headerRowNum + 1; r <= lastRow; r++)
        {
            var name = GetCellString(sheet.Row(r), colName);
            if (!string.IsNullOrWhiteSpace(name))
                allNames.Add(name);
        }

        var resolved = await _mapper.ResolveBatchAsync(allNames, ct);

        for (int r = headerRowNum + 1; r <= lastRow; r++)
        {
            var row = sheet.Row(r);
            var name = GetCellString(row, colName);
            if (string.IsNullOrWhiteSpace(name)) continue;

            if (!resolved.TryGetValue(name, out var lecturerId))
            {
                errors.Add($"Row '{name}': Cannot resolve lecturer. Skipped.");
                continue;
            }

            var monSlots = GetCellString(row, colMon);
            var tueSlots = GetCellString(row, colTue);
            var wedSlots = GetCellString(row, colWed);
            var thuSlots = GetCellString(row, colThu);
            var friSlots = GetCellString(row, colFri);
            var satSlots = GetCellString(row, colSat);

            var parsed = ParseSlotPreferences(monSlots, tueSlots, wedSlots, thuSlots, friSlots, satSlots);
            if (parsed.Count == 0)
            {
                errors.Add($"Row '{name}': No valid slots found. Skipped.");
                continue;
            }

            // TODO: Write to Availability service via gRPC
            // The resolved slots per lecturer can be sent to Availability service
            successCount++;
        }

        return new ImportResultDto(allNames.Count, successCount, allNames.Count - successCount, errors);
    }

    private Dictionary<DayOfWeek, List<int>> ParseSlotPreferences(
        string? mon, string? tue, string? wed, string? thu, string? fri, string? sat = null)
    {
        var result = new Dictionary<DayOfWeek, List<int>>();

        if (!string.IsNullOrWhiteSpace(mon))
            result[DayOfWeek.Monday] = ParseSlotString(mon);
        if (!string.IsNullOrWhiteSpace(tue))
            result[DayOfWeek.Tuesday] = ParseSlotString(tue);
        if (!string.IsNullOrWhiteSpace(wed))
            result[DayOfWeek.Wednesday] = ParseSlotString(wed);
        if (!string.IsNullOrWhiteSpace(thu))
            result[DayOfWeek.Thursday] = ParseSlotString(thu);
        if (!string.IsNullOrWhiteSpace(fri))
            result[DayOfWeek.Friday] = ParseSlotString(fri);
        if (!string.IsNullOrWhiteSpace(sat))
            result[DayOfWeek.Saturday] = ParseSlotString(sat);

        return result;
    }

    private static List<int> ParseSlotString(string input)
    {
        var slots = new List<int>();
        var parts = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            var cleaned = part.Replace("Slot", "", StringComparison.OrdinalIgnoreCase).Trim();
            if (int.TryParse(cleaned, out var slot) && slot >= 1 && slot <= 4)
                slots.Add(slot);
        }
        return slots.Distinct().ToList();
    }

    /// <summary>
    /// Scan rows from top to find the row that contains a target column name.
    /// Handles Excel files with merged title rows above the actual header.
    /// </summary>
    private static IXLRow? FindHeaderRow(IXLWorksheet sheet, params string[] targetColumnNames)
    {
        var firstRow = sheet.FirstRowUsed()?.RowNumber() ?? 1;
        var lastRow = Math.Min(firstRow + 10, sheet.LastRowUsed()?.RowNumber() ?? firstRow + 10);

        for (int r = firstRow; r <= lastRow; r++)
        {
            var row = sheet.Row(r);
            if (FindColumn(row, targetColumnNames) >= 0)
                return row;
        }
        return null;
    }

    /// <summary>
    /// Finds a column by name in the given header row.
    /// Matching strategy (in order):
    /// 1. Exact match (case-insensitive)
    /// 2. Exact match ignoring spaces
    /// 3. StartsWith match (for headers like "Mon (20/4)" matching "Mon")
    /// </summary>
    private static int FindColumn(IXLRow row, params string[] possibleNames)
    {
        var maxCol = row.LastCellUsed()?.Address.ColumnNumber ?? 20;

        // Pass 1: Exact match (original behavior)
        foreach (var name in possibleNames)
        {
            for (int c = 1; c <= maxCol; c++)
            {
                var cellValue = row.Cell(c).GetString().Trim();
                if (string.Equals(cellValue, name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cellValue, name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
                {
                    return c;
                }
            }
        }

        // Pass 2: StartsWith match (for "Mon (20/4)" matching "Mon", etc.)
        foreach (var name in possibleNames)
        {
            for (int c = 1; c <= maxCol; c++)
            {
                var cellValue = row.Cell(c).GetString().Trim();
                if (cellValue.Length > name.Length &&
                    cellValue.StartsWith(name, StringComparison.OrdinalIgnoreCase))
                {
                    return c;
                }
            }
        }

        // Pass 3: Contains match (for "Tên đề tài Tiếng Anh/ Tiếng Nhật" containing "Tên đề tài Tiếng Anh")
        foreach (var name in possibleNames)
        {
            if (name.Length < 4) continue; // Avoid short names matching random cells
            for (int c = 1; c <= maxCol; c++)
            {
                var cellValue = row.Cell(c).GetString().Trim();
                if (cellValue.Length > name.Length &&
                    cellValue.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    return c;
                }
            }
        }

        return -1;
    }

    private static string GetCellString(IXLRow row, int col)
    {
        if (col < 1) return string.Empty;
        return row.Cell(col).GetString()?.Trim() ?? string.Empty;
    }

    private class GroupProjectInfo
    {
        public string GroupCode { get; set; } = string.Empty;
        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectNameEn { get; set; } = string.Empty;
        public string ProjectNameVn { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
    }
}
