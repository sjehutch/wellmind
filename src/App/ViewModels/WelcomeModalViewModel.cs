using System.Windows.Input;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class WelcomeModalViewModel : BaseViewModel
{
    private readonly IFirstRunStore _firstRunStore;

    public WelcomeModalViewModel(IFirstRunStore firstRunStore)
    {
        _firstRunStore = firstRunStore;
        CloseCommand = new Command(async () => await CloseAsync());
    }

    public ICommand CloseCommand { get; }

    private async Task CloseAsync()
    {
        // Mark it as seen so we only show it once.
        _firstRunStore.MarkWelcomeSeen();
        await Shell.Current.Navigation.PopModalAsync();
    }
}
