namespace UTM.Gamepad.Domain.DTOs
{
    public class UserProfileDto
    {
        /// <summary>
        /// ID пользователя
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// Email пользователя
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Название роли пользователя
        /// </summary>
        public string RoleName { get; set; }
    }
} 