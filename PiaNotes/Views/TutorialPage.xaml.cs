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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TutorialPage : Page
    {
        public TutorialPage()
        {
            this.InitializeComponent();
        }

        #region NavigationView event handlers
        private void TutorialNV_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem
            foreach (NavigationViewItemBase item in TutorialNV.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Home_Page")
                {
                    TutorialNV.SelectedItem = item;
                    break;
                }
            }
            contentFrame.Navigate(typeof(Views.TutorialPages._1Tutorial));
        }

        private void TutorialNV_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            TextBlock ItemContent = args.InvokedItem as TextBlock;
            if (ItemContent != null)
            {
                switch (ItemContent.Tag)
                {
                    case "First_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._1Tutorial));
                        break;

                    case "Second_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._2Tutorial));
                        break;

                    case "Third_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._3Tutorial));
                        break;

                    case "Fourth_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._4Tutorial));
                        break;

                    case "Fifth_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._5Tutorial));
                        break;

                    case "Sixth_Tutorial":
                        contentFrame.Navigate(typeof(Views.TutorialPages._6Tutorial));
                        break;
                }
            }
        }
        #endregion

        private void TutorialNV_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            this.Frame.Navigate(typeof(Views.SelectionPage));
        }
    }
}
