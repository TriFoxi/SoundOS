using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Song_Player
{
    public class Song
    {
        private string filename;    //file location override possible by user
        public string title;     //Song name, from filename or user override
        public string artist;   //Artist, input by user.
        public float length;      //In seconds

        public Song(string Length = "0", string FileName = "Track", string? Artist = null, string? Title = null) 
        {
            this.title = (Title == null) ? "Unknown Track" : Title;
            this.artist = (Artist == null) ? "Unknown Artist" : Artist;
            this.filename = FileName;
            try
            {
                this.length = int.Parse(Length);
            }catch(Exception) 
            {
                this.length = 0;
            }
        }

        public string ReturnFileName()
        {
            return filename;
        }
    }
}
