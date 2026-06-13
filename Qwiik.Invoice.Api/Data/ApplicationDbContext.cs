using Microsoft.EntityFrameworkCore;
using Qwiik.Invoice.Api.Entities;

namespace Qwiik.Invoice.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<Entities.Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>()
                .Property(x => x.Name)
                .HasMaxLength(100);

            modelBuilder.Entity<Entities.Invoice>()
                .Property(x => x.InvoiceNumber)
                .HasMaxLength(50);

            modelBuilder.Entity<Entities.Invoice>()
                .Property(x => x.CustomerName)
                .HasMaxLength(200);

            modelBuilder.Entity<Entities.Invoice>()
                .HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId);

            modelBuilder.Entity<Entities.Invoice>()
                .HasIndex(x => x.TenantId);

            modelBuilder.Entity<Entities.Invoice>()
                .HasIndex(x => new
                {
                    x.TenantId,
                    x.Status
                });

            modelBuilder.Entity<Entities.Invoice>()
                .HasIndex(x => new
                {
                    x.TenantId,
                    x.InvoiceNumber
                })
                .IsUnique();
        }
    }
}