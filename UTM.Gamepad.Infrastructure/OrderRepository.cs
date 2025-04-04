using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Infrastructure
{
    public class OrderRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        
        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .ToList();
        }
        
        public Order GetById(Guid id)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);
        }
        
        public bool Add(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool Update(Order order)
        {
            try
            {
                var existingOrder = _context.Orders.FirstOrDefault(o => o.Id == order.Id);
                if (existingOrder == null)
                    return false;
                
                existingOrder.Status = order.Status;
                existingOrder.Notes = order.Notes;
                
                // Другие поля заказа, которые можно обновить
                
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool Delete(Guid id)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.Id == id);
                if (order == null)
                    return false;
                
                // Удаляем связанные элементы заказа
                var orderItems = _context.OrderItems.Where(i => i.OrderId == id).ToList();
                foreach (var item in orderItems)
                {
                    _context.OrderItems.Remove(item);
                }
                
                _context.Orders.Remove(order);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 