using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Interfaces
{
    public interface IProductBL
    {
        List<Product> GetAllProducts();
        Product GetProductById(Guid id);
        List<Product> GetFeaturedProducts(int count = 8);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Guid id);
    }
} 