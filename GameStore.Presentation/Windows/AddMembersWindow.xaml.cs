using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для ManageTeamMembersWindow.xaml
	/// </summary>
	public partial class AddMembersWindow : Window
	{
		private readonly GameStoreContext _context = new();
		private readonly int _id;

		public AddMembersWindow(Team team)
		{
			InitializeComponent();

			_id = team.Id;

			membersListView.ItemsSource = _context.Users
				.AsNoTracking()
				.Include(u => u.Member)
				.Where(u => u.Member == null)
				.ToList();
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			var users = new List<User>();

			foreach (User user in membersListView.SelectedItems)
			{
				users.Add(user);
			}

			_context.TeamMembers
				.AddRange(users.Select(u => new Member { TeamId = _id, UserId = u.Id }));

			_context.SaveChanges();

			Close();
		}
	}
}
