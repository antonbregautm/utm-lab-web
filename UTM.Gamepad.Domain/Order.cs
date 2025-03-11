using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Gamepad.Domain
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}