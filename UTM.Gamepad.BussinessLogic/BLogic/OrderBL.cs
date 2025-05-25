using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Core;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;

namespace UTM.Gamepad.BussinessLogic.BLogic
{
    public class OrderBL : OrderApi, IOrderBL
    {
        public OrderBL() : base() { }

        public List<Order> GetAllOrders()
        {
            return GetAllOrdersFromDb();
        }
        
        public Order GetOrderById(Guid id)
        {
            return GetOrderByIdFromDb(id);
        }
        
        public List<Order> GetOrdersByUserId(Guid userId)
        {
            return GetOrdersByUserIdFromDb(userId);
        }
        
        // Перегрузка для поддержки int id
        public List<Order> GetOrdersByUserId(int userId)
        {
            return GetOrdersByUserId(Guid.Parse(userId.ToString()));
        }
        
        public bool CreateOrder(Order order)
        {
            if (order == null)
            {
                return false;
            }

            return SaveOrderToDb(order);
        }
        
        public bool UpdateOrderStatus(Guid orderId, string status, string statusNote = null)
        {
            var order = GetOrderById(orderId);
            if (order == null)
            {
                return false;
            }
            
            order.Status = status;
            
            return UpdateOrderInDb(order);
        }
        
        public bool CancelOrder(Guid orderId)
        {
            return UpdateOrderStatus(orderId, "Cancelled", "Отменен пользователем");
        }
        
        public Dictionary<string, object> GetOrderStatistics()
        {
            var orders = GetAllOrders();
            
            return new Dictionary<string, object>
            {
                { "TotalOrders", orders.Count },
                { "CompletedOrders", orders.Count(o => o.Status == "Completed") },
                { "PendingOrders", orders.Count(o => o.Status == "Pending") },
                { "CancelledOrders", orders.Count(o => o.Status == "Cancelled") },
                { "TotalSales", orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount) }
            };
        }
        
        public decimal GetTotalSales()
        {
            var orders = GetAllOrders();
            return orders.Where(o => o.Status == "Completed").Sum(o => o.TotalAmount);
        }

        public CartResultDto AddToCart(CartActionDto cartAction)
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

        public CartResultDto RemoveFromCart(Guid itemId, List<OrderItem> currentCart)
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

        public CartResultDto UpdateCartItem(Guid itemId, int quantity, List<OrderItem> currentCart)
        {
            var result = new CartResultDto
            {
                IsSuccess = false,
                Message = "Товар не найден в корзине",
                UpdatedCart = currentCart
            };
            
            if (quantity <= 0)
            {
                return RemoveFromCart(itemId, currentCart);
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

        public Order CreateOrder(int userId, List<OrderItem> cartItems, string shippingAddress)
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

        public bool CancelOrder(Guid orderId, int userId)
        {
            var order = GetOrderById(orderId);
            
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
            
            return CancelOrder(orderId);
        }

        public Order GetOrderById(Guid orderId, int userId)
        {
            var order = GetOrderById(orderId);
            
            // Проверяем, что заказ принадлежит пользователю
            if (order == null || order.UserId != Guid.Parse(userId.ToString()))
            {
                return null;
            }
            
            return order;
        }
    }
} 