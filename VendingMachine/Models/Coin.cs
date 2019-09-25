using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VendingMachine.Models
{
    public class Coin
    {
        public int Id { get; set; }
        [Required]
        public float FaceValue { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsBlocked { get; set; } = false;
    }
}
