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
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using System.Threading;
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

        // Menustrip: File > New MIDI File
        private void FileNewMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            // Dialog
        }

        // Menustrip: File > Open MIDI File
        private void FileOpenMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(SelectionPage));
        }

        // Menustrip: Options > Settings
        private void OptionsSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }


        // Menustrip: Options > Credits
        private void OptionsCredits_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreditsPage));
        }

        // Menustrip: Options > Practice
        private async void OptionsPractice_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.midiInPort != null && Settings.midiOutPort != null)
                // Go to the practice page
                this.Frame.Navigate(typeof(PracticePage));
            else
            {
                // Create ContenDialog object
                ContentDialog noMidiInOut = new ContentDialog
                {
                    Title = "There is no MIDI in- or output!",
                    Content = "Check your MIDI input and output device before practicing.",
                    CloseButtonText = "Ok"
                };

                // Show dialog
                await noMidiInOut.ShowAsync();

                // Go to the settings page
                this.Frame.Navigate(typeof(SettingsPage));
            }
        }

    }
}
