using GameStore.DB;
using GameStore.DB.Models;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для RegisterWindow.xaml
	/// </summary>
	public partial class RegisterWindow : Window
	{
		private readonly Regex loginPattern = new(@"[-._a-z0-9]{4,32}");
		private readonly User _user = new();

		public RegisterWindow(string login = "", string password = "")
		{
			InitializeComponent();
			DataContext = _user;

			loginField.Text = login;
			passwordField.Password = password;
			dateOfBirthField.SelectedDate = DateTime.Now - TimeSpan.FromDays(18 * 365);
		}

		private void OnLoginButtonClick(object sender, RoutedEventArgs e)
		{
			new LoginWindow()
							.Show();
			Close();
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			if (!captcha.IsSuccessful)
			{
				MessageBox.Show("Роботам вход воспрещён", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (!CheckFields())
				return;

			using var context = new GameStoreContext();

			_user.PasswordHash = LoginWindow.GetPasswordHash(passwordField.Password);
			_user.DateOfBirth = DateOnly.FromDateTime(dateOfBirthField.SelectedDate ?? DateTime.Now);
			_user.Avatar = avatar.Source;

			context.Users.Add(_user);

			try
			{
				context.SaveChanges();

				new LoginWindow(loginField.Text, passwordField.Password).Show();

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

		private bool CheckFields()
		{
			var errors = new StringBuilder();

			if (loginPattern.IsMatch(loginField.Text) == false)
			{
				errors.AppendLine($"Неверный формат логина (не соответствует паттерну {loginPattern})");
			}

			using (var context = new GameStoreContext())
			{
				if (context.Users.Any(u => u.Login == loginField.Text))
					errors.AppendLine($"Логин '{loginField.Text}' уже занят");
			}

			if (usernameField.Text.Length < 4)
				errors.AppendLine("Имя пользователя не может быть короче 4 символов");

			if (dateOfBirthField.SelectedDate is DateTime selectedDate)
			{
				if (DateTime.Now - selectedDate < TimeSpan.FromDays(18 * 365))
					errors.AppendLine("Недостаточный возраст для регистрации");

				if (DateTime.Now - selectedDate > TimeSpan.FromDays(115 * 365))
					errors.AppendLine("Избыточный возраст для регистрации");
			}
			else
			{
				errors.AppendLine("Не указана дата рождения");
			}

			if (passwordField.Password != repeatedPasswordField.Password)
				errors.AppendLine("Пароли не совпадают");

			if (errors.Length == 0)
			{
				return true;
			}

			MessageBox.Show(errors.ToString(), "Возникли проблемы", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		private void OnUsernameInputEnded(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(loginField.Text))
				loginField.Text = usernameField.Text;
		}

		private void OnLoginChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			loginField.TextChanged -= OnLoginChanged;

			var position = loginField.SelectionStart;
			loginField.Text = new Regex(@"[^-._a-z0-9]").Replace(loginField.Text.ToLower(), "_");
			loginField.SelectionStart = position;

			loginField.TextChanged += OnLoginChanged;
		}
	}
}
