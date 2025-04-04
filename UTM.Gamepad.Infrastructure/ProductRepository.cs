using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Infrastructure
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        
        public List<Product> GetAll()
        {
            return _context.Products.ToList();
        }
        
        public Product GetById(Guid id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }
        
        public bool Add(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool Update(Product product)
        {
            try
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
                if (existingProduct == null)
                    return false;
                
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.Category = product.Category;
                existingProduct.UpdatedDate = DateTime.Now;
                
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    existingProduct.ImageUrl = product.ImageUrl;
                }
                
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
                var product = _context.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                    return false;
                
                _context.Products.Remove(product);
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