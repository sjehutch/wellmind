using System.Text.Json;
using Microsoft.Maui.Storage;

namespace WellMind.Features.Support.TalkToSomeone;

public sealed class TalkToSomeoneConfigLoader
{
    private const string AssetName = "talk-to-someone.json";
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<TalkToSomeoneConfig> LoadAsync(CancellationToken ct = default)
    {
        // If the file is missing or broken, we return a safe fallback.
        // This keeps the app calm and prevents crashes.
        try
        {
            await using var stream = await FileSystem.OpenAppPackageFileAsync(AssetName);
            var config = await JsonSerializer.DeserializeAsync<TalkToSomeoneConfig>(stream, _jsonOptions, ct);
            return config ?? BuildFallback();
        }
        catch
        {
            return BuildFallback(loadFailed: true);
        }
    }

    private static TalkToSomeoneConfig BuildFallback(bool loadFailed = false)
    {
        return new TalkToSomeoneConfig
        {
            Enabled = false,
            IntroTitle = "Talk to Someone",
            IntroBody = "Optional, anonymous support.",
            Options = new List<TalkToSomeoneOption>(),
            LoadFailed = loadFailed
        };
    }
}
