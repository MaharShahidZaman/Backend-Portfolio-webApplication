using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Models;

namespace Portfolio.Api.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.ToTable("ContactMessages");
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Name).HasMaxLength(100).IsRequired();
            entity.Property(message => message.Email).HasMaxLength(254).IsRequired();
            entity.Property(message => message.Subject).HasMaxLength(200).IsRequired();
            entity.Property(message => message.Message).HasMaxLength(5000).IsRequired();
            // Npgsql maps this to PostgreSQL timestamptz; values must be UTC.
            entity.Property(message => message.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();
            entity.Property(message => message.IsRead).HasDefaultValue(false).IsRequired();
        });
    }
}
