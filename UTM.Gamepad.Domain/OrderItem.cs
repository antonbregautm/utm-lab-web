using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Gamepad.Domain
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }
}