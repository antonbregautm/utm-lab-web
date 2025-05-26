using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class OrderApi
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly OrderRepository _orderRepository;
        private readonly ProductRepository _productRepository;
        
        public OrderApi()
        {
            _dbContext = new ApplicationDbContext();
            _orderRepository = new OrderRepository();
            _productRepository = new ProductRepository();
        }
        
        protected List<Order> GetAllOrdersFromDb()
        {
            return _orderRepository.GetAll().ToList();
        }
        
        protected Order GetOrderByIdFromDb(Guid id)
        {
            return _orderRepository.GetById(id);
        }
        
        protected List<Order> GetOrdersByUserIdFromDb(Guid userId)
        {
            return _orderRepository.GetAll().Where(o => o.UserId == userId).ToList();
        }
        
        protected bool SaveOrderToDb(Order order)
        {
            try
            {
                _orderRepository.Add(order);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool UpdateOrderInDb(Order order)
        {
            try
            {
                _orderRepository.Update(order);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected Product GetProductById(Guid productId)
        {
            return _productRepository.GetById(productId);
        }
        
        protected bool CancelOrderInDb(Guid orderId, string note = "Отменен пользователем")
        {
            try
            {
                var order = GetOrderByIdFromDb(orderId);
                if (order == null)
                {
                    return false;
                }
                
                order.Status = "Cancelled";
                return UpdateOrderInDb(order);
            }
            catch
            {
                return false;
            }
        }
        
        protected Dictionary<string, object> GetOrderStatisticsFromDb()
        {
            var orders = GetAllOrdersFromDb();
            
            return new Dictionary<string, object>
            {
                { "TotalOrders", orders.Count },
                { "CompletedOrders", orders.Count(o => o.Status == "Completed") },
                { "PendingOrders", orders.Count(o => o.Status == "Pending") },
                { "CancelledOrders", orders.Count(o => o.Status == "Cancelled") },
                { "TotalSales", orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount) }
            };
        }
        
        protected decimal GetTotalSalesFromDb()
        {
            var orders = GetAllOrdersFromDb();
            return orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount);
        }
        
        protected CartResultDto AddToCartInDb(CartActionDto cartAction)
        {
            var result = new CartResultDto
            {
                IsSuccess = false,
                Message = "Товар не найден",
                UpdatedCart = cartAction.CurrentCart
            };
            
            var product = GetProductById(cartAction.ProductId);
            if (product == null)
            {
                return result;
            }
            
            var cart = cartAction.CurrentCart ?? new List<OrderItem>();
            
            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = cart.FirstOrDefault(i => i.ProductId == cartAction.ProductId);
            if (existingItem != null)
            {
                // Увеличиваем количество
                existingItem.Quantity += cartAction.Quantity;
                result.Message = $"Количество {product.Name} увеличено";
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
                    Quantity = cartAction.Quantity
                });
                result.Message = $"{product.Name} добавлен в корзину";
            }
            
            result.IsSuccess = true;
            result.UpdatedCart = cart;
            result.TotalAmount = cart.Sum(i => i.Price * i.Quantity);
            
            return result;
        }
        
        protected CartResultDto RemoveFromCartInDb(Guid itemId, List<OrderItem> currentCart)
        {
            var result = new CartResultDto
            {
                IsSuccess = false,
                Message = "Товар не найден в корзине",
                UpdatedCart = currentCart
            };
            
            var cart = currentCart ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.Id == itemId);
            
            if (item != null)
            {
                cart.Remove(item);
                result.IsSuccess = true;
                result.Message = "Товар удален из корзины";
                result.UpdatedCart = cart;
                result.TotalAmount = cart.Sum(i => i.Price * i.Quantity);
            }
            
            return result;
        }
        
        protected CartResultDto UpdateCartItemInDb(Guid itemId, int quantity, List<OrderItem> currentCart)
        {
            var result = new CartResultDto
            {
                IsSuccess = false,
                Message = "Товар не найден в корзине",
                UpdatedCart = currentCart
            };
            
            if (quantity <= 0)
            {
                return RemoveFromCartInDb(itemId, currentCart);
            }
            
            var cart = currentCart ?? new List<OrderItem>();
            var item = cart.FirstOrDefault(i => i.Id == itemId);
            
            if (item != null)
            {
                item.Quantity = quantity;
                result.IsSuccess = true;
                result.Message = "Корзина обновлена";
                result.UpdatedCart = cart;
                result.TotalAmount = cart.Sum(i => i.Price * i.Quantity);
            }
            
            return result;
        }
        
        protected Order CreateOrderInDb(int userId, List<OrderItem> cartItems, string shippingAddress)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return null;
            }
            
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId.ToString()), // Преобразование int в Guid для примера
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = cartItems.Sum(i => i.Price * i.Quantity),
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductImageUrl = item.ProductImageUrl,
                    Price = item.Price,
                    Quantity = item.Quantity
                }).ToList(),
                ShippingAddress = shippingAddress
            };
            
            if (SaveOrderToDb(order))
            {
                return order;
            }
            
            return null;
        }
        
        protected bool CancelOrderByUserInDb(Guid orderId, int userId)
        {
            var order = GetOrderByIdFromDb(orderId);
            
            // Проверяем, что заказ принадлежит пользователю
            if (order == null || order.UserId != Guid.Parse(userId.ToString()))
            {
                return false;
            }
            
            // Проверяем, что заказ можно отменить (только в статусе "Pending")
            if (order.Status != "Pending")
            {
                return false;
            }
            
            return CancelOrderInDb(orderId);
        }
        
        protected Order GetOrderByIdForUserFromDb(Guid orderId, int userId)
        {
            var order = GetOrderByIdFromDb(orderId);
            
            // Проверяем, что заказ принадлежит пользователю
            if (order == null || order.UserId != Guid.Parse(userId.ToString()))
            {
                return null;
            }
            
            return order;
        }
    }
} 