using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для UserProfile.xaml
	/// </summary>
	public partial class UserProfile : UserControl
	{
		private readonly GameStoreContext _context = new GameStoreContext();
		private readonly int _id;

		public UserProfile(int userId)
		{
			InitializeComponent();

			_id = userId;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			var user = _context.Users
				.AsNoTracking()
				.Include(u => u.Member)
				.ThenInclude(m => m!.Team)
				.Include(u => u.Games)
				.Where(u => u.Id == _id)
				.FirstOrDefaultAsync();

			SetUser(await user);
		}

		private void OnTeamClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Тут должен быть переход на страницу команды");
		}

		private void OnGameClick(object sender, RoutedEventArgs e)
		{
			var selectedGame = (Game)((Button)sender).Tag;

			MainWindow.SetActivePage(new GamePage(selectedGame));
		}

		private async void SetUser(User? user)
		{
			DataContext = user;

			if (user?.Member != null)
			{
				noTeamLabel.Visibility = Visibility.Hidden;
				teamLink.Visibility = Visibility.Visible;
			}

			if (user is not null)
			{
				gamesListView.ItemsSource = await _context.Games
					.AsNoTracking()
					.Include(g => g.Users)
					.Include(g => g.Reviews)
					.Include(g => g.Tags)
					.Where(g => g.Users.Any(u => u.Id == user.Id))
					.ToListAsync();
			}
		}

		private async void OnEditClick(object sender, RoutedEventArgs e)
		{
			var changed = new EditProfile((DataContext as User)!)
			{
				Owner = MainWindow.Instance
			}.ShowDialog() == true;

			if (changed)
			{
				var user = _context.Users
					.AsNoTracking()
					.Include(u => u.Member)
					.ThenInclude(m => m!.Team)
					.Include(u => u.Games)
					.Where(u => u.Id == _id)
					.FirstOrDefaultAsync();

				SetUser(await user);
			}
		}
	}
}
