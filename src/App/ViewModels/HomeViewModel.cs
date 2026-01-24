using System.Windows.Input;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class HomeViewModel : BaseViewModel
{
    private readonly ITrendService _trendService;
    private readonly INavigationService _navigationService;
    private readonly ICheckInService _checkInService;
    private readonly ITipService _tipService;
    private readonly IResourceLinkService _resourceLinkService;
    private IReadOnlyList<Trend> _trends = Array.Empty<Trend>();
    private IReadOnlyList<Tip> _tips = Array.Empty<Tip>();
    private IReadOnlyList<ResourceLink> _links = Array.Empty<ResourceLink>();
    private bool _hasTips;
    private bool _hasLinks;

    public HomeViewModel(
        ITrendService trendService,
        INavigationService navigationService,
        ICheckInService checkInService,
        ITipService tipService,
        IResourceLinkService resourceLinkService)
    {
        _trendService = trendService;
        _navigationService = navigationService;
        _checkInService = checkInService;
        _tipService = tipService;
        _resourceLinkService = resourceLinkService;

        StartCheckInCommand = new Command(async () => await _navigationService.GoToCheckInAsync());
        OpenLinkCommand = new Command<ResourceLink>(async link => await OpenLinkAsync(link));
        SummaryText = "A calm snapshot of how your week has been going.";

        _checkInService.CheckInsChanged += HandleCheckInsChanged;
        _ = RefreshAsync();
    }

    public string SummaryText { get; }

    public IReadOnlyList<Trend> Trends
    {
        get => _trends;
        private set => SetProperty(ref _trends, value);
    }

    public IReadOnlyList<Tip> Tips
    {
        get => _tips;
        private set => SetProperty(ref _tips, value);
    }

    public bool HasTips
    {
        get => _hasTips;
        private set => SetProperty(ref _hasTips, value);
    }

    public IReadOnlyList<ResourceLink> Links
    {
        get => _links;
        private set => SetProperty(ref _links, value);
    }

    public bool HasLinks
    {
        get => _hasLinks;
        private set => SetProperty(ref _hasLinks, value);
    }

    public ICommand StartCheckInCommand { get; }
    public ICommand OpenLinkCommand { get; }

    private void HandleCheckInsChanged(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () => await RefreshAsync());
    }

    private async Task RefreshAsync()
    {
        Trends = await _trendService.GetWeeklyTrendsAsync();
        Tips = await _tipService.GetGentleTipsAsync();
        Links = await _resourceLinkService.GetLinksAsync();
        UpdateVisibility();
    }

    private Task OpenLinkAsync(ResourceLink? link)
    {
        if (link is null || string.IsNullOrWhiteSpace(link.Url))
        {
            return Task.CompletedTask;
        }

        var title = string.IsNullOrWhiteSpace(link.Title) ? "Learn more" : link.Title;
        return _navigationService.GoToResourceAsync(title, link.Url);
    }

    private void UpdateVisibility()
    {
        HasTips = Tips.Count > 0;
        HasLinks = HasTips && Links.Count > 0;
    }
}
