﻿using System;
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
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {

        public MainPage()
        {
            InitializeComponent();
           
            for (int g = 0; g < 10; g++)
            {
                StackPanel Topicgroup = new StackPanel() { Orientation = Orientation.Vertical };

                Label TopicLabel = new Label() { Content = "\"CONTEXT\"", HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(20, 10, 0, 0), VerticalAlignment = VerticalAlignment.Top, Height = 68, Width = 534, Foreground = Brushes.White, FontFamily = new FontFamily("Yu Gothic Medium"), FontSize = 44 };
                Topicgroup.Children.Add(TopicLabel);

                ScrollViewer ScrollList = new ScrollViewer() { Height = 256, VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible };
                Topicgroup.Children.Add(ScrollList);
                StackPanel List = new StackPanel() { Orientation = Orientation.Horizontal };
                ScrollList.Content = List;


                for (int i = 0; i < 20; i++)
                {
                    Button button = new Button
                    {
                        Width = 200,
                        Height = 200,
                        Margin = new Thickness(10),
                        
                        Background = new ImageBrush
                        {
                            ImageSource = new BitmapImage(new Uri("C:\\Users\\Admin\\source\\repos\\sorbe\\sorbe\\Image\\PRAY FOR PARIS WALLPAPER 1920x1080 - Imgur.png")),
                            Stretch = Stretch.UniformToFill
                        }
                    };
                    
                    button.Click += EnterToInfoPage_Click;
                    List.Children.Add(button);
                }

                GridTopic.Children.Add(Topicgroup);
            }

        }
        private void EnterToInfoPage_Click(object sender, RoutedEventArgs e)
        {

            InfoPage infoPage = new InfoPage();
            NavigationService?.Navigate(infoPage);
        }

    }
}
