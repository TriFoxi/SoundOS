using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Song_Player
{
    internal class Player
    {
        private MediaPlayer player = new MediaPlayer();
        public Song activeSong = new Song();
        private Playlist activePlaylist = new Playlist();
        private int activeSongIndex = 0;

        public void LoadAudio(string path = "", int index = -1)
        {
            if (path != "") { player.Open(new Uri("@" + path)); }
            else if (index != -1) 
            {
                activeSongIndex = index;
                activeSong = activePlaylist.playlist[activeSongIndex];
                player.Open(new Uri(activeSong.ReturnFileName())); 
            }
        }
        public void LoadPlaylist(Playlist pl, bool all = false)
        {
            activePlaylist = pl;
            activeSongIndex = (all) ? 0 : -1;
            TrackSwitch(true);
            PlayAudio(true);
        }

        public void PlayAudio(bool play)
        {
            switch (play)
            {
                case true: player.Play(); break;
                case false: player.Pause(); break;
            }
            activeSong = activePlaylist.playlist[activeSongIndex];
        }
        public void TrackSwitch(bool forward)
        {
            switch (forward)
            {
                case true: activeSongIndex++; break;
                case false: activeSongIndex--; break;
            }

            activeSong = activePlaylist.playlist[activeSongIndex];
            Uri uri = new Uri(activeSong.ReturnFileName());
            player.Open(uri);
            PlayAudio(true);
        }
    }
}
