using System;
using System.ComponentModel.DataAnnotations;

namespace Cw5.Models
{
    public class Warehouse
    {
        [Required(ErrorMessage = "Warehouse Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Warehouse Id must be higher or equal 0")]
        public int IdWarehouse { get; set; }
        [Required(ErrorMessage = "Product Id is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product Id must be higher or equal 0")]
        public int IdProduct { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be higher or equal 1")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Creation date is required")]
        public DateTime CreatedAt { get; set; }
    }
}