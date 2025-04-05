using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTM.Gamepad.Application.Services;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;

namespace UTM.Gamepad.Web.Controllers
{
    // Временно убираю для отладки
    // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserService _userService = new UserService();
        private readonly RoleService _roleService = new RoleService();
        private readonly ProductService _productService = new ProductService();
        private readonly OrderService _orderService = new OrderService();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/UserList
        public ActionResult UserList()
        {
            var users = _userService.GetAllUsers();
            return View(users);
        }
        
        // GET: Admin/EditUser/id
        public ActionResult EditUser(Guid id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            var roles = _roleService.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", user.RoleId);
            return View(user);
        }
        
        // POST: Admin/EditUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Guid id, string fullName, string email, Guid roleId)
        {
            if (_userService.UpdateUser(id, fullName, email))
            {
                _userService.ChangeUserRole(id, roleId);
                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("UserList");
            }
            
            ViewBag.ErrorMessage = "Error updating user";
            var user = _userService.GetUserById(id);
            var roles = _roleService.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
            return View(user);
        }
        
        // GET: Admin/ResetPassword/id
        public ActionResult ResetPassword(Guid id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            return View(user);
        }
        
        // POST: Admin/ResetPassword/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(Guid id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match";
                var user = _userService.GetUserById(id);
                return View(user);
            }
            
            if (_userService.ResetPassword(id, newPassword))
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
                return RedirectToAction("UserList");
            }
            
            ViewBag.ErrorMessage = "Error resetting password";
            var userModel = _userService.GetUserById(id);
            return View(userModel);
        }
        
        // POST: Admin/DeleteUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(Guid id)
        {
            if (_userService.DeleteUser(id))
            {
                TempData["SuccessMessage"] = "User deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user";
            }
            
            return RedirectToAction("UserList");
        }
        
        // GET: Admin/CreateUser
        public ActionResult CreateUser()
        {
            var roles = _roleService.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }
        
        // POST: Admin/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(string fullName, string email, string password, string confirmPassword, Guid roleId)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Please fill in all fields";
                var roles = _roleService.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match";
                var roles = _roleService.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }
            
            var role = _roleService.GetRoleById(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Invalid role selected";
                var roles = _roleService.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }

            var user = _userService.CreateUserWithRole(fullName, email, password, roleId);
            if (user != null)
            {
                TempData["SuccessMessage"] = "User created successfully";
                return RedirectToAction("UserList");
            }

            ViewBag.ErrorMessage = "Error creating user. Email may already be in use.";
            var allRoles = _roleService.GetAllRoles();
            ViewBag.Roles = new SelectList(allRoles, "Id", "Name", roleId);
            return View();
        }
        
        // GET: Admin/Products
        public ActionResult Products()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }
        
        // GET: Admin/CreateProduct
        public ActionResult CreateProduct()
        {
            return View();
        }
        
        // POST: Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Обработка загрузки изображения
                    var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(imageFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Content/images/products"), fileName);
                    
                    // Проверяем существование директории и создаем ее при необходимости
                    var directory = System.IO.Path.GetDirectoryName(path);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                    
                    imageFile.SaveAs(path);
                    product.ImageUrl = "/Content/images/products/" + fileName;
                }
                
                if (_productService.CreateProduct(product))
                {
                    TempData["SuccessMessage"] = "Product created successfully";
                    return RedirectToAction("Products");
                }
            }
            
            ViewBag.ErrorMessage = "Error creating product";
            return View(product);
        }
        
        // GET: Admin/EditProduct/id
        public ActionResult EditProduct(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }
        
        // POST: Admin/EditProduct/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Обработка загрузки нового изображения
                    var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(imageFile.FileName);
                    var path = System.IO.Path.Combine(Server.MapPath("~/Content/images/products"), fileName);
                    
                    // Проверяем существование директории и создаем ее при необходимости
                    var directory = System.IO.Path.GetDirectoryName(path);
                    if (!System.IO.Directory.Exists(directory))
                    {
                        System.IO.Directory.CreateDirectory(directory);
                    }
                    
                    imageFile.SaveAs(path);
                    product.ImageUrl = "/Content/images/products/" + fileName;
                }
                
                if (_productService.UpdateProduct(product))
                {
                    TempData["SuccessMessage"] = "Product updated successfully";
                    return RedirectToAction("Products");
                }
            }
            
            ViewBag.ErrorMessage = "Error updating product";
            return View(product);
        }
        
        // POST: Admin/DeleteProduct/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(Guid id)
        {
            if (_productService.DeleteProduct(id))
            {
                TempData["SuccessMessage"] = "Product deleted successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting product";
            }
            
            return RedirectToAction("Products");
        }
        
        // GET: Admin/Orders
        public ActionResult Orders()
        {
            var orders = _orderService.GetAllOrders();
            return View(orders);
        }
        
        // GET: Admin/OrderDetails/id
        public ActionResult OrderDetails(Guid id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            return View(order);
        }
        
        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateOrderStatus(Guid orderId, string status, string statusNote)
        {
            if (_orderService.UpdateOrderStatus(orderId, status, statusNote))
            {
                TempData["SuccessMessage"] = "Order status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error updating order status";
            }
            
            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        
        // GET: Admin/UpdateOrderStatus
        public ActionResult UpdateOrderStatus(Guid id, string status)
        {
            if (_orderService.UpdateOrderStatus(id, status))
            {
                TempData["SuccessMessage"] = "Order status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error updating order status";
            }
            
            return RedirectToAction("OrderDetails", new { id = id });
        }
        
        // GET: Admin/Statistics
        public ActionResult Statistics()
        {
            var model = new Dictionary<string, object>();
            
            // Получаем статистику заказов
            model["OrderStats"] = _orderService.GetOrderStatistics();
            
            // Получаем общую сумму продаж
            model["TotalSales"] = _orderService.GetTotalSales();
            
            // Получаем все продукты для анализа
            var products = _productService.GetAllProducts();
            
            // Топ категорий
            model["Categories"] = products
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToDictionary(x => x.Category, x => x.Count);
            
            // Товары с низким запасом (5 и меньше)
            model["LowStockProducts"] = products
                .Where(p => p.StockQuantity <= 5 && p.StockQuantity > 0)
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .ToList();
            
            // Товары с нулевым запасом
            model["OutOfStockProducts"] = products
                .Where(p => p.StockQuantity == 0)
                .ToList();
            
            return View(model);
        }
    }
} 