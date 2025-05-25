using System;
using System.Web;

namespace UTM.Gamepad.Domain.DTOs
{
    public class AuthResultDto
    {
        /// <summary>
        /// Результат операции аутентификации
        /// </summary>
        public bool IsSuccess { get; set; }
        
        /// <summary>
        /// Сообщение об ошибке (если IsSuccess = false)
        /// </summary>
        public string ErrorMessage { get; set; }
        
        /// <summary>
        /// ID пользователя (если IsSuccess = true)
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Email пользователя (если IsSuccess = true)
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Роль пользователя (если IsSuccess = true)
        /// </summary>
        public string UserRole { get; set; }
        
        /// <summary>
        /// Cookie для аутентификации (если IsSuccess = true)
        /// </summary>
        public HttpCookie AuthCookie { get; set; }
    }
} 