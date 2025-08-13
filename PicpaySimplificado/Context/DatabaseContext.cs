using Microsoft.EntityFrameworkCore;
using PicpaySimplificado.Models;

namespace PicpaySimplificado.Context;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=PicpaySimplificado.db");
    }

    protected override void OnModelCreating(ModelBuilder options)
    {
        options.Entity<User>().HasIndex(x => new {x.Email, x.Identifier}).IsUnique();
    }
}