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

namespace PiaNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        #region NavigationView event handlers
        private void SettingsNV_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in SettingsNV.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Home_Page")
                {
                    SettingsNV.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(Views.SettingsPages.MIDI_SettingsPage));
        }

        private void SettingsNV_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            TextBlock ItemContent = args.InvokedItem as TextBlock;
            if (ItemContent != null)
            {
                switch (ItemContent.Tag)
                {
                    case "MIDI_SettingsPage":
                        contentFrame.Navigate(typeof(Views.SettingsPages.MIDI_SettingsPage));
                        break;

                    case "Sound_SettingsPage":
                        contentFrame.Navigate(typeof(Views.SettingsPages.Sound_SettingsPage));
                        break;

                    case "Theme_SettingsPage":
                        contentFrame.Navigate(typeof(Views.SettingsPages.Theme_SettingsPage));
                        break;
                }
            }
        }
        #endregion

        private void SettingsNV_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack(); 

            else 
            this.Frame.Navigate(typeof(Views.SelectionPage));
        }
    }
}
