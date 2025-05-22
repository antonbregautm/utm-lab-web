using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class ProductBL : IProductBL
    {
        private readonly ProductRepository _productRepository;
        
        public ProductBL()
        {
            _productRepository = new ProductRepository();
        }
        
        public List<Product> GetAllProducts()
        {
            return _productRepository.GetAll();
        }
        
        public Product GetProductById(Guid id)
        {
            return _productRepository.GetById(id);
        }
        
        public bool CreateProduct(Product product)
        {
            if (product == null)
                return false;
                
            if (string.IsNullOrEmpty(product.Name) || product.Price <= 0)
                return false;
                
            product.Id = Guid.NewGuid();
            product.CreatedDate = DateTime.Now;
            product.UpdatedDate = DateTime.Now;
            
            return _productRepository.Add(product);
        }
        
        public bool UpdateProduct(Product product)
        {
            if (product == null || product.Id == Guid.Empty)
                return false;
                
            if (string.IsNullOrEmpty(product.Name) || product.Price <= 0)
                return false;
                
            var existingProduct = _productRepository.GetById(product.Id);
            if (existingProduct == null)
                return false;
                
            product.CreatedDate = existingProduct.CreatedDate;
            product.UpdatedDate = DateTime.Now;
            
            return _productRepository.Update(product);
        }
        
        public bool DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
                return false;
                
            return _productRepository.Delete(id);
        }
    }
} 