using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;

namespace UTM.Gamepad.Application.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        
        public ProductService()
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
        
        public List<Product> GetProductsByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
                return new List<Product>();
                
            return _productRepository.GetAll().Where(p => p.Category == category).ToList();
        }
        
        public List<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return new List<Product>();
                
            return _productRepository.GetAll().Where(p => 
                p.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 || 
                p.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }
        
        public List<string> GetCategories()
        {
            return _productRepository.GetAll()
                .Select(p => p.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .ToList();
        }
    }
} 