using Storage1.Models;

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

    private async void AddToCartClicked(object sender, EventArgs e)
    {
        var collectionView = this.FindByName<CollectionView>("ShoppingItemsCollection");

        if (collectionView != null && collectionView.SelectedItem is ShoppingItem selectedItem)
        {
            // Explicitly set the selected item in the view model
            _viewModel.SelectedItem = selectedItem;
            Console.WriteLine($"Selected item for cart: {selectedItem.Name}");

            // Call AddToCartAsync with the selected item
            await _viewModel.AddToCartAsync(selectedItem);
        }
        else
        {
            await DisplayAlert("Error", "Please select an item first", "OK");
        }
    }

    private async void ViewCartClicked(object sender, EventArgs e)
    {
        try
        {
            // Pass the existing ViewModel instance to the ShoppingCartPage
            await Navigation.PushAsync(new ShoppingCartPage(_viewModel));
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

    private async void QuickViewClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ShoppingItem item)
        {
            // Create a more detailed view for the quick view popup
            var detailStack = new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(15)
            };

            // Add image if available
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                detailStack.Children.Add(new Image
                {
                    Source = item.ImageUrl,
                    HeightRequest = 200,
                    HorizontalOptions = LayoutOptions.Center
                });
            }

            // Add name
            detailStack.Children.Add(new Label
            {
                Text = item.Name,
                FontSize = 22,
                FontAttributes = FontAttributes.Bold
            });

            // Add description
            detailStack.Children.Add(new Label
            {
                Text = item.Description,
                FontSize = 16
            });

            // Add price
            detailStack.Children.Add(new Label
            {
                Text = $"Price: R{item.Price:N2}",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold
            });

            // Add stock information
            detailStack.Children.Add(new Label
            {
                Text = $"Available Stock: {item.StockQuantity}",
                FontSize = 16
            });

            // Show the popup
            await DisplayAlert(item.Name, $"{item.Description}\n\nPrice: R{item.Price:N2}\nStock: {item.StockQuantity}", "Close");
        }
    }

}