using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using WellMind.Models;

namespace WellMind.Services;

public sealed class HistoryReminderService
{
    private const string AssetName = "history_reminders.json";
    private readonly ITonePackStore _tonePackStore;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private IReadOnlyList<HistoryReminder>? _cache;

    public HistoryReminderService(ITonePackStore tonePackStore)
    {
        _tonePackStore = tonePackStore;
    }

    public async Task<HistoryReminder> GetForTodayAsync(DateTime? now = null)
    {
        var today = (now ?? DateTime.Now).Date;
        try
        {
            var reminders = await GetAllAsync();
            var match = reminders.FirstOrDefault(item => item.Month == today.Month && item.Day == today.Day);
            return match ?? await GetFallbackAsync(today);
        }
        catch
        {
            return await GetFallbackAsync(today);
        }
    }

    private async Task<IReadOnlyList<HistoryReminder>> GetAllAsync()
    {
        if (_cache is not null)
        {
            return _cache;
        }

        await _gate.WaitAsync();
        try
        {
            if (_cache is not null)
            {
                return _cache;
            }

            await using var stream = await FileSystem.OpenAppPackageFileAsync(AssetName);
            var payload = await JsonSerializer.DeserializeAsync<List<HistoryReminder>>(stream, _jsonOptions);
            _cache = payload ?? new List<HistoryReminder>();
            return _cache;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<HistoryReminder> GetFallbackAsync(DateTime today)
    {
        var tonePack = await GetTonePackSafeAsync();

        return new HistoryReminder
        {
            Month = today.Month,
            Day = today.Day,
            Event = "someone chose to slow down and notice what was already working.",
            Reflection = tonePack switch
            {
                TonePack.Focus => "Clear beats perfect. One next step is enough for now.",
                TonePack.Recovery => "Gentle progress counts. Rest and momentum can exist together.",
                TonePack.Confidence => "Progress often looks small up close. Your effort still matters.",
                _ => "Gentle progress counts. A small, steady step is still a step."
            }
        };
    }

    private async Task<TonePack> GetTonePackSafeAsync()
    {
        try
        {
            return await _tonePackStore.GetAsync();
        }
        catch
        {
            return TonePack.Grounding;
        }
    }
}
