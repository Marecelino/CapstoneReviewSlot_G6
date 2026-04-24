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

        // Step 1: Try exact match on FullName (SQL-safe)
        var lecturer = await _identityDb.Lecturers
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.User.FullName == fullName, ct);

        if (lecturer != null)
            return lecturer.LecturerId;

        // Step 2: Try match on LecturerCode (SQL-safe)
        var codeMatch = await _identityDb.Lecturers
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.LecturerCode == fullName, ct);

        if (codeMatch != null)
            return codeMatch.LecturerId;

        // Step 3: Load all lecturers to memory for fuzzy matching
        // (normalized/contains match cannot be translated to SQL)
        var allLecturers = await _identityDb.Lecturers
            .Include(l => l.User)
            .ToListAsync(ct);

        // Step 3a: Normalized name match
        var normalizedMatch = allLecturers.FirstOrDefault(l =>
            Normalize(l.User.FullName) == normalized);
        if (normalizedMatch != null)
            return normalizedMatch.LecturerId;

        // Step 3b: LecturerCode normalized match
        var codeNormMatch = allLecturers.FirstOrDefault(l =>
            l.LecturerCode != null &&
            Normalize(l.LecturerCode) == normalized);
        if (codeNormMatch != null)
            return codeNormMatch.LecturerId;

        // Step 3c: Contains match (partial name match)
        var containsMatch = allLecturers.FirstOrDefault(l =>
            Normalize(l.User.FullName).Contains(normalized) ||
            normalized.Contains(Normalize(l.User.FullName)));

        return containsMatch?.LecturerId;
    }

    public async Task<Dictionary<string, Guid>> ResolveBatchAsync(
        IEnumerable<string> names, CancellationToken ct = default)
    {
        var result = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var nameList = names.Distinct().ToList();

        if (nameList.Count == 0)
            return result;

        // Load all lecturers once for batch resolution (avoids N+1 queries)
        var allLecturers = await _identityDb.Lecturers
            .Include(l => l.User)
            .ToListAsync(ct);

        // Pass 1: Exact FullName match
        foreach (var lecturer in allLecturers)
        {
            var matchedName = nameList.FirstOrDefault(n =>
                string.Equals(n, lecturer.User.FullName, StringComparison.OrdinalIgnoreCase));
            if (matchedName != null && !result.ContainsKey(matchedName))
                result[matchedName] = lecturer.LecturerId;
        }

        // Pass 2: LecturerCode match
        var unresolved = nameList.Where(n => !result.ContainsKey(n)).ToList();
        foreach (var name in unresolved)
        {
            var match = allLecturers.FirstOrDefault(l =>
                l.LecturerCode != null &&
                string.Equals(l.LecturerCode, name, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                result[name] = match.LecturerId;
        }

        // Pass 3: Normalized match
        unresolved = nameList.Where(n => !result.ContainsKey(n)).ToList();
        foreach (var name in unresolved)
        {
            var normalized = Normalize(name);
            var match = allLecturers.FirstOrDefault(l =>
                Normalize(l.User.FullName) == normalized ||
                (l.LecturerCode != null && Normalize(l.LecturerCode) == normalized));
            if (match != null)
                result[name] = match.LecturerId;
        }

        // Pass 4: Contains/partial match
        unresolved = nameList.Where(n => !result.ContainsKey(n)).ToList();
        foreach (var name in unresolved)
        {
            var normalized = Normalize(name);
            var match = allLecturers.FirstOrDefault(l =>
                Normalize(l.User.FullName).Contains(normalized) ||
                normalized.Contains(Normalize(l.User.FullName)));
            if (match != null)
                result[name] = match.LecturerId;
        }

        return result;
    }

    public string Normalize(string name)
    {
        return name.Trim().ToLowerInvariant()
            .Replace(".", "").Replace(",", "").Replace("  ", " ");
    }
}
