using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Midi;

namespace PiaNotes.ViewModels
{

    class MidiDeviceWatcher 
    {
        private DeviceWatcher deviceWatcher;
        private string DeviceSelectorString { get; set; }
        private ListBox deviceListBox;
        private CoreDispatcher coreDispatcher;

        public DeviceInformationCollection DeviceInformationCollection { get; set; }

        public MidiDeviceWatcher(string midiDeviceSelectorString, ListBox midiDeviceListBox, CoreDispatcher dispatcher)
        {
            // Constructor
            deviceListBox = midiDeviceListBox;
            coreDispatcher = dispatcher;

            DeviceSelectorString = midiDeviceSelectorString;

            deviceWatcher = DeviceInformation.CreateWatcher(DeviceSelectorString);
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        ~MidiDeviceWatcher()
        {
            // Destructor to unregister the watcher event handlers and set the device watcher to null
            deviceWatcher.Added -= DeviceWatcher_Added;
            deviceWatcher.Removed -= DeviceWatcher_Removed;
            deviceWatcher.Updated -= DeviceWatcher_Updated;
            deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
            deviceWatcher = null;
        }

        public void StartWatcher()
        {
            // Start the watcher
            deviceWatcher.Start();
        }

        public void StopWatcher()
        {
            // Stop the watcher
            deviceWatcher.Stop();
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // Event which is raised when a device is added
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Event which is raised when the information assosiated with an excisting device is changed
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Event which is raised when a device is removed
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // Event which is raised when the watcher has completed its enumeration of the requested device type
            await coreDispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                // Update the device list
                UpdateDevices();
            });
        }

        private async void UpdateDevices()
        {
            // Get a list of all MIDI devices
            this.DeviceInformationCollection = await DeviceInformation.FindAllAsync(DeviceSelectorString);

            deviceListBox.Items.Clear();

            if (!this.DeviceInformationCollection.Any())
            {
                deviceListBox.Items.Add("No MIDI devices found!");
            }

            foreach (var deviceInformation in this.DeviceInformationCollection)
            {
                deviceListBox.Items.Add(deviceInformation.Name);
            }
        }

    }

}
