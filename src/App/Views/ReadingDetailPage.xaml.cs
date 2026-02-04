using WellMind.Models;

namespace WellMind.Views;

public partial class ReadingDetailPage : ContentPage
{
    public ReadingDetailPage(ReadingArticle article)
    {
        InitializeComponent();
        Title = article.Title;
        TitleLabel.Text = article.Title;
        BodyLabel.Text = article.Body;
    }
}
