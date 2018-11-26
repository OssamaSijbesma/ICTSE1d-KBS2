using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PracticePage : Page
    {
        public bool KeyboardIsOpen { get; set; } = true;
        public int OctavesAmount { get; set; } = 5;

        private List<Rectangle> keysWhite = new List<Rectangle>();
        private List<Rectangle> keysBlack = new List<Rectangle>();

        private enum PianoKey { C, D, E, F, G, A, B };
        private enum PianoKeySharp { Csharp, Dsharp, Fsharp, Gsharp, Asharp };

        List<Rectangle> Notes = new List<Rectangle>();

        public PracticePage()
        {
            this.InitializeComponent();

            // Register a handler for the MessageReceived event
            Settings.midiInPort.MessageReceived += MidiInPort_MessageReceived;

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "";

            CreateKeyboard();
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            // Recieved message
            IMidiMessage receivedMidiMessage = args.Message;

            System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Velocity);

                // Play the note
                byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                //If the player releases the key there should be no sound
                byte velocity;
                if (((MidiNoteOnMessage)receivedMidiMessage).Velocity != 0)
                {
                    if (Settings.feedback == true)
                    {
                        //Use the input from the keyboard the see what the normal velocity is and then add the volume the user chose
                        velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                        
                        if (velocity + DoubleToByte(Settings.volume) <= 127 && velocity + DoubleToByte(Settings.volume) >= 0)
                        {
                            velocity += DoubleToByte(Settings.volume);
                        } else if(velocity + DoubleToByte(Settings.volume) > 127)
                        {
                            velocity = 127;
                        } else
                        {
                            velocity = 0;
                        }
                        //Else use the static velocity the user chose
                    } else velocity = DoubleToByte(Settings.velocity);
                    //Else do not produce any sound, when the input is 0
                } else velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;

                IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);
                FillKey(note);
                Settings.midiOutPort.SendMessage(midiMessageToSend);
            }
        }

        private async void FillKey(byte note)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Notes[note].Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            });
        }



        public static byte DoubleToByte(double doubleVal)
        {
            byte byteVal = 0;

            // Double to byte conversion can overflow.
            try
            {
                byteVal = System.Convert.ToByte(doubleVal);
                return byteVal;
            }
            catch (System.OverflowException)
            {
                System.Console.WriteLine(
                    "Overflow in double-to-byte conversion.");
            }

            // Byte to double conversion cannot overflow.
            doubleVal = System.Convert.ToDouble(byteVal);
            return byteVal;
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

        // Menustrip: View > Keyboard
        private void ViewKeyboard_Click(object sender, RoutedEventArgs e)
        {
            ToggleKeyboard();
        }



        // Toggles the keyboard to show/hide.
        public void ToggleKeyboard()
        {
            // Iterate through all keyboard items to hide/show them.
            if (KeyboardIsOpen)
            {
                foreach (Rectangle key in keysWhite)
                {
                    key.Width = 0;
                }
                foreach (Rectangle key in keysBlack)
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
                    key.Width = (windowWidth) / keyWhiteAmount;
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
                    keyWhiteWidth = (windowWidth) / keyWhiteAmount;
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

        // Is executed when the window is resized.
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (KeyboardIsOpen)
                // If the keyboard is shown, it will be updated.
                UpdateKeyboard();
        }

        /// <summary>
        /// On click navigation
        /// </summary>

        // Navigate to the settings page
        private void NavSettings_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SettingsPage));

        // Navigate to the credits page
        private void NavCredits_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(CreditsPage));

        // Navigate to the selection page
        private void NavSelection_Click(object sender, RoutedEventArgs e)
        {
            Settings.midiInPort.MessageReceived -= MidiInPort_MessageReceived;
            this.Frame.Navigate(typeof(SelectionPage));
        }
    }
}
