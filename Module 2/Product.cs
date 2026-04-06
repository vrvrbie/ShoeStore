
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreApp.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string CategoryName { get; set; }
        public decimal DiscountPercent { get; set; }
        public int QuantityInStock { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public decimal FinalPrice => Price - (Price * DiscountPercent / 100);
        public bool IsHighDiscount => DiscountPercent > 15;
        public bool IsInStock => QuantityInStock > 0;
    }
}
