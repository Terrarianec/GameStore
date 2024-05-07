using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace GameStore.Presentation.Pages
{
	/// <summary>
	/// Логика взаимодействия для TeamPage.xaml
	/// </summary>
	public partial class TeamPage : UserControl
	{
		private readonly GameStoreContext _context = new();
		private readonly int _id;

		public TeamPage(int teamId)
		{
			InitializeComponent();

			_id = teamId;
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			var team = _context.Teams
				.AsNoTracking()
				.Include(t => t.Owner)
				.Include(t => t.Members)
					.ThenInclude(m => m.User)
				.Include(t => t.Games)
					.ThenInclude(g => g.Tags)
				.Where(t => t.Id == _id)
				.FirstOrDefault();

			membersListView.ItemsSource = team?.Members
				.Where(m => m.UserId != team.OwnerId)
				.Select(m => m.User)
				.ToList();


			var isOwner = await _context.IsOwnerOfTeam(_id, MainWindow.User!.Id) == true;
			var isMember = await _context.IsMemberOfTeam(_id, MainWindow.User!.Id) == true;

			ownerPanel.Visibility = isOwner
				? Visibility.Visible
				: Visibility.Collapsed;

			leaveButton.Visibility = !isOwner && isMember
				? Visibility.Visible
				: Visibility.Collapsed;

			editButton.IsEnabled = isMember;
			editButton.Cursor = isMember ? Cursors.Hand : Cursors.Arrow;

			DataContext = team;
		}

		private void OnEditClick(object sender, RoutedEventArgs e)
		{
			var changed = new EditTeamWindow((DataContext as Team)!) { Owner = MainWindow.Instance }
					.ShowDialog() == true;

			if (changed)
			{
				MainWindow.SetActivePage(new TeamPage(_id));
			}
		}

		private void OnGameClick(object sender, RoutedEventArgs e)
		{
			var game = (Game)((ContentControl)sender).Tag;

			MainWindow.SetActivePage(new GamePage(game.Id));
		}

		private void OnSetOwnerClick(object sender, RoutedEventArgs e)
		{
			var user = (User)((ContentControl)sender).Tag;

			_context.Teams
				.Where(t => t.Id == _id)
				.ExecuteUpdate(t => t.SetProperty(t => t.OwnerId, user.Id));

			MessageBox.Show($"Владелец команды сменён на {user.Username}");

			MainWindow.SetActivePage(new TeamPage(_id));
		}

		private void OnKickClick(object sender, RoutedEventArgs e)
		{
			var user = (User)((ContentControl)sender).Tag;

			_context.TeamMembers
				.Where(m => m.UserId == user.Id)
				.ExecuteDelete();

			System.Windows.MessageBox.Show($"{user.Username} больше не участник этой команды");

			MainWindow.SetActivePage(new TeamPage(_id));
		}

		private void OnManageButtonClick(object sender, RoutedEventArgs e)
		{
			new AddMembersWindow(new Team { Id = _id }) { Owner = MainWindow.Instance }
				.ShowDialog();

			MainWindow.SetActivePage(new TeamPage(_id));
		}

		private void OnLeaveClick(object sender, RoutedEventArgs e)
		{
			_context.TeamMembers
				.Where(m => m.UserId == MainWindow.User!.Id)
				.ExecuteDelete();

			MainWindow.SetActivePage(new TeamPage(_id));
		}

		private void OnDeleteClick(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Это необратимое действие. Вы уверены?", "Удаление команды", MessageBoxButton.YesNo, MessageBoxImage.Stop) == MessageBoxResult.Yes)
			{
				_context.Teams
					.Where(t => t.Id == _id)
					.ExecuteDelete();

				MainWindow.SetActivePage(new MainStorePage());
			}
		}

		private void OnCreateGameClick(object sender, RoutedEventArgs e)
		{
			var game = new Game
			{
				Name = "Невероятная игра",
				Description = "Невероятное описание",
				PublishDate = DateOnly.FromDateTime(DateTime.Now),
				TeamId = _id
			};
			_context.Games.Add(game);
			_context.SaveChanges();

			var result = new EditGameWindow(game.Id) { Owner = MainWindow.Instance }
				.ShowDialog();

			if (result == true)
				MainWindow.SetActivePage(new TeamPage(_id));
			else
				_context.Games
					.Where(g => g.Id == game.Id)
					.ExecuteDelete();
		}
	}
}
