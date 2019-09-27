using System.Collections.Generic;

namespace VendingMachine.Models
{
    public class AdminModel
    {
        public Product NewProduct { get; set; }
        public List<Cash> Cashes { get; set; }
        public List<Product> Products { get; set; }

        public AdminModel(List<Product> products, List<Cash> cashes)
        {
            Products = products;
            Cashes = cashes;
        }

        public AdminModel() {}
    }
}
