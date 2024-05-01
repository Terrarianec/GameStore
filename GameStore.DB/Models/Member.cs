namespace GameStore.DB.Models;

public partial class Member
{
	public int UserId { get; set; }

	public int TeamId { get; set; }

	public virtual Team Team { get; set; } = null!;

	public virtual User User { get; set; } = null!;
}
