using System.Web;
using System.Web.Optimization;

namespace UTM.Gamepad.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Бандл для jQuery (загружается первым, так как остальные библиотеки зависят от него)
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/js/jquery.min.js",                     // Основной jQuery
                "~/Content/js/jquery-3.0.0.min.js",               // Альтернативная версия jQuery
                "~/Content/js/jquery.validate.js",               // Валидация форм
                "~/Content/js/jquery.mCustomScrollbar.concat.min.js")); // Кастомный скроллбар

            // Бандл для Bootstrap и Popper.js (загружается после jQuery)
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/js/popper.min.js",       // Popper.js (необходим для Bootstrap)
                "~/Content/js/bootstrap.bundle.js")); // Основной файл Bootstrap

            // Бандл для различных плагинов (загружаются последними)
            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                "~/Content/js/plugin.js",            // Дополнительные плагины
                "~/Content/js/owl.carousel.js",      // Карусель для слайдеров
                "~/Content/js/custom.js",            // Кастомные скрипты
                "~/Content/js/modernizer.js",        // Modernizer для поддержки старых браузеров
                "~/Content/js/slider-setting.js"));  // Настройки слайдера

            // Бандл для CSS (все стили объединены в один файл)
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/bootstrap.css",                    // Основной CSS Bootstrap
                "~/Content/css/responsive.css",                   // CSS для адаптивности
                "~/Content/css/jquery.mCustomScrollbar.min.css",  // Стили для кастомного скролла
                "~/Content/css/owl.carousel.min.css",             // Стили для карусели
                "~/Content/css/owl.theme.default.min.css",        // Тема для карусели
                "~/Content/css/jquery.fancybox.min.css",          // Fancybox (галерея изображений)
                "~/Content/css/font-awesome.min.css",             // Иконки FontAwesome
                "~/Content/css/animate.min.css",                  // Анимации
                "~/Content/css/default-skin.css",                 // Основная тема оформления
                "~/Content/css/icomoon.css",                      // Дополнительные иконки
                "~/Content/css/jquery-ui.css",                    // jQuery UI (например, для календаря)
                "~/Content/css/meanmenu.css",                     // Меню для мобильных устройств
                "~/Content/css/nice-select.css",                  // Красивые выпадающие списки
                "~/Content/css/normalize.css",                    // Базовые стили для кросс-браузерной совместимости
                "~/Content/css/slick.css"));                      // Слайдер Slick

            // Включаем минификацию и объединение файлов (ускоряет загрузку сайта)
            BundleTable.EnableOptimizations = true;
        }
    }
}
