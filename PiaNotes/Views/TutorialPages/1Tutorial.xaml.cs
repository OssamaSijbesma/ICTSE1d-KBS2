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
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Text;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using PiaNotes.ViewModels;
using PiaNotes.Models;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views.TutorialPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class _1Tutorial : Page
    {
        Databaser DB = new Databaser();

        MidiParser midiParser;

        public _1Tutorial()
        {
            this.InitializeComponent();
        }

        public async void Exercise_Click(object sender, RoutedEventArgs e)
        {
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
                    StorageFile storageFileMIDI = await DB.GetAFileAsync(29);
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
        }
    }
}
