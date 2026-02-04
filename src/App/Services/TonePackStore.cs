using Microsoft.Maui.Storage;
using WellMind.Models;

namespace WellMind.Services;

public sealed class TonePackStore : ITonePackStore
{
    private const string TonePackKey = "home.tone.pack";

    public Task<TonePack> GetAsync(CancellationToken ct = default)
    {
        var raw = Preferences.Get(TonePackKey, TonePack.Grounding.ToStorageValue());
        return Task.FromResult(
            TonePackExtensions.TryParse(raw, out var tonePack)
                ? tonePack
                : TonePack.Grounding);
    }

    public Task SaveAsync(TonePack tonePack, CancellationToken ct = default)
    {
        Preferences.Set(TonePackKey, tonePack.ToStorageValue());
        return Task.CompletedTask;
    }
}
