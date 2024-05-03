﻿using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
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
		private readonly Game _game;
		private GameReview _review = new();

		public GamePage(Game game)
		{
			InitializeComponent();

			_game = _context.Games
				.AsNoTracking()
				.Include(g => g.Team)
				.Include(g => g.Tags)
				.Include(g => g.Reviews)
				.ThenInclude(r => r.User)
				.Where(g => g.Id == game.Id)
				.FirstOrDefault() ?? new Game();

			DataContext = _game;

			if (_game?.Reviews.Count > 0)
			{
				var rate = _game.Reviews.Average(r => r.Rate);
				rateFrogs.Width = rateFrogs.MaxWidth * rate / 5;
				rateValue.Text = $"{rate:N2}";
			}

			reviewsListView.ItemsSource = _game!.Reviews.Where(r => r.UserId != MainWindow.User?.Id).ToList();

			if (MainWindow.User != null)
			{
				CheckPurchasedGame(MainWindow.User, _game!);

				_review = _game.Reviews
					.Where(r => r.UserId == MainWindow.User.Id)
					.FirstOrDefault()
					?? new GameReview { User = MainWindow.User, Rate = 1, PublishDate = DateOnly.FromDateTime(DateTime.Now) };

				reviewRate.Width = _review.Rate / 5D * reviewRate.MaxWidth;

				myReview.DataContext = _review;
			}
		}

		private void OnTeamClick(object sender, RoutedEventArgs e)
		{
			var team = (Team)((ContentControl)sender).Tag;

			MainWindow.SetActivePage(new TeamPage(team));
		}

		private async void OnPurcaseButtonClickAsync(object sender, RoutedEventArgs e)
		{
			if (MainWindow.User == null)
			{
				MessageBox.Show("Самое время зарегистрироваться!");
				return;
			}

			if (MainWindow.User?.Balance < _game.Price)
			{
				MessageBox.Show("Недостаточно средств");
				return;
			}

			var user = await _context.Users.Include(u => u.Games).FirstAsync(u => u.Id == MainWindow.User!.Id);
			var game = await _context.Games.FirstAsync(g => g.Id == _game.Id);

			user.Games.Add(game);

			await _context.SaveChangesAsync();

			MessageBox.Show("Приобретено :0");

			if (MainWindow.User != null)
				CheckPurchasedGame(MainWindow.User, _game);
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
					.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _game.Id)
					.ExecuteUpdate(r => r
						.SetProperty(r => r.Rate, _review.Rate)
						.SetProperty(r => r.Content, _review.Content)
						.SetProperty(r => r.PublishDate, DateOnly.FromDateTime(DateTime.Now))
				);

				return;
			}

			_context.GameReviews.Add(_review);

			_context.SaveChanges();
		}

		private void OnDeleteReviewClick(object sender, RoutedEventArgs e)
		{
			if (_review.UserId > 0)
			{
				_context.GameReviews
					.Where(r => r.UserId == MainWindow.User!.Id && r.GameId == _game.Id)
					.ExecuteDelete();
			}

			_review = new GameReview { User = MainWindow.User!, Rate = 1, PublishDate = DateOnly.FromDateTime(DateTime.Now) };
		}
	}
}
