using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class ProductApi
    {
        private readonly ProductRepository _productRepository;
        
        public ProductApi()
        {
            var dbContext = new ApplicationDbContext();
            _productRepository = new ProductRepository();
        }
        
        protected List<Product> GetAllProductsFromDb()
        {
            return _productRepository.GetAll().ToList();
        }
        
        protected Product GetProductByIdFromDb(Guid id)
        {
            return _productRepository.GetById(id);
        }
        
        protected bool SaveProductToDb(Product product)
        {
            try
            {
                _productRepository.Add(product);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool UpdateProductInDb(Product product)
        {
            try
            {
                _productRepository.Update(product);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 