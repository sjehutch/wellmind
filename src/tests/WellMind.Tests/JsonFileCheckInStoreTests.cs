using WellMind.Models;
using WellMind.Services;
using Xunit;

namespace WellMind.Tests;

public sealed class JsonFileCheckInStoreTests
{
    [Fact]
    public async Task GetTodayReturnsNullWhenMissing()
    {
        using var temp = new TempFolder();
        var store = new JsonFileCheckInStore(temp.Path);

        var today = await store.GetTodayAsync();

        Assert.Null(today);
    }

    [Fact]
    public async Task UpsertTodayOverwritesSameDate()
    {
        using var temp = new TempFolder();
        var store = new JsonFileCheckInStore(temp.Path);

        await store.UpsertTodayAsync(new CheckIn
        {
            Energy = 1,
            Stress = 1,
            Focus = 1,
            SleepQuality = 1
        });

        await store.UpsertTodayAsync(new CheckIn
        {
            Energy = 5,
            Stress = 4,
            Focus = 3,
            SleepQuality = 2
        });

        var checkIns = await store.GetLastDaysAsync(7);

        Assert.Single(checkIns);
        Assert.Equal(5, checkIns[0].Energy);
        Assert.Equal(4, checkIns[0].Stress);
    }
}

internal sealed class TempFolder : IDisposable
{
    public TempFolder()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        System.IO.Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        if (System.IO.Directory.Exists(Path))
        {
            System.IO.Directory.Delete(Path, true);
        }
    }
}
