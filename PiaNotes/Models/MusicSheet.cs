using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiaNotes.Models
{
    public class MusicSheet
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }

        public MusicSheet(int id, string title, string path)
        {
            Id = id;
            Title = title;
            Path = path;
        }
    }
}
