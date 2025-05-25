using System.Collections.Generic;

namespace UTM.Gamepad.Domain.DTOs
{
    public class CartResultDto
    {
        /// <summary>
        /// Результат операции с корзиной
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Сообщение операции
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Обновленная корзина
        /// </summary>
        public List<OrderItem> UpdatedCart { get; set; }
        
        /// <summary>
        /// Общая сумма корзины
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
} 