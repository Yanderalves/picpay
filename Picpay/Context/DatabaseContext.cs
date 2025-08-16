using Microsoft.EntityFrameworkCore;
using Picpay.Models;

namespace Picpay.Context;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Transfer> Transfers { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder options)
    {
        options.Entity<User>().HasIndex(x => new {x.Email, x.Identifier}).IsUnique();
    }
}