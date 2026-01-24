using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class WeeklyTrendsViewModel : BaseViewModel
{
    private readonly ITrendService _trendService;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();

    public WeeklyTrendsViewModel(ITrendService trendService)
    {
        _trendService = trendService;
        Title = "Weekly trends";

        _ = LoadTrendsAsync();
    }

    public string Title { get; }

    public IReadOnlyList<Trend> Trends
    {
        get => _trends;
        private set => SetProperty(ref _trends, value);
    }

    private async Task LoadTrendsAsync()
    {
        Trends = await _trendService.GetWeeklyTrendsAsync();
    }
}
