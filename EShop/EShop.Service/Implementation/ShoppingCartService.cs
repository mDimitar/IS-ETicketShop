using EShop.Domain.DomainModels;
using EShop.Domain.DTO;
using EShop.Domain.Relationship;
using EShop.Repository.Interface;
using EShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EShop.Service.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IRepository<ProductInOrder> _productInOrderRepository;
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;

        public ShoppingCartService(IUserRepository userRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepositor, IRepository<ProductInOrder> productInOrderRepository, IRepository<ShoppingCart> shoppingCartRepository, IRepository<Order> orderRepository)
        {
            _userRepository = userRepository;
            _productInShoppingCartRepository = productInShoppingCartRepositor;
            _productInOrderRepository = productInOrderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _orderRepository = orderRepository;
        }

        public Order CreateOrder(string userId)
        {
            var loggedInUser = _userRepository.Get(userId);

            var UserCart = loggedInUser.UserShoppingCart;

            Order userOrder = new Order
            {
                Id = Guid.NewGuid(),
                OwnerId = loggedInUser.Id,
                Owner = loggedInUser
            };

            _orderRepository.Insert(userOrder);

            var productsInOrder = UserCart.ProductInShoppingCarts.Select(z => new ProductInOrder
            {
                Id = Guid.NewGuid(),
                ProductId = z.Product.Id,
                Product = z.Product,
                OrderId = userOrder.Id,
                UserOrder = userOrder
            }).ToList();

            foreach (var item in productsInOrder)
            {
                _productInOrderRepository.Insert(item);
            }

            UserCart.ProductInShoppingCarts.Clear();

            _shoppingCartRepository.Update(UserCart);

            return userOrder;

        }

        public ProductInShoppingCart DeleteProductFromShoppingCart(Guid? productId, string userId)
        {
            var loggedInUser = _userRepository.Get(userId);

            var UserCart = loggedInUser.UserShoppingCart;

            var itemToDelete = _productInShoppingCartRepository.Get(productId);

            UserCart.ProductInShoppingCarts.Remove(itemToDelete);

            _shoppingCartRepository.Update(UserCart);

            return itemToDelete;
        }

        public ShoppingCartDto GetProductForShoppingCart(string userId)
        {
            var loggedInUser = _userRepository.Get(userId);

            var UserCart = loggedInUser.UserShoppingCart;

            var allProducts = UserCart.ProductInShoppingCarts.ToList();

            var allProductPrices = allProducts.Select(z => new
            {
                ProductPrice = z.Product.ProductPrice,
                Quantity = z.Quantity
            }).ToList();

            double totalPrice = 0.0;

            foreach (var item in allProductPrices)
            {
                totalPrice += item.Quantity * item.ProductPrice;
            }

            ShoppingCartDto model = new ShoppingCartDto
            {
                Products = allProducts,
                TotalPrice = totalPrice
            };

            return model;
            
        }
    }
}
