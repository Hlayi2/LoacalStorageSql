namespace Storage1.Views;

public partial class ShoppingCartPage : ContentPage
{
    private readonly DatabaseViewModel _viewModel;

    public ShoppingCartPage()
    {
        InitializeComponent();
        _viewModel = new DatabaseViewModel(this);
        BindingContext = _viewModel;
        LoadCartData();
    }

    public ShoppingCartPage(DatabaseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        LoadCartData();
    }

    private async void LoadCartData()
    {
        await _viewModel.LoadCartDataAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Always reload cart data when page appears
        await _viewModel.LoadCartDataAsync();
    }

    private async void ContinueShoppingClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(ShoppingItemPage)}");
    }

    private async void CheckoutClicked(object sender, EventArgs e)
    {
        if (_viewModel.CartItems.Count == 0)
        {
            await DisplayAlert("Cart Empty", "Add items to cart first", "OK");
            return;
        }
        var result = await DisplayAlert("Confirm", "Proceed to checkout?", "Yes", "No");
        if (result)
        {
            await _viewModel.ClearCart();
            await DisplayAlert("Success", "Order placed!", "OK");
            await Navigation.PopAsync();
        }
    }

    private async void ClearCartClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Clear Cart", "Are you sure you want to clear the cart?", "Yes", "No");
        if (result)
        {
            await _viewModel.ClearCart();
            await DisplayAlert("Success", "Cart cleared successfully", "OK");
        }
    }
}