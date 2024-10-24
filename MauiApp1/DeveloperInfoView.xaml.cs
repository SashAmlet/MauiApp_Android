namespace MauiApp1;

public partial class DeveloperInfoView : ContentPage
{
	public DeveloperInfoView()
	{
		InitializeComponent();
    }
    private async void OnCloseButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopModalAsync();
    }

}