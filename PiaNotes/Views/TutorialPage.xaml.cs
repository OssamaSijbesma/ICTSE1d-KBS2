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
using Windows.UI.Xaml.Navigation;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using PiaNotes.ViewModels;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.Devices.Midi;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Shapes;
using PiaNotes.Models;
using System.Numerics;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Text;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TutorialPage : Page
    {
        //Get Search Functionality from Databaser Class
        Databaser DB = new Databaser();

        MidiParser midiParser;

        int i = 0;

        public TutorialPage()
        {
            this.InitializeComponent();
        }

        #region NavigationView event handlers
        private void TutorialNV_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in TutorialNV.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "First_Tutorial")
                {
                    TutorialNV.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(Views.TutorialPages._1Tutorial));
        }

        private async void TutorialNV_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            TextBlock ItemContent = args.InvokedItem as TextBlock;
            if (ItemContent != null)
            {
                switch (ItemContent.Tag)
                {
                    case "First_Tutorial":
                        i = 0;
                        contentFrame.Navigate(typeof(Views.TutorialPages._1Tutorial));
                        break;

                    case "Second_Tutorial":
                        i = 1;
                        contentFrame.Navigate(typeof(Views.TutorialPages._2Tutorial));
                        break;

                    case "Third_Tutorial":
                        i = 2;
                        contentFrame.Navigate(typeof(Views.TutorialPages._3Tutorial));
                        break;

                    case "Fourth_Tutorial":
                        i = 3;
                        contentFrame.Navigate(typeof(Views.TutorialPages._4Tutorial));
                        break;

                    case "Fifth_Tutorial":
                        i = 4;
                        contentFrame.Navigate(typeof(Views.TutorialPages._5Tutorial));
                        break;

                    case "Sixth_Tutorial":
                        i = 5;
                        contentFrame.Navigate(typeof(Views.TutorialPages._6Tutorial));
                        break;
                    case "Play":
                        // Navigate to the practice page unless MIDI is not set then show a dialog and go to the settings page
                        if (Settings.midiInPort == null || Settings.midiOutPort == null)
                        {
                            await StaticObjects.NoMidiInOutDialog.ShowAsync();
                            this.Frame.Navigate(typeof(SettingsPage));
                        }
                        else
                        {
                            if (DB.CheckConnection() == true)
                            {
                                StorageFile storageFileMIDI = await DB.GetAFileAsync(i);
                                Stream streamMIDI = await storageFileMIDI.OpenStreamForReadAsync();
                                MidiFile midiFile = MidiFile.Read(streamMIDI);
                                midiParser = new MidiParser(midiFile);

                                // Navigate to the practice page 
                                this.Frame.Navigate(typeof(PracticePage), midiParser.sheetMusic);
                            }
                            else
                            {
                                //uploads local file if offline
                                await StaticObjects.NoDatabaseConnectionDialog.ShowAsync();
                                this.Frame.Navigate(typeof(UploadPage));
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        private void TutorialNV_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            this.Frame.Navigate(typeof(Views.SelectionPage));
        }
    }
}
