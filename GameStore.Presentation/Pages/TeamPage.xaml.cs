using GameStore.DB;
using GameStore.DB.Models;
using GameStore.Presentation.Windows;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

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

		private void OnLoaded(object sender, RoutedEventArgs e)
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

			if (team?.OwnerId == MainWindow.User?.Id)
				ownerPanel.Visibility = Visibility.Visible;
			else if (team?.Members.Any(u => u.UserId == MainWindow.User?.Id) ?? false)
				leaveButton.Visibility = Visibility.Visible;

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

			MessageBox.Show($"{user.Username} больше не участник этой команды");

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
	}
}
