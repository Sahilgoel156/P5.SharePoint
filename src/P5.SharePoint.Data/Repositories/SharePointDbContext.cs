using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;

namespace P5.SharePoint.Data.Repositories;

public class SharePointDbContext : DbContextWithTriggers
{
    public SharePointDbContext(DbContextOptions<SharePointDbContext> options)
        : base(options)
    {
    }

    protected SharePointDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<SharePointEntity>().ToTable("SharePoint").HasKey(x => x.Id);
        //modelBuilder.Entity<SharePointEntity>().Property(x => x.Id).HasMaxLength(128).ValueGeneratedOnAdd();
    }
}
