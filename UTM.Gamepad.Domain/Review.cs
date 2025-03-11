using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UTM.Gamepad.Domain
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
    }
}