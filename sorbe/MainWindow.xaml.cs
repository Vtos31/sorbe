using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sorbe
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainPage mainPage = new MainPage();
            Main.Content = mainPage;
        }
        public void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {
            InfoPage InfoPage = new InfoPage();
            Main.Content = InfoPage;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Реєстрація";
            NonLogInPole.Visibility = Visibility.Visible;
            NonLogInText.Visibility = Visibility.Visible;
        }

        private void AcountEnterPanelExit_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Collapsed;
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Авторизація";
            NonLogInPole.Visibility = Visibility.Collapsed;
            NonLogInText.Visibility = Visibility.Collapsed;
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainPage mainPage = new MainPage();
            Main.Content = mainPage;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {

            ArtistPage profilePage = new ArtistPage();
            Main.Content = profilePage;
        }
    }

}