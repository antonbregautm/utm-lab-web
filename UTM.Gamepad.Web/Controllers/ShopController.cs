using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTM.Gamepad.Application.Services;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService = new ProductService();
        private readonly OrderService _orderService = new OrderService();
        private readonly UserService _userService = new UserService();
        
        // GET: Catalog - отображение всех продуктов
        public ActionResult Catalog()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }
        
        // GET: Product Details
        public ActionResult ProductDetails(Guid id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }
        
        // GET: Cart - корзина пользователя
        [Authorize]
        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            return View(cart);
        }
        
        // POST: Add to Cart - добавление товара в корзину
        [HttpPost]
        [Authorize]
        public ActionResult AddToCart(Guid productId, int quantity = 1)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found";
                return RedirectToAction("Catalog");
            }
            
            // Получаем корзину из сессии или создаем новую
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            
            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                // Увеличиваем количество
                existingItem.Quantity += quantity;
            }
            else
            {
                // Добавляем новый товар в корзину
                cart.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity
                });
            }
            
            // Сохраняем корзину в сессии
            Session["Cart"] = cart;
            
            TempData["SuccessMessage"] = $"{product.Name} added to cart";
            return RedirectToAction("Cart");
        }
        
        // POST: Remove from Cart - удаление товара из корзины
        [HttpPost]
        [Authorize]
        public ActionResult RemoveFromCart(Guid itemId)
        {
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.Id == itemId);
            
            if (item != null)
            {
                cart.Remove(item);
                Session["Cart"] = cart;
                TempData["SuccessMessage"] = "Item removed from cart";
            }
            
            return RedirectToAction("Cart");
        }
        
        // POST: Update Cart Item - обновление количества товара в корзине
        [HttpPost]
        [Authorize]
        public ActionResult UpdateCartItem(Guid itemId, int quantity)
        {
            if (quantity <= 0)
            {
                return RedirectToAction("RemoveFromCart", new { itemId });
            }
            
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.Id == itemId);
            
            if (item != null)
            {
                item.Quantity = quantity;
                Session["Cart"] = cart;
                TempData["SuccessMessage"] = "Cart updated";
            }
            
            return RedirectToAction("Cart");
        }
        
        // GET: Checkout - оформление заказа
        [Authorize]
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }
            
            // Создаем модель для оформления заказа
            var order = new Order
            {
                OrderItems = cart,
                TotalAmount = cart.Sum(i => i.Price * i.Quantity)
            };
            
            return View(order);
        }
        
        // POST: Place Order - размещение заказа
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(Order orderDetails)
        {
            var cart = Session["Cart"] as List<OrderItem> ?? new List<OrderItem>();
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }
            
            try
            {
                // Получаем текущего пользователя
                var userEmail = User.Identity.Name;
                var user = _userService.GetUserByEmail(userEmail);
                
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found. Please login again.";
                    return RedirectToAction("SignIn", "UserAccount");
                }
                
                // Создаем заказ
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    UserName = user.FullName,
                    UserEmail = user.Email,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalAmount = cart.Sum(i => i.Price * i.Quantity),
                    OrderItems = cart.Select(item => new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductImageUrl = item.ProductImageUrl,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList(),
                    ShippingAddress = orderDetails.ShippingAddress,
                    City = orderDetails.City,
                    State = orderDetails.State,
                    ZipCode = orderDetails.ZipCode,
                    Country = orderDetails.Country,
                    PhoneNumber = orderDetails.PhoneNumber,
                    PaymentMethod = orderDetails.PaymentMethod
                };
                
                // Сохраняем заказ в базе данных
                if (_orderService.CreateOrder(order))
                {
                    // Очищаем корзину
                    Session["Cart"] = null;
                    
                    TempData["OrderSuccess"] = true;
                    TempData["OrderId"] = order.Id;
                    return RedirectToAction("OrderConfirmation");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error creating order";
                    return RedirectToAction("Checkout");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }
        
        // GET: Order Confirmation - подтверждение заказа
        [Authorize]
        public ActionResult OrderConfirmation()
        {
            if (TempData["OrderSuccess"] == null || !(bool)TempData["OrderSuccess"])
            {
                return RedirectToAction("Catalog");
            }
            
            var orderId = (Guid)TempData["OrderId"];
            var order = _orderService.GetOrderById(orderId);
            
            return View(order);
        }
        
        // GET: My Orders - список заказов пользователя
        [Authorize]
        public ActionResult MyOrders()
        {
            try
            {
                var userEmail = User.Identity.Name;
                var user = _userService.GetUserByEmail(userEmail);
                
                if (user == null)
                {
                    return RedirectToAction("SignIn", "UserAccount");
                }
                
                var orders = _orderService.GetOrdersByUserId(user.Id);
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading orders: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
        
        // GET: Order Details - детали заказа
        [Authorize]
        public ActionResult OrderDetails(Guid id)
        {
            try
            {
                var userEmail = User.Identity.Name;
                var user = _userService.GetUserByEmail(userEmail);
                
                if (user == null)
                {
                    return RedirectToAction("SignIn", "UserAccount");
                }
                
                var order = _orderService.GetOrderById(id);
                
                // Проверяем, принадлежит ли заказ пользователю или является ли пользователь админом/менеджером
                if (order == null || (order.UserId != user.Id && !User.IsInRole("Admin") && !User.IsInRole("Manager")))
                {
                    return HttpNotFound();
                }
                
                return View(order);
            }
            catch
            {
                return HttpNotFound();
            }
        }
        
        // GET: Cancel Order - отмена заказа пользователем
        [Authorize]
        public ActionResult CancelOrder(Guid id)
        {
            try
            {
                var userEmail = User.Identity.Name;
                var user = _userService.GetUserByEmail(userEmail);
                
                if (user == null)
                {
                    return RedirectToAction("SignIn", "UserAccount");
                }
                
                var order = _orderService.GetOrderById(id);
                
                // Проверяем, принадлежит ли заказ пользователю и находится ли он в статусе, который можно отменить
                if (order == null || order.UserId != user.Id || 
                    !(order.Status == "Pending" || order.Status == "Processing"))
                {
                    TempData["ErrorMessage"] = "Cannot cancel this order";
                    return RedirectToAction("MyOrders");
                }
                
                if (_orderService.CancelOrder(id))
                {
                    TempData["SuccessMessage"] = "Order cancelled successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error cancelling order";
                }
                
                return RedirectToAction("MyOrders");
            }
            catch
            {
                TempData["ErrorMessage"] = "Error processing your request";
                return RedirectToAction("MyOrders");
            }
        }
    }
} 