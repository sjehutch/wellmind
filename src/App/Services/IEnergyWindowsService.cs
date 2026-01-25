using WellMind.Models;

namespace WellMind.Services;

public interface IEnergyWindowsService
{
    EnergyWindowsResult BuildMessage(IReadOnlyList<CheckIn> checkIns);
}
