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
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiaNotes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 60;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 61;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 62;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 63;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 64;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            midiOutPort.SendMessage(midiMessageToSend);
        }
    }
}
