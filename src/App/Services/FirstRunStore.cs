using Microsoft.Maui.Storage;

namespace WellMind.Services;

public sealed class FirstRunStore : IFirstRunStore
{
    private const string WelcomeSeenKey = "WelcomeSeen";

    public bool HasSeenWelcome()
    {
        return Preferences.Default.Get(WelcomeSeenKey, false);
    }

    public void MarkWelcomeSeen()
    {
        Preferences.Default.Set(WelcomeSeenKey, true);
    }
}
