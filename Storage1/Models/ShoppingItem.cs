using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage1.Models
{
    public partial class ShoppingItem : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }  // Add this for discount calculation
        public int StockQuantity { get; set; }
        public string ImageUrl { get; set; }  // Add this for product images

        [ObservableProperty]
        private int _quantity = 1;
        public int DiscountPercentage => OriginalPrice > 0 ?
            (int)((1 - Price / OriginalPrice) * 100) : 0;
    }
}
