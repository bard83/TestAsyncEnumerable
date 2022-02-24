using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TestAsyncEnumerable;

public class MyDbContext : DbContext
{
    private bool _isMigrated = false;

#pragma warning disable CS8618
    public DbSet<MyItem> Items { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
        if(!_isMigrated)
        {
            Database.EnsureCreatedAsync();
            _isMigrated = true;
        }
    }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MyItem>()
                .HasKey(m => m.Id);
        modelBuilder.Entity<MyItem>()
                .Property<string>(m => m.Description)
                    .IsRequired();
    }
}
