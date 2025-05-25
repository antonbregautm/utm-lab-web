using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UTM.Gamepad.BussinessLogic;
using UTM.Gamepad.BussinessLogic.BLogic;

namespace UTM.Gamepad.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            try
            {
                // Инициализация стандартных ролей
                var roleBL = new RoleBL();
                // здесь можно вызвать метод инициализации ролей если нужно
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                System.Diagnostics.Debug.WriteLine($"Ошибка в Application_Start: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}