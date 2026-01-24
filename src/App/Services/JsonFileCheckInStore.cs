using System.Text.Json;
using Microsoft.Maui.Storage;
using WellMind.Models;

namespace WellMind.Services;

public sealed class JsonFileCheckInStore : ICheckInStore
{
    private const string FolderName = "WellMind";
    private const string FileName = "checkins.json";
    // Single-file store, so a gate keeps file reads/writes from colliding.
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    private readonly string _filePath;

    public JsonFileCheckInStore()
        : this(null)
    {
    }

    public JsonFileCheckInStore(string? baseDirectory)
    {
        var root = string.IsNullOrWhiteSpace(baseDirectory)
            ? FileSystem.AppDataDirectory
            : baseDirectory;

        var folder = Path.Combine(root, FolderName);
        _filePath = Path.Combine(folder, FileName);
    }

    public async Task<CheckIn?> GetTodayAsync(CancellationToken ct = default)
    {
        var todayKey = GetTodayKey();
        var data = await LoadAsync(ct);
        return data.CheckIns.LastOrDefault(checkIn => checkIn.DateLocal == todayKey);
    }

    public async Task<IReadOnlyList<CheckIn>> GetLastDaysAsync(int days, CancellationToken ct = default)
    {
        if (days <= 0)
        {
            return Array.Empty<CheckIn>();
        }

        // Filter by local date string so results match the user's day, not UTC.
        var cutoff = DateTime.Now.Date.AddDays(-days + 1);
        var data = await LoadAsync(ct);

        return data.CheckIns
            .Where(checkIn => TryParseDate(checkIn.DateLocal, out var date) && date >= cutoff)
            .OrderBy(checkIn => checkIn.DateLocal)
            .ToList();
    }

    public async Task UpsertTodayAsync(CheckIn input, CancellationToken ct = default)
    {
        var todayKey = GetTodayKey();

        // Always set local date keys on save so today is the single source of truth.
        var updated = new CheckIn
        {
            DateLocal = todayKey,
            TimestampLocal = DateTime.Now.ToString("O"),
            Energy = input.Energy,
            Stress = input.Stress,
            Focus = input.Focus,
            SleepQuality = input.SleepQuality,
            Note = string.IsNullOrWhiteSpace(input.Note) ? null : input.Note
        };

        await _gate.WaitAsync(ct);
        try
        {
            var data = await LoadUnlockedAsync(ct);
            var index = data.CheckIns.FindIndex(checkIn => checkIn.DateLocal == todayKey);

            if (index >= 0)
            {
                data.CheckIns[index] = updated;
            }
            else
            {
                data.CheckIns.Add(updated);
            }

            await SaveUnlockedAsync(data, ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task DeleteAllAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<CheckInData> LoadAsync(CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            return await LoadUnlockedAsync(ct);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<CheckInData> LoadUnlockedAsync(CancellationToken ct)
    {
            // If the file is missing or corrupt, return an empty set instead of crashing.
            if (!File.Exists(_filePath))
            {
                return new CheckInData();
            }

        try
        {
            await using var stream = File.OpenRead(_filePath);
            var data = await JsonSerializer.DeserializeAsync<CheckInData>(stream, _jsonOptions, ct);
            return data ?? new CheckInData();
        }
        catch
        {
            return new CheckInData();
        }
    }

    private async Task SaveUnlockedAsync(CheckInData data, CancellationToken ct)
    {
        var folder = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, data, _jsonOptions, ct);
    }

    private static string GetTodayKey()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    private static bool TryParseDate(string dateLocal, out DateTime date)
    {
        return DateTime.TryParse(dateLocal, out date);
    }

    private sealed class CheckInData
    {
        public int Version { get; set; } = 1;
        public List<CheckIn> CheckIns { get; set; } = new();
    }
}
