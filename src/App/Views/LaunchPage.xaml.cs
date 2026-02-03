using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace WellMind.Views;

public partial class LaunchPage : ContentPage
{
    private const string FavoritesKey = "AffirmationsFavorites";
    private readonly List<AffirmationSet> _sets;
    private readonly HashSet<string> _favoriteIds;
    private int _orderedIndex;
    private string _line1 = "";
    private string _line2 = "";
    private string _line3 = "";
    private string _line4 = "";
    private string _line5 = "";
    private string _favoriteButtonText = "Favorite this set";
    private string _currentSetId = "";

    public LaunchPage()
    {
        InitializeComponent();
        BindingContext = this;
        _sets = BuildSets();
        _favoriteIds = LoadFavorites();
        ShowFirstSet();
    }

    public string Line1
    {
        get => _line1;
        private set
        {
            _line1 = value;
            OnPropertyChanged();
        }
    }

    public string Line2
    {
        get => _line2;
        private set
        {
            _line2 = value;
            OnPropertyChanged();
        }
    }

    public string Line3
    {
        get => _line3;
        private set
        {
            _line3 = value;
            OnPropertyChanged();
        }
    }

    public string Line4
    {
        get => _line4;
        private set
        {
            _line4 = value;
            OnPropertyChanged();
        }
    }

    public string Line5
    {
        get => _line5;
        private set
        {
            _line5 = value;
            OnPropertyChanged();
        }
    }

    public string FavoriteButtonText
    {
        get => _favoriteButtonText;
        private set
        {
            _favoriteButtonText = value;
            OnPropertyChanged();
        }
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        if (Application.Current is App app)
        {
            app.ShowHomeShellFromLaunch();
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        var screenshot = await Screenshot.Default.CaptureAsync();
        if (screenshot is null)
        {
            return;
        }

        var fileName = $"daily-affirmations-{DateTime.Now:yyyyMMdd-HHmmss}.png";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await using (var imageStream = await screenshot.OpenReadAsync())
        await using (var fileStream = File.OpenWrite(filePath))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share affirmations",
            File = new ShareFile(filePath)
        });
    }

    private void OnMoreAffirmationsClicked(object sender, EventArgs e)
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Haptics can be unsupported on some devices/emulators.
        }

        AdvanceSet();
    }

    private void OnFavoriteClicked(object sender, EventArgs e)
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Haptics can be unsupported on some devices/emulators.
        }

        if (_favoriteIds.Contains(_currentSetId))
        {
            _favoriteIds.Remove(_currentSetId);
        }
        else
        {
            _favoriteIds.Add(_currentSetId);
        }

        SaveFavorites();
        UpdateFavoriteButton();
        ReorderKeepCurrent();
    }

    private void ShowFirstSet()
    {
        _orderedIndex = 0;
        ApplySet(GetOrderedSets()[_orderedIndex]);
    }

    private void AdvanceSet()
    {
        var ordered = GetOrderedSets();
        _orderedIndex = (_orderedIndex + 1) % ordered.Count;
        ApplySet(ordered[_orderedIndex]);
    }

    private void ApplySet(AffirmationSet set)
    {
        _currentSetId = set.Id;
        Line1 = set.Lines[0];
        Line2 = set.Lines[1];
        Line3 = set.Lines[2];
        Line4 = set.Lines[3];
        Line5 = set.Lines[4];
        UpdateFavoriteButton();
    }

    private void UpdateFavoriteButton()
    {
        FavoriteButtonText = _favoriteIds.Contains(_currentSetId) ? "Favorited" : "Favorite this set";
    }

    private void ReorderKeepCurrent()
    {
        var ordered = GetOrderedSets();
        var index = ordered.FindIndex(set => set.Id == _currentSetId);
        _orderedIndex = index >= 0 ? index : 0;
    }

    private List<AffirmationSet> GetOrderedSets()
    {
        var favorites = _sets.Where(set => _favoriteIds.Contains(set.Id)).ToList();
        var rest = _sets.Where(set => !_favoriteIds.Contains(set.Id)).ToList();
        favorites.AddRange(rest);
        return favorites;
    }

    private HashSet<string> LoadFavorites()
    {
        var raw = Preferences.Default.Get(FavoritesKey, string.Empty);
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return new HashSet<string>(
            raw.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            StringComparer.Ordinal);
    }

    private void SaveFavorites()
    {
        var raw = string.Join('|', _favoriteIds);
        Preferences.Default.Set(FavoritesKey, raw);
    }

    private static List<AffirmationSet> BuildSets()
    {
        return new List<AffirmationSet>
        {
            new("set-1", new[]
            {
                "“I am the best.”",
                "“I can do it alone.” (Reflecting self-reliance)",
                "“God is always with me.”",
                "“I am a winner.”",
                "“Today is my day.”"
            }),
            new("set-2", new[]
            {
                "Asking for help is a sign of self-respect and self-awareness.",
                "Changing my mind is a strength, not a weakness.",
                "Every decision I make is supported by my whole and inarguable experience.",
                "I affirm and encourage others, as I do myself.",
                "I alone hold the truth of who I am."
            }),
            new("set-3", new[]
            {
                "All I need is within me now.",
                "Life doesn't happen to me, it happens FOR me.",
                "Every day, in every way, I'm getting stronger and stronger, healthier and healthier, and more and more happy.",
                "I am the architect of my life; I build its foundation and choose its contents.",
                "I am not a creature of circumstance, I am a creator of circumstance."
            }),
            new("set-4", new[]
            {
                "What am I happy about in my life right now?",
                "What am I excited about in my life right now?",
                "What am I proud about in my life right now?",
                "What am I committed to in my life right now?",
                "Who do I love? Who loves me?"
            }),
            new("set-5", new[]
            {
                "On Resilience: \"I will not be broken by adversity; I will rise every time I fall, turning my setbacks into lessons\".",
                "On Courage: \"I will embrace fear, for true courage is overcoming it, not being without it\".",
                "On Impact: \"I will make a meaningful difference in the lives of others, for that is the true measure of a life\".",
                "On Love/Forgiveness: \"I choose to understand and love, for it comes more naturally to the human heart than its opposite\".",
                "On Persistence: \"I believe that it always seems impossible until it is done\"."
            }),
            new("set-6", new[]
            {
                "Be passionate: \"Everyone can rise above their circumstances and achieve success if they are dedicated to and passionate about what they do.\"",
                "Nothing's impossible: \"It always seems impossible, until it is done.\"",
                "Lead from the back: \"Lead from the back – and let others believe they are in front.\"",
                "Exercise: \"I have always believed exercise is a key not only to physical health but to peace of mind.\"",
                "Make a difference: \"What counts in life is not the mere fact that we have lived. It is what difference we have made to the lives of others that will determine the significance of the life we lead.\""
            }),
            new("set-7", new[]
            {
                "Perseverance: \"If I cannot fly, I will run. If I cannot run, I will walk. If I cannot walk, I will crawl, but I will keep moving forward\".",
                "Hope: \"I will accept finite disappointment, but never lose infinite hope\".",
                "Action: \"The time is always right to do what is right\".",
                "Love/Forgiveness: \"I will not let anyone pull me so low as to hate them\".",
                "Service: \"I can be great because I can serve\"."
            }),
            new("set-8", new[]
            {
                "On Courage: \"I will take a stand for what is right, even if it is not popular\".",
                "On Impact: \"I will rise above individualistic concerns to the broader concerns of humanity\".",
                "On Justice: \"I will work to ensure peace is not just the absence of tension, but the presence of justice\".",
                "On Faith: \"I will take the first step in faith, even when I cannot see the whole staircase\".",
                "On Truth: \"I believe that unarmed truth and unconditional love will have the final word\"."
            }),
            new("set-9", new[]
            {
                "I focus only on what I can control, and I do it well.",
                "I meet difficulty with reason, not panic.",
                "I don't need external approval to live with integrity.",
                "What happens to me does not define me — how I respond does.",
                "I am enough when I act with honesty and discipline."
            }),
            new("set-10", new[]
            {
                "I am allowed to take up space exactly as I am.",
                "I carry wisdom earned through both joy and pain.",
                "I choose courage even when fear is present.",
                "My voice matters, and my story has value.",
                "I rise — again and again — because it's who I am."
            }),
            new("set-11", new[]
            {
                "I don't need all the answers to move forward.",
                "My value is not measured by speed, but by depth.",
                "Confusion is not failure — it's the beginning of understanding.",
                "I trust my curiosity to guide me.",
                "I allow myself to think differently."
            }),
            new("set-12", new[]
            {
                "My thoughts are not facts — they are signals, and I can observe them.",
                "Feeling overwhelmed does not mean things are actually falling apart.",
                "I have survived moments that felt just as heavy — and I'm still here.",
                "I don't need to solve my entire life today.",
                "Right now, I am safe enough to slow down."
            }),
            new("set-13", new[]
            {
                "There is no right way to grieve — the way I'm feeling is allowed.",
                "Missing someone is a sign of love, not weakness.",
                "I don't have to be strong all the time.",
                "Grief comes in waves, and I'm allowed to rest between them.",
                "I can carry this and still experience moments of peace."
            })
        };
    }

    private sealed record AffirmationSet(string Id, string[] Lines);
}
