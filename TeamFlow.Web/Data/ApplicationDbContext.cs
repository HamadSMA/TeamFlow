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
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();
    public DbSet<Message> Messages => Set<Message>();

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

        modelBuilder.Entity<StatusHistory>(entity =>
        {
            entity.Property(s => s.Timestamp).IsRequired();
        });

        modelBuilder.Entity<StatusHistory>()
            .HasOne(s => s.User)
            .WithMany(u => u.StatusHistoryEntries)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany()
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
