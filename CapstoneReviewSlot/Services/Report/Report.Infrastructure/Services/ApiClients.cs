using Report.Application.Interfaces;

namespace Report.Infrastructure.Services;

public class SessionApiClient : ISessionApiClient
{
    private readonly HttpClient _client;

    public SessionApiClient(HttpClient client) => _client = client;

    public async Task<CampaignData?> GetCampaignAsync(Guid campaignId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/campaigns/{campaignId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("value").GetProperty("data");
            return new CampaignData(
                Guid.Parse(data.GetProperty("id").GetString()!),
                data.GetProperty("name").GetString() ?? "");
        }
        catch { return null; }
    }

    public async Task<List<SlotData>> GetSlotsByCampaignAsync(Guid campaignId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/campaigns/{campaignId}/slots", ct);
            if (!response.IsSuccessStatusCode) return new List<SlotData>();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("value").GetProperty("data");

            var slots = new List<SlotData>();
            foreach (var item in arr.EnumerateArray())
            {
                slots.Add(new SlotData(
                    Guid.Parse(item.GetProperty("id").GetString()!),
                    campaignId,
                    DateOnly.Parse(item.GetProperty("reviewDate").GetString()!),
                    item.GetProperty("slotNumber").GetInt32(),
                    item.GetProperty("startTime").GetString() ?? "",
                    item.GetProperty("endTime").GetString() ?? "",
                    item.TryGetProperty("room", out var room) ? room.GetString() ?? "" : ""));
            }
            return slots;
        }
        catch { return new List<SlotData>(); }
    }

    public async Task<SlotData?> GetSlotAsync(Guid slotId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/campaigns/slots/{slotId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("value").GetProperty("data");
            return new SlotData(
                Guid.Parse(data.GetProperty("id").GetString()!),
                Guid.Parse(data.GetProperty("campaignId").GetString()!),
                DateOnly.Parse(data.GetProperty("reviewDate").GetString()!),
                data.GetProperty("slotNumber").GetInt32(),
                data.GetProperty("startTime").GetString() ?? "",
                data.GetProperty("endTime").GetString() ?? "",
                data.TryGetProperty("room", out var room) ? room.GetString() ?? "" : "");
        }
        catch { return null; }
    }

    public async Task<GroupData?> GetCapstoneGroupAsync(Guid groupId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/groups/{groupId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("value").GetProperty("data");
            return new GroupData(
                Guid.Parse(data.GetProperty("id").GetString()!),
                data.GetProperty("groupCode").GetString() ?? "",
                data.TryGetProperty("subjectCode", out var sc) ? sc.GetString() ?? "" : "",
                data.TryGetProperty("projectNameEn", out var pne) ? pne.GetString() ?? "" : "",
                data.TryGetProperty("projectNameVn", out var pvn) ? pvn.GetString() ?? "" : "");
        }
        catch { return null; }
    }
}

public class AssignmentApiClient : IAssignmentApiClient
{
    private readonly HttpClient _client;

    public AssignmentApiClient(HttpClient client) => _client = client;

    public async Task<List<AssignmentData>> GetAllAssignmentsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync("api/reviewassignment", ct);
            if (!response.IsSuccessStatusCode) return new List<AssignmentData>();

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("value").GetProperty("data");

            var results = new List<AssignmentData>();
            foreach (var item in arr.EnumerateArray())
            {
                results.Add(new AssignmentData(
                    Guid.Parse(item.GetProperty("id").GetString()!),
                    Guid.Parse(item.GetProperty("capstoneGroupId").GetString()!),
                    Guid.Parse(item.GetProperty("reviewSlotId").GetString()!),
                    item.GetProperty("status").GetString() ?? "",
                    item.GetProperty("reviewOrder").GetInt32()));
            }
            return results;
        }
        catch { return new List<AssignmentData>(); }
    }

    public async Task<List<ReviewerData>> GetReviewersByAssignmentIdsAsync(List<Guid> assignmentIds, CancellationToken ct = default)
    {
        var allReviewers = new List<ReviewerData>();
        foreach (var id in assignmentIds)
        {
            try
            {
                var response = await _client.GetAsync($"api/ReviewAssignmentReviewer/by-assignment/{id}", ct);
                if (!response.IsSuccessStatusCode) continue;

                var json = await response.Content.ReadAsStringAsync(ct);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var arr = doc.RootElement.GetProperty("value").GetProperty("data");

                foreach (var item in arr.EnumerateArray())
                {
                    allReviewers.Add(new ReviewerData(
                        item.TryGetProperty("id", out var rid) ? Guid.Parse(rid.GetString()!) : Guid.Empty,
                        Guid.Parse(item.GetProperty("lecturerId").GetString()!),
                        Guid.Parse(item.GetProperty("reviewAssignmentId").GetString()!),
                        item.GetProperty("role").GetString() ?? ""));
                }
            }
            catch { /* continue */ }
        }
        return allReviewers;
    }
}

public class IdentityApiClient : IIdentityApiClient
{
    private readonly HttpClient _client;

    public IdentityApiClient(HttpClient client) => _client = client;

    public async Task<string?> GetLecturerNameAsync(Guid lecturerId, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync($"api/lecturers/{lecturerId}", ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("value")
                .GetProperty("data")
                .GetProperty("fullName").GetString();
        }
        catch { return null; }
    }
}
