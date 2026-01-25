using System.Text.Json;
using System.Linq;
using Microsoft.Maui.Storage;
using WellMind.Models;

namespace WellMind.Services;

public sealed class GroundedService : IGrounded
{
    private const string AssetName = "Grounded.json";
    private const string PrefShufflePrefix = "Grounded.Shuffle.";
    private const string PrefIndexPrefix = "Grounded.Index.";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private IReadOnlyList<GroundedQuote>? _cache;

    public async Task<GroundedQuote> GetNextAsync(string? tagFilter, CancellationToken ct = default)
    {
        var quotes = await GetAllAsync(ct);
        var filtered = FilterQuotes(quotes, tagFilter);

        if (filtered.Count == 0)
        {
            filtered = quotes;
            tagFilter = null;
        }

        await _gate.WaitAsync(ct);
        try
        {
            var ids = GetOrCreateShuffle(filtered, tagFilter);
            var index = Preferences.Default.Get(GetIndexKey(tagFilter), 0);

            if (index >= ids.Count)
            {
                ids = ShuffleIds(filtered);
                index = 0;
                SaveShuffle(tagFilter, ids);
            }

            var nextId = ids[index];
            Preferences.Default.Set(GetIndexKey(tagFilter), index + 1);

            return filtered.First(quote => quote.Id == nextId);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<GroundedQuote>> GetAllAsync(CancellationToken ct = default)
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await _gate.WaitAsync(ct);
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            await using var stream = await FileSystem.OpenAppPackageFileAsync(AssetName);
            var payload = await JsonSerializer.DeserializeAsync<GroundedPayload>(stream, _jsonOptions, ct);
            _cache = payload?.Quotes ?? new List<GroundedQuote>();
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    private static IReadOnlyList<GroundedQuote> FilterQuotes(IReadOnlyList<GroundedQuote> quotes, string? tagFilter)
    {
        if (string.IsNullOrWhiteSpace(tagFilter))
        {
            return quotes;
        }

        return quotes
            .Where(quote => quote.Tags.Contains(tagFilter))
            .ToList();
    }

    private static string GetIndexKey(string? tagFilter)
    {
        return PrefIndexPrefix + (string.IsNullOrWhiteSpace(tagFilter) ? "all" : tagFilter);
    }

    private static string GetShuffleKey(string? tagFilter)
    {
        return PrefShufflePrefix + (string.IsNullOrWhiteSpace(tagFilter) ? "all" : tagFilter);
    }

    private static List<string> ShuffleIds(IReadOnlyList<GroundedQuote> quotes)
    {
        var ids = quotes.Select(quote => quote.Id).ToList();

        // Fisher-Yates shuffle so every quote appears once per cycle.
        var random = Random.Shared;
        for (var i = ids.Count - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (ids[i], ids[swapIndex]) = (ids[swapIndex], ids[i]);
        }

        return ids;
    }

    private static void SaveShuffle(string? tagFilter, List<string> ids)
    {
        var json = JsonSerializer.Serialize(ids);
        Preferences.Default.Set(GetShuffleKey(tagFilter), json);
    }

    private List<string> GetOrCreateShuffle(IReadOnlyList<GroundedQuote> quotes, string? tagFilter)
    {
        var json = Preferences.Default.Get(GetShuffleKey(tagFilter), string.Empty);
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var ids = JsonSerializer.Deserialize<List<string>>(json, _jsonOptions);
                if (ids is not null && ids.Count == quotes.Count && ids.All(id => quotes.Any(q => q.Id == id)))
                {
                    return ids;
                }
            }
            catch
            {
                // If preferences are corrupted, rebuild a clean shuffle.
            }
        }

        var fresh = ShuffleIds(quotes);
        SaveShuffle(tagFilter, fresh);
        Preferences.Default.Set(GetIndexKey(tagFilter), 0);
        return fresh;
    }

    private sealed class GroundedPayload
    {
        public int Version { get; set; } = 1;
        public List<GroundedQuote> Quotes { get; set; } = new();
    }
}
