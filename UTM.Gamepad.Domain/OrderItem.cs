using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Gamepad.Domain
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Required]
        public string ProductName { get; set; }
        
        public string ProductImageUrl { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}