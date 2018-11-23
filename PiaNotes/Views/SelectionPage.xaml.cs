using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.ApplicationModel.Core;

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
            
            // Titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;


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



        //private async void OptionsPractice_Click(object sender, RoutedEventArgs e)
        //{
        //    if (Settings.midiInPort != null && Settings.midiOutPort != null)
        //        // Go to the practice page
        //        this.Frame.Navigate(typeof(PracticePage));
        //    else
        //    {
        //        // Create ContenDialog object
        //        ContentDialog noMidiInOut = new ContentDialog
        //        {
        //            Title = "There is no MIDI in- or output!",
        //            Content = "Check your MIDI input and output device before practicing.",
        //            CloseButtonText = "Ok"
        //        };

        //        // Show dialog
        //        await noMidiInOut.ShowAsync();

        //        // Go to the settings page
        //        this.Frame.Navigate(typeof(SettingsPage));
        //    }
        //}

        // Creates the previews of the most recent MIDI files.
        public void CreateMostRecent()
        {
            for (int i = 1; i < 21; i++)
            {
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

                MIDIFilesWG.Children.Add(MusicPieceSP);
            }
        }

        // Updates the most recent MIDI files. Is used after first initializing or after resizing the window height.
        public void UpdateMostRecent()
        {
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
                        (child as StackPanel).Visibility = Visibility.Visible;
                    }
                    else
                    {
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

        // Is executed when the window is resized.
        private void Preview_Tapped(object sender, RoutedEventArgs e)
        {
            // 
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
