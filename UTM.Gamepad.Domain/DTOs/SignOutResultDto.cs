using System.Web;

namespace UTM.Gamepad.Domain.DTOs
{
    public class SignOutResultDto
    {
        /// <summary>
        /// Cookie для удаления аутентификации
        /// </summary>
        public HttpCookie AuthCookie { get; set; }
    }
} 