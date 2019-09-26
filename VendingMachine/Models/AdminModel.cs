using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VendingMachine.Models
{
    public class AdminModel
    {
        public List<VendingMachine.Models.Product> Products { get; set; }

        public AdminModel(List<VendingMachine.Models.Product> products)
        {
            Products = products;
        }

        public AdminModel() {}
    }
}
