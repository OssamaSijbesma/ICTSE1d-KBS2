using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using PiaNotes.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiaNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public bool SidebarIsOpen { get; set; } = true;
        public bool KeyboardIsOpen { get; set; } = true;
        public int OctavesAmount { get; set; } = 5;

        private List<Rectangle> keysWhite = new List<Rectangle>();
        private List<Rectangle> keysBlack = new List<Rectangle>();

        private enum PianoKey { C, D, E, F, G, A, B };
        private enum PianoKeySharp { Csharp, Dsharp, Fsharp, Gsharp, Asharp };
        
        public MainPage()
        {
            this.InitializeComponent();

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "";

            // Titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            CreateKeyboard();
            CreateSidebar();
        }

        // Menustrip: File > New MIDI File
        private void FileNewMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            // Dialog
        }

        // Menustrip: File > Open MIDI File
        private void FileOpenMIDIFile_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectionPage));
        }

        // Menustrip: View > Sidebar
        private void ViewSidebar_Click(object sender, RoutedEventArgs e)
        {
            ToggleSidebar();
            if (KeyboardIsOpen)
            {
                CreateKeyboard();
            }
        }

        // Menustrip: View > Keyboard
        private void ViewKeyboard_Click(object sender, RoutedEventArgs e)
        {
            ToggleKeyboard();
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

        // Sidebar: More
        private void SidebarMore_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SelectionPage));
        }

        // Toggles the keyboard to show/hide.
        public void ToggleKeyboard()
        {
            // Iterate through all keyboard items to hide/show them.
            if (KeyboardIsOpen)
            {
                foreach (Rectangle key in KeysWhiteSP.Children)
                {
                    key.Width = 0;
                }
                foreach (Rectangle key in KeysBlackSP.Children)
                {
                    key.Width = 0;
                }
                KeyboardBG.MinHeight = 0;
            }
            else
            {
                CreateKeyboard();
                KeyboardBG.MinHeight = 200;
            }
            KeyboardIsOpen = !KeyboardIsOpen;
        }

        public void ToggleSidebar()
        {
            if (SidebarIsOpen)
            {
                Sidebar.MinWidth = 0;
                SidebarSP.MinWidth = 0;
                SidebarSP.Margin = new Thickness(-250,0,0,0);
            }
            else
            {
                Sidebar.MinWidth = 250;
                SidebarSP.MinWidth = 250;
                SidebarSP.Margin = new Thickness(0, 0, 0, 0);
            }
            SidebarIsOpen = !SidebarIsOpen;
        }
        
        // Creates the keyboard.
        public void CreateKeyboard()
        {
            for (int i = 0; i < OctavesAmount; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Rectangle keyWhiteRect = new Rectangle();
                    keyWhiteRect.Name = $"{((PianoKey)j).ToString()}{i}";
                    keyWhiteRect.Stroke = new SolidColorBrush(Colors.Black);
                    keyWhiteRect.Fill = new SolidColorBrush(Colors.White);
                    keyWhiteRect.StrokeThickness = 4;
                    keyWhiteRect.Height = 200;
                    KeysWhiteSP.Children.Add(keyWhiteRect);
                }

                for (int j = 0; j < 5; j++)
                {
                    Rectangle keyBlackRect = new Rectangle();
                    keyBlackRect.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                    keyBlackRect.Fill = new SolidColorBrush(Colors.Black);
                    keyBlackRect.Height = 150;
                    KeysBlackSP.Children.Add(keyBlackRect);
                }
            }
            UpdateKeyboard();
        }

        // Updates the keyboard. Is used after first initializing the keyboard or after resizing the window width.
        public void UpdateKeyboard()
        {
            int windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);

            // Count white keys.
            int keyWhiteAmount = 7 * OctavesAmount;
            
            // Set width for white keys.
            foreach (Rectangle key in KeysWhiteSP.Children)
            {
                try
                {
                    // Calculate width for the white keys.
                    key.Width = (windowWidth - Sidebar.MinWidth) / keyWhiteAmount;
                }
                catch (Exception)
                {
                    // If width can't be calculated, change width to a set value.
                    key.Width = 40;
                }
            }

            // Set width and location for black keys.
            bool initialCsharp = true;
            foreach (Rectangle key in KeysBlackSP.Children)
            {
                double keyWhiteWidth;
                try
                {
                    // Calculate width for the black keys.
                    keyWhiteWidth = (windowWidth - Sidebar.MinWidth) / keyWhiteAmount;
                    key.Width = keyWhiteWidth / 100 * 60;
                }
                catch (Exception)
                {
                    // If width can't be calculated, change width to a set value.
                    keyWhiteWidth = 40;
                    key.Width = keyWhiteWidth / 100 * 60;
                }

                if (key.Name.Contains("Csharp"))
                {
                    // Calculate location for C# key.
                    // The first key has a different calculation than the rest, because there are no prior keys.
                    double location;
                    if (initialCsharp)
                    {
                        location = keyWhiteWidth - (key.Width / 2);
                        initialCsharp = false;
                    }
                    else
                    {
                        location = (keyWhiteWidth - (key.Width / 2)) * 2;
                    }
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
                else if (key.Name.Contains("Dsharp") || key.Name.Contains("Gsharp") || key.Name.Contains("Asharp"))
                {
                    // Calculate location for D#/G#/A# keys.
                    double location = keyWhiteWidth - key.Width;
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
                else if (key.Name.Contains("Fsharp"))
                {
                    // Calculate location for F# key.
                    double location = keyWhiteWidth * 2 - key.Width;
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
            }
        }

        // Creates the sidebar.
        public void CreateSidebar()
        {
            SidebarSP.Children.Clear();

            for (int i = 1; i < 15; i++)
            {
                StackPanel MusicPieceSP = new StackPanel();
                
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

                SidebarSP.Children.Add(MusicPieceSP);
            }

            Button btn_SidebarMore = new Button();
            btn_SidebarMore.Content = "More...";
            btn_SidebarMore.Click += SidebarMore_Click;
            btn_SidebarMore.HorizontalAlignment = HorizontalAlignment.Right;
            btn_SidebarMore.Margin = new Thickness(0, 10, 10, 10);

            SidebarSP.Children.Add(btn_SidebarMore);
            
            UpdateSidebar();
        }

        // Updates the sidebar. Is used after first initializing the sidebar or after resizing the window height.
        public void UpdateSidebar()
        {
            int windowHeight = Convert.ToInt32(Window.Current.Bounds.Height);
            int amount = (windowHeight - 35 - 50) / (90);
            int count = 0;

            // Iterates through the sidebar children and decides whether or not a child should be shown or not. 
            foreach (object child in SidebarSP.Children)
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
            if (KeyboardIsOpen)
            {
                // If the keyboard is shown, it will be updated.
                UpdateKeyboard();
            }
            if (SidebarIsOpen)
            {
                // If the sidebar is shown, it will be updated.
                UpdateSidebar();
            }
        }

       
        private async void Navigation_Click(object sender, RoutedEventArgs e)
        {
            // Get the current button click
            Button curButton = (Button)sender;

            switch (curButton.Name)
            {
                case "nav_settings":
                    // Go to the settings page
                    this.Frame.Navigate(typeof(SettingsPage));
                    break;
                case "nav_practice":
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
                    break;
            }
        }

    }
}
