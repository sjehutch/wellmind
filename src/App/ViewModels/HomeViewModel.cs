using System.Windows.Input;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class HomeViewModel : BaseViewModel
{
    private readonly ITrendService _trendService;
    private readonly INavigationService _navigationService;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();

    public HomeViewModel(ITrendService trendService, INavigationService navigationService)
    {
        _trendService = trendService;
        _navigationService = navigationService;

        StartCheckInCommand = new Command(async () => await _navigationService.GoToCheckInAsync());
        SummaryText = "A calm snapshot of how your week has been going.";

        _ = LoadTrendsAsync();
    }

    public string SummaryText { get; }

    public IReadOnlyList<Trend> Trends
    {
        get => _trends;
        private set => SetProperty(ref _trends, value);
    }

    public ICommand StartCheckInCommand { get; }

    private async Task LoadTrendsAsync()
    {
        Trends = await _trendService.GetWeeklyTrendsAsync();
    }
}
