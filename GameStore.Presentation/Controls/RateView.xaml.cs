using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Controls
{
	/// <summary>
	/// Логика взаимодействия для RateView.xaml
	/// </summary>
	public partial class RateView : UserControl
	{
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(RateView), new PropertyMetadata(OnValueChanged));
		public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(RateView), new PropertyMetadata(OnMaxValueChanged));

		public double Value
		{
			get => (double)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
		public double MaxValue
		{
			get => (double)GetValue(MaxValueProperty);
			set => SetValue(MaxValueProperty, value);
		}

		public RateView()
		{
			InitializeComponent();
		}

		private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (RateView)d;
			var value = (double)e.NewValue;

			instance.SetRate(value, instance.MaxValue);
		}

		private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (RateView)d;
			var value = (double)e.NewValue;

			instance.SetRate(instance.Value, value);
		}

		private void OnLoaded(object sender, RoutedEventArgs e) => SetRate(Value, MaxValue);

		private void SetRate(double value, double maxValue)
		{
			rateFrogs.Width = value * stackPanel.ActualWidth / maxValue;
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			foreach (Image item in rateFrogs.Children)
				item.Width = ActualWidth / MaxValue;
		}
	}
}
