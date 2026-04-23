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
        var firstRow = projectsSheet.FirstRowUsed()?.RowNumber() ?? 2;
        var headerRow = projectsSheet.Row(firstRow);

        int colGroupCode = FindColumn(headerRow, "Mã nhóm", "GroupCode", "Group Code");
        int colProjectCode = FindColumn(headerRow, "Mã đề tài", "ProjectCode", "Project Code");
        int colProjectNameEn = FindColumn(headerRow, "Tên đề tài Tiếng Anh", "ProjectNameEn", "ProjectName");
        int colProjectNameVn = FindColumn(headerRow, "Tên đề tài Tiếng Việt", "ProjectNameVn");
        int colMentor = FindColumn(headerRow, "GVHD", "Mentor", "MentorName");

        var lastRow = projectsSheet.LastRowUsed()?.RowNumber() ?? firstRow;
        for (int r = firstRow + 1; r <= lastRow; r++)
        {
            var row = projectsSheet.Row(r);
            var groupCode = GetCellString(row, colGroupCode);
            if (string.IsNullOrWhiteSpace(groupCode)) continue;

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

        var studentFirstRow = studentSheet.FirstRowUsed()?.RowNumber() ?? 2;
        var studentHeader = studentSheet.Row(studentFirstRow);
        int colMssv = FindColumn(studentHeader, "MSSV");
        int colStudentName = FindColumn(studentHeader, "Họ và tên", "FullName", "StudentName");
        int colStudentGroup = FindColumn(studentHeader, "Mã nhóm", "GroupCode");
        int colDepartment = FindColumn(studentHeader, "Ngành", "Department", "Khoa");

        var studentLastRow = studentSheet.LastRowUsed()?.RowNumber() ?? studentFirstRow;
        var studentsByGroup = new Dictionary<string, List<StudentMemberDto>>(StringComparer.OrdinalIgnoreCase);

        for (int r = studentFirstRow + 1; r <= studentLastRow; r++)
        {
            var row = studentSheet.Row(r);
            var mssv = GetCellString(row, colMssv);
            var studentGroup = GetCellString(row, colStudentGroup);
            if (string.IsNullOrWhiteSpace(mssv) || string.IsNullOrWhiteSpace(studentGroup)) continue;

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
            .Select(g => g.MentorName)
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

            if (!resolvedMentors.TryGetValue(info.MentorName, out var mentorId))
            {
                errors.Add($"Row '{groupCode}': Cannot resolve mentor '{info.MentorName}'. Skipped.");
                continue;
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
                mentorId);

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

        var firstRow = sheet.FirstRowUsed()?.RowNumber() ?? 2;
        var headerRow = sheet.Row(firstRow);

        int colName = FindColumn(headerRow, "Full Name", "Name", "Họ và tên");
        int colMon = FindColumn(headerRow, "Mon", "Monday", "Mon (20/4)");
        int colTue = FindColumn(headerRow, "Tue", "Tuesday", "Tue(21/4)");
        int colWed = FindColumn(headerRow, "Wed", "Wednesday", "Wed (22/4)");
        int colThu = FindColumn(headerRow, "Thu", "Thursday", "Thu(23/4)");
        int colFri = FindColumn(headerRow, "Fri", "Friday", "Fri (24/4)");

        if (colName < 0)
            throw ErrorHelper.BadRequest("Could not find 'Full Name' column in Review sheet.");

        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? firstRow;

        var allNames = new HashSet<string>();
        for (int r = firstRow + 1; r <= lastRow; r++)
        {
            var name = GetCellString(sheet.Row(r), colName);
            if (!string.IsNullOrWhiteSpace(name))
                allNames.Add(name);
        }

        var resolved = await _mapper.ResolveBatchAsync(allNames, ct);

        for (int r = firstRow + 1; r <= lastRow; r++)
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

            var parsed = ParseSlotPreferences(monSlots, tueSlots, wedSlots, thuSlots, friSlots);
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
        string? mon, string? tue, string? wed, string? thu, string? fri)
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

    private static int FindColumn(IXLRow row, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            for (int c = 1; c <= (row.LastCellUsed()?.Address.ColumnNumber ?? 20); c++)
            {
                var cellValue = row.Cell(c).GetString().Trim();
                if (string.Equals(cellValue, name, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(cellValue, name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
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
