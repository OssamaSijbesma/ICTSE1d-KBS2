using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Midi;
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
    public sealed partial class PracticePage : Page
    {
        public PracticePage()
        {
            this.InitializeComponent();

            // Register a handler for the MessageReceived event
            Settings.midiInPort.MessageReceived += MidiInPort_MessageReceived;
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            // Recieved message
            IMidiMessage receivedMidiMessage = args.Message;

            System.Diagnostics.Debug.WriteLine(receivedMidiMessage.Timestamp.ToString());

            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOnMessage)receivedMidiMessage).Velocity);

                // Play the note
                byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                //If the player releases the key there should be no sound else their should be more sound
                byte velocity;
                if (((MidiNoteOnMessage)receivedMidiMessage).Velocity != 0)
                {
                    if (Settings.feedback == true)
                    {
                        //Use the input from the keyboard the see what the normal velocity is and then add the volume the user chose
                        velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                        velocity += DoubleToByte(Settings.volume);
                        //Else use the static velocity the user chose
                    } else velocity = DoubleToByte(Settings.velocity);
                    //Else do not produce any sound, when the input is 0
                } else velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;

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

        private void Nav_main_Click(object sender, RoutedEventArgs e)
        {
            Settings.midiInPort.MessageReceived -= MidiInPort_MessageReceived;
            this.Frame.Navigate(typeof(MainPage));
        }

        public static byte DoubleToByte(double doubleVal)
        {
            byte byteVal = 0;

            // Double to byte conversion can overflow.
            try
            {
                byteVal = System.Convert.ToByte(doubleVal);
                return byteVal;
            }
            catch (System.OverflowException)
            {
                System.Console.WriteLine(
                    "Overflow in double-to-byte conversion.");
            }

            // Byte to double conversion cannot overflow.
            doubleVal = System.Convert.ToDouble(byteVal);
            return byteVal;
        }
    }
}
