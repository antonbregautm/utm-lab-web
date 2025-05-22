using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UTM.Gamepad.BussinessLogic;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Web.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductBL _productBL;
        private readonly IOrderBL _orderBL;
        private readonly IUserBL _userBL;
        
        public ShopController()
        {
            var factory = BusinessLogicFactory.Instance;
            _productBL = factory.GetProductBL();
            _orderBL = factory.GetOrderBL();
            _userBL = factory.GetUserBL();
        }
        
        // GET: Catalog - отображение всех продуктов
        public ActionResult Catalog()
        {
            var products = _productBL.GetAllProducts();
            return View(products);
        }
        
        // GET: Product Details
        public ActionResult ProductDetails(Guid id)
        {
            var product = _productBL.GetProductById(id);
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
            var product = _productBL.GetProductById(productId);
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
                var user = _userBL.GetUserByEmail(userEmail);
                
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
                
                // Сохраняем заказ
                if (_orderBL.CreateOrder(order))
                {
                    // Очищаем корзину
                    Session["Cart"] = null;
                    
                    // Перенаправляем на страницу подтверждения заказа
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
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("Checkout");
            }
        }
        
        // GET: Order Confirmation - подтверждение заказа
        [Authorize]
        public ActionResult OrderConfirmation()
        {
            var orderId = TempData["OrderId"] as Guid?;
            if (!orderId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            
            var order = _orderBL.GetOrderById(orderId.Value);
            return View(order);
        }
        
        // GET: My Orders - просмотр заказов пользователя
        [Authorize]
        public ActionResult MyOrders()
        {
            var userEmail = User.Identity.Name;
            var user = _userBL.GetUserByEmail(userEmail);
            
            if (user == null)
            {
                return RedirectToAction("SignIn", "UserAccount");
            }
            
            var orders = _orderBL.GetOrdersByUserId(user.Id);
            return View(orders);
        }
        
        // GET: Order Details - детальная информация о заказе
        [Authorize]
        public ActionResult OrderDetails(Guid id)
        {
            var order = _orderBL.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            // Проверяем, что заказ принадлежит текущему пользователю
            var userEmail = User.Identity.Name;
            var user = _userBL.GetUserByEmail(userEmail);
            
            if (user == null || order.UserId != user.Id)
            {
                return RedirectToAction("MyOrders");
            }
            
            return View(order);
        }
        
        // POST: Cancel Order - отмена заказа
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(Guid id)
        {
            var order = _orderBL.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            // Проверяем, что заказ принадлежит текущему пользователю
            var userEmail = User.Identity.Name;
            var user = _userBL.GetUserByEmail(userEmail);
            
            if (user == null || order.UserId != user.Id)
            {
                return RedirectToAction("MyOrders");
            }
            
            // Проверяем, что заказ можно отменить (только в статусе "Pending")
            if (order.Status != "Pending")
            {
                TempData["ErrorMessage"] = "Only pending orders can be cancelled";
                return RedirectToAction("OrderDetails", new { id });
            }
            
            if (_orderBL.CancelOrder(id))
            {
                TempData["SuccessMessage"] = "Order cancelled successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Error cancelling order";
            }
            
            return RedirectToAction("MyOrders");
        }
    }
} 