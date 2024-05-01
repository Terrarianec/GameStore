namespace GameStore.DB.Models;

public partial class Tag
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public virtual ICollection<Game> Games { get; set; } = [];
}
