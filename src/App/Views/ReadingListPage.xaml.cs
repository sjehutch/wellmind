using WellMind.Models;

namespace WellMind.Views;

public partial class ReadingListPage : ContentPage
{
    public ReadingListPage()
    {
        InitializeComponent();
        ArticlesCollection.ItemsSource = ReadingContent.All;
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ReadingArticle article)
        {
            return;
        }

        ArticlesCollection.SelectedItem = null;
        await Navigation.PushAsync(new ReadingDetailPage(article));
    }
}
