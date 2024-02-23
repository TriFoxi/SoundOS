using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Song_Player
{
    public class Playlist
    {
        public string name;
        public string author;
        public string filename;
        public List<Song> playlist = new List<Song>();
        public float playtime = 0;
        private Song placeholderSong = new Song();

        public Playlist(string Filename = "0", string Name = "A Playlist", string Author = "Unknown Author") 
        {
            this.name = Name;
            this.author = Author;
            this.filename = Filename;
        }

        public void addSong(Song song)
        {
            this.playlist.Add(song);
            this.playtime += int.Parse(song.length.Split(':')[0]);
        }
    }
}
