namespace WellMind.Features.Support.TalkToSomeone;

public partial class TalkToSomeonePage : ContentPage
{
    public TalkToSomeonePage(TalkToSomeoneViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TalkToSomeoneViewModel viewModel)
        {
            await viewModel.LoadAsync();
        }
    }
}
