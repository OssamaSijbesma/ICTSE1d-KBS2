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

        public static ContentDialog NoMidiInOutDialog = new ContentDialog
        {
            Title = "There is no MIDI in- or output!",
            Content = "Check your MIDI input and output device before practicing.",
            CloseButtonText = "Ok"
        };
    }
}
