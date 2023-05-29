using API.FurnitureStore.Shared;
using Microsoft.EntityFrameworkCore;


namespace API.FurnitureStore.Data
{
    public class APIFurnitureStoreContext : DbContext
    {
        public APIFurnitureStoreContext(DbContextOptions options) : base(options) { }
        //tablas
        public DbSet<Client> Clients { get; set; }
        public DbSet <Product> Products { get; set; }
        public DbSet <Order> Orders { get; set; }
        public DbSet <ProductCategory> ProductCategories { get; set; }
        public DbSet <OrderDetail> OrderDetails { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
        }

        //Luego del metodo se hace Add-Migration "Lo que quieras"
        //Update-Database para updatear
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Estoy estableciendo manuelmente que orderDetail tiene la clave de orderid y productId
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new {od.OrderId, od.ProductId});
        }
    }
}
