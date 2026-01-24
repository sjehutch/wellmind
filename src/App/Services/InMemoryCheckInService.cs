using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryCheckInService : ICheckInService
{
    private readonly List<CheckIn> _checkIns = new();

    public Task SaveAsync(CheckIn checkIn, CancellationToken cancellationToken = default)
    {
        // Placeholder in-memory store so ViewModels stay simple.
        _checkIns.Add(checkIn);
        return Task.CompletedTask;
    }
}
