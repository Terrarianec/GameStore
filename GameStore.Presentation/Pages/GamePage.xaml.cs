using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для GamePage.xaml
	/// </summary>
	public partial class GamePage : UserControl
	{
		private readonly GameStoreContext _context = new();
		private readonly int _id;
		private GameReview _review = new();

		public GamePage(int gameId)
		{
			InitializeComponent();

			_id = gameId;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			var game = _context.Games
				.AsNoTracking()
				.Include(g => g.Team)
				.Include(g => g.Tags)
				.Include(g => g.Reviews)
				.ThenInclude(r => r.User)
				.Where(g => g.Id == _id)
				.FirstOrDefault() ?? new Game();

			DataContext = game;

			if (game?.Reviews.Count > 0)
			{
				var rate = game.Reviews.Average(r => r.Rate);
				rateFrogs.Width = rateFrogs.MaxWidth * rate / 5;
				rateValue.Text = $"{rate:N2}";
			}

			reviewsListView.ItemsSource = game!.Reviews.Where(r => r.UserId != MainWindow.User?.Id).ToList();

			if (MainWindow.User != null)
			{
				CheckPurchasedGame(MainWindow.User, game!);

				_review = game.Reviews
					.Where(r => r.UserId == MainWindow.User.Id)
					.FirstOrDefault()
					?? new GameReview { GameId = game.Id, User = MainWindow.User, Rate = 1, PublishDate = DateOnly.FromDateTime(DateTime.Now) };

				reviewRate.Width = _review.Rate / 5D * reviewRate.MaxWidth;

				myReview.DataContext = _review;

				editButton.Visibility = await _context.IsDeveloperOfGame(_id, MainWindow.User.Id) == true
					? Visibility.Visible
					: Visibility.Collapsed;
				deleteButton.Visibility = await _context.IsOwnerOfGame(_id, MainWindow.User.Id) == true
					? Visibility.Visible
					: Visibility.Collapsed;
			}
		}

		private void OnTeamClick(object sender, RoutedEventArgs e)
		{
			var team = (Team)((ContentControl)sender).Tag;

			MainWindow.SetActivePage(new TeamPage(team.Id));
		}

		private async void OnPurchaseButtonClick(object sender, RoutedEventArgs e)
		{
			if (MainWindow.User == null)
			{
				MessageBox.Show("Самое время зарегистрироваться!");
				return;
			}

			var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == _id);

			if (MainWindow.User.Balance < game!.Price)
			{
				MessageBox.Show("Недостаточно средств");
				return;
			}

			SqlParameter statusCode = new()
			{
				ParameterName = "@StatusCode",
				SqlDbType = System.Data.SqlDbType.Int,
				Direction = System.Data.ParameterDirection.Output
			};

			try
			{
				_context.Database.ExecuteSqlRaw("dbo.PurchaseGame @p0, @p1, @StatusCode OUT", _id, MainWindow.User!.Id, statusCode);

				if ((int)statusCode.Value != 0)
					MessageBox.Show("Не удалось приобрести игру");
				else
					MessageBox.Show("Приобретено");
			}
			catch
			{
				MessageBox.Show("Не удалось приобрести игру");
			}

			if (MainWindow.User != null)
				CheckPurchasedGame(MainWindow.User, game);
		}

		private async void CheckPurchasedGame(User user, Game game)
		{
			if (await _context.IsGamePurchased(game.Id, user.Id) == true)
			{
				purchaseButton.Visibility = Visibility.Hidden;
				playButton.Visibility = Visibility.Visible;
			}
		}

		private void OnPlayButtonClick(object sender, RoutedEventArgs e)
		{
			new GameWindow() { Owner = MainWindow.Instance }
				.ShowDialog();
		}

		private void OnReviewContentChanged(object sender, TextChangedEventArgs e)
		{
			var reviewContent = (TextBox)sender;
			charactersLeftIndicator.Text = $"{reviewContent.MaxLength - reviewContent.Text.Length}";
		}

		private void OnRateChanged(object sender, RoutedEventArgs e)
		{
			var selectedFrog = (Button)sender;
			var rate = ((StackPanel)selectedFrog.Parent).Children.IndexOf(selectedFrog) + 1;

			if (_review is GameReview myReview)
				myReview.Rate = rate;

			reviewRate.Width = rate / 5D * reviewRate.MaxWidth;
		}

		private void OnPublishClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (_review.UserId != 0)
				{
					_context.GameReviews
						.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _id)
						.ExecuteUpdate(r => r
							.SetProperty(r => r.Rate, _review.Rate)
							.SetProperty(r => r.Content, _review.Content)
							.SetProperty(r => r.PublishDate, DateOnly.FromDateTime(DateTime.Now))
					);

					MessageBox.Show("Оценка обновлена", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

					return;
				}

				_review.UserId = _review.User.Id;
				_review.User = null;

				_context.GameReviews.Add(_review);

				_context.SaveChanges();

				MessageBox.Show("Оценка создана", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.Data}\n\n{ex.StackTrace}");
#if DEBUG
				throw;
#endif
			}
		}

		private void OnDeleteReviewClick(object sender, RoutedEventArgs e)
		{
			try
			{
				if (_review.UserId > 0)
				{
					_context.GameReviews
						.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _id)
						.ExecuteDelete();
				}

				_review = new GameReview { GameId = _id, User = MainWindow.User!, Rate = 1, PublishDate = DateOnly.FromDateTime(DateTime.Now) };
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.Data}\n\n{ex.StackTrace}");
#if DEBUG
				throw;
#endif
			}
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			if (new EditGameWindow(_id) { Owner = MainWindow.Instance }.ShowDialog() == true)
				MainWindow.SetActivePage(new GamePage(_id));
		}

		private void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Вы уверены, что хотите удалить игру? Это действие необратимо.", "Удаление игры", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
			{
				try
				{
					_context.Games
						.Where(g => g.Id == _id)
						.ExecuteDelete();

					MainWindow.SetActivePage(new MainStorePage());
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
}
