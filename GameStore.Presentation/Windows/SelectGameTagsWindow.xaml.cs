using GameStore.DB;
using GameStore.DB.Models;
using System.Windows;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для SelectGameTagsWindow.xaml
	/// </summary>
	public partial class SelectGameTagsWindow : Window
	{
		private readonly GameStoreContext _context = new();
		private readonly Game _game;

		public SelectGameTagsWindow(Game game)
		{
			InitializeComponent();

			var allTags = _context.Tags.ToList();
			tags.ItemsSource = allTags;

			_game = game;

			tags.SelectionChanged -= OnSelectionChanged;

			foreach (var tag in game.Tags)
			{
				var selected = allTags.First(t => t.Id == tag.Id);

				tags.SelectedItems.Add(selected);
			}

			tags.SelectionChanged += OnSelectionChanged;
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			foreach (Tag added in e.AddedItems)
				_game.Tags.Add(added);

			foreach (Tag removed in e.RemovedItems)
				_game.Tags.Remove(removed);
		}
	}
}
