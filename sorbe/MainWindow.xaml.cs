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
            LoadDataAsync();


        }
        private async void LoadDataAsync()
        {
            List<Dictionary<string, object>> list = await FireBaseController.Instance.ViewData("projects");
            MainPage mainPage = new MainPage(list);
            Main.Content = mainPage;
        }
        private async void AddDataAsync(FireBaseController fireBaseController)
        {
           
        }

        private void ButtonSignIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Реєстрація";
            TextboxPasswordCheck.Visibility = Visibility.Visible;
            NonLogInText.Visibility = Visibility.Visible;
            DbLogButton.Visibility = Visibility.Collapsed;                                         
            DbSignButton.Visibility = Visibility.Visible;
            ButtonShow.Visibility = Visibility.Visible;
            Name.Visibility = Visibility.Visible;
            TextboxName.Visibility = Visibility.Visible;

            TextboxPasswordCheckShow.Text = "";
            TextboxPasswordShow.Text = "";
            TextboxEmail.Text = "";
            TextboxPassword.Password = "";
            TextboxPasswordCheck.Password = "";
            TextboxName.Text = "";
        }

        private void AcountEnterPanelExit_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Collapsed;
        }

        private void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            AcountEnterPanel.Visibility = Visibility.Visible;
            AcountEnterTitle.Content = "Авторизація";
            TextboxPasswordCheck.Visibility = Visibility.Collapsed;
            NonLogInText.Visibility = Visibility.Collapsed;
            DbLogButton.Visibility = Visibility.Visible;
            DbSignButton.Visibility = Visibility.Collapsed;
            ButtonShow.Visibility = Visibility.Collapsed;
            Name.Visibility = Visibility.Collapsed;
            TextboxName.Visibility = Visibility.Collapsed;

            TextboxPasswordCheckShow.Text = "";
            TextboxPasswordShow.Text = "";
            TextboxEmail.Text = "";
            TextboxPassword.Password = "";
            TextboxPasswordCheck.Password = "";
            
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {

            LoadDataAsync();
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            var controller = FireBaseController.Instance;
            ProfilePage profilePage = new ProfilePage();
            Main.Content = profilePage;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Visibility = Visibility.Collapsed;
            if (TextboxPasswordCheck.Password == TextboxPassword.Password)
            {
                FireBaseController.Instance.UserRegistration(TextboxEmail.Text, TextboxPassword.Password,TextboxName.Text);
            }
            else
            {
                ErrorLabel.Visibility = Visibility.Visible;
                ErrorLabel.Content = "Паролі не співпадають";
            }
        }

        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (TextboxPasswordShow.Visibility == Visibility.Visible)
            {
                TextboxPasswordShow.Visibility = Visibility.Collapsed;
                TextboxPassword.Password = TextboxPasswordShow.Text;
                TextboxPassword.Visibility = Visibility.Visible;
            }
            else
            {
                TextboxPassword.Visibility = Visibility.Collapsed;
                TextboxPasswordShow.Text = TextboxPassword.Password;
                TextboxPasswordShow.Visibility = Visibility.Visible;
            }
        }

        private void ShowPasswordCheck_Click(object sender, RoutedEventArgs e)
        {
            if (TextboxPasswordCheckShow.Visibility == Visibility.Visible)
            {
                TextboxPasswordCheckShow.Visibility = Visibility.Collapsed ;
                TextboxPasswordCheck.Password = TextboxPasswordCheckShow.Text;
                TextboxPasswordCheck.Visibility = Visibility.Visible;
            }
            else
            {
                TextboxPasswordCheck.Visibility = Visibility.Collapsed;
                TextboxPasswordCheckShow.Text = TextboxPasswordCheck.Password;
                TextboxPasswordCheckShow.Visibility = Visibility.Visible;
            }  
        }

        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            ///FireBaseController.Instance.UserAuth(TextboxEmail.Text, TextboxPassword.Password);
        }
    }

}