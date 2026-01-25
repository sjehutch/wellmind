using WellMind.Models;

namespace WellMind.Services;

public interface IGrounded
{
    Task<GroundedQuote> GetNextAsync(string? tagFilter, CancellationToken ct = default);
    Task<IReadOnlyList<GroundedQuote>> GetAllAsync(CancellationToken ct = default);
}
