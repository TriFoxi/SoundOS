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
        public bool isPlaying { get; protected set; }

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
            player.Pause();
            switch (play)
            {
                case true: player.Play(); isPlaying = true; break;
                case false: player.Pause(); isPlaying = false; break;
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
            try {
                activeSong = activePlaylist.playlist[activeSongIndex];
                Uri uri = new Uri(activeSong.ReturnFileName());
                player.Open(uri);
                PlayAudio(true);
            }//If this fails, it means it has reached the end of a playlist, so the catch loops it back round to the start, or end, of the PL.
            catch
            {
                switch (forward)
                {
                    case true: activeSongIndex = 0; break;
                    case false: activeSongIndex = activePlaylist.playlist.ToArray().Length - 1; break;
                }
                try
                {
                    activeSong = activePlaylist.playlist[activeSongIndex];
                }//If this fails it means there is nothing selected to play at the moment. So it does nothing.
                catch { }
                try
                {
                    Uri uri = new Uri(activeSong.ReturnFileName());
                    player.Open(uri);
                    PlayAudio(true);
                }//If this fails at any point it is trying to play a song where the audio file is missing in a playlist. The catch pauses any audio.
                catch { PlayAudio(false); }
            }
        }
    }
}
