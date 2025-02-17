

namespace Storage1.Views;

public partial class ShoppingItemPage : ContentPage
{
    private readonly DatabaseViewModel _viewModel;

    public ShoppingItemPage()
    {
        InitializeComponent();
        _viewModel = new DatabaseViewModel(this);
        BindingContext = _viewModel;
    }

    private async void ViewCartClicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new ShoppingCartPage());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Navigation error: {ex.Message}");
            await DisplayAlert("Error", "Failed to open cart", "OK");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadData();
    }
}
