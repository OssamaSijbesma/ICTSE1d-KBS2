using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes
{

    public sealed partial class SelectionPage : Page
    {
        
        public SelectionPage()
        {
            this.InitializeComponent();
        }

        public void RecentFileSelectionCreate()
        {
            for (int i = 0; i < 10; i++)
            {
                //Database recent loaded midi files
                Rectangle RecentMIDI = new Rectangle();
                RecentMIDI.Name = $"Music Piece #{i}";
                RecentMIDI.Stroke = new SolidColorBrush(Colors.White);
                RecentMIDI.StrokeThickness = 1;
                RecentMIDI.Height = 50;
                RecentMIDI.Width = 50;

                RecentMIDIPanel.Children.Add(RecentMIDI);
            }

        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
            //Closes Selection Screen, opens Main page
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            //Open File Explorer
            
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MoreMIDI));

        }
    }
}
