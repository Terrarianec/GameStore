using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GameStore.Presentation.Controls
{
	/// <summary>
	/// Логика взаимодействия для CaptchaBox.xaml
	/// </summary>
	public partial class CaptchaBox : UserControl
	{
		public static readonly DependencyProperty QuestionProperty = DependencyProperty.Register(nameof(Question), typeof(object), typeof(CaptchaBox), new PropertyMetadata(OnQuestionChanged));

		private static void OnQuestionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (CaptchaBox)d;

			instance.questionLabel.Content = e.NewValue;
		}

		public object Question
		{
			get => GetValue(QuestionProperty);
			set => SetValue(QuestionProperty, value);
		}
		public string ExpectedAnswer { get; set; } = string.Empty;
		public bool IsSuccessful { get; private set; } = false;

		public CaptchaBox()
		{
			InitializeComponent();
		}

		private void OnClicked(object sender, RoutedEventArgs e)
		{
			popup.IsOpen = true;
			answerBox.Focus();

			e.Handled = true;
		}

		private void OnSubmit(object sender, RoutedEventArgs e)
		{
			IsSuccessful = answerBox.Text == ExpectedAnswer;

			if (IsSuccessful)
			{
				checkBox.IsEnabled = false;
				popup.IsOpen = false;

				var animation = new DoubleAnimation(0, 1, TimeSpan.FromMicroseconds(2500));
				checkmark.BeginAnimation(OpacityProperty, animation);
			}
		}
	}
}
