using EShop.Domain.DomainModels;
using EShop.Domain.IdentityModels;
using EShop.Domain.Relationship;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace EShop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<EShopApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductInShoppingCart> ProductInShoppingCarts { get; set; }
        public virtual DbSet<ProductInOrder> ProductInOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
               .Property(z => z.Id)
               .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();


            builder.Entity<ProductInShoppingCart>().HasKey(psc => new { psc.ProductId, psc.ShoppingCartId });

            builder.Entity<ProductInShoppingCart>()
                .HasOne(p => p.Product)
                .WithMany(sc => sc.ProductInShoppingCarts)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInShoppingCart>()
              .HasOne(sc => sc.ShoppingCart)
              .WithMany(sc => sc.ProductInShoppingCarts)
              .HasForeignKey(z => z.ShoppingCartId);


            builder.Entity<ShoppingCart>()
               .HasOne<EShopApplicationUser>(z => z.Owner)
               .WithOne(z => z.UserShoppingCart)
               .HasForeignKey<ShoppingCart>(z => z.OwnerId);


            builder.Entity<ProductInOrder>().HasKey(psc => new { psc.ProductId, psc.OrderId });

            builder.Entity<ProductInOrder>()
                .HasOne(p => p.Product)
                .WithMany(sc => sc.ProductInOrders)
                .HasForeignKey(z => z.ProductId);

            builder.Entity<ProductInOrder>()
              .HasOne(sc => sc.UserOrder)
              .WithMany(sc => sc.ProductInOrders)
              .HasForeignKey(z => z.OrderId);

        }
    }
}
