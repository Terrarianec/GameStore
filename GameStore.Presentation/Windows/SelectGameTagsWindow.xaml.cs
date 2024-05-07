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

			_game = game;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{

			var allTags = _context.Tags.ToList();
			tags.ItemsSource = allTags;

			tags.SelectionChanged -= OnSelectionChanged;

			foreach (var tag in _game.Tags)
			{
				var selected = allTags.FirstOrDefault(t => t.Id == tag.Id);

				if (selected is not null)
					tags.SelectedItems.Add(selected);
			}

			tags.SelectionChanged += OnSelectionChanged;

			searchField.Focus();
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

		private void OnSearchValueChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			tags.SelectionChanged -= OnSelectionChanged;

			var selectedTags = (searchField.Text.Length > 0
				? _context.Tags.Where(t => t.Name.ToLower().Contains(searchField.Text.ToLower()))
				: _context.Tags)
				.ToList();

			tags.ItemsSource = selectedTags;


			foreach (var tag in _game.Tags)
			{
				var selected = selectedTags.FirstOrDefault(t => t.Id == tag.Id);

				if (selected is not null)
					tags.SelectedItems.Add(selected);
			}

			tags.SelectionChanged += OnSelectionChanged;
		}
	}
}
