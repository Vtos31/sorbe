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
using sorbe.Utilities;

namespace sorbe
{

    public partial class MainPage : Page
    {

        public MainPage(Dictionary<string, List<Dictionary<string, object>>> list)
        {

            InitializeComponent();
           
            for (int g = 0; g < list.Count; g++)
            {
                string key = list.ElementAt(g).Key;
                if (list[key].Count == 0)
                    continue;
                StackPanel Topicgroup = new StackPanel() { Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Stretch };

                Label TopicLabel = new Label() { Content = list.ElementAt(g).Key, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, Height = 68, Foreground = Brushes.White, FontFamily = new FontFamily("Bahnschrift Light"), FontSize = 44 };
                Topicgroup.Children.Add(TopicLabel);

                ScrollViewer ScrollList = new ScrollViewer() {  VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible, VerticalAlignment = VerticalAlignment.Stretch };
                ScrollList.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                Topicgroup.Children.Add(ScrollList);

                StackPanel List = new StackPanel() { Orientation = Orientation.Horizontal ,VerticalAlignment = VerticalAlignment.Stretch};
                ScrollList.Content = List;

                
                int k = 0;
                for (int i = 0; i < list[key].Count; i++)
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
                            Tag = list[key][i],
                            Content = new Image { Source = Tools.CreateImageFromBase64(list[key][i]["image"].ToString()), Stretch= Stretch.Fill} 
                        };
                        button.Click += EnterToInfoPage_Click;
                        stackPanel.Children.Add(button);
                        Label AlbumLabel = new Label
                        {
                            FontSize = 18,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            Foreground = Brushes.White,
                            Height = 33,
                            Content = list[key][i]["name"]
                        };
                    
                        stackPanel.Children.Add(AlbumLabel);
                        Label ArtistLabel = new Label
                        {
                            FontSize = 10,
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                            Height = 23,
                            Foreground = Brushes.White,
                            Content = list[key][i]["artist"]
                        };
                        stackPanel.Children.Add(ArtistLabel);
                        List.Children.Add(stackPanel);
                    k++;
                    }
                
                GridTopic.Children.Add(Topicgroup);
            }

        }

        private void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            InfoPage infoPage = new InfoPage((Dictionary<string, object>)button.Tag);
            NavigationService?.Navigate(infoPage);
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            e.Handled = true;


            var eventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent,
                Source = sender
            };


            MainContent.RaiseEvent(eventArgs);
        }

    }

}
