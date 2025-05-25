using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IProductBL
    {
        List<Product> GetAllProducts();
        Product GetProductById(Guid id);
        bool CreateProduct(Product product);
        bool UpdateProduct(Product product);
        bool DeleteProduct(Guid id);
    }
} 