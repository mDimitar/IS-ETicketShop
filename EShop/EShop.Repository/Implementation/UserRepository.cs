using EShop.Domain.IdentityModels;
using EShop.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<EShopApplicationUser> entities;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            entities = _context.Set<EShopApplicationUser>();
        }
        public EShopApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserShoppingCart)
                .Include("UserShoppingCart.ProductInShoppingCarts")
                .Include("UserShoppingCart.ProductInShoppingCarts.Product")
                .SingleOrDefault(z => z.Id == id);
                
        }

        public IEnumerable<EShopApplicationUser> GetAll()
        {
            return entities.Include(z => z.UserShoppingCart)
                .Include("UserShoppingCart.ProductInShoppingCarts")
                .Include("UserShoppingCart.ProductInShoppingCarts.Product")
                .AsEnumerable();
        }
    }
}
