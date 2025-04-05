using System.Web.Mvc; // Подключаем библиотеку для работы с веб-страницами (MVC)
using System.Linq;
using UTM.Gamepad.Application.Services;

namespace UTM.Gamepad.Web.Controllers // Пространство имён (где находится этот код)
{
    public class HomeController : Controller // Этот класс управляет страницами сайта
    {
        private readonly UserService _userService = new UserService();
        private readonly RoleService _roleService = new RoleService();
        
        // Метод (экшен) для главной страницы (Главная / Home)
        public ActionResult Index()
        {
            return View(); // Открывает страницу "Index.cshtml" (главную)
        }

        // Метод для страницы "О нас"
        public ActionResult About()
        {
            ViewBag.Title = "Your application description page."; // Передаём заголовок страницы
            return View(); // Открывает страницу "About.cshtml"
        }

        // Метод для страницы "Продукты"
        public ActionResult Product()
        {
            ViewBag.Title = "Our products page."; // Заголовок страницы
            return View(); // Открывает страницу "Product.cshtml"
        }

        // Метод для страницы "Видеоигры"
        public ActionResult Video()
        {
            ViewBag.Title = "Video games section."; // Заголовок страницы
            return View(); // Открывает страницу "Video.cshtml"
        }

        // Метод для страницы "Пульты управления"
        public ActionResult Remote()
        {
            ViewBag.Title = "Remote control section."; // Заголовок страницы
            return View(); // Открывает страницу "Remote.cshtml"
        }

        // Метод для страницы "Контакты"
        public ActionResult Contact()
        {
            ViewBag.Title = "Your contact page."; // Заголовок страницы
            return View(); // Открывает страницу "Contact.cshtml"
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminPanel()
        {
            return View();
        }

        // Метод для перенаправления на страницу входа
        public ActionResult Login()
        {
            return RedirectToAction("SignIn", "UserAccount");
        }
        
        // Метод для перенаправления на страницу регистрации
        public ActionResult Register()
        {
            return RedirectToAction("SignUp", "UserAccount");
        }
    }
}