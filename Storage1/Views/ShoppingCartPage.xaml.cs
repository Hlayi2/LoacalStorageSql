

namespace Storage1.Views;

public partial class ShoppingCartPage : ContentPage
{
    private readonly DatabaseViewModel _viewModel;

    public ShoppingCartPage()
    {
        InitializeComponent();
        // Create a new instance with the current page
        _viewModel = new DatabaseViewModel(this);
        BindingContext = _viewModel;
        // Reload cart data when page appears
        LoadCartData();
    }

    private async void LoadCartData()
    {
        await _viewModel.LoadCartDataAsync();
    }

    private async void ContinueShoppingClicked(object sender, EventArgs e)
    {
        // Pop back to previous page
        await Navigation.PopAsync();
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
}