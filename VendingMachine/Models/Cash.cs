using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Models
{
    public class Cash
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Face Value")]
        public float FaceValue { get; set; }

        public int Quantity { get; set; } = 0;
        [DisplayName("Blocked")]
        public bool IsBlocked { get; set; } = false;
    }
}
