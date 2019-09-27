using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingMachine.Models
{
    public class VendingMachineModel
    {
        public List<Cash> Cashes { get; set; }
        public List<Product> Products { get; set; }
    }
}
