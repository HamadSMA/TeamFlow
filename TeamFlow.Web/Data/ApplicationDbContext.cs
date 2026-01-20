using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Data;

public class ApplicationDbContext
    : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(t => t.Name).IsRequired();
        });

        modelBuilder.Entity<Team>()
            .HasMany(t => t.Users)
            .WithOne(u => u.Team)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
