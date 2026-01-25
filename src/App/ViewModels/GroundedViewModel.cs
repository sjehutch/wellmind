using System.Windows.Input;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using WellMind.Models;
using WellMind.Services;

namespace WellMind.ViewModels;

public sealed class GroundedViewModel : BaseViewModel
{
    private readonly IGrounded _grounded;
    private GroundedQuote? _current;
    private string _quoteText = "";
    private string _quoteAuthor = "";
    private IReadOnlyList<string> _tags = Array.Empty<string>();
    private string _filterLabel = "All";
    private string? _activeFilter;

    public GroundedViewModel(IGrounded grounded)
    {
        _grounded = grounded;
        NextQuoteCommand = new Command(async () => await LoadNextAsync());
        ShareCommand = new Command(async () => await ShareAsync());
        TagSelectedCommand = new Command<string>(async tag => await ToggleFilterAsync(tag));
    }

    public string QuoteText
    {
        get => _quoteText;
        private set => SetProperty(ref _quoteText, value);
    }

    public string QuoteAuthor
    {
        get => _quoteAuthor;
        private set => SetProperty(ref _quoteAuthor, value);
    }

    public IReadOnlyList<string> Tags
    {
        get => _tags;
        private set => SetProperty(ref _tags, value);
    }

    public string FilterLabel
    {
        get => _filterLabel;
        private set => SetProperty(ref _filterLabel, value);
    }

    public ICommand NextQuoteCommand { get; }
    public ICommand ShareCommand { get; }
    public ICommand TagSelectedCommand { get; }

    public async Task LoadAsync()
    {
        if (_current is not null)
        {
            return;
        }

        await LoadNextAsync();
    }

    private async Task LoadNextAsync()
    {
        // Ask the service for the next quote based on the active tag filter.
        var quote = await _grounded.GetNextAsync(_activeFilter);
        ApplyQuote(quote);
    }

    private void ApplyQuote(GroundedQuote quote)
    {
        _current = quote;
        QuoteText = quote.Text;
        QuoteAuthor = $"- {quote.Author}";
        Tags = quote.Tags;
    }

    private async Task ToggleFilterAsync(string tag)
    {
        // Tap again to clear the filter back to All.
        if (_activeFilter == tag)
        {
            _activeFilter = null;
            FilterLabel = "All";
        }
        else
        {
            _activeFilter = tag;
            FilterLabel = tag;
        }

        await LoadNextAsync();
    }

    private async Task ShareAsync()
    {
        if (_current is null)
        {
            return;
        }

        var text = $"\"{_current.Text}\" — {_current.Author}\n#WellMind";
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = text,
            Title = "Share quote"
        });
    }
}
