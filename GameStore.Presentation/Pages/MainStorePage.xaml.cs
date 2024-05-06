using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для MainStorePage.xaml
	/// </summary>
	public partial class MainStorePage : UserControl
	{
		public MainStorePage()
		{
			InitializeComponent();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			using var context = new GameStoreContext();
			gamesListView.ItemsSource = context.Games
				.Include(g => g.Tags)
				.ToList();
		}

		private void OnGameClick(object sender, RoutedEventArgs e)
		{
			var selectedGame = (Game)((Button)sender).Tag;

			MainWindow.SetActivePage(new GamePage(selectedGame.Id));
		}
	}
}
