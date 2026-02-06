using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using WellMind.Models;

namespace WellMind.Views;

public partial class ReadingDetailPage : ContentPage
{
    private readonly ReadingArticle _article;

    public ReadingDetailPage(ReadingArticle article)
    {
        InitializeComponent();
        _article = article;
        Title = article.Title;
        TitleLabel.Text = article.Title;
        SummaryLabel.Text = article.Summary;
        BodyLabel.Text = article.Body;
    }

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Some devices do not support haptics.
        }

        var text = $"By : Well Mind - https://apps.apple.com/us/app/wellmind-daily/id6758276490\n{_article.Title}\n{_article.Body}";

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share reading",
            Text = text
        });
    }
}
