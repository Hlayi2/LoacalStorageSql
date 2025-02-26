using Storage1.Models;
using System;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage1.Services
{
    public class DatabaseService
    {
        private const string DB_NAME = "banking_db";
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            Initialize();
        }

        private void Initialize()
        {
            Task.Run(async () => await InitializeDatabaseAsync()).Wait();
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                Console.WriteLine("Starting database initialization");

                // Create tables if they don't exist
                await _connection.CreateTableAsync<Profile>(CreateFlags.None);
                await _connection.CreateTableAsync<ShoppingItem>(CreateFlags.None);
                await _connection.CreateTableAsync<ShoppingCart>(CreateFlags.None);

                Console.WriteLine("Tables created successfully");

                await SeedInitialDataIfEmpty();
                Console.WriteLine("Data seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        private async Task SeedInitialDataIfEmpty()
        {
            // Seed Profile
            if (await _connection.Table<Profile>().CountAsync() == 0)
            {
                var defaultProfile = new Profile
                {
                    Name = "Katlego",
                    Surname = "Lekhuleni",
                    Email = "katlego@example.com",
                    Bio = "Sample bio"
                };
                await _connection.InsertAsync(defaultProfile);
            }

            // Seed Shopping Items
            if (await _connection.Table<ShoppingItem>().CountAsync() == 0)
            {
                var items = new List<ShoppingItem>
                {
                    new() {
                        Name = "Cauliflower",
                        Description = "Fresh organic violet cauliflower",
                        Price = 25.00m,
                        OriginalPrice = 35.00m,
                        StockQuantity = 50,
                        ImageUrl = "caulifower.png"
                    },
                    new() {
                        Name = "Organic Lemon",
                        Description = "Fresh organic lemons",
                        Price = 20.00m,
                        OriginalPrice = 25.00m,
                        StockQuantity = 30,
                        ImageUrl = "lemons.png"
                    },
                    new() {
                        Name = "Aloe",
                        Description = "Fresh Aloe",
                        Price = 10.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 5,
                        ImageUrl = "aloe.png"
                    },
                    new() {
                        Name = "Avocado",
                        Description = "Fresh Avocados",
                        Price = 40.00m,
                        OriginalPrice = 35.00m,
                        StockQuantity = 30,
                        ImageUrl = "Avocado.png"
                    },
                    new() {
                        Name = "Banana",
                        Description = "Fresh organic Bananas",
                        Price = 10.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 10,
                        ImageUrl = "banana.png"
                    },
                    new() {
                        Name = "Purple Cabbage",
                        Description = "Fresh Cabbage",
                        Price = 20.00m,
                        OriginalPrice = 25.00m,
                        StockQuantity = 3,
                        ImageUrl = "cabbage.png"
                    },
                    new() {
                        Name = "Chillie Papper",
                        Description = "Fresh Chillie Papers",
                        Price = 10.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 15,
                        ImageUrl = "chillie_paper.png"
                    },
                    new() {
                        Name = "Organic Grapes",
                        Description = "Fresh Seedless Grapes",
                        Price = 30.00m,
                        OriginalPrice = 48.00m,
                        StockQuantity = 30,
                        ImageUrl = "grapes.png"
                    },
                    new() {
                        Name = "Apples",
                        Description = "Fresh Green Apples",
                        Price = 10.00m,
                        OriginalPrice = 10.00m,
                        StockQuantity = 10,
                        ImageUrl = "green_apples.png"
                    },
                    new() {
                        Name = "Organic onions",
                        Description = "Fresh organic Onions",
                        Price = 20.00m,
                        OriginalPrice = 20.00m,
                        StockQuantity = 17,
                        ImageUrl = "onions.png"
                    },
                    new() {
                        Name = "Oranges",
                        Description = "Fresh Citrus Oranges",
                        Price = 15.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 10,
                        ImageUrl = "oranges.png"
                    },
                    new() {
                        Name = "Pemogranate",
                        Description = "Fresh organic Pemogranate",
                        Price = 30.00m,
                        OriginalPrice = 30.00m,
                        StockQuantity = 7,
                        ImageUrl = "pemogranate.png"
                    },
                    new() {
                        Name = "Potatoes",
                        Description = "Fresh organic Potatoes",
                        Price = 10.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 9,
                        ImageUrl = "potatoes.png"
                    },
                    new() {
                        Name = "Red Apples",
                        Description = "Fresh Red Apples",
                        Price = 40.00m,
                        OriginalPrice = 50.00m,
                        StockQuantity = 8,
                        ImageUrl = "red_apples.png"
                    },
                    new() {
                        Name = "Shushi Leaves",
                        Description = "Fresh Shushi Leaves",
                        Price = 10.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity = 10,
                        ImageUrl = "shushi_leaves.png"
                    },
                    new() {
                        Name = "Strawberries",
                        Description = "Fresh organic Strawberries",
                        Price = 29.00m,
                        OriginalPrice = 35.00m,
                        StockQuantity = 10,
                        ImageUrl = "strawberries.png"
                    },
                    new() {
                        Name = "Tomatoes",
                        Description = "Fresh organic Tomatoes",
                        Price= 15.00m,
                        OriginalPrice = 15.00m,
                        StockQuantity =15,
                        ImageUrl = "tomatoes.png"
                    },
                    new() {
                        Name = "Yellow Pappers",
                        Description = "Fresh organic Yellow Pappers",
                        Price = 20.00m,
                        OriginalPrice = 25.00m,
                        StockQuantity = 5,
                        ImageUrl = "yellow_papers.png"
                    },
                };
                await _connection.InsertAllAsync(items);
            }
        }

        public async Task UpdateProfileAsync(Profile profile)
        {
            await _connection.UpdateAsync(profile);
        }

        public async Task UpdateCartItemAsync(ShoppingCart cartItem)
        {
            await _connection.UpdateAsync(cartItem);
        }

        #region Profile Operations
        public async Task<Profile> GetProfileAsync(int id = 1)
        {
            return await _connection.Table<Profile>()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        #endregion

        #region Shopping Item Operations
        public async Task<List<ShoppingItem>> GetAllShoppingItemsAsync()
        {
            Console.WriteLine("Attempting to get shopping items");
            return await _connection.Table<ShoppingItem>().ToListAsync();
        }

        public async Task<ShoppingItem> GetShoppingItemAsync(int id)
        {
            return await _connection.Table<ShoppingItem>()
                .FirstOrDefaultAsync(i => i.Id == id);
        }
        #endregion

        #region Shopping Cart Operations
        public async Task AddToCartAsync(ShoppingCart item)
        {
            Console.WriteLine($"Adding to cart - ItemId: {item.ShoppingItemId}, Quantity: {item.Quantity}");

            var existing = await _connection.Table<ShoppingCart>()
                .FirstOrDefaultAsync(c => c.ProfileId == item.ProfileId &&
                                         c.ShoppingItemId == item.ShoppingItemId);

            if (existing != null)
            {
                Console.WriteLine($"Updating existing cart item. Old quantity: {existing.Quantity}, New quantity: {existing.Quantity + item.Quantity}");
                existing.Quantity += item.Quantity;
                await _connection.UpdateAsync(existing);
            }
            else
            {
                Console.WriteLine($"Inserting new cart item");
                await _connection.InsertAsync(item);
            }

            // Verify cart state after operation
            var cartCount = await _connection.Table<ShoppingCart>().CountAsync();
            Console.WriteLine($"Total cart items after operation: {cartCount}");
        }

        public async Task<ObservableCollection<ShoppingCart>> GetCartItemsAsync(int profileId)
        {
            Console.WriteLine($"Getting cart items for profile: {profileId}");
            var cartItems = await _connection.Table<ShoppingCart>()
                .Where(c => c.ProfileId == profileId)
                .ToListAsync();

            Console.WriteLine($"Found {cartItems.Count} cart items");

            foreach (var item in cartItems)
            {
                item.ShoppingItem = await _connection.Table<ShoppingItem>()
                    .FirstOrDefaultAsync(i => i.Id == item.ShoppingItemId);

                if (item.ShoppingItem != null)
                    Console.WriteLine($"Loaded item details: {item.ShoppingItem.Name}, Quantity: {item.Quantity}");
                else
                    Console.WriteLine($"Failed to load item details for ID: {item.ShoppingItemId}");
            }

            return new ObservableCollection<ShoppingCart>(cartItems);
        }

        public async Task RemoveCartItemAsync(ShoppingCart item)
        {
            await _connection.DeleteAsync(item);
        }
        #endregion

        #region Stock Management
        public async Task<bool> ValidateStockAsync(int itemId, int requestedQuantity)
        {
            var item = await GetShoppingItemAsync(itemId);
            return item != null && item.StockQuantity >= requestedQuantity;
        }

        public async Task UpdateStockAsync(int itemId, int quantityChange)
        {
            var item = await GetShoppingItemAsync(itemId);
            if (item != null)
            {
                item.StockQuantity += quantityChange;
                await _connection.UpdateAsync(item);
            }
        }
        #endregion
    }
}