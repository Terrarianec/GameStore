using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для UserProfile.xaml
	/// </summary>
	public partial class UserProfile : UserControl
	{
		private readonly int _id;

		public UserProfile(int userId)
		{
			InitializeComponent();

			_id = userId;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			using var context = new GameStoreContext();

			var user = context.Users
				.Include(u => u.Member)
				.ThenInclude(m => m.Team)
				.Include(u => u.Games)
				.Where(u => u.Id == _id)
				.FirstOrDefaultAsync();

			SetUser(await user);
		}

		private void OnTeamButtonClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Тут должен быть переход на страницу команды");
		}

		private void SetUser(User? user)
		{
			DataContext = user;

			if (user?.Member != null)
			{
				noTeamLabel.Visibility = Visibility.Hidden;
				teamLink.Visibility = Visibility.Visible;
			}
		}
	}
}
