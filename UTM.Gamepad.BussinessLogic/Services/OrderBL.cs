using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class OrderBL : IOrderBL
    {
        private readonly OrderRepository _orderRepository;
        
        public OrderBL()
        {
            _orderRepository = new OrderRepository();
        }
        
        public List<Order> GetAllOrders()
        {
            return _orderRepository.GetAll();
        }
        
        public Order GetOrderById(Guid id)
        {
            return _orderRepository.GetById(id);
        }
        
        public List<Order> GetOrdersByUserId(Guid userId)
        {
            return _orderRepository.GetAll().Where(o => o.UserId == userId).ToList();
        }
        
        public bool CreateOrder(Order order)
        {
            if (order == null || order.OrderItems == null || !order.OrderItems.Any())
                return false;
                
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";
            
            order.TotalAmount = order.OrderItems.Sum(item => item.Price * item.Quantity);
            
            return _orderRepository.Add(order);
        }
        
        public bool UpdateOrderStatus(Guid orderId, string status, string statusNote = null)
        {
            if (orderId == Guid.Empty || string.IsNullOrEmpty(status))
                return false;
                
            var order = _orderRepository.GetById(orderId);
            if (order == null)
                return false;
                
            order.Status = status;
            
            if (!string.IsNullOrEmpty(statusNote))
            {
                if (string.IsNullOrEmpty(order.Notes))
                {
                    order.Notes = statusNote;
                }
                else
                {
                    order.Notes += Environment.NewLine + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ": " + statusNote;
                }
            }
            
            return _orderRepository.Update(order);
        }
        
        public bool CancelOrder(Guid orderId)
        {
            return UpdateOrderStatus(orderId, "Cancelled");
        }
        
        public Dictionary<string, object> GetOrderStatistics()
        {
            var allOrders = _orderRepository.GetAll();
            var statistics = new Dictionary<string, object>
            {
                { "Total", allOrders.Count },
                { "Pending", allOrders.Count(o => o.Status == "Pending") },
                { "Processing", allOrders.Count(o => o.Status == "Processing") },
                { "Shipped", allOrders.Count(o => o.Status == "Shipped") },
                { "Completed", allOrders.Count(o => o.Status == "Completed") },
                { "Cancelled", allOrders.Count(o => o.Status == "Cancelled") }
            };
            
            return statistics;
        }
        
        public decimal GetTotalSales()
        {
            var completedOrders = _orderRepository.GetAll().Where(o => o.Status == "Completed").ToList();
            return completedOrders.Sum(o => o.TotalAmount);
        }
    }
} 