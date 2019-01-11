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
    public sealed partial class ScorePage : Page
    {

        public ScorePage()
        {
            this.InitializeComponent();
            double division = (double) Scores.notesHit / (double) Scores.notesAmount;
            System.Diagnostics.Debug.WriteLine("Notes hit: " + Scores.notesHit + ", Note amount: " + Scores.notesAmount + ", Percentage: " + division);
            if (Scores.score == 0)
            {
                TXTBlock_PresetMSG.Text = "Oops.. You missed every note...";
            }
            else if (division > 0.00001 && division < 0.2)
            {
                TXTBlock_PresetMSG.Text = "Close but not close...";
            }
            else if (division > 0.20001 && division < 0.4)
            {
                TXTBlock_PresetMSG.Text = "You probably need some more practice...";
            }
            else if (division > 0.40001 && division < 0.6)
            {
                TXTBlock_PresetMSG.Text = "You're getting there...";
            }
            else if (division > 0.60001 && division < 0.8)
            {
                TXTBlock_PresetMSG.Text = "You're officially above average, congratulations.";
            }
            else if (division > 0.80001 && division < 0.999999999)
            {
                TXTBlock_PresetMSG.Text = "You're getting close! Almost 100%!";
            }
            else
            {
                TXTBlock_PresetMSG.Text = "Good job! You aced the song!";
            }

            TXTBlock_Score.Text = "Score: " + Scores.score;
            TXTBlock_Notes.Text = "Accuracy: " + Scores.notesHit + " / " + Scores.notesAmount;
            TXTBlock_Percentage.Text = "Percentage: " + Math.Round(division, 4) * 100 + "%";
        }

        // Navigate to the selection page
        private void NavSelection_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SelectionPage));

    }
}

