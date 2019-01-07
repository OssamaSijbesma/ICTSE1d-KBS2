using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using PiaNotes.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Windows.ApplicationModel.Core;
using Windows.Devices.Midi;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using PiaNotes.Models;
using System.Numerics;
using Windows.Foundation;

namespace PiaNotes.Views
{
    /// <summary>
    /// Practice Page. Contains Sheet Music and Keyboard.
    /// </summary>
    public sealed partial class PracticePage : Page
    {
        //DispatcherTimer is the regular timer. It fires its Tick event on the UI thread, you can do anything you want with the UI. System.Timers.Timer is an asynchronous timer, its Elapsed event runs on a thread pool thread. You have to be very careful in your event handler, you are not allowed to touch any UI component or data-bound variables. And you'll need to use the lock statement where ever you access class members that are also used on the UI thread.
        private DispatcherTimer timerGameUI;
        private static Timer timerGameLogic;
        private SheetMusic SM;

        public bool KeyboardIsOpen { get; set; } = true;
        public bool LoadPage { get; set; } = false;

        //List for White keys and Black keys of the keyboard
        private List<Rectangle> keysWhite = new List<Rectangle>();
        private List<Rectangle> keysBlack = new List<Rectangle>();

        // Length of 127 because of 127 notes
        private Rectangle[] Notes = new Rectangle[127];
        private int Keys = SettingsPages.MIDI_SettingsPage.OctaveAmount * 12;
        //private double oldWidth;
        private enum PianoKey { C = 0, D = 2, E = 4, F = 5, G = 7, A = 9, B = 11 };
        private enum PianoKeySharp { CSharp = 1, DSharp = 3, FSharp = 6, GSharp = 8, ASharp = 10 };

        // GameCanvas
        private int windowWidth;
        private int windowHeight;

        private int staffMargin;
        private int staffStart;
        private int staffEnd;
        private int staffWidth;
        private int staffSpacing;
        private int tickCount;
        private float tickDistance;

        private double gameCanvasWidth;
        private double gameCanvasHeight;

        // Needs to become a settings
        private int FPS = 60;
        private int UPS = 100;

        // UI Assets
        List<Models.Line> lines = new List<Models.Line>();
        List<Note> notes = new List<Note>();
        Clef[] clefs = new Clef[2];

        public PracticePage()
        {
            this.InitializeComponent();

            // Initialize the page.
            var appView = ApplicationView.GetForCurrentView();
            appView.Title = "Practice";

            //Titlebar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            // Sets the decimal seperator to a dot.
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            // Initialize variables
            windowWidth = Convert.ToInt32(Window.Current.Bounds.Width);
            windowHeight = Convert.ToInt32(Window.Current.Bounds.Height);

            // Subscribes a handler for the MessageReceived event.
            Settings.midiInPort.MessageReceived += MidiInPort_MessageReceived;

            //Generate the amount of Keys
            Keys = (SettingsPages.MIDI_SettingsPage.OctaveAmount != 0) ? Settings.octaveAmount * 12 : Settings.octaveAmount * 12;

            //Create the keyboard to show on the screen and set a timer
            CreateKeyboard();
            GameTimerUI();
        }

        private void MidiInPort_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            // Converts received message into IMidiMessage.
            IMidiMessage receivedMidiMessage = args.Message;

            // Checks if a key has been pressed.
            if (receivedMidiMessage.Type == MidiMessageType.NoteOn)
            {
                // Retrieves channel, note from the MidiMessage, and sets the velocity.
                byte channel = ((MidiNoteOnMessage)receivedMidiMessage).Channel;
                byte note = ((MidiNoteOnMessage)receivedMidiMessage).Note;
                byte velocity = ((MidiNoteOnMessage) receivedMidiMessage).Velocity;

                if (Settings.disableUserFeedback)
                {
                    if (velocity + Utilities.DoubleToByte(Settings.volume) <= 127 && velocity + Utilities.DoubleToByte(Settings.volume) >= 0)
                        velocity += Utilities.DoubleToByte(Settings.volume);
                    else
                        velocity = 127;
                } else
                    velocity = Utilities.DoubleToByte(Settings.velocity);


                // Creates the message that will be send to play.
                IMidiMessage midiMessageToSend = new MidiNoteOnMessage(channel, note, velocity);
                Settings.midiOutPort.SendMessage(midiMessageToSend);
                FillKey(note);

            }

            // Checks if note has been released if so unfill the key.
            if (receivedMidiMessage.Type == MidiMessageType.NoteOff)
                UnFillKey(((MidiNoteOffMessage)receivedMidiMessage).Note);
        }

        // Method for coloring the played key
        private async void FillKey(byte note)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SolidColorBrush primaryColor = new SolidColorBrush(Color.FromArgb(255, Settings.redPrimary, Settings.greenPrimary, Settings.bluePrimary));
                SolidColorBrush secondaryColor = new SolidColorBrush(Color.FromArgb(255, Settings.redSecondary, Settings.greenSecondary, Settings.blueSecondary));

                try
                {
                    // If it is a black key, the color will be slightly darker (-25 on all values except A) than a white key.
                    Notes[note].Fill = Notes[note].Name.Contains("Sharp") ? primaryColor : secondaryColor;
                }
                catch (Exception e)
                {
                    // If it doesn't work, display the error message in the debug console
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            });
        }

        // Method for changing the color of the played key back to its default color.
        private async void UnFillKey(byte note)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    // The keys original colour will be restored
                    Notes[note].Fill = Notes[note].Name.Contains("Sharp") ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
                }
                catch (Exception e)
                {
                    // If it doesn't work, display the error message in the debug console
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            });
        }

        // Menustrip: View > Keyboard
        private void ViewKeyboard_Click(object sender, RoutedEventArgs e)
        {
            ToggleKeyboard();
        }

        // Toggles the keyboard to show/hide.
        public void ToggleKeyboard()
        {
            // Iterates through all keyboard items to hide/show them.
            if (KeyboardIsOpen)
            {
                foreach (Rectangle key in KeysWhiteSP.Children)
                {
                    key.Width = 0;
                }
                foreach (Rectangle key in KeysBlackSP.Children)
                {
                    key.Width = 0;
                }
                KeyboardBG.MinHeight = 0;
            }
            else
            {
                CreateKeyboard();
                KeyboardBG.MinHeight = 200;
            }
            KeyboardIsOpen = !KeyboardIsOpen;
        }


        // Creates the keyboard.
        public void CreateKeyboard()
        {
            int oct = 0;
            //First go through each Octave to make keys
            for (int i = Settings.startingOctave; i < (Settings.octaveAmount + Settings.startingOctave); i++)
            {
                //In each Octave make 12 keys, 7 white and 5 black keys
                for (int j = 0; j < 12; j++)
                {
                    //See if the key is a white or black key
                    if(j == 1 || j == 3 || j == 6 || j == 8 || j == 10) AddKey(false, j, i);
                    else AddKey(true, j, i);
                }
                oct = i + 1;
            }
            //Add an extra C note at the end
            AddKey(true, 0, oct);
            UpdateKeyboard();
        }

        public void AddKey(bool white, int j, int i)
        {
            if(white)
            {
                Rectangle keyWhiteRect = new Rectangle
                {
                    Name = $"{((PianoKey)j).ToString()}{i}",
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.White),
                    StrokeThickness = 4,
                    Height = 200
                };
                KeysWhiteSP.Children.Add(keyWhiteRect);
                if (j == 0)
                {
                    if (i == 0) Notes[j] = (keyWhiteRect);
                    else Notes[(i * 12)] = (keyWhiteRect);
                }
                else
                {
                    if (i == 0) Notes[j] = (keyWhiteRect);
                    else Notes[(j + (i * 12))] = (keyWhiteRect);
                }
                System.Diagnostics.Debug.WriteLine(keyWhiteRect.Name);
            }
            else
            {
                Rectangle keyBlackRect = new Rectangle
                {
                    Name = $"{((PianoKeySharp)j).ToString()}{i}",
                    Fill = new SolidColorBrush(Colors.Black),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 4,
                    Height = 150
                };
                KeysBlackSP.Children.Add(keyBlackRect);
                if (i == 0) Notes[j] = (keyBlackRect);
                else Notes[(j + (i * 12))] = (keyBlackRect);
                System.Diagnostics.Debug.WriteLine(keyBlackRect.Name);
            }
        }

        // Updates the keyboard. Is used after first initializing the keyboard or after resizing the window width.
        public void UpdateKeyboard()
        {
            double keyWidthWhite = 40;

            // Counts amount of white keys.
            int keyWhiteAmount = 8;
            if (Settings.octaveAmount != 0) keyWhiteAmount = (Keys - (Settings.octaveAmount * 5) + 1);

            // Sets width for white keys.
            foreach (Rectangle key in KeysWhiteSP.Children)
            {
                try
                {
                    // Calculates width for the white keys.
                    keyWidthWhite = (windowWidth / keyWhiteAmount);
                    key.Width = keyWidthWhite;
                }
                catch (Exception)
                {
                    // If width can't be calculated, change width to a set value.
                    key.Width = keyWidthWhite;
                }
            }

            // Sets width and location for black keys.
            bool initialCsharp = true;
            foreach (Rectangle key in KeysBlackSP.Children)
            {
                //double keyWhiteWidth;
                try
                {
                    // Calculates width for the black keys.
                    key.Width = keyWidthWhite / 100 * 60;

                    if (key.Name.Contains("CSharp"))
                    {
                        // Calculates location for C# key.
                        // The first key has a different calculation than the rest, because there are no prior keys.
                        double location;
                        if (initialCsharp)
                        {
                            location = keyWidthWhite - (key.Width / 2);
                            initialCsharp = false;
                        }
                        else location = (keyWidthWhite - (key.Width / 2)) * 2;
                        key.Margin = new Thickness(location, 0, 0, 50);
                    }
                    else if (key.Name.Contains("DSharp") || key.Name.Contains("GSharp") || key.Name.Contains("ASharp"))
                    {
                        // Calculates location for D#/G#/A# keys.
                        double location = keyWidthWhite - key.Width;
                        key.Margin = new Thickness(location, 0, 0, 50);
                    }
                    else if (key.Name.Contains("FSharp"))
                    {
                        // Calculates location for F# key.
                        double location = keyWidthWhite * 2 - key.Width;
                        key.Margin = new Thickness(location, 0, 0, 50);
                    }
                } catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Logic Thread where game logic gets updated
        /// </summary>

        private void GameTimerLogic()
        {
            // Create a timer with a sixty-fourth tick which represents the 1/64 note which is the smallest note we support.
            timerGameLogic = new Timer(15.625)
            {
                AutoReset = true,
                Enabled = true
            };

            // Hook up the Elapsed event for the timer. 
            timerGameLogic.Elapsed += GameTickLogic;
        }

        private void GameTickLogic(Object source, ElapsedEventArgs e)
        {
            tickCount += 15625;

            for (int i = 0; i < SM.notes.Count && i < 6; i++)
            {
                if (tickCount >= SM.notes[i].MetricTiming.TotalMicroseconds)
                {
                    notes.Add(SM.notes[i]);
                    SM.notes.Remove(SM.notes[i]);
                }
            }

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].BitmapLocation.X <= staffStart)
                {
                    notes.Remove(notes[i]);
                    return;
                }

                notes[i].BitmapLocation = new Vector2(notes[i].BitmapLocation.X - tickDistance, notes[i].BitmapLocation.Y);
            }
        }

        /// <summary>
        /// GameCanvas is a Win2D canvas which makes 2D graphics rendering with GPU acceleration possible.
        /// This includes all kind of cool stuf as particles, effects etc...
        /// </summary> 

        private void GameTimerUI()
        {
            timerGameUI = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / FPS)
            };
            timerGameUI.Tick += GameTickUI;
            timerGameUI.Start();
        }

        private void GameTickUI(object sender, object e)
        {
            // Redraw screen.
            GameCanvas.Invalidate();
        }

        // Initialize images and stuff.
        private void GameCanvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            gameCanvasHeight = sender.ActualHeight;
            gameCanvasWidth = sender.ActualWidth;

            // Canvas values
            staffMargin = 24;
            staffStart = staffMargin;
            staffEnd = (int)gameCanvasWidth - staffMargin;
            staffWidth = (int)gameCanvasWidth - staffMargin * 2;
            staffSpacing = 8;

            // Tickdistance is now 12/24 seconds for the entire line: distance / (time in milliseconds / ticks per millisecons)
            //tickDistance = (windowWidth-staffStart*2) / 1536;
            tickDistance = (windowWidth - staffStart * 2) / 768;


            // Create staff
            for (int i = 0; i < 13; i++)
            {
                int y = staffSpacing * (i + 1) + staffMargin * 2;

                if (i % 2 == 0 && i != 0 && i != 12)
                    lines.Add(new Models.Line(staffStart, y, staffEnd, y));
            }

            for (int i = 0; i < 13; i++)
            {
                int y = staffSpacing * (i + 1) + 12 * staffSpacing + (staffMargin * 2);

                if (i % 2 == 0 && i != 0 && i != 12)
                    lines.Add(new Models.Line(staffStart, y, staffEnd, y));
            }

            // Create Guidelines
            lines.Add(new Models.Line(staffWidth / 4,
                    staffSpacing * 2 + staffMargin * 2,
                    staffWidth / 4,
                    staffSpacing * 12 + staffMargin * 2));

            lines.Add(new Models.Line(staffWidth / 4,
                    staffSpacing * 14 + staffMargin * 2,
                    staffWidth / 4,
                    staffSpacing * 25 + staffMargin * 2));

            clefs[0] = new Clef(new Vector2(10,10));
            clefs[1] = new Clef(new Vector2(10,10));

            //Set images inside of the ContentPipeline for futher re-use.
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());

            // Start GameTimer
            GameTimerLogic();
        }

        private async Task CreateResourcesAsync(CanvasControl sender)
        {
            await clefs[0].SetBitmap(sender, @"Assets/Clefs/CClef.png");
            await clefs[1].SetBitmap(sender, @"Assets/Clefs/FClef.png");

            // Add the resources to the ContentPipeline for reuse purposes
            ContentPipeline.ParentCanvas = sender;
            
            await ContentPipeline.AddImage("1", @"Assets/Notes/WholeNote.png");
            await ContentPipeline.AddImage("0.5", @"Assets/Notes/HalfNote.png");
            await ContentPipeline.AddImage("0.25", @"Assets/Notes/QuarterNote.png");
            await ContentPipeline.AddImage("0.125", @"Assets/Notes/EighthNote.png");
            await ContentPipeline.AddImage("0.0625", @"Assets/Notes/SixteenthNote.png");
            await ContentPipeline.AddImage("0.03125", @"Assets/Notes/ThirtySecondNote.png");
            
            MidiConverter midiConverter = new MidiConverter();
            List<int> flatKeysAll = midiConverter.GetFlatKeys();
            LoadPage = true;

            // Give the notes a bitmap
            for (int i = 0; i < SM.notes.Count; i++)
            {
                SM.notes[i].SetBitmap(SM.notes[i].NoteType.ToString());

                int key = 0;

                // Check if key is flat.
                key = (flatKeysAll.Contains(SM.notes[i].Number)) ? key = SM.notes[i].Number : key = SM.notes[i].Number - 1;

                // Set height of each note.
                int index = flatKeysAll.IndexOf(key);
                int negativeNote = (midiConverter.GetOctaveInfo(SM).Item1 * 7 * -1) + index;
                int notePos = Math.Abs(negativeNote * staffSpacing) - 4;
                SM.notes[i].BitmapLocation = new Vector2(staffEnd, notePos);
            }
        }
        
        private void GameCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (LoadPage)
            {
                // Draw staff and guidelines.
                for (int i = 0; i < lines.Count; i++)
                {
                    args.DrawingSession.DrawLine(lines[i].StartPoint, lines[i].EndPoint, Colors.White);
                }
                /*
                // Draw clef.
                for (int i = 0; i < 2; i++)
                {
                    args.DrawingSession.DrawImage(clefs[i].Bitmap, clefs[i].BitmapLocation, new Rect(new Point(0, 0), clefs[i].BitmapSize));
                }
                */
                // Draw notes.
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i] != null)
                        args.DrawingSession.DrawImage(notes[i].Bitmap, notes[i].BitmapLocation);
                        // size 
                        //args.DrawingSession.DrawImage(notes[i].Bitmap, notes[i].BitmapLocation, new Rect(new Point(0, 0), notes[i].BitmapSize));
                }
            }
        }

        /// <summary>
        /// On click events navigation
        /// </summary>

        // Navigate to the settings page
        private void NavSettings_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SettingsPage));

        // Navigate to the credits page
        private void NavCredits_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(CreditsPage));

        // Navigate to the selection page
        private void NavSelection_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(typeof(SelectionPage));

        /// <summary>
        /// Page events
        /// </summary>

        // Handler for when navigated to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SM = (SheetMusic)e.Parameter;
        }

        // Handler for when the page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stop the GameLoop and UIloop
            timerGameLogic.Stop();
            timerGameUI.Stop();

            // Unsubscribe the MidiInPort_MessageReceived
            Settings.midiInPort.MessageReceived -= MidiInPort_MessageReceived;

            // Dispose of the Win2D resources
            this.GameCanvas.RemoveFromVisualTree();
            this.GameCanvas = null;

            // Clear the ContentPipeline
            ContentPipeline.ImageDictionary.Clear();
        }

        // Handler for when the page is resized
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If the keyboard is shown, it will be updated. 
            if (KeyboardIsOpen) UpdateKeyboard();

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
        }
    }
}
