using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;

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

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Select MIDI";

            CreateMostRecent();
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

        public void CreateMostRecent()
        {
            for (int i = 1; i < 10; i++)
            {
                StackPanel MusicPieceSP = new StackPanel();
                MusicPieceSP.Width = 250;

                // Creates rectangle for MIDI preview.
                Rectangle musicSheetRectangle = new Rectangle();
                musicSheetRectangle.Name = $"Music Piece #{i}";
                musicSheetRectangle.Stroke = new SolidColorBrush(Colors.White);
                musicSheetRectangle.StrokeThickness = 1;
                musicSheetRectangle.Height = 50;
                musicSheetRectangle.Width = 230;
                musicSheetRectangle.Margin = new Thickness(0, 0, 0, 0);
                
                // Creates textblock for MIDI name.
                TextBlock musicSheetTextBlock = new TextBlock();
                musicSheetTextBlock.TextWrapping = TextWrapping.Wrap;
                musicSheetTextBlock.TextAlignment = TextAlignment.Center;
                musicSheetTextBlock.Height = 30;
                musicSheetTextBlock.Margin = new Thickness(0, 10, 0, 0);
                musicSheetTextBlock.Text = musicSheetRectangle.Name;

                // Adds rectangle and children to stackpanel.
                MusicPieceSP.Children.Add(musicSheetTextBlock);
                MusicPieceSP.Children.Add(musicSheetRectangle);
                MusicPieceSP.HorizontalAlignment = HorizontalAlignment.Left;

                MIDIFilesSP.Children.Add(MusicPieceSP);
            }
        }

        // Is executed when the window is resized.
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        // New MIDI File
        private void NewMIDIFile(object sender, RoutedEventArgs e)
        {
            // Dialog
        }


        /// <summary>
        /// On click standard navigation
        /// </summary>
        /// 

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
