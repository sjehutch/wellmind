using WellMind.Models;

namespace WellMind.Services;

public interface ICheckInService
{
    Task SaveAsync(CheckIn checkIn, CancellationToken cancellationToken = default);
}
