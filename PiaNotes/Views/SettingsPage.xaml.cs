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
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using PiaNotes.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        MidiDeviceWatcher inputDeviceWatcher;
        MidiDeviceWatcher outputDeviceWatcher;

        private bool showMessage = true;

        public SettingsPage()
        {
            this.InitializeComponent();

            //Device watchers added to watch for the MIDI input and output devices
            inputDeviceWatcher = new MidiDeviceWatcher(MidiInPort.GetDeviceSelector(), midiInPortListBox, Dispatcher);
            inputDeviceWatcher.StartWatcher();
            outputDeviceWatcher = new MidiDeviceWatcher(MidiOutPort.GetDeviceSelector(), midiOutPortListBox, Dispatcher);

            //Start the Device Watchers
            outputDeviceWatcher.StartWatcher();

            //Set the slider back to the values the user put in and activate the correct settings
            velocitySlider.Value = (Settings.velocity - 27);
            volumeSlider.Value = (Settings.volume + 50);
            if (Settings.feedback == true)
            {
                volumeSlider.IsEnabled = true;
                velocitySlider.IsEnabled = false;
            } else
            {
                Feedback.IsChecked = true;
                volumeSlider.IsEnabled = false;
                velocitySlider.IsEnabled = true;
            }
        }

        private async void MidiInPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                var popups = VisualTreeHelper.GetOpenPopups(Window.Current);

                foreach (var popup in popups)
                {
                    if (!(popup.Child is ContentDialog))
                    {
                        await StaticObjects.NoMidiInOutDialog.ShowAsync();
                        showMessage = false;
                    }
                }
                if(showMessage)
                {
                    await StaticObjects.NoMidiInOutDialog.ShowAsync();
                }

                System.Diagnostics.Debug.WriteLine(b.Message);
            }  
        }

        private void Velocity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the velocity slider, set the new value +27
            //+27 is there so the slider goes from 0 to 100, instead of 27 to 127
            Settings.velocity = (e.NewValue + 27); 
        }

        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the volume slider, set the new value -50.
            //-50 is there so 50 = 0 and 0 = -50. This is so the volume can be lowered.
            Settings.volume = (e.NewValue - 50);
        }

        private async void MidiOutPortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Velocity_Checked(object sender, RoutedEventArgs e)
        {
            //Checking if the feedback setting is turned off or on and act accordingly
            if(Settings.feedback == true)
            {
                Settings.feedback = false;
                volumeSlider.IsEnabled = false;
                velocitySlider.IsEnabled = true;
            } else
            {
                Settings.feedback = true;
                volumeSlider.IsEnabled = true;
                velocitySlider.IsEnabled = false;
            }
        }

        /// <summary>
        /// On click navigation
        /// </summary>

        // Return to previous page
        private void NavBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
            else
                this.Frame.Navigate(typeof(SelectionPage));
        }
    }
}
