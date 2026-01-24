using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryCheckInService : ICheckInService
{
    private readonly List<CheckIn> _checkIns = new();

    public event EventHandler? CheckInsChanged;

    public Task SaveAsync(CheckIn checkIn, CancellationToken cancellationToken = default)
    {
        // Placeholder in-memory store so ViewModels stay simple.
        _checkIns.Add(checkIn);
        CheckInsChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<CheckIn>> GetRecentAsync(int days, CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.Date.AddDays(-days + 1);
        IReadOnlyList<CheckIn> recent = _checkIns
            .Where(checkIn => checkIn.Date >= cutoff)
            .OrderBy(checkIn => checkIn.Date)
            .ToList();

        return Task.FromResult(recent);
    }
}
