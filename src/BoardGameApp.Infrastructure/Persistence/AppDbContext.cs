using BoardGameApp.Domain.Authors;
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

    public DbSet<Author> Authors => Set<Author>();

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

            entity.HasIndex(game => new { game.Name, game.PublisherId })
                .IsUnique();

            entity.HasOne<Publisher>()
                .WithMany()
                .HasForeignKey(game => game.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Genre>()
                .WithMany()
                .HasForeignKey(game => game.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Author>()
                .WithMany()
                .HasForeignKey(game => game.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(genre => genre.Name)
                .IsRequired();

            entity.HasData(
                new Genre { Id = 1, Name = "Strategy" },
                new Genre { Id = 2, Name = "Family" },
                new Genre { Id = 3, Name = "Party" });
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.Property(publisher => publisher.Name)
                .IsRequired();

            entity.HasData(
                new Publisher { Id = 1, Name = "Galapagos" },
                new Publisher { Id = 2, Name = "Devir" },
                new Publisher { Id = 3, Name = "Meeple BR" });
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(author => author.Name)
                .IsRequired();

            entity.HasData(
                new Author { Id = 1, Name = "Michael Kiesling" },
                new Author { Id = 2, Name = "Klaus Teuber" },
                new Author { Id = 3, Name = "Jacob Fryxelius" });
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(match => match.PlayerIds)
                .IsRequired();

            entity.Property(match => match.Scores)
                .IsRequired();

            entity.Property(match => match.CreatedAt)
                .IsRequired();

            entity.HasOne<Game>()
                .WithMany()
                .HasForeignKey(match => match.GameId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(match => match.WinnerPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
