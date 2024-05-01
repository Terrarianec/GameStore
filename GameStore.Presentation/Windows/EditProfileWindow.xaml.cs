using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для EditProfile.xaml
	/// </summary>
	public partial class EditProfileWindow : Window
	{
		private readonly User _user;

		public EditProfileWindow(User user)
		{
			InitializeComponent();

			_user = user;

			DataContext = _user;

			avatar.Source = _user.Avatar;
			dateOfBirthField.SelectedDate = _user.DateOfBirth.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero));
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			if (!CheckFields())
				return;

			using var context = new GameStoreContext();

			_user.DateOfBirth = DateOnly.FromDateTime(dateOfBirthField.SelectedDate ?? DateTime.Now);
			_user.Avatar = avatar.Source;

			try
			{
				context.Users
					.Where(u => u.Id == _user.Id)
					.ExecuteUpdate(u => u
						.SetProperty(u => u.Username, _user.Username)
						.SetProperty(u => u.DateOfBirth, _user.DateOfBirth)
						.SetProperty(u => u.Avatar, _user.Avatar)
					);

				MainWindow.User = _user;
				DialogResult = true;

				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.Data}\n\n{ex.StackTrace}");
			}
		}

		private bool CheckFields()
		{
			var errors = new StringBuilder();

			if (dateOfBirthField.SelectedDate is DateTime selectedDate)
			{
				if (DateTime.Now - selectedDate < TimeSpan.FromDays(18 * 365))
					errors.AppendLine("Недостаточный возраст");

				if (DateTime.Now - selectedDate > TimeSpan.FromDays(115 * 365))
					errors.AppendLine("Избыточный возраст");
			}
			else
			{
				errors.AppendLine("Не указана дата рождения");
			}

			if (errors.Length == 0)
			{
				return true;
			}

			MessageBox.Show(errors.ToString(), "Возникли проблемы", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}
}
