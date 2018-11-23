﻿using System;
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

        private List<Rectangle> keysWhite = new List<Rectangle>();
        private List<Rectangle> keysBlack = new List<Rectangle>();

        private Rectangle[] Notes = new Rectangle[127];
        private enum PianoKey { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };
        private enum PianoKeySharp { CSharp = 1, DSharp = 3, FSharp = 6, GSharp = 8, ASharp = 10 };

        public PracticePage()
        {
            this.InitializeComponent();

            // Register a handler for the MessageReceived event
            Settings.midiInPort.MessageReceived += MidiInPort_MessageReceived;

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "";

            // Titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

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
                FillKey(midiMessageToSend);
                Settings.midiOutPort.SendMessage(midiMessageToSend);
            }

            if (receivedMidiMessage.Type == MidiMessageType.NoteOff)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Velocity);

                byte channel = ((MidiNoteOffMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOffMessage)receivedMidiMessage).Note;
                byte velocity = ((MidiNoteOffMessage)receivedMidiMessage).Velocity;

                IMidiMessage midiMessageToSend = new MidiNoteOffMessage(channel, note, velocity);
                UnFillKey(midiMessageToSend);
            }

        }

        private async void FillKey(IMidiMessage IM)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte note = ((MidiNoteOnMessage)IM).Note;
                try
                {
                    byte neg = 25;
                    if (Notes[note].Name.Contains("Sharp"))
                    {
                        Notes[note].Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, DoubleToByte(Settings.R - neg), DoubleToByte(Settings.G - neg), DoubleToByte(Settings.B - neg)));
                    }
                    else
                    {
                        Notes[note].Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, Settings.R, Settings.G, Settings.B));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        private async void UnFillKey(IMidiMessage IM)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte note = ((MidiNoteOffMessage)IM).Note;
                try
                {
                    if (Notes[note].Name.Contains("Sharp"))
                    {
                        Notes[note].Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                    } else
                    {
                        Notes[note].Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
            for (int i = Settings.OctaveStart; i < (Settings.OctaveAmount + Settings.OctaveStart); i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    switch (j)
                    {
                        case 1:
                            Rectangle keyBlackRect = new Rectangle();
                            keyBlackRect.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                            keyBlackRect.Fill = new SolidColorBrush(Colors.Black);
                            keyBlackRect.Height = 150;
                            KeysBlackSP.Children.Add(keyBlackRect);
                            if (i == 0) Notes[j] = (keyBlackRect);
                            else Notes[(j + (i * 12))] = (keyBlackRect);
                            break;
                        case 3:
                            Rectangle keyBlackRect3 = new Rectangle();
                            keyBlackRect3.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                            keyBlackRect3.Fill = new SolidColorBrush(Colors.Black);
                            keyBlackRect3.Height = 150;
                            KeysBlackSP.Children.Add(keyBlackRect3);
                            if (i == 0) Notes[j] = (keyBlackRect3);
                            else Notes[(j + (i * 12))] = (keyBlackRect3);
                            break;
                        case 6:
                            Rectangle keyBlackRect6 = new Rectangle();
                            keyBlackRect6.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                            keyBlackRect6.Fill = new SolidColorBrush(Colors.Black);
                            keyBlackRect6.Height = 150;
                            KeysBlackSP.Children.Add(keyBlackRect6);
                            if (i == 0) Notes[j] = (keyBlackRect6);
                            else Notes[(j + (i * 12))] = (keyBlackRect6);
                            break;
                        case 8:
                            Rectangle keyBlackRect8 = new Rectangle();
                            keyBlackRect8.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                            keyBlackRect8.Fill = new SolidColorBrush(Colors.Black);
                            keyBlackRect8.Height = 150;
                            KeysBlackSP.Children.Add(keyBlackRect8);
                            if (i == 0) Notes[j] = (keyBlackRect8);
                            else Notes[(j + (i * 12))] = (keyBlackRect8);
                            break;
                        case 10:
                            Rectangle keyBlackRect10 = new Rectangle();
                            keyBlackRect10.Name = $"{((PianoKeySharp)j).ToString()}{i}";
                            keyBlackRect10.Fill = new SolidColorBrush(Colors.Black);
                            keyBlackRect10.Height = 150;
                            KeysBlackSP.Children.Add(keyBlackRect10);
                            if (i == 0) Notes[j] = (keyBlackRect10);
                            else Notes[(j + (i * 12))] = (keyBlackRect10);
                            break;
                        default:
                            Rectangle keyWhiteRect = new Rectangle();
                            keyWhiteRect.Name = $"{((PianoKey)j).ToString()}{i}";
                            keyWhiteRect.Stroke = new SolidColorBrush(Colors.Black);
                            keyWhiteRect.Fill = new SolidColorBrush(Colors.White);
                            keyWhiteRect.StrokeThickness = 4;
                            keyWhiteRect.Height = 200;
                            KeysWhiteSP.Children.Add(keyWhiteRect);
                            System.Diagnostics.Debug.WriteLine(keyWhiteRect.Name);
                            if (j == 0)
                            {
                                if (i == 0) Notes[j] = (keyWhiteRect);
                                else Notes[(i * 12)] = (keyWhiteRect);
                            } else
                            {
                                if (i == 0) Notes[j] = (keyWhiteRect);
                                else Notes[(j + (i * 12))] = (keyWhiteRect);
                            }
                            break;
                    }
                }
            }
            UpdateKeyboard();
        }

        // Updates the keyboard. Is used after first initializing the keyboard or after resizing the window width.
        public void UpdateKeyboard()
        {
            int windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);

            // Count white keys.
            int keyWhiteAmount = 7 * Settings.OctaveAmount;

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

                if (key.Name.Contains("CSharp"))
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
                else if (key.Name.Contains("DSharp") || key.Name.Contains("GSharp") || key.Name.Contains("ASharp"))
                {
                    // Calculate location for D#/G#/A# keys.
                    double location = keyWhiteWidth - key.Width;
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
                else if (key.Name.Contains("FSharp"))
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
