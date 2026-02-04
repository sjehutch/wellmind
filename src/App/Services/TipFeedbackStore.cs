using Microsoft.Maui.Storage;

namespace WellMind.Services;

public sealed class TipFeedbackStore : ITipFeedbackStore
{
    private const string HelpfulPrefix = "tips.helpful.";

    public Task<int> GetHelpfulCountAsync(string tipId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(tipId))
        {
            return Task.FromResult(0);
        }

        return Task.FromResult(Preferences.Get(GetKey(tipId), 0));
    }

    public Task MarkHelpfulAsync(string tipId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(tipId))
        {
            return Task.CompletedTask;
        }

        var key = GetKey(tipId);
        var current = Preferences.Get(key, 0);
        Preferences.Set(key, current + 1);
        return Task.CompletedTask;
    }

    private static string GetKey(string tipId)
    {
        return HelpfulPrefix + tipId.Trim().ToLowerInvariant();
    }
}
