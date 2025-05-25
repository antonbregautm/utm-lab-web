using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Core;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.BLogic
{
    public class ProductBL : ProductApi, IProductBL
    {
        public ProductBL() : base() { }
        
        public List<Product> GetAllProducts()
        {
            return GetAllProductsFromDb();
        }
        
        public Product GetProductById(Guid id)
        {
            return GetProductByIdFromDb(id);
        }
        
        public List<Product> GetFeaturedProducts(int count = 8)
        {
            return GetAllProducts()
                .OrderByDescending(p => p.Price)
                .Take(count)
                .ToList();
        }
        
        public bool CreateProduct(Product product)
        {
            if (product == null)
            {
                return false;
            }
            
            // Устанавливаем дату создания
            if (product.Id == Guid.Empty)
            {
                product.Id = Guid.NewGuid();
            }
            
            return SaveProductToDb(product);
        }
        
        public bool UpdateProduct(Product product)
        {
            if (product == null || product.Id == Guid.Empty)
            {
                return false;
            }
            
            var existingProduct = GetProductById(product.Id);
            if (existingProduct == null)
            {
                return false;
            }
            
            return UpdateProductInDb(product);
        }
        
        public bool DeleteProduct(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }
            
            var product = GetProductById(id);
            if (product == null)
            {
                return false;
            }
            
            try
            {
                // Здесь должна быть логика удаления продукта
                // Так как у нас нет метода DeleteProductFromDb, то этот метод будет заглушкой
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 