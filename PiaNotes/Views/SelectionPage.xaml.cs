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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectionPage : Page
    {
        public SelectionPage()
        {
            this.InitializeComponent();

            // Add text to titlebar.
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Select MIDI";
            
            // Adds titlebar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
            
            // Creates most recent MIDI files.
            CreateMostRecent();
        }
        
        // Creates the previews of the most recent MIDI files.
        public void CreateMostRecent()
        {
            for (int i = 1; i < 21; i++)
            {
                // Creates StackPanel.
                StackPanel MusicPieceSP = new StackPanel();
                MusicPieceSP.Width = 280;
                MusicPieceSP.Name = $"Music Piece #{i}";
                MusicPieceSP.Tapped += Preview_Tapped;

                // Creates rectangle for MIDI preview.
                Rectangle musicSheetRectangle = new Rectangle();
                musicSheetRectangle.Name = MusicPieceSP.Name;
                musicSheetRectangle.Stroke = new SolidColorBrush(Colors.White);
                musicSheetRectangle.Fill = new SolidColorBrush(Colors.Transparent);
                musicSheetRectangle.StrokeThickness = 1;
                musicSheetRectangle.Height = 50;
                musicSheetRectangle.Width = 260;
                musicSheetRectangle.Margin = new Thickness(0, 0, 0, 0);
                
                // Creates textblock for MIDI name.
                TextBlock musicSheetTextBlock = new TextBlock();
                musicSheetTextBlock.TextWrapping = TextWrapping.Wrap;
                musicSheetTextBlock.TextAlignment = TextAlignment.Center;
                musicSheetTextBlock.Height = 30;
                musicSheetTextBlock.Margin = new Thickness(0, 10, 0, 0);
                musicSheetTextBlock.Text = MusicPieceSP.Name;

                // Adds rectangle and children to stackpanel.
                MusicPieceSP.Children.Add(musicSheetTextBlock);
                MusicPieceSP.Children.Add(musicSheetRectangle);

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
        private void Preview_Tapped(object sender, RoutedEventArgs e)
        {
            //TO DO
        }

        // New MIDI File
        private async void NewMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            // DOING

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".mid");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (file != null)
            {
                var stream = await file.OpenStreamForReadAsync();
                ConvertMidiToText(stream, "C:\\Users\\Martijn\\Music\\midi.txt"); // hardcoded voor nu

                //MidiConverter midiConverter = new MidiConverter(file);
                var dialog = new MessageDialog("File opened ;)");
                await dialog.ShowAsync();
            }
            else
            {
                // Cancel operation.
                var dialog = new MessageDialog("Canceling operation.");
                await dialog.ShowAsync();
            }
        }

        public static void ConvertMidiToText(Stream midiFilePath, string textFilePath)
        {
            var midiFile = MidiFile.Read(midiFilePath);

            System.IO.File.WriteAllLines(textFilePath, midiFile.GetNotes()
                .Select(n => $"{n.NoteNumber} {n.Time} {n.Length}"));
        }       

        // Search bar text
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search(SearchBar.Text);
        }

        // Search function.
        public void Search(string search)
        {
            
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
