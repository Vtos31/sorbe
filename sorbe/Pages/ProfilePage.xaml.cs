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
            for (int i = 0; i < 20; i++)
            {
                StackPanel stackPanel = new StackPanel
                {
                    Margin = new Thickness(10,0,0,0)
                };
                Button button = new Button
                {
                    Width = 300,
                    Height = 300,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("C:\\Users\\Admin\\source\\repos\\sorbe\\sorbe\\Image\\PRAY FOR PARIS WALLPAPER 1920x1080 - Imgur.png")),
                        Stretch = Stretch.UniformToFill
                    }
                };
                stackPanel.Children.Add(button);
                Label AlbumLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White,
                    Height = 33,
                    Content = "Album Name"
                };
                i = i;
                stackPanel.Children.Add(AlbumLabel);
                Label ArtistLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Height = 33,
                    Foreground = Brushes.White,
                    Content = "Artist Name"
                };
                stackPanel.Children.Add(ArtistLabel);
                AlbumContent.Children.Add(stackPanel);
            }
            for (int i = 0; i < 20; i++)
            {
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
                    Background = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri("C:\\Users\\Admin\\source\\repos\\sorbe\\sorbe\\Image\\PRAY FOR PARIS WALLPAPER 1920x1080 - Imgur.png")),
                        Stretch = Stretch.UniformToFill
                    }
                };
                stackPanel.Children.Add(button);
                Label albumLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White,
                    Height = 33,
                    Content = "Album Name"
                };
                i = i;
                stackPanel.Children.Add(albumLabel);
                Label ArtistLabel = new Label
                {
                    FontSize = 18,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Height = 33,
                    Foreground = Brushes.White,
                    Content = "Artist Name"
                };
                stackPanel.Children.Add(ArtistLabel);
                LikeAlbumContent.Children.Add(stackPanel);
            }

            UserName.Content = user["name"].ToString();
            UserEmail.Content = user["email"].ToString();
            UserfavoriteGenre.Content = user["favgenre"].ToString();
            UserImage.Source = Tools.CreateImageFromBase64(user["image"].ToString());
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
                    await FireBaseController.Instance.UpdateUserData("users",FireBaseController.Instance.Uid, new Dictionary<string, object> { { "image", imageInBase64 } });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка завантаження зображення: {ex.Message}");
                }
            }
        }

        private void ChangeName_Click(object sender, RoutedEventArgs e)
        {
            if(UserNameChange.Visibility == Visibility.Visible)
            {
                UserNameChange.Visibility = Visibility.Collapsed;
                CancelChangeNameButton.Visibility = Visibility.Collapsed;
                UserName.Visibility = Visibility.Visible;
                UserName.Content = UserNameChange.Text;
                FireBaseController.Instance.UpdateUserData("users", FireBaseController.Instance.Uid, new Dictionary<string, object> { { "name", UserNameChange.Text } });
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

