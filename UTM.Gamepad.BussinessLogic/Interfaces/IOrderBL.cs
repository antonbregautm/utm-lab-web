using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;

namespace UTM.Gamepad.BussinessLogic.Interfaces
{
    public interface IOrderBL
    {
        List<Order> GetAllOrders();
        Order GetOrderById(Guid id);
        List<Order> GetOrdersByUserId(Guid userId);
        List<Order> GetOrdersByUserId(int userId);
        bool CreateOrder(Order order);
        bool UpdateOrderStatus(Guid orderId, string status, string statusNote = null);
        bool CancelOrder(Guid orderId);
        Dictionary<string, object> GetOrderStatistics();
        decimal GetTotalSales();
        CartResultDto AddToCart(CartActionDto cartAction);
        CartResultDto RemoveFromCart(Guid itemId, List<OrderItem> currentCart);
        CartResultDto UpdateCartItem(Guid itemId, int quantity, List<OrderItem> currentCart);
        Order CreateOrder(int userId, List<OrderItem> cartItems, string shippingAddress);
        bool CancelOrder(Guid orderId, int userId);
        Order GetOrderById(Guid orderId, int userId);
    }
} 