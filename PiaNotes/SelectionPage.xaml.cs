using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectionPage : Page
    {
        public SelectionPage()
        {
            this.InitializeComponent();

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Select MIDI";
        }

        // Is executed when the window is resized.
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        // Return
        private void OpenMainPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // New MIDI File
        private void NewMIDIFile(object sender, RoutedEventArgs e)
        {
            // Dialog
        }


    }
}
