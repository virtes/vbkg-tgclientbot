using Microsoft.EntityFrameworkCore;
using VBkg.TgClientBot.Data.Entities;

namespace VBkg.TgClientBot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<UserState> UserStates { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.TelegramUserId)
            .IsUnique()
            .HasFilter("\"RemovedAt\" is not null");

        modelBuilder.Entity<UserState>()
            .HasKey(us => new { us.TelegramUserId, us.TelegramChatId });
    }
}