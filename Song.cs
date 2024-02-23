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
        public string length;      //In minutes

        public Song(string Length = "0", string FileName = "Track", string? Artist = null, string? Title = null) 
        {
            this.title = (Title == null) ? "Unknown Track" : Title;
            this.artist = (Artist == null) ? "Unknown Artist" : Artist;
            this.filename = FileName;
            try
            {
                int lseconds = int.Parse(Length) / 1000;
                int lminutes = lseconds / 60;
                lseconds = lseconds % 60;
                this.length = lminutes.ToString() + ":" + lseconds.ToString();
            }catch(Exception) 
            {
                this.length = "0:00";
            }
        }

        public string ReturnFileName()
        {
            return filename;
        }
    }
}
