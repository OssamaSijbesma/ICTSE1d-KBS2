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
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using System.Threading;

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

        MidiDeviceWatcher inputDeviceWatcher;
        MidiDeviceWatcher outputDeviceWatcher;

        MidiInPort midiInPort;
        IMidiOutPort midiOutPort;

        public MainPage()
        {
            this.InitializeComponent();

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "";

            // MIDI zooi
            inputDeviceWatcher =
                new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), midiInPortListBox, Dispatcher);

            inputDeviceWatcher.StartWatcher();

            outputDeviceWatcher =
                new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), midiOutPortListBox, Dispatcher);

            outputDeviceWatcher.StartWatcher();

            // Titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;
            
            CreateKeyboard();
            CreateSidebar();
        }

        // Menustrip: File > New MIDI File
        private void FileNewMIDIFile(object sender, RoutedEventArgs e)
        {
            // Dialog
        }

        // Menustrip: File > New MIDI File
        private void FileOpenMIDIFile(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(SelectionMenu));
        }

        // Menustrip: View > Sidebar
        private void ViewSidebar(object sender, RoutedEventArgs e)
        {
            ToggleSidebar();
            if (KeyboardIsOpen)
            {
                CreateKeyboard();
            }
        }

        // Menustrip: View > Keyboard
        private void ViewKeyboard(object sender, RoutedEventArgs e)
        {
            ToggleKeyboard();
        }
        
        // Menustrip: Options > Settings
        private void OptionsSettings(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }


        // Menustrip: Options > Credits
        private void OptionsCredits(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Credits));
        }



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
        
        public void ToggleSidebar()
        {
            if (SidebarIsOpen)
            {
                Sidebar.MinWidth = 0;
            }
            else
            {
                Sidebar.MinWidth = 250;
            }
            SidebarIsOpen = !SidebarIsOpen;
        }

        enum PianoKey { C, D, E, F, G, A, B };
        enum PianoKeySharp { Csharp, Dsharp, Fsharp, Gsharp, Asharp };

        public void CreateKeyboard()
        {
            for (int i = 0; i < OctavesAmount; i++)
            {
                for (int j = 0; j < 6; j++)
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

            int windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);
            
            // Count keys
            int keyWhiteAmount = 7 * OctavesAmount;

            
            // Set width for white keys.
            foreach (Rectangle key in KeysWhiteSP.Children)
            {
                try
                {
                    key.Width = (windowWidth - Sidebar.MinWidth) / keyWhiteAmount;
                }
                catch (Exception)
                {
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
                    keyWhiteWidth = (windowWidth - Sidebar.MinWidth) / keyWhiteAmount;

                    key.Width = keyWhiteWidth / 100 * 60;
                }
                catch (Exception)
                {
                    keyWhiteWidth = 40;
                    key.Width = keyWhiteWidth / 100 * 60;
                }

                if (key.Name.Contains("Csharp"))
                {
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
                    double location = keyWhiteWidth - key.Width;
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
                else if (key.Name.Contains("Fsharp"))
                {
                    double location = keyWhiteWidth * 2 - key.Width;
                    key.Margin = new Thickness(location, 0, 0, 50);
                }
            }
        }
        
        public void CreateSidebar()
        {
            int windowHeight = Convert.ToInt32(Window.Current.Bounds.Height);
            
            int amount = (windowHeight - 30) / (50 + 20);
            
            SidebarSP.Children.Clear();

            for (int i = 0; i < amount; i++)
            {
                Rectangle musicSheetRectangle = new Rectangle();
                musicSheetRectangle.Name = $"Music Piece #{i}";
                musicSheetRectangle.Stroke = new SolidColorBrush(Colors.White);
                musicSheetRectangle.StrokeThickness = 1;
                musicSheetRectangle.Height = 50;
                musicSheetRectangle.Width = 230;
                musicSheetRectangle.Margin =  new Thickness(0, 10, 0, 10);
                SidebarSP.Children.Add(musicSheetRectangle);
            }

            
        }

        // Is executed when the window is resized.
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (KeyboardIsOpen)
            {
                CreateKeyboard();
            }

            if (SidebarIsOpen)
            {
                CreateSidebar();
            }


        }
        
        private async void midiInPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deviceInformationCollection = inputDeviceWatcher.DeviceInformationCollection;

            if (deviceInformationCollection == null)
            {
                return;
            }

            DeviceInformation devInfo = deviceInformationCollection[midiInPortListBox.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }

            midiInPort = await MidiInPort.FromIdAsync(devInfo.Id);

            if (midiInPort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                return;
            }
            midiInPort.MessageReceived += MidiInPort_MessageReceived;
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            IMidiMessage receivedMidiMessage = args.Message;

            System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Velocity);

                byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                byte velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

                midiOutPort.SendMessage(midiMessageToSend);
            }
        }

        private async void midiOutPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var deviceInformationCollection = outputDeviceWatcher.DeviceInformationCollection;

            if (deviceInformationCollection == null)
            {
                return;
            }

            DeviceInformation devInfo = deviceInformationCollection[midiOutPortListBox.SelectedIndex];

            if (devInfo == null)
            {
                return;
            }

            midiOutPort = await MidiOutPort.FromIdAsync(devInfo.Id);

            if (midiOutPort == null)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from output device");
                return;
            }

        }

        private async Task EnumerateMidiInputDevices()
        {
            // Find all input MIDI devices
            string midiInputQueryString = MidiInPort.GetDeviceSelector();
            DeviceInformationCollection midiInputDevices = await DeviceInformation.FindAllAsync(midiInputQueryString);

            midiInPortListBox.Items.Clear();

            // Return if no external devices are connected
            if (midiInputDevices.Count == 0)
            {
                this.midiInPortListBox.Items.Add("No MIDI input devices found!");
                this.midiInPortListBox.IsEnabled = false;
                return;
            }

            // Else, add each connected input device to the list
            foreach (DeviceInformation deviceInfo in midiInputDevices)
            {
                this.midiInPortListBox.Items.Add(deviceInfo.Name);
            }
            this.midiInPortListBox.IsEnabled = true;
        }

        private async Task EnumerateMidiOutputDevices()
        {

            // Find all output MIDI devices
            string midiOutportQueryString = MidiOutPort.GetDeviceSelector();
            DeviceInformationCollection midiOutputDevices = await DeviceInformation.FindAllAsync(midiOutportQueryString);

            midiOutPortListBox.Items.Clear();

            // Return if no external devices are connected
            if (midiOutputDevices.Count == 0)
            {
                this.midiOutPortListBox.Items.Add("No MIDI output devices found!");
                this.midiOutPortListBox.IsEnabled = false;
                return;
            }

            // Else, add each connected input device to the list
            foreach (DeviceInformation deviceInfo in midiOutputDevices)
            {
                this.midiOutPortListBox.Items.Add(deviceInfo.Name);
            }
            this.midiOutPortListBox.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 60;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 61;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 62;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 63;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 64;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

    }
}
