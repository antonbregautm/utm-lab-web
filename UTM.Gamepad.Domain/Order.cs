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
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        public string UserEmail { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        [Required]
        public string Status { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public string ShippingAddress { get; set; }
        
        public string City { get; set; }
        
        public string State { get; set; }
        
        public string ZipCode { get; set; }
        
        public string Country { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string PaymentMethod { get; set; }
        
        public string Notes { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}