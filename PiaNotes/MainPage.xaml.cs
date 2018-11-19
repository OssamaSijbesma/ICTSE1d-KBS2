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

            SetMidiInput();
        }

        private void SetMidiInput()
        {
            int i = 0;
            while (i == 0)
            {
                if (Settings.midiInPort == null) ;
                else
                {
                    Settings.midiInPort.MessageReceived += MidiInPort_MessageReceived;
                }
            }
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            IMidiMessage receivedMidiMessage = args.Message;

            System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Velocity);

                byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                byte velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

                Settings.midiOutPort.SendMessage(midiMessageToSend);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 60;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 61;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 62;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 63;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            byte channel = 0;
            byte note = 64;
            byte velocity = 127;
            IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);

            Settings.midiOutPort.SendMessage(midiMessageToSend);
        }
    }
}
