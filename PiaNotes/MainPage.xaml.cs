using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using PiaNotes.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiaNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Navigation_Click(object sender, RoutedEventArgs e)
        {
            Button curButton = (Button)sender;
            switch (curButton.Name)
            {
                case "nav_settings":
                    this.Frame.Navigate(typeof(SettingsPage));
                    break;
                case "nav_practice":
                    this.Frame.Navigate(typeof(PracticePage));
                    break;
            }
        }
    }
}
