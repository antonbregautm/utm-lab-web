using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IOrderBL
    {
        List<Order> GetAllOrders();
        Order GetOrderById(Guid id);
        List<Order> GetOrdersByUserId(Guid userId);
        bool CreateOrder(Order order);
        bool UpdateOrderStatus(Guid orderId, string status, string statusNote = null);
        bool CancelOrder(Guid orderId);
        Dictionary<string, object> GetOrderStatistics();
        decimal GetTotalSales();
    }
} 