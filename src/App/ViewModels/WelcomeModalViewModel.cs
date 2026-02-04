using System.Windows.Input;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class WelcomeModalViewModel : BaseViewModel
{
    private readonly IFirstRunStore _firstRunStore;
    private bool _isClosing;

    public WelcomeModalViewModel(IFirstRunStore firstRunStore)
    {
        _firstRunStore = firstRunStore;
        CloseCommand = new Command(async () => await CloseAsync());
    }

    public ICommand CloseCommand { get; }

    private async Task CloseAsync()
    {
        if (_isClosing)
        {
            return;
        }

        _isClosing = true;

        // Mark it as seen so we only show it once.
        _firstRunStore.MarkWelcomeSeen();
        var navigation = Shell.Current?.Navigation
            ?? Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;

        if (navigation is null || navigation.ModalStack.Count == 0)
        {
            _isClosing = false;
            return;
        }

        try
        {
            await navigation.PopModalAsync();
        }
        finally
        {
            _isClosing = false;
        }
    }
}
