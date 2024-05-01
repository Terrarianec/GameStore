namespace GameStore.DB.Models;

public partial class User
{
	public int Id { get; set; }

	public string Username { get; set; } = string.Empty;

	public byte[]? Avatar { get; set; }

	public string Login { get; set; } = string.Empty;

	public string PasswordHash { get; set; } = null!;

	public DateOnly DateOfBirth { get; set; }

	public decimal Balance { get; set; }

	public virtual ICollection<GameReview> Reviews { get; set; } = [];

	public virtual Team? Team { get; set; }

	public virtual Member? Member { get; set; }

	public virtual ICollection<Game> Games { get; set; } = [];
}
