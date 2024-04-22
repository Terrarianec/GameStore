using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для GamePage.xaml
	/// </summary>
	public partial class GamePage : UserControl
	{
		private readonly GameStoreContext _context = new();
		private readonly Game? _game;

		public GamePage(Game game)
		{
			InitializeComponent();

			_game = _context.Games
				.Include(g => g.Team)
				.Include(g => g.Tags)
				.Include(g => g.Reviews)
				.Where(g => g.Id == game.Id)
				.FirstOrDefault();

			DataContext = _game;

			if (_game?.Reviews.Count > 0)
			{
				var rate = _game.Reviews.Average(r => r.Rate);
				rateFrogs.Width = rateFrogs.MaxWidth * rate;
				rateValue.Text = $"{rate:N2}";
			}
		}

		private void OnTeamClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Тут должен быть переход на страницу команды");
		}
	}
}
