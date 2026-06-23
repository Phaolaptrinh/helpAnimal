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
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "workstation id=petlor1111.mssql.somee.com;packet size=4096;user id=ghe;pwd=123456loi;data source=petlor1111.mssql.somee.com;persist security info=False;initial catalog=petlor1111;TrustServerCertificate=True;"
            );
        }
    }

    public DbSet<ServiceItem> Services { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<PriceTier> PriceTiers { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<FaqItem> FaqItems { get; set; }
}