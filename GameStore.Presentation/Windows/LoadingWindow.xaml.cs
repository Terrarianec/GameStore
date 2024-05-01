using System.Windows;
using System.Windows.Threading;

namespace GameStore.Presentation.Windows
{
	/// <summary>
	/// Логика взаимодействия для LoadingWindow.xaml
	/// </summary>
	public partial class LoadingWindow : Window
	{
		private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(1) };
		private readonly double step = 10;
		public LoadingWindow()
		{
			InitializeComponent();
			_timer.Start();

			_timer.Tick += (s, e) =>
			{
				rotateTransform.Angle += step;
			};
		}
	}
}
