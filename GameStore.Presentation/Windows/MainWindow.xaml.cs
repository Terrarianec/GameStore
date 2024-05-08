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
			KeyDown += (s, e) =>
			{
				var context = new GameStoreContext();
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
				SetActivePage(new TeamPage(myTeam.Id));
				return;
			}

			var team = new Team { OwnerId = User!.Id };

			var created = new EditTeamWindow(team) { Owner = Instance }
					.ShowDialog() == true;

			if (created)
			{
				SetActivePage(new TeamPage(team.Id));
			}
		}

		private void OnMinimizeClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private async void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			using var context = new GameStoreContext();

			if (await context.TeamMembers.AnyAsync(m => m.UserId == User!.Id))
			{
				MessageBox.Show("Пока вы не покинете команду, вы не можете удалить свой аккаунт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			try
			{
				await context.Users
					.Where(u => u.Id == User!.Id)
					.ExecuteDeleteAsync();

				MessageBox.Show("Аккаунт больше не существует", "", MessageBoxButton.OK, MessageBoxImage.Information);

				TempStorage.Delete(nameof(LoginWindow));

				new LoginWindow(User?.Login ?? string.Empty)
					.Show();

				_window = null;

				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.Data}\n\n{ex.StackTrace}");
#if DEBUG
				throw;
#endif
			}
		}
	}
}