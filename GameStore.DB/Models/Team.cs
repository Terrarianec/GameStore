namespace GameStore.DB.Models;

public partial class Team
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public byte[]? Logo { get; set; }

	public int? OwnerId { get; set; }

	public virtual ICollection<Game> Games { get; set; } = [];

	public virtual User? Owner { get; set; }

	public virtual ICollection<Member> Members { get; set; } = [];
}
