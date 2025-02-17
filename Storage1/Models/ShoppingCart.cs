using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage1.Models
{
    public class ShoppingCart
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ProfileId { get; set; }  // Foreign key to Profile
        public int ShoppingItemId { get; set; }  // Foreign key to ShoppingItem
        public int Quantity { get; set; }
        [Ignore]
        public ShoppingItem ShoppingItem { get; set; }
    }
}
