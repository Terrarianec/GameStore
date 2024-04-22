namespace GameStore.DB.Models
{
	public partial class Game
	{
		public string StringifiedPrice => Price > 0 ? $"{Price:N2} руб." : "Бесплатно";
	}
}
