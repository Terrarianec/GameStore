using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для EditGameWindow.xaml
	/// </summary>
	public partial class EditGameWindow : Window
	{
		private readonly Game _game;
		private readonly GameStoreContext _context = new();
		public EditGameWindow(int id)
		{
			InitializeComponent();

			_game = _context.Games
				.Include(g => g.Tags)
				.FirstOrDefault(g => g.Id == id) ?? new Game() { PublishDate = DateOnly.FromDateTime(DateTime.Now) };

			logo.Source = _game.Logo;
			publishDate.SelectedDate = _game.PublishDate.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.Zero));

			DataContext = _game;
		}
		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			if (!CheckFields())
				return;

			using var context = new GameStoreContext();

			_game.PublishDate = DateOnly.FromDateTime(publishDate.SelectedDate ?? DateTime.Now);
			_game.Logo = logo.Source;

			var tags = _game.Tags.Select(t => t.Id).ToArray();

			try
			{
				_game.Tags = _context.Tags.Where(t => tags.Contains(t.Id)).ToList();

				if (_game.Id == 0)
					_context.Games.Add(_game);

				_context.SaveChanges();

				DialogResult = true;

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

			if (nameField.Text.Length < 4)
				errors.AppendLine("Название не может быть короче 4 символов");

			if (publishDate.SelectedDate is not DateTime)
			{
				errors.AppendLine("Не указана дата выхода");
			}

			if (errors.Length == 0)
			{
				return true;
			}

			MessageBox.Show(errors.ToString(), "Возникли проблемы", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}

		private void OnTagsClick(object sender, RoutedEventArgs e)
		{
			new SelectGameTagsWindow(_game) { Owner = this }
				.ShowDialog();
		}
	}
}
