namespace Session.Domain.Interfaces;

/// <summary>
/// Resolves lecturer names from Excel to system IDs (Guid).
/// Handles fuzzy matching because Excel has full names while DB stores codes (e.g., "DucDNM").
/// </summary>
public interface ILecturerNameMapper
{
    /// <summary>Resolve a lecturer name (from Excel) to their Guid ID.</summary>
    Task<Guid?> ResolveAsync(string fullName, CancellationToken ct = default);

    /// <summary>Resolve a collection of names at once for batch performance.</summary>
    Task<Dictionary<string, Guid>> ResolveBatchAsync(IEnumerable<string> names, CancellationToken ct = default);

    /// <summary>Normalize a name string for comparison (lowercase, trim, remove accents).</summary>
    string Normalize(string name);
}
