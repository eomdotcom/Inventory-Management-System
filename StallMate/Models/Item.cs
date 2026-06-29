using System.ComponentModel.DataAnnotations;

namespace StallMate.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }

        public decimal? SalePrice { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsSold { get; set; } = false;

        public string UserId { get; set; } = "";

        public string? PhotoPath { get; set; }
    }
}