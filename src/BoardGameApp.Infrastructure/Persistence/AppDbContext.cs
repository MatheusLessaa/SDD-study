using BoardGameApp.Domain.Games;
using BoardGameApp.Domain.Genres;
using BoardGameApp.Domain.Matches;
using BoardGameApp.Domain.Players;
using BoardGameApp.Domain.Publishers;
using Microsoft.EntityFrameworkCore;

namespace BoardGameApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();

    public DbSet<Game> Games => Set<Game>();

    public DbSet<Match> Matches => Set<Match>();

    public DbSet<Genre> Genres => Set<Genre>();

    public DbSet<Publisher> Publishers => Set<Publisher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(player => player.FullName)
                .IsRequired();

            entity.Property(player => player.WhatsApp)
                .IsRequired();

            entity.HasIndex(player => player.FullName)
                .IsUnique();

            entity.HasIndex(player => player.WhatsApp)
                .IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(game => game.Name)
                .IsRequired();

            entity.Property(game => game.Author)
                .IsRequired();

            entity.HasIndex(game => new { game.Name, game.PublisherId })
                .IsUnique();
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(genre => genre.Name)
                .IsRequired();
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(publisher => publisher.Name)
                .IsRequired();
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(match => match.PlayerIds)
                .IsRequired();

            entity.Property(match => match.Scores)
                .IsRequired();
        });
    }
}
