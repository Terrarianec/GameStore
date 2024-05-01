namespace GameStore.DB.Models;

public partial class GameReview
{
	public int UserId { get; set; }

	public int GameId { get; set; }

	public int Rate { get; set; }

	public string Content { get; set; } = string.Empty;

	public DateOnly PublishDate { get; set; }

	public virtual Game Game { get; set; } = null!;

	public virtual User User { get; set; } = null!;
}
