namespace WellMind.ViewModels;

public sealed class InAppBrowserViewModel : BaseViewModel
{
    private string _title = "";
    private string _url = "";

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public string Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }
}
