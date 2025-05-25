using System;
using System.Collections.Generic;

namespace UTM.Gamepad.Domain.DTOs
{
    public class CartActionDto
    {
        /// <summary>
        /// ID продукта для добавления в корзину
        /// </summary>
        public Guid ProductId { get; set; }
        
        /// <summary>
        /// Количество продукта
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// Текущее содержимое корзины
        /// </summary>
        public List<OrderItem> CurrentCart { get; set; }
    }
} 