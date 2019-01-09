using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using PiaNotes.Models;
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
using Windows.Storage.FileProperties;
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
        
        
        private Byte[] fileByte;
        private string fileName;
        public bool FileSelected { get; set; } = false;
        private StorageFile file;
        private SheetMusic SM;
        private Stream streamMIDI;
        private MidiFile midiFile;
        private MidiParser midiParser;

        public UploadPage()
        {
            this.InitializeComponent();
        }
        
        // FilePicker for Midi File.
        private async void OnOpenFile(object sender, RoutedEventArgs e)
        {
            TXTBox_Title.Text = "";
            // Open file picker.
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.Downloads
            };
            openPicker.FileTypeFilter.Add(".midi");
            openPicker.FileTypeFilter.Add(".mid");
            file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                // File selected.
                TXTBlock_Status.Text = "Converting MIDI file, please wait...";
                TXTBox_Title.Text = file.DisplayName;
                var stream2 = await file.OpenStreamForReadAsync();

                MidiConverter midiConverter = new MidiConverter();
                // Change for midiUpload.
                fileByte = midiConverter.MidiToBytes(stream2);
                fileName = file.Name;
                BasicProperties fileSize = await file.GetBasicPropertiesAsync();
                
                // Check file size.
                if (fileSize.Size > 20000)
                {
                    // File size too large, stop operation.
                    TXTBlock_Status.Text = "MIDI file is too large! Please try another file.";
                    Reset();
                }
                else
                {
                    // File size isn't too large, continue to reading MIDI file
                    streamMIDI = await file.OpenStreamForReadAsync();
                    midiFile = MidiFile.Read(streamMIDI);
                    midiParser = new MidiParser(midiFile);
                    SM = midiParser.sheetMusic;
                
                    // Check range of selected MIDI file.
                    if (midiConverter.GetOctaveInfo(SM).Item3 > 2)
                    {
                        // MIDI file not in range, not usable.
                        TXTBlock_Status.Text = "MIDI file is out of range! Please try another file.";
                        Reset();
                    }
                    else
                    {
                        // MIDI file in range, usable.
                        TXTBlock_Status.Text = "MIDI file converted.";
                        FileSelected = true;
                    }
                    // Check uploaded file's name length and shorten it if necessary.
                    if (FileSelected && TXTBox_Title.Text.Length > 100)
                    {
                        TXTBox_Title.Text = TXTBox_Title.Text.Substring(0, 100);
                        TXTBlock_Status.Text += "\nThe file name is too long, it has been shortened.";
                    }
                }
            }
        }

        public void Reset()
        {
            TXTBox_Title.Text = "";
            midiFile.RemoveNotes();
        }

        // Submit midi file functionality.
        private void OnSubmit(object sender, RoutedEventArgs e)
        {
            if (FileSelected && TXTBox_Title.Text.Length <= 100)
            {
                if (DB.CheckConnection() == true)
                {
                    DB.Upload(TXTBox_Title.Text, fileByte, fileName, 1);
                    TXTBlock_Status.Text = "File uploaded.";

                    //Added a navigation to the selection after uploading succesfully
                    this.Frame.Navigate(typeof(SelectionPage));
                }
                else if (DB.CheckConnection() == false)
                {
                    // Navigate to the practice page unless MIDI is not set then show a dialog and go to the settings page
                    StorageFile storageFileMIDI = file;
                    midiParser = new MidiParser(midiFile);

                    // Navigate to the practice page
                    this.Frame.Navigate(typeof(PracticePage), midiParser.sheetMusic);
                }
                FileSelected = false;
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

