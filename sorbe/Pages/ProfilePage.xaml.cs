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

namespace sorbe
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
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
                LikeAlbumContent.Children.Add(stackPanel);
            }
        }
    }
}

