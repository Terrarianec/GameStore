namespace GameStore.DB.Models;

public partial class Game
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public byte[]? Logo { get; set; }

	public string Description { get; set; } = string.Empty;

	public int TeamId { get; set; }

	public int RequiredFreeSpace { get; set; }

	public decimal Price { get; set; }

	public DateOnly PublishDate { get; set; }

	public virtual ICollection<GameReview> Reviews { get; set; } = [];

	public virtual Team Team { get; set; } = null!;

	public virtual ICollection<Tag> Tags { get; set; } = [];

	public virtual ICollection<User> Users { get; set; } = [];
}
