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
using sorbe.Utilities;

namespace sorbe
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var controller = FireBaseController.Instance;
            LoadDataAsync(controller);


        }
        private async void LoadDataAsync(FireBaseController fireBaseController)
        {
            List<Dictionary<string, object>> list = await fireBaseController.ViewData("projects");
            MainPage mainPage = new MainPage(list);
            Main.Content = mainPage;
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
            var controller = FireBaseController.Instance;
            LoadDataAsync(controller);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {

            ProfilePage profilePage = new ProfilePage();
            Main.Content = profilePage;
        }
    }

}