using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VendingMachine.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public float Price { get; set; }
        [Required] public int Quantity { get; set; }
        [Required]
        [DisplayName("Image URL")]
        public string ImageUrl { get; set; }
    }
}
