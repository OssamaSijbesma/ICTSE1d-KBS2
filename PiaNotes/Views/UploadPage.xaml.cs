using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace PiaNotes.Views
{
    /// <summary>
    /// UploadPage, used for uploading Midi files to the database.
    /// </summary>
    public sealed partial class UploadPage : Page
    {
        public UploadPage()
        {
            this.InitializeComponent();
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void OnOpenFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".midi");
            openPicker.FileTypeFilter.Add(".mid");
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            }
            else
            {
                //  
            }

        }

        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            return;
        }

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

