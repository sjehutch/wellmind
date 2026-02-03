using System.Linq;
using System.Text.Json;
using Microsoft.Maui.Storage;
using WellMind.Models;

namespace WellMind.Services;

public sealed class HistoryReminderService
{
    private const string AssetName = "history_reminders.json";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private IReadOnlyList<HistoryReminder>? _cache;

    public async Task<HistoryReminder> GetForTodayAsync(DateTime? now = null)
    {
        var today = (now ?? DateTime.Now).Date;
        try
        {
            var reminders = await GetAllAsync();
            var match = reminders.FirstOrDefault(item => item.Month == today.Month && item.Day == today.Day);
            return match ?? GetFallback();
        }
        catch
        {
            return GetFallback();
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

    private static HistoryReminder GetFallback()
    {
        return new HistoryReminder
        {
            Month = DateTime.Now.Month,
            Day = DateTime.Now.Day,
            Event = "someone chose to slow down and notice what was already working.",
            Reflection = "Gentle progress counts. A small, steady step is still a step."
        };
    }
}
