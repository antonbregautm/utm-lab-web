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
            return CancelOrderInDb(orderId);
        }
        
        public Dictionary<string, object> GetOrderStatistics()
        {
            return GetOrderStatisticsFromDb();
        }
        
        public decimal GetTotalSales()
        {
            return GetTotalSalesFromDb();
        }

        public CartResultDto AddToCart(CartActionDto cartAction)
        {
            return AddToCartInDb(cartAction);
        }

        public CartResultDto RemoveFromCart(Guid itemId, List<OrderItem> currentCart)
        {
            return RemoveFromCartInDb(itemId, currentCart);
        }

        public CartResultDto UpdateCartItem(Guid itemId, int quantity, List<OrderItem> currentCart)
        {
            return UpdateCartItemInDb(itemId, quantity, currentCart);
        }

        public Order CreateOrder(int userId, List<OrderItem> cartItems, string shippingAddress)
        {
            return CreateOrderInDb(userId, cartItems, shippingAddress);
        }

        public bool CancelOrder(Guid orderId, int userId)
        {
            return CancelOrderByUserInDb(orderId, userId);
        }

        public Order GetOrderById(Guid orderId, int userId)
        {
            return GetOrderByIdForUserFromDb(orderId, userId);
        }
    }
} 