namespace GameStore.DB.Models
{
	public partial class Game
	{
		public string StringifiedPrice => Price > 0 ? $"{Price:N2} руб." : "Бесплатно";
		public string StringifiedRequiredSpace => RequiredFreeSpace > 1000 ? $"{RequiredFreeSpace / 1000D} GB" : $"{RequiredFreeSpace} MB";
	}
}
