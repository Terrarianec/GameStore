using GameStore.DB.Models;
using GameStore.Presentation.Pages;
using GameStore.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace GameStore.Presentation.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow? _window = null;
        private static User? _user = null;

        public static MainWindow Instance
        {
            get
            {
                return _window ??= new MainWindow(User);
            }
            private set
            {
                _window = value;
            }
        }
        public static User? User {  
        get => _user; 
        set  {
        _user = value;
        UpdateUser(_user);
        } }

        private MainWindow(User? user)
        {
            InitializeComponent();

            if (user is User authorizedUser)
                DataContext = authorizedUser;
        }

        public static void UpdateUser(User? user)
        {
            Instance.DataContext = user;
        }

        public static void SetActivePage(UserControl page)
        {
            var pageContainer = Instance.currentPage;

            pageContainer.Children.Clear();
            pageContainer.Children.Add(page);
        }

        private void OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            TempStorage.Delete(nameof(LoginWindow));

            new LoginWindow(User?.Login ?? string.Empty)
                .Show();

            _window = null;

            Close();
        }

        private void OnStoreButtonClick(object sender, RoutedEventArgs e)
        {
            SetActivePage(new MainStorePage());
        }

        private void OnProfileButtonClick(object sender, RoutedEventArgs e)
        {
            if (User is User authorizedUser)
                SetActivePage(new UserProfile(authorizedUser.Id));
        }
    }
}