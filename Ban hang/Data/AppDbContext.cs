using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ban_hang.Models;

namespace Ban_hang.Data;

// Đổi từ DbContext sang IdentityDbContext
// IdentityDbContext tự tạo các bảng: AspNetUsers, AspNetRoles...
public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Connection is configured via Program.cs using the ConnectionStrings:DefaultConnection
        // setting (set through the ConnectionStrings__DefaultConnection environment variable / secret).
        // Do not hardcode credentials here.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.OldPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<BookingPrice>()
            .Property(bp => bp.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Voucher>().Property(v => v.DiscountValue).HasPrecision(18, 2);
        modelBuilder.Entity<Voucher>().Property(v => v.MinOrderAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Voucher>().Property(v => v.MaxDiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.DiscountAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Pet>().Property(p => p.WeightKg).HasPrecision(6, 2);
        modelBuilder.Entity<PetWeightLog>().Property(p => p.WeightKg).HasPrecision(6, 2);
    }

    public DbSet<ServiceItem> Services { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<PriceTier> PriceTiers { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<FaqItem> FaqItems { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingPrice> BookingPrices { get; set; }
    public DbSet<GalleryItem> GalleryItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<PetWeightLog> PetWeightLogs { get; set; }
}

