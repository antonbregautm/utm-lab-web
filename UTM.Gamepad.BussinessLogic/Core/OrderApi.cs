using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
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
    }
} 