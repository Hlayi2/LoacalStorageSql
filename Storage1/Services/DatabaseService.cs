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
                // Drop existing tables to refresh data
                await _connection.DropTableAsync<Profile>();
                await _connection.DropTableAsync<ShoppingItem>();
                await _connection.DropTableAsync<ShoppingCart>();
               

                await _connection.CreateTableAsync<Profile>();
                await _connection.CreateTableAsync<ShoppingItem>();
                await _connection.CreateTableAsync<ShoppingCart>();
                Console.WriteLine("Tables created successfully");

                await SeedInitialData();
                Console.WriteLine("Data seeded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        public async Task UpdateCartItemAsync(ShoppingCart cartItem)
        {
          
            await _connection.UpdateAsync(cartItem);
        }
        private async Task SeedInitialData()
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
                Description = "Fresh organic violet cauliflower",
                Price = 10.00m,
                OriginalPrice = 15.00m,
                StockQuantity = 5,
                ImageUrl = "aloe.png"
            },
            new() {
                Name = "Avocado",
                Description = "Fresh organic lemons",
                Price = 40.00m,
                OriginalPrice = 35.00m,
                StockQuantity = 30,
                ImageUrl = "Avocado.png"
            },
             new() {
                Name = "Banana",
                Description = "Fresh organic violet cauliflower",
                Price = 10.00m,
                OriginalPrice = 15.00m,
                StockQuantity = 10,
                ImageUrl = "banana.png"
            },
            new() {
                Name = "Purple Cabbage",
                Description = "Fresh organic lemons",
                Price = 20.00m,
                OriginalPrice = 25.00m,
                StockQuantity = 3,
                ImageUrl = "cabbage.png"
            },
             new() {
                Name = "Chillie Papper",
                Description = "Fresh organic violet cauliflower",
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

            }, new() {
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

        #region Profile Operations
        public async Task<Profile> GetProfileAsync(int id = 1)
        {
            return await _connection.Table<Profile>()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateProfileAsync(Profile profile)
        {
            await _connection.UpdateAsync(profile);
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
            var existing = await _connection.Table<ShoppingCart>()
                .FirstOrDefaultAsync(c => c.ProfileId == item.ProfileId &&
                                         c.ShoppingItemId == item.ShoppingItemId);

            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                await _connection.UpdateAsync(existing);
            }
            else
            {
                await _connection.InsertAsync(item);
            }
        }

        public async Task<ObservableCollection<ShoppingCart>> GetCartItemsAsync(int profileId)
        {
            var cartItems = await _connection.Table<ShoppingCart>()
                .Where(c => c.ProfileId == profileId)
                .ToListAsync();

            foreach (var item in cartItems)
            {
                item.ShoppingItem = await _connection.Table<ShoppingItem>()
                    .FirstOrDefaultAsync(i => i.Id == item.ShoppingItemId);
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
    

