using System.Linq;
using System.Windows.Input;
using WellMind.ViewModels;

namespace WellMind.Features.Support.TalkToSomeone;

public sealed class TalkToSomeoneViewModel : BaseViewModel
{
    private readonly TalkToSomeoneConfigLoader _loader;
    private bool _hasLoaded;
    private string _introTitle = "";
    private string _introBody = "";
    private IReadOnlyList<TalkToSomeoneOptionItem> _options = Array.Empty<TalkToSomeoneOptionItem>();
    private bool _showState;
    private bool _showContent;
    private string _stateTitle = "";
    private string _stateBody = "";
    private string _stateFooter = "";

    public TalkToSomeoneViewModel(TalkToSomeoneConfigLoader loader)
    {
        _loader = loader;
        ContinueCommand = new Command<TalkToSomeoneOptionItem>(async option => await ContinueAsync(option));
    }

    public string IntroTitle
    {
        get => _introTitle;
        private set => SetProperty(ref _introTitle, value);
    }

    public string IntroBody
    {
        get => _introBody;
        private set => SetProperty(ref _introBody, value);
    }

    public IReadOnlyList<TalkToSomeoneOptionItem> Options
    {
        get => _options;
        private set => SetProperty(ref _options, value);
    }

    public bool ShowState
    {
        get => _showState;
        private set => SetProperty(ref _showState, value);
    }

    public bool ShowContent
    {
        get => _showContent;
        private set => SetProperty(ref _showContent, value);
    }

    public string StateTitle
    {
        get => _stateTitle;
        private set => SetProperty(ref _stateTitle, value);
    }

    public string StateBody
    {
        get => _stateBody;
        private set => SetProperty(ref _stateBody, value);
    }

    public string StateFooter
    {
        get => _stateFooter;
        private set => SetProperty(ref _stateFooter, value);
    }

    public ICommand ContinueCommand { get; }

    public async Task LoadAsync()
    {
        if (_hasLoaded)
        {
            return;
        }

        var config = await _loader.LoadAsync();
        _hasLoaded = true;

        // If we cannot read the config, show a safe empty state instead of breaking.
        if (config.LoadFailed)
        {
            SetState(
                "Support temporarily unavailable",
                "We couldn't load support options right now.",
                "No information was saved or shared.");
            return;
        }

        // A disabled feature should be visible but clearly inactive.
        if (!config.Enabled)
        {
            SetState(
                "Support is not enabled",
                "This option isn't available right now. You can still use check-ins and resources anytime.",
                string.Empty);
            return;
        }

        var enabledOptions = config.Options
            .Where(option => option.Enabled)
            .Select(option => new TalkToSomeoneOptionItem(option.Id, option.Title, option.Body))
            .ToList();

        // Enabled feature with no options still needs a calm explanation.
        if (enabledOptions.Count == 0)
        {
            SetState(
                "Nothing to choose yet",
                "Support options will appear here when they're available.",
                "You don't need to do anything.");
            return;
        }

        IntroTitle = config.IntroTitle;
        IntroBody = config.IntroBody;
        Options = enabledOptions;
        ShowState = false;
        ShowContent = true;
    }

    private void SetState(string title, string body, string footer)
    {
        ShowState = true;
        ShowContent = false;
        StateTitle = title;
        StateBody = body;
        StateFooter = footer;
    }

    private static async Task ContinueAsync(TalkToSomeoneOptionItem? option)
    {
        if (option is null)
        {
            return;
        }

        // Ask for confirmation so the user stays in control.
        var confirm = await Shell.Current.DisplayAlert(
            "Before you continue",
            "This option is anonymous. No identity is shared, and nothing happens unless you choose to proceed.\nYou can cancel at any time.",
            "Continue",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        await Shell.Current.DisplayAlert("Coming soon", "This feature is coming soon.", "OK");
    }
}

public sealed record TalkToSomeoneOptionItem(string Id, string Title, string Body);
