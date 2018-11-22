using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Midi;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

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

            // zet notes in array zodat ze gekleurd kunnen worden, de "s" in bijv. cs0, cs2, cs4 betekent # / fis.
            //List<Rectangle> Notes = new List<Rectangle>(new Rectangle[] 

            //    { cn1, csn1, dn1, dsn1, en1, esn1, fn1, fsn1, gn1, gsn1, an1, asn1, bn1, bsn1
            //    , c0, cs0, d0, ds0, e0, es0, f0, fs0, g0, gs0, a0, as0, b0, bs0
            //    , c1, cs1, d1, ds1, e1, es1, f1, fs1, g1, gs1, a1, as1, b1, bs1
            //    , c2, cs2, d2, ds2, e2, es2, f2, fs2, g2, gs2, a2, as2, b2, bs2
            //    , c3, cs3, d3, ds3, e3, es3, f3, fs3, g3, gs3, a3, as3, b3, bs3
            //    , c4, cs4, d4, ds4, e4, es4, f4, fs4, g4, gs4, a4, as4, b4, bs4
            //    , c5, cs5, d5, ds5, e5, es5, f5, fs5, g5, gs5, a5, as5, b5, bs5
            //    , c6, cs6, d6, ds6, e6, es6, f6, fs6, g6, gs6, a6, as6, b6, bs6
            //    , c7, cs7, d7, ds7, e7, es7, f7, fs7, g7, gs7, a7, as7, b7, bs7
            //    , c8, cs8, d8, ds8, e8, es8, f8, fs8, g8, gs8, a8, as8, b8, bs8
            //    , c9, cs9, d9, ds9, e9, es9, f9, fs9, g9, gs9, a9, as9, b9, bs9
            //    , c10, cs10, d10, ds10, e10, es10, f10, fs10, g10, gs10, a10, as10, b10, bs10});
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
                //If the player releases the key there should be no sound
                byte velocity;
                if (((MidiNoteOnMessage)receivedMidiMessage).Velocity != 0)
                {
                    if (Settings.feedback == true)
                    {
                        //Use the input from the keyboard the see what the normal velocity is and then add the volume the user chose
                        velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;
                        
                        if (velocity + DoubleToByte(Settings.volume) <= 127 && velocity + DoubleToByte(Settings.volume) >= 0)
                        {
                            velocity += DoubleToByte(Settings.volume);
                        } else if(velocity + DoubleToByte(Settings.volume) > 127)
                        {
                            velocity = 127;
                        } else
                        {
                            velocity = 0;
                        }
                        //Else use the static velocity the user chose
                    } else velocity = DoubleToByte(Settings.velocity);
                    //Else do not produce any sound, when the input is 0
                } else velocity = ((MidiNoteOnMessage)receivedMidiMessage).Velocity;

                IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);
                FillKey(midiMessageToSend);
                Settings.midiOutPort.SendMessage(midiMessageToSend);
            }
            if (receivedMidiMessage.Type == MidiMessageType.NoteOff)
            {
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Channel);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Note);
                System.Diagnostics.Debug.WriteLine(((MidiNoteOffMessage)receivedMidiMessage).Velocity);

                byte channel = ((MidiNoteOffMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOffMessage)receivedMidiMessage).Note;
                byte velocity = ((MidiNoteOffMessage)receivedMidiMessage).Velocity;

                IMidiMessage midiMessageToSend = new MidiNoteOffMessage(channel, note, velocity);
                UnFillKey(midiMessageToSend);
            }
        }

        private async void FillKey(IMidiMessage IM)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte note = ((MidiNoteOnMessage)IM).Note;
                try
                {
                    //Notes[note].Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
                }
                catch (Exception e)
                {
                    return;
                }
            });
        }

        private async void UnFillKey(IMidiMessage IM)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                byte note = ((MidiNoteOffMessage)IM).Note;
                try
                {
                    //Notes[note].Fill = new SolidColorBrush(Windows.UI.Colors.White);
                }
                catch (Exception e)
                {
                    return;
                }
            });
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
