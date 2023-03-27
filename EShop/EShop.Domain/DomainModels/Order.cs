
using EShop.Domain.IdentityModels;
using EShop.Domain.Relationship;
using System;
using System.Collections.Generic;

namespace EShop.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual EShopApplicationUser Owner { get; set; }

        public virtual ICollection<ProductInOrder> ProductInOrders { get; set; }
    }
}
