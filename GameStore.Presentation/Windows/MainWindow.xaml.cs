using GameStore.DB.Models;
using GameStore.Presentation.Pages;
using GameStore.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly MainWindow _window = new MainWindow();

		public static MainWindow Instance
		{
			get
			{
				return _window;
			}
		}

		private MainWindow()
		{
			InitializeComponent();

			if (SessionStorage.User is User authorizedUser)
				DataContext = (authorizedUser);
		}

		public static void UpdateUser(User user)
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

			new LoginWindow(SessionStorage.User?.Login ?? string.Empty)
				.Show();

			Close();
		}

		private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnCloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnStoreButtonClick(object sender, RoutedEventArgs e)
		{
			SetActivePage(new MainStorePage());
		}

		private void OnProfileButtonClick(object sender, RoutedEventArgs e)
		{
			if (SessionStorage.User is User authorizedUser)
				SetActivePage(new UserProfile(authorizedUser.Id));
		}
	}
}