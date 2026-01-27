namespace WellMind.Services;

public interface IFirstRunStore
{
    bool HasSeenWelcome();
    void MarkWelcomeSeen();
}
