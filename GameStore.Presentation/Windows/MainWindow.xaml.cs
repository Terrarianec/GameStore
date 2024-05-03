using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Pages;
using GameStore.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static MainWindow? _window = null;
		private static User? _user = null;

		public static MainWindow Instance
		{
			get
			{
				return _window ??= new MainWindow(User);
			}
			private set
			{
				_window = value;
			}
		}
		public static User? User
		{
			get => _user;
			set
			{
				_user = value;
				UpdateUser(_user);
			}
		}

		private MainWindow(User? user)
		{
			InitializeComponent();

			if (user is User authorizedUser)
				DataContext = authorizedUser;

#if DEBUG
			var context = new GameStoreContext();

			KeyDown += (s, e) =>
			{
				User? user = null;

				switch (e.Key)
				{
					case Key.F4:
						{
							if (User is User authorizedUser)
								user = context.Users
									.Where(u => u.Id < authorizedUser.Id)
									.OrderBy(u => u.Id)
									.LastOrDefault();

							user ??= context.Users
								.OrderBy(u => u.Id)
								.LastOrDefault();
						}
						break;

					case Key.F6:
						{
							if (User is User authorizedUser)
								user = context.Users
									.Where(u => u.Id > authorizedUser.Id)
									.FirstOrDefault();

							user ??= context.Users
							.FirstOrDefault();
						}
						break;
				}

				if (user is User newUser)
					User = newUser;
			};
#endif
		}

		public static void UpdateUser(User? user)
		{
			Instance.DataContext = user;
		}

		public static void SetActivePage(UserControl page)
		{
			var pageContainer = Instance.currentPage;

			pageContainer.Children.Clear();
			pageContainer.Children.Add(page);
		}

		private void OnExitButtonClick(object sender, RoutedEventArgs e)
		{
			TempStorage.Delete(nameof(LoginWindow));

			new LoginWindow(User?.Login ?? string.Empty)
				.Show();

			_window = null;

			Close();
		}

		private void OnStoreButtonClick(object sender, RoutedEventArgs e)
		{
			SetActivePage(new MainStorePage());
		}

		private void OnProfileButtonClick(object sender, RoutedEventArgs e)
		{
			if (User is User authorizedUser)
				SetActivePage(new UserProfile(authorizedUser.Id));
		}

		private void OnMyTeamClick(object sender, RoutedEventArgs e)
		{
			using var context = new GameStoreContext();

			var myTeam = context.Teams
				.Include(t => t.Members)
				.Where(t => t.Members.Any(m => m.UserId == User!.Id))
				.FirstOrDefault();

			if (myTeam != null)
			{
				SetActivePage(new TeamPage(myTeam));
				return;
			}

			var team = new Team { OwnerId = User!.Id };

			var created = new EditTeamWindow(team) { Owner = Instance }
					.ShowDialog() == true;

			if (created)
			{
				SetActivePage(new TeamPage(team));
			}
		}
	}
}