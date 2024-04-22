using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.DB;

public partial class GameStoreContext : DbContext
{
	public GameStoreContext()
	{
	}

	public GameStoreContext(DbContextOptions<GameStoreContext> options)
		: base(options)
	{
	}

	public virtual DbSet<Game> Games { get; set; }

	public virtual DbSet<GameReview> GameReviews { get; set; }

	public virtual DbSet<Tag> Tags { get; set; }

	public virtual DbSet<Team> Teams { get; set; }

	public virtual DbSet<TeamMember> TeamMembers { get; set; }

	public virtual DbSet<User> Users { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress;encrypt=false;database=GameStore;user=исп-31;password=1234567890");

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Game>(entity =>
		{
			entity.Property(e => e.Name).HasMaxLength(64);
			entity.Property(e => e.Price).HasColumnType("money");

			entity.HasOne(d => d.Team).WithMany(p => p.Games)
				.HasForeignKey(d => d.TeamId)
				.HasConstraintName("FK_Games_Developers");

			entity.HasMany(d => d.Tags).WithMany(p => p.Games)
				.UsingEntity<Dictionary<string, object>>(
					"GameTag",
					r => r.HasOne<Tag>().WithMany()
						.HasForeignKey("TagId")
						.OnDelete(DeleteBehavior.ClientSetNull)
						.HasConstraintName("FK_GameTags_Tags"),
					l => l.HasOne<Game>().WithMany()
						.HasForeignKey("GameId")
						.HasConstraintName("FK_GameTags_Games"),
					j =>
					{
						j.HasKey("GameId", "TagId");
						j.ToTable("GameTags");
					});

			entity.HasMany(d => d.Users).WithMany(p => p.Games)
				.UsingEntity<Dictionary<string, object>>(
					"PurchasedGame",
					r => r.HasOne<User>().WithMany()
						.HasForeignKey("UserId")
						.HasConstraintName("FK_PurchasedGames_Users"),
					l => l.HasOne<Game>().WithMany()
						.HasForeignKey("GameId")
						.HasConstraintName("FK_PurchasedGames_Games"),
					j =>
					{
						j.HasKey("GameId", "UserId");
						j.ToTable("PurchasedGames");
					});
		});

		modelBuilder.Entity<GameReview>(entity =>
		{
			entity.HasKey(e => new { e.UserId, e.GameId });

			entity.Property(e => e.Content).HasMaxLength(1000);

			entity.HasOne(d => d.Game).WithMany(p => p.Reviews)
				.HasForeignKey(d => d.GameId)
				.HasConstraintName("FK_GameReviews_Games");

			entity.HasOne(d => d.User).WithMany(p => p.Reviews)
				.HasForeignKey(d => d.UserId)
				.HasConstraintName("FK_GameReviews_Users");
		});

		modelBuilder.Entity<Tag>(entity =>
		{
			entity.HasIndex(e => e.Name, "IX_Tags").IsUnique();

			entity.Property(e => e.Name)
				.HasMaxLength(64)
				.IsUnicode(false);
		});

		modelBuilder.Entity<Team>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK_Developers");

			entity.ToTable(tb => tb.HasTrigger("AddOwnerInTeam"));

			entity.Property(e => e.Name).HasMaxLength(32);

			entity.HasOne(d => d.Owner).WithMany(p => p.Teams)
				.HasForeignKey(d => d.OwnerId)
				.HasConstraintName("FK_Teams_Users");
		});

		modelBuilder.Entity<TeamMember>(entity =>
		{
			entity.HasKey(e => new { e.UserId, e.TeamId });

			entity.HasIndex(e => e.UserId, "IX_OnlyOneTeam").IsUnique();

			entity.HasOne(d => d.Team).WithMany(p => p.Members)
				.HasForeignKey(d => d.TeamId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_TeamMembers_Teams");

			entity.HasOne(d => d.User).WithOne(p => p.Member)
				.HasForeignKey<TeamMember>(d => d.UserId)
				.OnDelete(DeleteBehavior.ClientSetNull)
				.HasConstraintName("FK_TeamMembers_Users");
		});

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasIndex(e => e.Login, "IX_Users").IsUnique();

			entity.Property(e => e.Balance).HasColumnType("money");
			entity.Property(e => e.Login)
				.HasMaxLength(32)
				.IsUnicode(false);
			entity.Property(e => e.PasswordHash)
				.HasMaxLength(128)
				.IsUnicode(false);
			entity.Property(e => e.Username).HasMaxLength(32);
		});

		OnModelCreatingPartial(modelBuilder);
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
