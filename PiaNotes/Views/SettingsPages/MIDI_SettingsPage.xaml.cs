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

        public MIDI_SettingsPage()
        {
            this.InitializeComponent();

            // Device watchers added to watch for the MIDI input and output devices
            inputDeviceWatcher = new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), midiInPortListBox, Dispatcher);
            inputDeviceWatcher.StartWatcher();
            outputDeviceWatcher = new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), midiOutPortListBox, Dispatcher);

            //Start the Device Watchers
            outputDeviceWatcher.StartWatcher();
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

        }

        private void OctaveStart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }
        // TODO:
        // - Check on max amount of octaves. If start == 8 then max can't be higher than 3 because there are only 11 octaves.
        // - Cases for all possible selections.
        // - Setting the actual StartingOctave to the selection.

        // Setting the actual OctaveAmount to the selection.
        public int StartingOctave;

        private void OctaveAmount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { }
        // TODO:
        // - Check on starting octave. If start == 8 then max can't be higher than 3 because there are only 11 octaves.
        // - Setting the actual OctaveAmount to the selection.
        // - Making the keyboard get the OctaveAmount from this file instead of Settings.cs

        // Setting the actual OctaveAmount to the selection.
        public int OctaveAmount;

    }
}
