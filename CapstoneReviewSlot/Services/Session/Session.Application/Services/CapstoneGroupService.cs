using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;
using Session.Domain.Entities;
using Session.Domain.Interfaces;

namespace Session.Application.Services;

public class CapstoneGroupService : ICapstoneGroupService
{
    private readonly IUnitOfWork _uow;
    private readonly ILecturerNameMapper _mapper;

    public CapstoneGroupService(IUnitOfWork uow, ILecturerNameMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CapstoneGroupDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _uow.CapstoneGroups.GetByIdWithMembersAsync(id, ct);
        return group is null ? null : ToDto(group);
    }

    public async Task<CapstoneGroupDto?> GetByGroupCodeAsync(string groupCode, CancellationToken ct = default)
    {
        var group = await _uow.CapstoneGroups.GetByGroupCodeAsync(groupCode, ct);
        return group is null ? null : ToDto(group);
    }

    public async Task<List<CapstoneGroupDto>> GetByCampaignAsync(Guid campaignId, CancellationToken ct = default)
    {
        var groups = await _uow.CapstoneGroups.GetByCampaignIdAsync(campaignId, ct);
        var dtos = new List<CapstoneGroupDto>();
        foreach (var g in groups)
        {
            var dto = ToDto(g);
            // Resolve mentor name
            dto = dto with { MentorLecturerName = await ResolveMentorNameAsync(g.MentorLecturerId, ct) };
            dtos.Add(dto);
        }
        return dtos;
    }

    public async Task<CapstoneGroupDto> CreateAsync(CreateCapstoneGroupDto dto, CancellationToken ct = default)
    {
        var campaign = await _uow.Campaigns.GetByIdAsync(dto.CampaignId, ct)
            ?? throw ErrorHelper.NotFound($"Campaign {dto.CampaignId} not found.");

        var mentorId = await _mapper.ResolveAsync(dto.MentorLecturerName, ct)
            ?? throw ErrorHelper.BadRequest($"Cannot resolve mentor: '{dto.MentorLecturerName}'");

        var additionalSupervisors = new List<Guid>();
        if (dto.AdditionalSupervisorNames?.Any() == true)
        {
            foreach (var name in dto.AdditionalSupervisorNames)
            {
                var id = await _mapper.ResolveAsync(name, ct)
                    ?? throw ErrorHelper.BadRequest($"Cannot resolve additional supervisor: '{name}'");
                additionalSupervisors.Add(id);
            }
        }

        var group = CapstoneGroup.Create(
            dto.CampaignId,
            dto.GroupCode,
            dto.ProjectCode,
            dto.ProjectNameEn,
            dto.ProjectNameVn,
            mentorId,
            additionalSupervisors);

        if (dto.Members?.Any() == true)
        {
            foreach (var m in dto.Members)
            {
                group.Members.Add(CapstoneGroupMember.Create(
                    group.Id,
                    m.StudentMssv,
                    m.StudentName,
                    m.Department));
            }
        }

        await _uow.CapstoneGroups.AddAsync(group, ct);
        await _uow.SaveChangesAsync(ct);

        return await GetByIdAsync(group.Id, ct)
            ?? throw ErrorHelper.Internal("Failed to retrieve created group.");
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _uow.CapstoneGroups.GetByIdAsync(id, ct)
            ?? throw ErrorHelper.NotFound("Capstone group not found.");

        await _uow.CapstoneGroups.SoftRemove(group, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken ct = default)
        => await _uow.CapstoneGroups.CountByCampaignAsync(campaignId, ct);

    private CapstoneGroupDto ToDto(CapstoneGroup g)
    {
        return new CapstoneGroupDto(
            Id: g.Id,
            CampaignId: g.CampaignId,
            CampaignName: g.ReviewCampaign?.Name ?? string.Empty,
            GroupCode: g.GroupCode,
            ProjectCode: g.ProjectCode,
            ProjectNameEn: g.ProjectNameEn,
            ProjectNameVn: g.ProjectNameVn,
            MentorLecturerId: g.MentorLecturerId,
            MentorLecturerName: string.Empty,
            AdditionalSupervisorIds: g.GetAdditionalSupervisorIds(),
            AdditionalSupervisorNames: new List<string>(),
            Members: g.Members.Select(m => new CapstoneGroupMemberDto(
                m.Id, m.StudentMssv, m.StudentName, m.Department)).ToList());
    }

    private async Task<string> ResolveMentorNameAsync(Guid mentorId, CancellationToken ct)
    {
        return $"LecturerId:{mentorId}";
    }
}
