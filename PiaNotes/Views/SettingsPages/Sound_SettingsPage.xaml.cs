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
        

        public static double Velocity
        {
            get
            {
                if (localSettings.Values["Velocity"] != null)
                {
                    return (double)localSettings.Values["Velocity"];
                }
                else
                {
                    return 90;
                }
            }

            set
            {
                if (value >= 0 && value <= 100)
                {
                    localSettings.Values["Velocity"] = value;
                }
                else
                {
                    localSettings.Values["Velocity"] = 90;
                }
            }
        }

        public static double Volume {
            get
            {
                if (localSettings.Values["Volume"] != null)
                {
                    return (double)localSettings.Values["Volume"];
                } else
                {
                    return 0;
                }
            }

            set
            {
                if (value >= 0 && value <= 100)
                {
                    localSettings.Values["Volume"] = value;
                }
                else
                {
                    localSettings.Values["Volume"] = 0;
                }
            }
        }

        public Sound_SettingsPage()
        {
            this.InitializeComponent();

            

            //Set the slider back to the values the user put in and activate the correct settings
            if (localSettings.Values["Velocity"] != null)
            {
                double velocity = (double)localSettings.Values["Velocity"];
                velocitySlider.Value = (velocity - 27);
            }
            else
            {
                localSettings.Values["Velocity"] = 90;
                double velocity = (double)localSettings.Values["Velocity"];
                velocitySlider.Value = (velocity - 27);
            }

            if (localSettings.Values["Volume"] != null)
            {
                double volume = (double)localSettings.Values["Volume"];
                volumeSlider.Value = (volume + 50);
            }
            else
            {
                localSettings.Values["Volume"] = 90;
                double volume = (double)localSettings.Values["Volume"];
                volumeSlider.Value = (volume + 50);
            }


            if (localSettings.Values["Feedback"] != null)
            {
                bool feedback = (bool)localSettings.Values["Feedback"];
                if (feedback)
                {
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
                else
                {
                    Feedback.IsChecked = true;
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
            }
            else
            {
                localSettings.Values["Feedback"] = true;
                bool feedback = (bool)localSettings.Values["Feedback"];
                if (feedback)
                {
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
                else
                {
                    Feedback.IsChecked = true;
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
            }
        }

        private void Velocity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the velocity slider, set the new value +27
            //+27 is there so the slider goes from 0 to 100, instead of 27 to 127
            Velocity = (e.NewValue + 27);
        }

        private void Volume_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //If the slider value changed from the volume slider, set the new value -50.
            //-50 is there so 50 = 0 and 0 = -50. This is so the volume can be lowered.
            Volume = (e.NewValue - 50);
        }

        private void Velocity_Checked(object sender, RoutedEventArgs e)
        {
            //Checking if the feedback setting is turned off or on and act accordingly
            if (localSettings.Values["Feedback"] != null)
            {
                bool feedback = (bool)localSettings.Values["Feedback"];
                if (feedback)
                {
                    localSettings.Values["Feedback"] = false;
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
                else
                {
                    localSettings.Values["Feedback"] = true;
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
            }
            else
            {
                localSettings.Values["Feedback"] = true;
                bool feedback = (bool)localSettings.Values["Feedback"];
                if (feedback)
                {
                    localSettings.Values["Feedback"] = false;
                    volumeSlider.IsEnabled = false;
                    velocitySlider.IsEnabled = true;
                }
                else
                {
                    localSettings.Values["Feedback"] = true;
                    volumeSlider.IsEnabled = true;
                    velocitySlider.IsEnabled = false;
                }
            }
        }

        private void BTN_Save(object sender, RoutedEventArgs e)
        {

        }
    }
}
