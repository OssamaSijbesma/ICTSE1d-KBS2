﻿using System;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

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
        public void CreateMostRecent()
        {
            foreach (MusicSheet element in Sheets)
            {
                // Creates StackPanel.
                StackPanel MusicPieceSP = new StackPanel();
                MusicPieceSP.Width = 280;
                MusicPieceSP.Name = element.Title;
                
                Button musicSheetButton = new Button();
                musicSheetButton.Height = 35;
                musicSheetButton.Width = 260;
                musicSheetButton.Margin = new Thickness(0, 10, 0, 0);
                musicSheetButton.Click += MidiFile_Click;

                if (MusicPieceSP.Name.Length > 30)
                {
                    musicSheetButton.Content = MusicPieceSP.Name.Substring(0, 27) + "...";
                }
                else
                {
                    musicSheetButton.Content = MusicPieceSP.Name;
                }

                // Adds rectangle and children to stackpanel.
                MusicPieceSP.Children.Add(musicSheetButton);

                // Adds StackPanel to the VariableSizedWrapGrid.
                MIDIFilesWG.Children.Add(MusicPieceSP);
            }
        }

        // Updates the most recent MIDI files. Is used after first initializing or after resizing the window height.
        public void UpdateMostRecent()
        {
            // Creates variables for the height and width.
            int windowHeight = Convert.ToInt32(Window.Current.Bounds.Height);
            int windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);
            int amountHeight = (windowHeight - 35 - 160) / (90);
            int amountWidth = (windowWidth - 80) / (280);
            int amount = amountHeight * amountWidth;
            int count = 0;

            // Iterates through the sidebar children and decides whether or not a child should be shown or not. 
            foreach (object child in MIDIFilesWG.Children)
            {
                count++;
                if (child is StackPanel)
                {
                    if (count <= amount)
                    {
                        // Makes said StackPanel visible.
                        (child as StackPanel).Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // Makes said StackPanel invisible/collapsed.
                        (child as StackPanel).Visibility = Visibility.Collapsed;
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
        private void MidiFile_Click(object sender, RoutedEventArgs e)
        {
            //TO DO
        }
        
        private enum PianoKey { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };
        private enum PianoKeySharp { CSharp = 1, DSharp = 3, FSharp = 6, GSharp = 8, ASharp = 10 };

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


        // Display search changes
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MIDIFilesWG.Children.Clear();
            Search(SearchBar.Text);
        }

        // Search function.
        public void Search(string search)
        {
            //Creates a list of results where inserted Searchbar text will be displayed with the corresponding item in Database.
            List<MusicSheet> results = DB.Search(null, "MusicSheet.Title", search + "%", 0, 0);

            foreach (var element in results)
            {
                // Creates StackPanel.
                StackPanel MusicPieceSP = new StackPanel();
                MusicPieceSP.Width = 280;
                MusicPieceSP.Name = element.Title;
                
                Button musicSheetButton = new Button();
                musicSheetButton.Height = 35;
                musicSheetButton.Width = 260;
                musicSheetButton.Margin = new Thickness(0, 10, 0, 0);
                musicSheetButton.Click += MidiFile_Click;

                if (MusicPieceSP.Name.Length > 30)
                {
                    musicSheetButton.Content = MusicPieceSP.Name.Substring(0, 27) + "...";
                }
                else
                {
                    musicSheetButton.Content = MusicPieceSP.Name;
                }

                // Adds rectangle and children to stackpanel.
                MusicPieceSP.Children.Add(musicSheetButton);

                // Adds StackPanel to the VariableSizedWrapGrid.
                MIDIFilesWG.Children.Add(MusicPieceSP);
            }
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
