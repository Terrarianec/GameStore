namespace GameStore.DB.Models;

public partial class User
{
	public int Id { get; set; }

	public string Username { get; set; } = null!;

	public byte[]? Avatar { get; set; }

	public string Login { get; set; } = null!;

	public string PasswordHash { get; set; } = null!;

	public DateOnly DateOfBirth { get; set; }

	public decimal Balance { get; set; }

	public virtual ICollection<GameReview> Reviews { get; set; } = [];

	public virtual TeamMember? Member { get; set; }

	public virtual ICollection<Team> Teams { get; set; } = [];

	public virtual ICollection<Game> Games { get; set; } = [];
}
