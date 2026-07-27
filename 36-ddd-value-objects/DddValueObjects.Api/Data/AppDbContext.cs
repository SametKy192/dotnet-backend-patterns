using DddValueObjects.Api.Domain.Entities;
using DddValueObjects.Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DddValueObjects.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            // 1. Map Email using a custom Value Converter
            builder.Property(c => c.Email)
                .HasConversion(
                    email => email.Value,
                    value => new Email(value))
                .IsRequired()
                .HasMaxLength(150);

            // 2. Map BillingAddress as an Owned Type
            builder.OwnsOne(c => c.BillingAddress, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("Street")
                    .IsRequired()
                    .HasMaxLength(150);

                address.Property(a => a.City)
                    .HasColumnName("City")
                    .IsRequired()
                    .HasMaxLength(100);

                address.Property(a => a.ZipCode)
                    .HasColumnName("ZipCode")
                    .IsRequired()
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // 3. Map Balance (Money) as an Owned Type
            builder.OwnsOne(c => c.Balance, balance =>
            {
                balance.Property(m => m.Amount)
                    .HasColumnName("BalanceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                balance.Property(m => m.Currency)
                    .HasColumnName("BalanceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });
    }
}
