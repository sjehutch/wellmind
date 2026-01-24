namespace WellMind.Services;

public interface INavigationService
{
    Task GoToCheckInAsync();
    Task GoBackAsync();
}
