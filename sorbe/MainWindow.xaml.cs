using System.IO;
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
using static Google.Apis.Requests.BatchRequest;

namespace sorbe
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            _ = InitializeAppAsync();
            InitializeComponent();
            LoadDataAsync();
           
        }
        private async Task InitializeAppAsync()
        {
            await FireBaseController.Instance.InitializeUidAsync(); 

            if (FireBaseController.Instance.Uid != null)
            {
                ButtonLogInPanelOpen.Visibility = Visibility.Collapsed;
                ButtonSignInPanelOpen.Visibility = Visibility.Collapsed;
            }
        }
        private async void LoadDataAsync()
        {
            List<Dictionary<string, object>> list = await FireBaseController.Instance.ViewData("projects");
            MainPage mainPage = new MainPage(list);
            Main.Content = mainPage;
        }
        private async Task<Dictionary<string, object>> LoadUserDataAsync()
        {
            Dictionary<string, object> user = await FireBaseController.Instance.ViewData("users", FireBaseController.Instance.Uid);
            return user;
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

        private async void Profile_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> user = await LoadUserDataAsync();
            ProfilePage profilePage = new ProfilePage(user);
            Main.Content = profilePage;

        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            ErrorLabel.Visibility = Visibility.Collapsed;
            if (TextboxPasswordCheck.Password == TextboxPassword.Password)
            {
                FireBaseController.Instance.UserRegistration(TextboxEmail.Text, TextboxPassword.Password, TextboxName.Text);
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
            if(TextboxPassword.Visibility == Visibility.Collapsed)
                FireBaseController.Instance.UserAuth(TextboxEmail.Text, TextboxPasswordShow.Text);
            else
                FireBaseController.Instance.UserAuth(TextboxEmail.Text, TextboxPassword.Password);
        }
    }

}