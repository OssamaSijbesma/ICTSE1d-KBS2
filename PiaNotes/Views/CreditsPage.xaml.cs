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
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;
using System.ComponentModel.DataAnnotations;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiaNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreditsPage : Page
    {
        public CreditsPage()
        {
            this.InitializeComponent();

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Credits";

            LoadCredits();
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
        
        private void LoadCredits()
        {
            string[] names = {
                "Ossama Sijbesma",
                "Martijn Sikkema",
                "Jeffrey Norder",
                "Mart Schilthuis",
                "Sybren de Vries",
                "Kjell Kanis"
            };

            string[] credits = {
                "Lead Programmer",
                "Lead Designer\nProgrammer",
                "Programmer",
                "Programmer",
                "Programmer",
                "Programmer\nDesigner"
            };

            for (int i = 0; i < 6; i++)
            {
                TextBlock TXTBlock_Name = new TextBlock();
                TXTBlock_Name.Text = names[i];
                TXTBlock_Name.FontWeight = FontWeights.SemiBold;
                TXTBlock_Name.FontSize = 20;
                SPCredits.Children.Add(TXTBlock_Name);
                
                TextBlock TXTBlock_Credit = new TextBlock();
                TXTBlock_Credit.Text = credits[i];
                TXTBlock_Credit.Margin = new Thickness(10, 0, 0, 10);
                SPCredits.Children.Add(TXTBlock_Credit);
            }
        }
    }
}
