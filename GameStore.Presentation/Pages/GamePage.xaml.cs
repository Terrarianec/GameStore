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
		private readonly int _gameId;
		private GameReview _review = new();

		public GamePage(int gameId)
		{
			InitializeComponent();

			_gameId = gameId;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			var game = _context.Games
				.AsNoTracking()
				.Include(g => g.Team)
				.Include(g => g.Tags)
				.Include(g => g.Reviews)
				.ThenInclude(r => r.User)
				.Where(g => g.Id == _gameId)
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

			var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == _gameId);

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

			_context.Database.ExecuteSqlRaw("dbo.PurchaseGame @p0, @p1, @StatusCode OUT", _gameId, MainWindow.User!.Id, statusCode);

			if ((int)statusCode.Value != 0)
				MessageBox.Show("Не удалось приобрести игру");
			else
				MessageBox.Show("Приобретено");

			if (MainWindow.User != null)
				CheckPurchasedGame(MainWindow.User, game);
		}

		private async void CheckPurchasedGame(User user, Game game)
		{
			var purchasedGames = await _context.Games
				.Include(g => g.Users)
				.Where(g => g.Users.Any(u => u.Id == user.Id))
				.Select(g => g.Id)
				.ToListAsync();

			if (purchasedGames.Any(id => id == game.Id))
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
			if (_review.UserId != 0)
			{
				_context.GameReviews
					.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _gameId)
					.ExecuteUpdate(r => r
						.SetProperty(r => r.Rate, _review.Rate)
						.SetProperty(r => r.Content, _review.Content)
						.SetProperty(r => r.PublishDate, DateOnly.FromDateTime(DateTime.Now))
				);

				return;
			}

			_review.UserId = _review.User.Id;
			_review.User = null;

			_context.GameReviews.Add(_review);

			_context.SaveChanges();
		}

		private void OnDeleteReviewClick(object sender, RoutedEventArgs e)
		{
			if (_review.UserId > 0)
			{
				_context.GameReviews
					.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _gameId)
					.ExecuteDelete();
			}

			_review = new GameReview { GameId = _gameId, User = MainWindow.User!, Rate = 1, PublishDate = DateOnly.FromDateTime(DateTime.Now) };
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			new EditGameWindow(_gameId) { Owner = MainWindow.Instance }
				.ShowDialog();
		}
	}
}
