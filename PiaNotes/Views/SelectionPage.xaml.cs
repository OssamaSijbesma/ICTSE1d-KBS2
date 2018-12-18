using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using PiaNotes.ViewModels;
using PiaNotes.Models;
using System.Threading.Tasks;
using Windows.UI.Xaml.Input;
using Windows.Devices.Midi;

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectionPage : Page
    {
        //Get Search Functionality from Databaser Class
        Databaser DB = new Databaser();

        MidiParser MP;
        private string MidiF;

        private enum PianoKey { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };
        private enum PianoKeySharp { CSharp = 1, DSharp = 3, FSharp = 6, GSharp = 8, ASharp = 10 };
        
        //Creates a list of musicsheets
        List<MusicSheet> Sheets = new List<MusicSheet>();

        public SelectionPage()
        {
            this.InitializeComponent();

            // Add text to titlebar.
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Select MIDI";

            // Adds titlebar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            // Adds all items from Database in list of musicsheets
            Sheets = DB.Search(null, null, null, 0, 0);

            // Creates most recent MIDI files.
            CreateMostRecent();
        }

        // Menustrip: File > New MIDI File
        private void NewMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UploadPage));
        }


        // Creates the previews of the most recent MIDI files.
        public async void CreateMostRecent()
        {
            //Checks if Database is connected
            if (DB.CheckConnection() == true)
            {
                foreach (MusicSheet element in Sheets)
                {

                    VariableSizedWrapGrid SelectionGrid = new VariableSizedWrapGrid();
                    SelectionGrid.Width = 400;
                    SelectionGrid.Height = 80;


                    Button musicSheetButton = new Button();
                    musicSheetButton.Height = 50;
                    musicSheetButton.Width = 180;
                    musicSheetButton.Margin = new Thickness(10, 10, 10, 10);
                    musicSheetButton.Click += delegate (object sender, RoutedEventArgs e) { MidiFile_Click(sender, e, element); };
                    musicSheetButton.RightTapped += delegate (object sender, RightTappedRoutedEventArgs e) { MidiFile_RightTapped(sender, e, element); };

                    if (element.Title.Length > 25)
                    {
                        musicSheetButton.Content = element.Title.Substring(0, 23) + "...";
                    }
                    else
                    {
                        musicSheetButton.Content = element.Title;
                    }

                    Button previewButton = new Button();
                    previewButton.Height = 30;
                    previewButton.Width = 30;
                    previewButton.Content = "▶";
                    previewButton.Click += delegate (object sender, RoutedEventArgs e) {
                        if (previewButton.Content.Equals("▶"))
                        {
                            previewButton.Content = "■";
                            IMidiMessage receivedMidiMessage = args.Message;

                            byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                            byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                            byte velocity = 100;

                            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);


                        } else
                        {
                            previewButton.Content = "▶";
                        }
                    };




                    // Adds StackPanel to the VariableSizedWrapGrid.

                    MIDIFilesWG.Children.Add(SelectionGrid);
                    SelectionGrid.Children.Add(musicSheetButton);
                    SelectionGrid.Children.Add(previewButton);

                }
                //Navigates to UploadPage when Offline
            }
            else
            {
                await StaticObjects.NoDatabaseConnectionDialog.ShowAsync();
                this.Frame.Navigate(typeof(UploadPage));
            }
        }
        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Search function.
        public void Search(string search)
        {
            //Creates a list of results where inserted Searchbar text will be displayed with the corresponding item in Database.
            Sheets = DB.Search(null, "MusicSheet.Title", search + "%", 0, 0);
            CreateMostRecent();
        }

        // Updates the most recent MIDI files. Is used after first initializing or after resizing the window height.
        public void UpdateMostRecent()
        {
            // Creates variables for the height and width.
            int windowHeight = Convert.ToInt32(Window.Current.Bounds.Height);
            int windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);
            int amountHeight = (windowHeight - 35 - 160) / (55);
            int amountWidth = (windowWidth - 80) / (280);
            int amount = amountHeight * amountWidth;
            int count = 0;

            // Iterates through the sidebar children and decides whether or not a child should be shown or not. 
            foreach (object child in MIDIFilesWG.Children)
            {
                count++;
                if (child is Button)
                {
                    if (count <= amount)
                    {
                        // Makes said StackPanel visible.
                        (child as Button).Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Makes said StackPanel invisible/collapsed.
                        (child as Button).Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

            // Is executed when the window is resized.
            private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMostRecent();
        }

        // MIDI file click functionality.
        private async Task MidiFile_Click(object sender, RoutedEventArgs e, MusicSheet element)  
        {
            // Navigate to the practice page unless MIDI is not set then show a dialog and go to the settings page
            if (Settings.midiInPort == null || Settings.midiOutPort == null)
            {
                await StaticObjects.NoMidiInOutDialog.ShowAsync();
                this.Frame.Navigate(typeof(SettingsPage));
            } else
            {
                // element.Id;
                string Examp = "324 40 40-234 40 40-23 50 50";
                var count = Examp.Count(c => c == '-');
                List<string> notes = new List<string>();

                for(int i = 0; i <= count; i++)
                {
                    //Initialize vars
                    String sub;
                    int position = Examp.IndexOf("-");

                    if (i == count)
                    //if the for loop is at the end of the string make the last substring
                    {
                        sub = Examp.Substring(0, (Examp.Substring(0)).Length);
                    } else
                    //else make a substring and redo the string so the substring is deleted
                    {
                        sub = Examp.Substring(0, (Examp.Substring(0, position)).Length);
                        Examp = Examp.Substring(((Examp.Substring(0, position)).Length) + 1);
                    }
                    //Add substring to array of strings
                    notes.Add(sub);
                    //Debug line to see if substring is done correctly
                    System.Diagnostics.Debug.WriteLine(sub);
                }

                //Send array to MidiParser
                /*0MP = new MidiParser(notes);

                /*
                // Zo iets mart
                string[] notes = completeMidiStringExample.Split('-');
                string[] note = notes[0].Split(' ');
                */
            }
        }

        // MIDI file right click functionality.
        private void MidiFile_RightTapped(object sender, RightTappedRoutedEventArgs e, MusicSheet element)
        {
            MenuFlyout myFlyout = new MenuFlyout();
            MenuFlyoutItem delete = new MenuFlyoutItem { Text = "Delete" };
            delete.Click += delegate (object delete_sender, RoutedEventArgs delete_e) { MidiFileDelete_Click(sender, e, element); };
            myFlyout.Items.Add(delete);
            myFlyout.ShowAt(sender as UIElement, e.GetPosition(sender as UIElement));
        }

        // MIDI file removal functionality.
        private void MidiFileDelete_Click(object sender, RoutedEventArgs e, MusicSheet element)
        {
            DB.Delete(element.Id);
            MIDIFilesWG.Children.Clear();
            Search(SearchBar.Text);
        }

        // Display search changes
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MIDIFilesWG.Children.Clear();
            Search(SearchBar.Text);
        }
        
        //Yo can this be deleted? It has 0 references
        public string GetNote(int noteNumber)
        {
            int noteNumber2 = noteNumber;
            for (int i = noteNumber; i > 11; i -= 12)
            {
                noteNumber2 -= 12;
            }

            string returnValue = "";
            if (noteNumber2 == 0 || noteNumber2 == 2 ||
                noteNumber2 == 4 || noteNumber2 == 5 ||
                noteNumber2 == 7 || noteNumber2 == 9 ||
                noteNumber2 == 11)
            {
                returnValue = ((PianoKey)noteNumber2).ToString();
            }
            else if (noteNumber2 == 1 || noteNumber2 == 3 ||
                noteNumber2 == 6 || noteNumber2 == 8 ||
                noteNumber2 == 10)
            {
                returnValue = ((PianoKeySharp)noteNumber2).ToString();
            }
            else
            {
                returnValue = $"Something went wrong.\nnoteNumber: {noteNumber}\nnoteNumber2: {noteNumber2}";
            }

            return returnValue;
        }

        /// <summary>
        /// On click standard navigation
        /// </summary>

        private async void NavPractice_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the practice page unless MIDI is not set then show a dialog and go to the settings page
            if (Settings.midiInPort == null || Settings.midiOutPort == null)
            {
                await StaticObjects.NoMidiInOutDialog.ShowAsync();
                this.Frame.Navigate(typeof(SettingsPage));
            }
            else
                this.Frame.Navigate(typeof(PracticePage));
        }

        // Navigate to the settings page
        private void NavSettings_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SettingsPage));

        // Navigate to the credits page
        private void NavCredits_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(CreditsPage));

        // Navigate to the selection page
        private void NavSelection_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SelectionPage));
    }
}
