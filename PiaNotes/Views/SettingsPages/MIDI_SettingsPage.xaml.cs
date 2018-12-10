using PiaNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MIDI_SettingsPage : Page
    {
        MidiDeviceWatcher inputDeviceWatcher;
        MidiDeviceWatcher outputDeviceWatcher;

        // Storing in Local
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        static StorageFolder localFolder = ApplicationData.Current.LocalFolder;


        //Octave Settings
        //OctaveStart is the starting location of the octaves frequenty
        private static int DefaultStartingOctave = 6;
        public static int StartingOctave
        {
            get
            {
                if (localSettings.Values["StartingOctave"] != null)
                {
                    return (int) localSettings.Values["StartingOctave"];
                } else
                {
                    return DefaultStartingOctave;
                }
                
            }

            set
            {
                if (value > -1 && value < 9)
                    localSettings.Values["StartingOctave"] = value;
            }
        }


        //OctaveAmount is the amount of octaves on your screen at once
        private static int DefaultOctaveAmount = 4;
        public static int OctaveAmount
        {
            get
            {
                if (localSettings.Values["OctaveAmount"] != null)
                {
                    return (int)localSettings.Values["OctaveAmount"];
                }
                else
                {
                    return DefaultOctaveAmount;
                }
            }

            set
            {
                localSettings.Values["OctaveAmount"] = value;
            }
        }

        bool selected;

        public MIDI_SettingsPage()
        {
            this.InitializeComponent();

            CMB_OctaveAmount.IsEnabled = false;

            
            if (localSettings.Values["StartingOctave"] != null)
            {
                if ((int)localSettings.Values["StartingOctave"] > 0 || (int)localSettings.Values["StartingOctave"] < 9)
                {
                    CMB_StartingOctave.SelectedIndex = (int)localSettings.Values["StartingOctave"];
                }
            }

            if (localSettings.Values["OctaveAmount"] != null)
            {
                if ((int) localSettings.Values["OctaveAmount"] > 0 || (int)localSettings.Values["OctaveAmount"] < 9)
                {
                    CMB_OctaveAmount.SelectedIndex = (int)localSettings.Values["OctaveAmount"];
                } 
            }

            localFolder = ApplicationData.Current.LocalFolder;
            localSettings = ApplicationData.Current.LocalSettings;

            // Device watchers added to watch for the MIDI input and output devices
            inputDeviceWatcher = new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), midiInPortListBox, Dispatcher);
            inputDeviceWatcher.StartWatcher();
            outputDeviceWatcher = new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), midiOutPortListBox, Dispatcher);

            //Start the Device Watchers
            outputDeviceWatcher.StartWatcher();

            //Set the slider back to the values the user put in and activate the correct settings
        }

        private async void midiInPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Collect device information about the input devices, if there's no info -> return
            var deviceInformationCollection = inputDeviceWatcher.DeviceInformationCollection;
            if (deviceInformationCollection == null) return;

            //Checks if the list contains a Midi In device. If there's no In Device there will be a msg saying that there are no devices connected. If you click this msg an error will pop-up
            try
            {
                // Checks if selectedindex is -1. -1 Means that nothing is selected -> if you dont check this it shows the error pop-up
                if (midiInPortListBox.SelectedIndex != -1)
                {
                    DeviceInformation devInfo = deviceInformationCollection[midiInPortListBox.SelectedIndex];
                    if (devInfo == null) return;

                    //Await the response of the input device. If there is nothing return with debug writeline
                    Settings.midiInPort = await MidiInPort.FromIdAsync(devInfo.Id);
                    if (Settings.midiInPort == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to create MidiInPort from input device");
                        return;
                    }
                }
            }
            catch (Exception b)
            {
                // Sets SelectedIndex to -1, making it so that whatever was previously selected will now be unselected.
                midiInPortListBox.SelectedIndex = -1;
                await StaticObjects.NoMidiInOutDialog.ShowAsync();
                System.Diagnostics.Debug.WriteLine(b.Message);
            }
        }

        private async void midiOutPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Collect device information about the output devices, if there's no info -> return
            var deviceInformationCollection = outputDeviceWatcher.DeviceInformationCollection;
            if (deviceInformationCollection == null) return;

            //Checks if the list contains a Midi Out device. If there's no Out device there will be a msg saying that there are no devices connected. If you click this msg an error will pop-up
            try
            {
                // Checks if selectedindex is -1. -1 Means that nothing is selected -> if you dont check this it shows the error pop-up
                if (midiOutPortListBox.SelectedIndex != -1)
                {
                    DeviceInformation devInfo = deviceInformationCollection[midiOutPortListBox.SelectedIndex];
                    if (devInfo == null) return;

                    //Await the response of the output device. If there is nothing return with debug writeline
                    Settings.midiOutPort = await MidiOutPort.FromIdAsync(devInfo.Id);
                    if (Settings.midiOutPort == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Unable to create MidiOutPort from input device");
                        return;
                    }
                }
            }
            catch (Exception b)
            {
                // Sets SelectedIndex to -1, making it so that whatever was previously selected will now be unselected.
                midiInPortListBox.SelectedIndex = -1;
                await StaticObjects.NoMidiInOutDialog.ShowAsync();
                System.Diagnostics.Debug.WriteLine(b.Message);
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

        private void BTN_Save(object sender, RoutedEventArgs e)
        {
            // todo implementation, neccesary? probably not
        }

        private void OctaveStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected = true;
            CMB_OctaveAmount.IsEnabled = true;

            // Setting the actual StartingOctave to the selection.
            if (localSettings.Values["StartingOctave"] != null)
            {
                ComboBox cmb = (ComboBox)sender;
                string selectedIndex = cmb.SelectedIndex.ToString();
                int selectedValue = Int32.Parse(selectedIndex);
                StartingOctave = selectedValue;
            }
            else
            {
                localSettings.Values["StartingOctave"] = DefaultOctaveAmount;
                ComboBox cmb = (ComboBox)sender;
                string selectedIndex = cmb.SelectedIndex.ToString();
                int selectedValue = Int32.Parse(selectedIndex);
                StartingOctave = selectedValue;
            }

            ComboBoxItem StartSelection = (ComboBoxItem)CMB_StartingOctave.SelectedValue;
            string selString = StartSelection.Content.ToString();
            int selInt = Int32.Parse(selString);

            switch (selInt)
            {
                case 0:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 12; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 1:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 11; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 2:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 10; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;

                case 3:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 9; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 4:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 8; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 5:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 7; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 6:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 6; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 7:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 5; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 8:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 4; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 9:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 3; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
                case 10:
                    CMB_OctaveAmount.Items.Clear();
                    for (int i = 1; i < 2; i++)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = "" + i;
                        item.Value = i;
                        CMB_OctaveAmount.Items.Add(item);
                    }
                    break;
            }
        }


        private void OctaveAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            // Setting the actual OctaveAmount to the selection.
            if (localSettings.Values["OctaveAmount"] != null)
            {
                ComboBox cmb = (ComboBox)sender;
                string selectedIndex = cmb.SelectedIndex.ToString();
                int selectedValue = Int32.Parse(selectedIndex);
                OctaveAmount = selectedValue;
            }
            else
            {
                localSettings.Values["OctaveAmount"] = DefaultOctaveAmount;
                ComboBox cmb = (ComboBox)sender;
                string selectedIndex = cmb.SelectedIndex.ToString();
                int selectedValue = Int32.Parse(selectedIndex);
                OctaveAmount = selectedValue;
            }

        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
