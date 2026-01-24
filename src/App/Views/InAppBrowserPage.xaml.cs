using WellMind.ViewModels;

namespace WellMind.Views;

[QueryProperty(nameof(Url), "url")]
[QueryProperty(nameof(PageTitle), "title")]
public partial class InAppBrowserPage : ContentPage
{
    private readonly InAppBrowserViewModel _viewModel;

    public InAppBrowserPage(InAppBrowserViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public string Url
    {
        set => _viewModel.Url = Uri.UnescapeDataString(value ?? string.Empty);
    }

    public string PageTitle
    {
        set => _viewModel.Title = Uri.UnescapeDataString(value ?? string.Empty);
    }
}
