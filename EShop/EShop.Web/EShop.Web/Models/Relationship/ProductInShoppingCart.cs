using EShop.Web.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Web.Models.Relationship
{
    public class ProductInShoppingCart
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public Guid ShoppingCartId { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
