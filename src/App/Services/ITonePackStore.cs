using WellMind.Models;

namespace WellMind.Services;

public interface ITonePackStore
{
    Task<TonePack> GetAsync(CancellationToken ct = default);
    Task SaveAsync(TonePack tonePack, CancellationToken ct = default);
}
