using Microsoft.EntityFrameworkCore;
using Session.Domain.Interfaces;
using Session.Infrastructure.Persistence;
using IdentityDbContext = Identity.Infrastructure.Persistence.IdentityDbContext;

namespace Session.Infrastructure.Repositories;

/// <summary>
/// Maps lecturer full names (from Excel) to their system Guid IDs using IdentityDbContext.
/// Since SessionDB doesn't store lecturer data, it connects to the Identity database
/// to resolve names to LecturerIds. Falls back to fuzzy matching when exact match fails.
/// </summary>
public class LecturerNameMapper : ILecturerNameMapper
{
    private readonly IdentityDbContext _identityDb;
    private readonly SessionDbContext _sessionDb;

    public LecturerNameMapper(IdentityDbContext identityDb, SessionDbContext sessionDb)
    {
        _identityDb = identityDb;
        _sessionDb = sessionDb;
    }

    public async Task<Guid?> ResolveAsync(string fullName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return null;

        var normalized = Normalize(fullName);

        // Step 1: Try exact normalized match on FullName
        var lecturer = await _identityDb.Lecturers
            .Include(l => l.User)
            .FirstOrDefaultAsync(l =>
                l.User.FullName == fullName ||
                Normalize(l.User.FullName) == normalized, ct);

        if (lecturer != null)
            return lecturer.LecturerId;

        // Step 2: Try prefix match on LecturerCode (e.g., "DucDNM")
        var codeMatch = await _identityDb.Lecturers
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LecturerCode == normalized, ct);

        if (codeMatch != null)
            return codeMatch.LecturerId;

        // Step 3: Try contains match (partial name match)
        var containsMatch = await _identityDb.Lecturers
            .Include(l => l.User)
            .FirstOrDefaultAsync(l =>
                Normalize(l.User.FullName).Contains(normalized) ||
                normalized.Contains(Normalize(l.User.FullName)), ct);

        return containsMatch?.LecturerId;
    }

    public async Task<Dictionary<string, Guid>> ResolveBatchAsync(
        IEnumerable<string> names, CancellationToken ct = default)
    {
        var result = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var nameList = names.Distinct().ToList();

        var lecturers = await _identityDb.Lecturers
            .Include(l => l.User)
            .Where(l => nameList.Contains(l.User.FullName))
            .ToListAsync(ct);

        foreach (var lecturer in lecturers)
            result[lecturer.User.FullName] = lecturer.LecturerId;

        var unresolved = nameList.Where(n => !result.ContainsKey(n)).ToList();
        foreach (var name in unresolved)
        {
            var id = await ResolveAsync(name, ct);
            if (id.HasValue)
                result[name] = id.Value;
        }

        return result;
    }

    public string Normalize(string name)
    {
        return name.Trim().ToLowerInvariant()
            .Replace(".", "").Replace(",", "").Replace("  ", " ");
    }
}
