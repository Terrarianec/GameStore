using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DataFormats = System.Windows.Forms.DataFormats;
using DragEventArgs = System.Windows.DragEventArgs;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.Forms.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Rectangle = System.Drawing.Rectangle;
using UserControl = System.Windows.Controls.UserControl;

namespace GameStore.Presentation.Controls
{
	/// <summary>
	/// Логика взаимодействия для UploadImageArea.xaml
	/// </summary>
	public partial class UploadImageArea : UserControl
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(byte[]), typeof(UploadImageArea));

		private readonly OpenFileDialog _fileDialog = new()
		{
			Title = "Выбор изображения",
			Filter = "Изображения (*.png, *.jpg, *.gif)|*.png;*.jpg;*.gif"
		};
		private Uri _uri;

		public Uri TargetNullUri
		{
			get => _uri;
			set
			{
				_uri = value;

				targetNullBitmap.UriSource = _uri;
			}
		}
		public byte[]? Source
		{
			get => (byte[])GetValue(SourceProperty);
			set
			{
				SetValue(SourceProperty, value);

				DataContext = null;
				DataContext = this;
			}
		}

		public UploadImageArea()
		{
			InitializeComponent();

			Source = null;
		}

		private void HandleImage(string path)
		{
			try
			{
				var image = Image.FromFile(path);
				Source = GetResizedImage(image, 256, 256);
			}
			catch (Exception)
			{
				MessageBox.Show("Не удалось загрузить изображение");
				return;
			}
		}

		public static byte[]? GetResizedImage(Image image, int width, int height)
		{
			var area = new Rectangle(0, 0, width, height);
			var destinationImage = new Bitmap(width, height);
			destinationImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
			using (var graphics = Graphics.FromImage(destinationImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using var wrapMode = new ImageAttributes();

				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, area, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}

			ImageConverter converter = new();
			return (byte[]?)converter.ConvertTo(destinationImage, typeof(byte[]));
		}

		private void OnImageDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) == false) return;

			var files = (string[])e.Data.GetData(DataFormats.FileDrop);

			HandleImage(files[0]);
		}

		private void OnImageClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left) return;

			if (_fileDialog.ShowDialog() == DialogResult.OK)
			{
				HandleImage(_fileDialog.FileName);
			}
		}

		private void OnMouseEnterImage(object sender, MouseEventArgs e)
		{
			uploadImageGrid.Visibility = Visibility.Visible;
		}

		private void OnMouseLeaveImage(object sender, MouseEventArgs e)
		{
			uploadImageGrid.Visibility = Visibility.Hidden;
		}
	}
}
