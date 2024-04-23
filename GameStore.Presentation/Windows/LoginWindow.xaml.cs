using GameStore.DB;
using GameStore.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для AuthorizationWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private readonly string _key = "40";

		public static string AuthPath => nameof(LoginWindow);

		public LoginWindow(string login = "", string password = "") : this()
		{
			loginField.Text = login;
			passwordField.Password = password;
		}

		public LoginWindow()
		{
			InitializeComponent();
		}

		public static string GetPasswordHash(string password)
		{
			var bytes = Encoding.UTF8.GetBytes(password);
			var hashBytes = MD5.HashData(bytes);

			return Convert.ToHexString(hashBytes);
		}

		private void Authorize(string login, string passwordHash)
		{
			using var context = new GameStoreContext();

			var user = context.Users
				.AsNoTracking()
				.Where(u => u.Login == login && u.PasswordHash == passwordHash)
				.FirstOrDefault();

			if (user == null)
			{
				MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			TempStorage.Write(AuthPath, EasyEncryption.Encrypt($"{login};{passwordHash}", _key));

			SessionStorage.User = user;

			MainWindow.Instance.Show();

			Close();
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			try
			{
				var text = TempStorage.Read(AuthPath);
				var authData = EasyEncryption.Decrypt(text, _key).Split(";");

				var login = authData[0];
				var passwordHash = authData[1];

				Authorize(login, passwordHash);
			}
			catch { }
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			if (captcha.IsSuccessful)
				Authorize(loginField.Text, GetPasswordHash(passwordField.Password));
			else
				MessageBox.Show("Роботам вход воспрещён", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
		{
			new RegisterWindow(loginField.Text, passwordField.Password)
				.Show();
			Close();
		}
	}
}
