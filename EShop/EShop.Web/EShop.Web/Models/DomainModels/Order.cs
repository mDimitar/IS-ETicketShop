using EShop.Web.Models.IdentityModels;
using EShop.Web.Models.Relationship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Web.Models.DomainModels
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        public virtual EShopApplicationUser Owner { get; set; }

        public virtual ICollection<ProductInOrder> ProductInOrders { get; set; }
    }
}
