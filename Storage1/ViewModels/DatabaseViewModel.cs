using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Storage1.Models;
using Storage1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

public partial class DatabaseViewModel : ObservableObject
{
    private readonly DatabaseService _dbService = new();
    private Profile _currentProfile;
    private ShoppingItem _selectedItem;

    public ObservableCollection<ShoppingItem> ShoppingItems { get; } = new();
    public ObservableCollection<ShoppingCart> CartItems { get; } = new();

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string _surname;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _bio;

    public ICommand SaveProfileCommand { get; }
    public ICommand AddToCartCommand { get; }
    public ICommand RemoveCartItemCommand { get; }

    private readonly Page _page;

    public DatabaseViewModel(Page page)
    {
        _page = page;

        SaveProfileCommand = new RelayCommand(async () => await SaveProfile());
        AddToCartCommand = new RelayCommand<ShoppingItem>(async (item) => await AddToCart(item));
        RemoveCartItemCommand = new RelayCommand<ShoppingCart>(async (item) => await RemoveCartItem(item));

        LoadData();
    }

    public async void LoadData()
    {
        try
        {
            _currentProfile = await _dbService.GetProfileAsync();
            if (_currentProfile != null)
            {
                _name = _currentProfile.Name;
                _surname = _currentProfile.Surname;
                _email = _currentProfile.Email;
                _bio = _currentProfile.Bio;
            }

            var items = await _dbService.GetAllShoppingItemsAsync();
            ShoppingItems.Clear();
            foreach (var item in items) ShoppingItems.Add(item);

            await LoadCartDataAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }

    private async Task SaveProfile()
    {
        if (string.IsNullOrWhiteSpace(_name) || string.IsNullOrWhiteSpace(_email))
        {
            await _page.DisplayAlert("Error", "Name and Email are required", "OK");
            return;
        }

        var profile = new Profile
        {
            Id = _currentProfile?.Id ?? 1,
            Name = _name,
            Surname = _surname,
            Email = _email,
            Bio = _bio
        };

        await _dbService.UpdateProfileAsync(profile);
        _currentProfile = profile;
        await _page.DisplayAlert("Success", "Profile saved successfully", "OK");
    }

    private async Task AddToCart(ShoppingItem item)
    {
        try
        {
            if (_currentProfile == null)
            {
                await _page.DisplayAlert("Error", "Profile not loaded. Please try again or create a profile first.", "OK");
                return;
            }

            if (item == null)
            {
                await _page.DisplayAlert("Error", "Please select an item first", "OK");
                return;
            }

            Console.WriteLine($"Adding item to cart: {item.Name} with quantity {item.Quantity}");

            if (item.Quantity <= 0)
            {
                await _page.DisplayAlert("Error", "Please select a quantity greater than 0", "OK");
                return;
            }

            if (!await _dbService.ValidateStockAsync(item.Id, item.Quantity))
            {
                await _page.DisplayAlert("Error", $"Not enough stock available. Current stock: {item.StockQuantity}", "OK");
                return;
            }

            var cartItem = new ShoppingCart
            {
                ProfileId = _currentProfile.Id,
                ShoppingItemId = item.Id,
                Quantity = item.Quantity,
                ShoppingItem = item
            };

            await _dbService.AddToCartAsync(cartItem);
            await _dbService.UpdateStockAsync(item.Id, -item.Quantity);

            // Reload cart data
            await LoadCartDataAsync();

            // Reset quantity after adding to cart
            item.Quantity = 1;

            // Notify UI that properties have changed
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(CartItems));

            await _page.DisplayAlert("Success", $"{cartItem.Quantity} {item.Name}(s) added to cart", "OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding to cart: {ex.Message}");
            await _page.DisplayAlert("Error", "Failed to add item to cart: " + ex.Message, "OK");
        }
    }

    public async Task AddToCartAsync(ShoppingItem item)
    {
        await AddToCart(item);
    }

    private async Task RemoveCartItem(ShoppingCart item)
    {
        await _dbService.RemoveCartItemAsync(item);
        await _dbService.UpdateStockAsync(item.ShoppingItemId, item.Quantity);

        CartItems.Remove(item);
        OnPropertyChanged(nameof(CartTotal));
        await _page.DisplayAlert("Removed", "Item removed from cart", "OK");
    }

    public async Task LoadCartDataAsync()
    {
        if (_currentProfile != null)
        {
            try
            {
                var cartItems = await _dbService.GetCartItemsAsync(_currentProfile.Id);
                CartItems.Clear();
                foreach (var item in cartItems)
                {
                    CartItems.Add(item);
                }
                OnPropertyChanged(nameof(CartTotal));
                OnPropertyChanged(nameof(CartItems));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading cart data: {ex.Message}");
            }
        }
    }

    [RelayCommand]
    private void Increment(ShoppingItem item)
    {
        Console.WriteLine($"Incrementing item: {item.Name}, Current Quantity: {item.Quantity}");
        if (item.Quantity < item.StockQuantity)
        {
            item.Quantity++;
            Console.WriteLine($"New Quantity: {item.Quantity}");
            OnPropertyChanged(nameof(CartTotal));
        }
    }

    [RelayCommand]
    private void Decrement(ShoppingItem item)
    {
        Console.WriteLine($"Decrementing item: {item.Name}, Current Quantity: {item.Quantity}");
        if (item.Quantity > 1)
        {
            item.Quantity--;
            Console.WriteLine($"New Quantity: {item.Quantity}");
            OnPropertyChanged(nameof(CartTotal));
        }
    }

    public async Task ClearCart()
    {
        try
        {
            foreach (var item in CartItems)
            {
                await _dbService.RemoveCartItemAsync(item);
                await _dbService.UpdateStockAsync(item.ShoppingItemId, item.Quantity);
            }
            CartItems.Clear();
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(CartItems));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing cart: {ex.Message}");
        }
    }

    public ShoppingItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public decimal CartTotal => CartItems.Sum(i => i.Quantity * (i.ShoppingItem?.Price ?? 0m));
}