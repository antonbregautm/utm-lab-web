using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTM.Gamepad.BussinessLogic;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;

namespace UTM.Gamepad.Web.Controllers
{
    // Временно убираю для отладки
    // [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserBL _userBL;
        private readonly IRoleBL _roleBL;
        private readonly IProductBL _productBL;
        private readonly IOrderBL _orderBL;
        
        public AdminController()
        {
            var factory = BusinessLogicFactory.Instance;
            _userBL = factory.GetUserBL();
            _roleBL = factory.GetRoleBL();
            _productBL = factory.GetProductBL();
            _orderBL = factory.GetOrderBL();
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/UserList
        public ActionResult UserList()
        {
            var users = _userBL.GetAllUsers();
            return View(users);
        }
        
        // GET: Admin/EditUser/id
        public ActionResult EditUser(Guid id)
        {
            var user = _userBL.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            var roles = _roleBL.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", user.RoleId);
            return View(user);
        }
        
        // POST: Admin/EditUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(Guid id, string fullName, string email, Guid roleId)
        {
            if (_userBL.UpdateUser(id, fullName, email))
            {
                _userBL.ChangeUserRole(id, roleId);
                TempData["SuccessMessage"] = "User updated successfully";
                return RedirectToAction("UserList");
            }
            
            ViewBag.ErrorMessage = "Error updating user";
            var user = _userBL.GetUserById(id);
            var roles = _roleBL.GetAllRoles();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
            return View(user);
        }
        
        // GET: Admin/ResetPassword/id
        public ActionResult ResetPassword(Guid id)
        {
            var user = _userBL.GetUserById(id);
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
                var user = _userBL.GetUserById(id);
                return View(user);
            }
            
            if (_userBL.ResetPassword(id, newPassword))
            {
                TempData["SuccessMessage"] = "Password has been reset successfully";
                return RedirectToAction("UserList");
            }
            
            ViewBag.ErrorMessage = "Error resetting password";
            var userModel = _userBL.GetUserById(id);
            return View(userModel);
        }
        
        // POST: Admin/DeleteUser/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(Guid id)
        {
            if (_userBL.DeleteUser(id))
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
            var roles = _roleBL.GetAllRoles();
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
                var roles = _roleBL.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match";
                var roles = _roleBL.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }
            
            var role = _roleBL.GetRoleById(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = "Invalid role selected";
                var roles = _roleBL.GetAllRoles();
                ViewBag.Roles = new SelectList(roles, "Id", "Name", roleId);
                return View();
            }

            var user = _userBL.CreateUserWithRole(fullName, email, password, roleId);
            if (user != null)
            {
                TempData["SuccessMessage"] = "User created successfully";
                return RedirectToAction("UserList");
            }

            ViewBag.ErrorMessage = "Error creating user. Email may already be in use.";
            var allRoles = _roleBL.GetAllRoles();
            ViewBag.Roles = new SelectList(allRoles, "Id", "Name", roleId);
            return View();
        }
        
        // GET: Admin/Products
        public ActionResult Products()
        {
            var products = _productBL.GetAllProducts();
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
                
                if (_productBL.CreateProduct(product))
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
            var product = _productBL.GetProductById(id);
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
                
                if (_productBL.UpdateProduct(product))
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
            if (_productBL.DeleteProduct(id))
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
            var orders = _orderBL.GetAllOrders();
            return View(orders);
        }
        
        // GET: Admin/OrderDetails/id
        public ActionResult OrderDetails(Guid id)
        {
            var order = _orderBL.GetOrderById(id);
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
            if (_orderBL.UpdateOrderStatus(orderId, status, statusNote))
            {
                TempData["SuccessMessage"] = "Order status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error updating order status";
            }
            
            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        
        // GET: Admin/UpdateOrderStatus (для быстрого обновления статуса)
        public ActionResult UpdateOrderStatus(Guid id, string status)
        {
            if (_orderBL.UpdateOrderStatus(id, status))
            {
                TempData["SuccessMessage"] = "Order status updated successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error updating order status";
            }
            
            return RedirectToAction("Orders");
        }
        
        // GET: Admin/Statistics
        public ActionResult Statistics()
        {
            var statistics = _orderBL.GetOrderStatistics();
            ViewBag.TotalSales = _orderBL.GetTotalSales();
            
            return View(statistics);
        }
    }
} 