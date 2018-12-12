using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class Sound_SettingsPage : Page
    {
        // Storing in Local
        static StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        #region Properties
        public static double Velocity
        {
            get
            {
                if (localSettings.Values["Velocity"] != null)
                {
                    return (double) localSettings.Values["Velocity"];
                }
                else
                {
                    return 90;
                }
            }

            set
            {
                localSettings.Values["Velocity"] = value;
                Settings.velocity = value;
            }
        }

        public static double Volume
        {
            get
            {
                if (localSettings.Values["Volume"] != null)
                {
                    return (double) localSettings.Values["Volume"];
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                localSettings.Values["Volume"] = value;
                Settings.volume = value;
            }
        }

        public static bool DisableUserFeedback
        {
            get
            {
                if (localSettings.Values["DisableUserFeedback"] != null)
                {
                    return (bool)localSettings.Values["DisableUserFeedback"];
                }
                else
                {
                    return false;
                }
            }

            set
            {
                localSettings.Values["DisableUserFeedback"] = value;
                Settings.disableUserFeedback = value;
            }
        }
        #endregion

        public Sound_SettingsPage()
        {
            this.InitializeComponent();

            //Set the slider back to the values the user put in and activate the correct settings
            if (localSettings.Values["Velocity"] != null)
            {
                System.Diagnostics.Debug.WriteLine("Velocity: " + localSettings.Values["Velocity"]);
                System.Diagnostics.Debug.WriteLine("VolumeValue: " + localSettings.Values["Volume"]);
                Velocity = (double) localSettings.Values["Velocity"];
                velocitySlider.Value = Velocity;

            }
            else
            {
                localSettings.Values["Velocity"] = 90;
                Velocity = (double) localSettings.Values["Velocity"];
                velocitySlider.Value = Velocity;
            }

            if (localSettings.Values["Volume"] != null)
            {
                Velocity = (double)localSettings.Values["Volume"];
                volumeSlider.Value = Volume;
            }
            else
            {
                localSettings.Values["Volume"] = 0;
                Volume = (double) localSettings.Values["Volume"];
                volumeSlider.Value = Volume;
            }


            if (localSettings.Values["DisableFeedback"] != null)
            {
                DisableUserFeedback = (bool)localSettings.Values["DisableFeedback"];
                if (DisableUserFeedback)
                {
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
                else
                {
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
            }
            else
            {
                volumeSlider.IsEnabled = true;
                velocitySlider.IsEnabled = false;
                DisableUserFeedbackCheckbox.IsChecked = false;
            }
        }

        private void Velocity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the velocity slider, set the new value +27
            //+27 is there so the slider goes from 0 to 100, instead of 27 to 127
            Velocity = (e.NewValue);
            System.Diagnostics.Debug.WriteLine("Velocity: " + localSettings.Values["Velocity"]);
        }

        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the volume slider, set the new value -50.
            //-50 is there so 50 = 0 and 0 = -50. This is so the volume can be lowered.
            Volume = (e.NewValue);
            System.Diagnostics.Debug.WriteLine("Volume: " + localSettings.Values["Volume"]);
        }

        private void OnClickDisableUserFeedback(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("DisableUserFeedback: " + localSettings.Values["DisableUserFeedback"]);
            //Checking if the feedback setting is turned off or on and act accordingly
            if (localSettings.Values["DisableUserFeedback"] != null)
            {
                DisableUserFeedback = (bool)localSettings.Values["DisableUserFeedback"];
                if (!DisableUserFeedback)
                {
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
                else
                {
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
            }
            else
            {
                localSettings.Values["DisableUserFeedback"] = true;
                DisableUserFeedback = (bool) localSettings.Values["DisableUserFeedback"];
                if (!DisableUserFeedback)
                {
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
                else
                {
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
            }

            if ((bool)DisableUserFeedbackCheckbox.IsChecked)
            {
                DisableUserFeedback = true;
            }
            else
            {
                DisableUserFeedback = false;
            }

        }
    }
}
