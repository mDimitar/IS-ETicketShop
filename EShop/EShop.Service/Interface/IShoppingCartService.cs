using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Domain.Relationship;
using System;
using System.Collections.Generic;
using System.Text;

namespace EShop.Service.Interface
{
    public interface IShoppingCartService
    {
        public ShoppingCartDto GetProductForShoppingCart(string userId);

        public ProductInShoppingCart DeleteProductFromShoppingCart(Guid? productId, string userId);

        public Order CreateOrder(string userId);
    }
}
