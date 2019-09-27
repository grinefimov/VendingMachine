using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [DisplayName("Image")]
        public string ImageUrl { get; set; }
    }
}
