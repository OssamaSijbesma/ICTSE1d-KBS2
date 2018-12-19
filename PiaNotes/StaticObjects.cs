using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace PiaNotes
{
    static class StaticObjects
    {

        /// <summary>
        /// Dialogs
        /// </summary>

        //Popup dialog for if you have no midi devices but still want to run the practisepage
        public static ContentDialog NoMidiInOutDialog = new ContentDialog
        {
            Title = "There is no MIDI in- or output!",
            Content = "Check your MIDI input and output device before practicing.",
            CloseButtonText = "Ok"
        };

        //Popup Dialog in case of no connection with the Database
        public static ContentDialog NoDatabaseConnectionDialog = new ContentDialog
        {
            Title = "There is no Internet connection!",
            Content = "Add a local MIDI file before practicing.",
            CloseButtonText = "Ok"
        };

        public static ContentDialog AlreadyPreviewed = new ContentDialog
        {
            Title = "There is another MIDI file being previewed!",
            Content = "Stop the previewing MIDI to play another.",
            CloseButtonText = "Ok"
        };
    }
}
