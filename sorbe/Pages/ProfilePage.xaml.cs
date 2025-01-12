using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using sorbe.Utilities;

namespace sorbe
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage(Dictionary<string, object> user)
        {
            InitializeComponent();
            UserfavoriteGenre.Content = String.Empty;
            UserDataAboutProjects((List<object>)user["wantlistenproj"],"wantlisten");
            UserDataAboutProjects((List<object>)user["rateproj"], "rate");
            UserName.Content = user["name"].ToString();
            UserEmail.Content = user["email"].ToString();
            if (user["favgenre"] is List<object> favgenre)
            {
                UserfavoriteGenre.Content = String.Empty;
                foreach (var genre in favgenre)
                {
                    UserfavoriteGenre.Content += genre.ToString() + "|";
                }
            }

            
            //UserfavoriteGenre.Content = user["favgenre"].ToString();
            UserImage.Source = Tools.CreateImageFromBase64(user["image"].ToString());
        }

        private async Task UserDataAboutProjects(List<object> project,string value)
        {

            int projectsCount = 0;
            if (project.Count > 10)
            {
                projectsCount = 10;
            }
            else
            {
                projectsCount= project.Count;
            }
            for (int i = 0; i < projectsCount; i++)
            {
                Dictionary<string, object> data = await FireBaseController.Instance.ViewData("projects", project[i].ToString(),new List<string> {"name","artist","image"});
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(10, 0, 0, 0)
                };  
                Button button = new Button
                {
                    Width = 300,
                    Height = 300,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Tag = project[i],
                    Content = new Image { Source = Tools.CreateImageFromBase64(data["image"].ToString()), Stretch = Stretch.Fill }
                };
                button.Click += Profile_Click;
                stackPanel.Children.Add(button);
                if (value == "rate")
                {
                    List<Dictionary<string, object>> commentsdata = await FireBaseController.Instance.ViewData("comments", "projectId", project[i].ToString(), 10) ?? new List<Dictionary<string, object>>();
                    commentsdata = commentsdata.Where(x => x.ContainsKey("useruid") && x["useruid"].ToString() == FireBaseController.Instance.Uid).ToList();

                    var firstComment = commentsdata.FirstOrDefault();
                    string rateContent = firstComment != null && firstComment.ContainsKey("rate") ? firstComment["rate"].ToString() : "0";

                    Rectangle rect = new Rectangle
                    {
                        Width = 70,
                        Height = 40,
                        Fill = Brushes.Black,
                        Margin = new Thickness(0, -45, 225, 0)
                    };

                    Label label = new Label
                    {
                        Content = $"{rateContent}/100",
                        Foreground = Brushes.White,
                        FontSize = 18,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, -45, 230, 0)
                    };

                    stackPanel.Children.Add(rect);
                    stackPanel.Children.Add(label);
                }

                Label AlbumLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White,
                    Height = 33,
                    Content = data["name"].ToString()
                };
                stackPanel.Children.Add(AlbumLabel);
                Label ArtistLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Height = 33,
                    Foreground = Brushes.White,
                    Content = data["artist"].ToString()
                };
                stackPanel.Children.Add(ArtistLabel);
                if(value == "wantlisten")
                {
                    LikeAlbumContent.Children.Add(stackPanel);
                }
                if (value == "rate")
                {
                    AlbumContent.Children.Add(stackPanel);
                }
            }
        }
        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Виберіть фото",
                Filter = "Зображення (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                try
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(selectedFilePath));
                    UserImage.Source = bitmap;
                    string imageInBase64 = Tools.GetBase64FromImage(selectedFilePath);
                    await FireBaseController.Instance.UpdateData("users",FireBaseController.Instance.Uid, new Dictionary<string, object> { { "image", imageInBase64 } });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження зображення: {ex.Message}");
                }
            }
        }
        private async void Profile_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Dictionary<string, object> data = await FireBaseController.Instance.ViewData("projects", button.Tag.ToString());
            InfoPage infoPage = new InfoPage(data);
            NavigationService?.Navigate(infoPage);
        }
        private void ChangeName_Click(object sender, RoutedEventArgs e)
        {
            if(UserNameChange.Visibility == Visibility.Visible)
            {
                UserNameChange.Visibility = Visibility.Collapsed;
                CancelChangeNameButton.Visibility = Visibility.Collapsed;
                UserName.Visibility = Visibility.Visible;
                UserName.Content = UserNameChange.Text;
                FireBaseController.Instance.UpdateData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "name", UserNameChange.Text } });
                ChangeNameButton.Content = "Змінити ім'я";
            }
            else
            {
                CancelChangeNameButton.Visibility = Visibility.Visible;
                UserNameChange.Text = (string)UserName.Content ;
                ChangeNameButton.Content = "Зберегти";
                UserNameChange.Visibility = Visibility.Visible;
                UserName.Visibility = Visibility.Collapsed;
            }
        }

        private void CancelChangeName_Click(object sender, RoutedEventArgs e)
        {
            CancelChangeNameButton.Visibility = Visibility.Collapsed;
            UserNameChange.Visibility = Visibility.Collapsed;
            ChangeNameButton.Content = "Змінити ім'я";
            UserName.Visibility = Visibility.Visible;
        }
    }
}

