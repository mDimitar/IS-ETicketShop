using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace EShop.Service.Interface
{
    public interface IProductService
    {
        public List<Product> GetAllProductAsList();
        public Product GetSpecificProduct(Guid? id);
        public Product CreateNewProduct(Product newEntity);
        public Product UpdateExistingProduct(Product updatedProduct);
        public Product DeleteProduct(Guid? id);
        public Boolean ProductExist(Guid? id);

        public AddToShoppingCardDto GetAddToShoppingCartDto(Guid? id);

        public Boolean AddProductInShoppingCart(AddToShoppingCardDto entity, string userId);
    }
}
