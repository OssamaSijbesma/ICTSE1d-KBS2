using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using PiaNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
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
        Databaser DB = new Databaser();
        private string midiString;
        private Byte[] fileByte;
        private string fileName;
        public bool FileSelected { get; set; } = false;

        public UploadPage()
        {
            this.InitializeComponent();
        }

        private void Title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        // FilePicker for Midi File.
        private async void OnOpenFile(object sender, RoutedEventArgs e)
        {
            TXTBox_Title.Text = "";

            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            openPicker.FileTypeFilter.Add(".midi");
            openPicker.FileTypeFilter.Add(".mid");
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                TXTBlock_Status.Text = "Converting MIDI file...";
                TXTBox_Title.Text = file.DisplayName;
                var stream = await file.OpenStreamForReadAsync();
                var stream2 = await file.OpenStreamForReadAsync();

                MidiConverter midiConverter = new MidiConverter();
                midiString = midiConverter.MidiToString(stream);
                //Change for midiUpload
                fileByte = midiConverter.MidiToBytes(stream2);
                fileName = file.Name;


                if (midiString.Length < 2000000)
                {
                    FileSelected = true;
                    TXTBlock_Status.Text = "MIDI file converted.";
                }
                else
                {
                    TXTBox_Title.Text = "";
                    TXTBlock_Status.Text = "File is too large! Please try another file.";
                }
                if (FileSelected && TXTBox_Title.Text.Length > 100)
                {
                    TXTBox_Title.Text = TXTBox_Title.Text.Substring(0, 100);
                    TXTBlock_Status.Text += "\nThe file name is too long, it has been shortened.";
                }
            }
        }
        
        // Submit midi file functionality.
        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            if (FileSelected && midiString.Length < 2000000 && TXTBox_Title.Text.Length <= 100)
            {
                DB.Upload(TXTBox_Title.Text, fileByte, fileName);
                FileSelected = false;
                TXTBlock_Status.Text = "File uploaded.";

                //Added a navigation to the selection after uploading succesfully
                this.Frame.Navigate(typeof(SelectionPage));
            }
            else if (!FileSelected)
            {
                TXTBlock_Status.Text = "Please select a file.";
            }
            else if (TXTBox_Title.Text.Length > 100)
            {
                TXTBlock_Status.Text = "The entered title is too long, please make it shorter.";
            }
            else
            {
                TXTBlock_Status.Text = "Something went wrong. Please try again.";
            }
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

