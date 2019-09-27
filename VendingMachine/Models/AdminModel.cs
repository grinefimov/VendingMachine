using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VendingMachine.Models
{
    public class AdminModel
    {
        public Product NewProduct { get; set; }
        public List<Product> Products { get; set; }

        public AdminModel(List<Product> products)
        {
            Products = products;
        }

        public AdminModel() {}
    }
}
