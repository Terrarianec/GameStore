using GameStore.DB;
using GameStore.DB.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для EditTeamWindow.xaml
	/// </summary>
	public partial class EditTeamWindow : Window
	{
		private readonly Team _team;

		public EditTeamWindow(Team team)
		{
			InitializeComponent();

			_team = team;

			logo.Source = _team.Logo;

			DataContext = _team;
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			if (!CheckFields())
				return;

			using var context = new GameStoreContext();
			_team.Logo = logo.Source;

			try
			{
				if (_team.Id > 0)
				{

					context.Teams
						.Where(t => t.Id == _team.Id)
						.ExecuteUpdate(t => t
							.SetProperty(t => t.Name, _team.Name)
							.SetProperty(t => t.Logo, _team.Logo)
						);
				}
				else
				{
					context.Teams
						.Add(_team);

					context.SaveChanges();
				}

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

			if (errors.Length == 0)
			{
				return true;
			}

			MessageBox.Show(errors.ToString(), "Возникли проблемы", MessageBoxButton.OK, MessageBoxImage.Error);
			return false;
		}
	}
}
