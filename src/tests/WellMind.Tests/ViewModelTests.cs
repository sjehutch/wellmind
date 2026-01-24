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
            new FakeResourceLinkService());

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
            new FakeResourceLinkService());

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

    public Task GoBackAsync()
    {
        return Task.CompletedTask;
    }
}
