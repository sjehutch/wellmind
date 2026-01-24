using WellMind.Models;

namespace WellMind.Services;

public interface ITipService
{
    Task<IReadOnlyList<Tip>> GetGentleTipsAsync(CancellationToken cancellationToken = default);
}
