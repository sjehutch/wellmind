using WellMind.Models;
using WellMind.Services;
using WellMind.ViewModels;
using Xunit;

namespace WellMind.Tests;

public sealed class ViewModelTests
{
    [Fact]
    public async Task HomeViewModelSetsPrimaryActionForMissingToday()
    {
        var store = new FakeCheckInStore { Today = null };
        var viewModel = new HomeViewModel(
            new FakeTrendService(),
            new FakeNavigationService(),
            store,
            new FakeTipService(),
            new FakeResourceLinkService(),
            new FakeEnergyWindowsService(),
            new FakeReminderSettingsStore(),
            new FakeHomeBackgroundService(),
            new FakeTonePackStore(),
            new FakeTipFeedbackStore());

        await viewModel.LoadAsync();

        Assert.Equal("Start today's check-in", viewModel.PrimaryActionText);
    }

    [Fact]
    public async Task HomeViewModelSetsPrimaryActionForExistingToday()
    {
        var store = new FakeCheckInStore
        {
            Today = new CheckIn { Energy = 4, Stress = 2, Focus = 3, SleepQuality = 4 }
        };

        var viewModel = new HomeViewModel(
            new FakeTrendService(),
            new FakeNavigationService(),
            store,
            new FakeTipService(),
            new FakeResourceLinkService(),
            new FakeEnergyWindowsService(),
            new FakeReminderSettingsStore(),
            new FakeHomeBackgroundService(),
            new FakeTonePackStore(),
            new FakeTipFeedbackStore());

        await viewModel.LoadAsync();

        Assert.Equal("Update today's check-in", viewModel.PrimaryActionText);
    }

    [Fact]
    public async Task CheckInViewModelSavingTwiceKeepsSingleEntry()
    {
        using var temp = new TempFolder();
        var store = new JsonFileCheckInStore(temp.Path);
        var viewModel = new CheckInViewModel(store, new FakeNavigationService());

        viewModel.InitializeNewToday();
        viewModel.Energy = 2;
        await viewModel.SaveAsync();

        viewModel.Energy = 5;
        await viewModel.SaveAsync();

        var checkIns = await store.GetLastDaysAsync(7);

        Assert.Single(checkIns);
        Assert.Equal(5, checkIns[0].Energy);
    }
}

internal sealed class FakeCheckInStore : ICheckInStore
{
    public CheckIn? Today { get; set; }
    public List<CheckIn> Items { get; } = new();
    public string? TodayHeavyNote { get; set; }
    public DateTime? TodayHeavyNoteUpdatedAt { get; set; }

    public Task<CheckIn?> GetTodayAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Today);
    }

    public Task<IReadOnlyList<CheckIn>> GetLastDaysAsync(int days, CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<CheckIn>>(Items);
    }

    public Task UpsertTodayAsync(CheckIn input, CancellationToken ct = default)
    {
        Today = input;
        if (Items.Count == 0)
        {
            Items.Add(input);
        }
        else
        {
            Items[0] = input;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAllAsync(CancellationToken ct = default)
    {
        Items.Clear();
        Today = null;
        return Task.CompletedTask;
    }

    public Task<string?> GetTodayHeavyNoteAsync(CancellationToken ct = default)
    {
        return Task.FromResult(TodayHeavyNote);
    }

    public Task<DateTime?> GetTodayHeavyNoteUpdatedAtAsync(CancellationToken ct = default)
    {
        return Task.FromResult(TodayHeavyNoteUpdatedAt);
    }

    public Task UpsertTodayHeavyNoteAsync(string? note, DateTime updatedAt, CancellationToken ct = default)
    {
        TodayHeavyNote = note;
        TodayHeavyNoteUpdatedAt = string.IsNullOrWhiteSpace(note) ? null : updatedAt;
        return Task.CompletedTask;
    }
}

internal sealed class FakeTrendService : ITrendService
{
    public Task<IReadOnlyList<Trend>> GetWeeklyTrendsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Trend>>(Array.Empty<Trend>());
    }
}

internal sealed class FakeTipService : ITipService
{
    public Task<IReadOnlyList<Tip>> GetGentleTipsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Tip>>(Array.Empty<Tip>());
    }
}

internal sealed class FakeResourceLinkService : IResourceLinkService
{
    public Task<IReadOnlyList<ResourceLink>> GetLinksAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<ResourceLink>>(Array.Empty<ResourceLink>());
    }
}

internal sealed class FakeNavigationService : INavigationService
{
    public Task GoToCheckInAsync()
    {
        return Task.CompletedTask;
    }

    public Task GoToResourceAsync(string title, string url)
    {
        return Task.CompletedTask;
    }

    public Task OpenGentleReminderAsync()
    {
        return Task.CompletedTask;
    }

    public Task OpenHistoryReminderAsync()
    {
        return Task.CompletedTask;
    }

    public Task CloseModalAsync()
    {
        return Task.CompletedTask;
    }

    public Task GoBackAsync()
    {
        return Task.CompletedTask;
    }
}

internal sealed class FakeHomeBackgroundService : IHomeBackgroundService
{
    private string? _colorHex;

    public Task<string?> GetBackgroundColorAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_colorHex);
    }

    public Task SetBackgroundColorAsync(string colorHex, CancellationToken ct = default)
    {
        _colorHex = colorHex;
        return Task.CompletedTask;
    }

    public Task ResetAsync(CancellationToken ct = default)
    {
        _colorHex = null;
        return Task.CompletedTask;
    }
}

internal sealed class FakeTonePackStore : ITonePackStore
{
    public TonePack TonePack { get; set; } = TonePack.Grounding;

    public Task<TonePack> GetAsync(CancellationToken ct = default)
    {
        return Task.FromResult(TonePack);
    }

    public Task SaveAsync(TonePack tonePack, CancellationToken ct = default)
    {
        TonePack = tonePack;
        return Task.CompletedTask;
    }
}

internal sealed class FakeTipFeedbackStore : ITipFeedbackStore
{
    public Task<int> GetHelpfulCountAsync(string tipId, CancellationToken ct = default)
    {
        return Task.FromResult(0);
    }

    public Task MarkHelpfulAsync(string tipId, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}

internal sealed class FakeEnergyWindowsService : IEnergyWindowsService
{
    public EnergyWindowsResult BuildMessage(IReadOnlyList<CheckIn> checkIns)
    {
        return new EnergyWindowsResult("No pattern yet. That's normal.", "insufficient");
    }
}

internal sealed class FakeReminderSettingsStore : IReminderSettingsStore
{
    public bool IsEnabled { get; set; }
    public TimeSpan Time { get; set; } = new(9, 0, 0);

    public Task<bool> GetIsEnabledAsync(CancellationToken ct = default)
    {
        return Task.FromResult(IsEnabled);
    }

    public Task<TimeSpan> GetTimeAsync(CancellationToken ct = default)
    {
        return Task.FromResult(Time);
    }

    public Task SaveAsync(bool isEnabled, TimeSpan time, CancellationToken ct = default)
    {
        IsEnabled = isEnabled;
        Time = time;
        return Task.CompletedTask;
    }
}
