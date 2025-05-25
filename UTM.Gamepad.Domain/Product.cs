using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Gamepad.Domain
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a positive number")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        public string ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}