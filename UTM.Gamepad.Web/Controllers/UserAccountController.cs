using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UTM.Gamepad.Application.Services;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Web.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly AuthService _authService = new AuthService();
        private readonly OrderService _orderService = new OrderService();

        // GET: Login page
        public ActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string email, string password, string returnUrl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Введите email и пароль";
                return View();
            }

            var user = _authService.AuthenticateUser(email, password);
            if (user != null)
            {
                AuthenticateAndRedirect(user);
                
                // Сохраняем TempData чтобы оно было доступно после редиректа
                TempData.Keep("UserRole");
                
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            
            ViewBag.ErrorMessage = "Неверный email или пароль";
            return View();
        }

        // GET: Register page
        public ActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Register process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(string fullName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Заполните все поля";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Пароли не совпадают";
                return View();
            }

            var user = _authService.CreateAccount(fullName, email, password);
            if (user != null)
            {
                AuthenticateAndRedirect(user);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Пользователь с таким email уже существует";
            return View();
        }

        // GET: UserAccount/Profile
        [Authorize]
        public ActionResult Profile()
        {
            var email = User.Identity.Name;
            var user = _authService.GetUserByEmail(email);
            
            if (user == null)
            {
                return RedirectToAction("SignIn");
            }
            
            // Получаем заказы пользователя
            var orders = _orderService.GetOrdersByUserId(user.Id);
            
            // Устанавливаем простые свойства пользователя напрямую в ViewBag
            ViewBag.UserFullName = user.FullName;
            ViewBag.UserEmail = user.Email;
            ViewBag.UserRole = user.Role?.Name ?? "Пользователь";
            
            return View(orders);
        }

        // Log out current user
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            HttpContext.Response.Cookies.Add(authCookie);

            return RedirectToAction("Index", "Home");
        }

        // Helper method to handle user authentication
        private void AuthenticateAndRedirect(User user)
        {
            string roleName = user.Role?.Name ?? "User";
            
            var ticket = new FormsAuthenticationTicket(
                1, // ticket version
                user.Email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30), // expiration
                true, // persistent cookie
                roleName, // user data
                "/");

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Response.Cookies.Add(cookie);
            
            // Устанавливаем TempData для отображения роли в интерфейсе
            TempData["UserRole"] = roleName;
        }
    }
}