using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        public MainPage(List<Dictionary<string, object>> list)
        {
            InitializeComponent();
           
            for (int g = 0; g < 10; g++)
            {
                StackPanel Topicgroup = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Stretch };

                Label TopicLabel = new Label() { Content = "\"CONTEXT\"", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 68, Width = 534, Foreground = Brushes.White, FontFamily = new FontFamily("Yu Gothic Medium"), FontSize = 44 };
                Topicgroup.Children.Add(TopicLabel);

                ScrollViewer ScrollList = new ScrollViewer() {  VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalAlignment = VerticalAlignment.Stretch };
                Topicgroup.Children.Add(ScrollList);
                StackPanel List = new StackPanel() { Orientation = Orientation.Horizontal ,VerticalAlignment = VerticalAlignment.Stretch};
                ScrollList.Content = List;
                int k = 0;
                for (int i = 0; i < 20; i++)
                    {
                    if(k == list.Count-1)
                    {
                        k = 0;
                    }
                    StackPanel stackPanel = new StackPanel
                        {
                            Margin = new Thickness(10, 0, 0, 0),
                            Orientation = Orientation.Vertical,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        Button button = new Button
                        {
                            Width = 200,
                            Height = 200,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            Content = new Image { Source = CreateImageFromBase64(list[k]["image"].ToString()) } 
                        };
                        stackPanel.Children.Add(button);
                        Label AlbumLabel = new Label
                        {
                            FontSize = 18,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            Foreground = Brushes.White,
                            Height = 33,
                            Content = list[k]["name"].ToString()
                        };
                    
                        stackPanel.Children.Add(AlbumLabel);
                        Label ArtistLabel = new Label
                        {
                            FontSize = 10,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            Height = 23,
                            Foreground = Brushes.White,
                            Content = list[k]["artist"].ToString()
                        };
                        stackPanel.Children.Add(ArtistLabel);
                        List.Children.Add(stackPanel);
                    k++;
                    }
                
                GridTopic.Children.Add(Topicgroup);
            }

        }
        private ImageSource CreateImageFromBase64(string base64)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64);
                using (var ms = new MemoryStream(imageBytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження зображення: {ex.Message}");
                return null;
            }
        }
        
        private void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {

            InfoPage infoPage = new InfoPage();
            NavigationService?.Navigate(infoPage);
        }

    }
}
