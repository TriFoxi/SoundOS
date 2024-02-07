using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib;

namespace Song_Player
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int activePlaylistIndex = 0;

        private Playlist activePlaylist = new Playlist();
        private Player player = new Player();

        private List<Playlist> allPlaylists = new List<Playlist>();
        private List<Song> allSongs = new List<Song>();

        private string SONGSPATH = "D:/VS/Song Player/Files/Songs/";
        private string PLPATH = "D:/VS/Song Player/Files/Playlists/";

        //Utilities
        private string removeStrFromStart(string basic, string toRemove)
        {
            List<char> temp = new List<char>();
            int j = 0;
            for (int i = 0; i < basic.Length; i++)
            {
                if (!(i < toRemove.Length))
                {
                    temp.Add(basic[i]);
                    j++;
                }
            }
            return new string(temp.ToArray());
        }

        //Song Stuffs
        private void LoadSongs(string path)
        {
            foreach (string filename in Directory.GetFiles(path, "*.mp3"))
            {
                var mp3 = TagLib.File.Create(filename);
                string tempTitle = (mp3.Tag.Title != null) ? mp3.Tag.Title : removeStrFromStart(filename, SONGSPATH);
                string tempAuthor = mp3.Tag.FirstAlbumArtist;
                string tempLength = mp3.Tag.Length;

                allSongs.Add(new Song(tempLength, filename, tempAuthor, tempTitle));
            }
        }

        //Playlist Stuffs
        private void LoadPlaylists()
        {
            List<string> loadedPlaylists = new List<string>();
            foreach (string filename in Directory.GetFiles(PLPATH))
            {
                string[] contents = System.IO.File.ReadAllText(filename).Split("~");
                string tempName = contents[0];
                string tempAuthor = contents[1];

                Playlist tempPlaylist = new Playlist(filename, tempName, tempAuthor);

                for (int i = 2; i < contents.Count(); i++)
                {
                    bool fail = false;
                    int fails = 0;
                    bool done = false;
                    int j = 0;
                    string requiredSong = "";
                    while (fails < 5 && !done)
                    {
                        fail = false;
                        switch (fails)
                        {
                            case 0: requiredSong = SONGSPATH + contents[i]; break;
                            case 1: requiredSong = SongFolder1 + contents[i]; break;
                            case 2: requiredSong = SongFolder2 + contents[i]; break;
                            case 3: requiredSong = SongFolder3 + contents[i]; break;
                            case 4: requiredSong = SongFolder4 + contents[i]; break;
                        }
                        while (!done && !fail)
                        {
                            if (j == allSongs.Count() && !done)
                            {
                                fail = true;
                                fails++;
                            }
                            else
                            {
                                string currentSong = allSongs[j].ReturnFileName();
                                if (currentSong == requiredSong)
                                {
                                    tempPlaylist.addSong(allSongs[j]);
                                    done = true;
                                }
                                j++;
                            }
                        }
                    }
                    if (fails >= 4) { tempPlaylist.playlist.Add(new Song("0", " ", "Missing File?", "404 - Failed to load")); }
                }//DefaultSongFolder

                allPlaylists.Add(tempPlaylist);
            }
        }
        private void UpdatePlaylistDisplay()
        {
            activePlaylist = allPlaylists[activePlaylistIndex];
            SongList_PlaylistPanel.Text = "";

            for (int i = 0; i < activePlaylist.playlist.Count; i++)
            {
                SongList_PlaylistPanel.Text = SongList_PlaylistPanel.Text + activePlaylist.playlist[i].title + " || " + activePlaylist.playlist[i].artist + " | " + (Math.Round(activePlaylist.playlist[i].length / 60, 2)).ToString() + ":00\n";
            }
            PlaylistName_Panel.Text = activePlaylist.name;
            PlaylistAuthor_Panel.Text = activePlaylist.author;
        }
        private void UpdatePlaylistList()
        {
            PlaylistList_Panel.Text = "\n";
            for (int i = 0; i < allPlaylists.Count; i++)
            {
                PlaylistList_Panel.Text += allPlaylists[i].name + "\n\n";
            }
        }

        //Player controls
        private void SelectPlaylist(bool all = false)
        {
            switch (all)
            {
                case false:
                    player.LoadPlaylist(activePlaylist);
                    LoadActiveSongDetails();
                    break;
                case true:
                    Playlist AllSongs = new Playlist();
                    AllSongs.playlist = allSongs;
                    player.LoadPlaylist(AllSongs, true);
                    LoadActiveSongDetails();
                    break;
            }
        }
        private void Play(bool play)
        {
            player.PlayAudio(play);
        }
        private void SwitchTrack(bool forward)
        {
            player.TrackSwitch(forward);
            LoadActiveSongDetails();
        }
        private void LoadActiveSongDetails()
        {
            SongName_Small.Text = player.activeSong.title;
            SongArtist_Small.Text = player.activeSong.artist;
        }

        //Main
        public MainWindow()
        {
            Song owo = new Song();
            allSongs.Add(owo);

            InitializeComponent();
            LoadSongs(SONGSPATH);
            try { LoadSongs(SongFolder1.Text); } catch { }
            try { LoadSongs(SongFolder2.Text); } catch { }
            try { LoadSongs(SongFolder3.Text); } catch { }
            try { LoadSongs(SongFolder4.Text); } catch { }
            LoadPlaylists();
            UpdatePlaylistList();
            UpdatePlaylistDisplay();
        }

        //UI Elements/Buttons
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            AllSongs_Panel.Text = "";
            for (int i = 0; i < allSongs.Count; i++)
            {
                AllSongs_Panel.Text = AllSongs_Panel.Text + allSongs[i].title + " || " + allSongs[i].artist + " | " + (Math.Round(allSongs[i].length / 60,2)).ToString() + ":00\n";
            }
            UpdatePlaylistDisplay();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                activePlaylistIndex += 1;
                activePlaylist = allPlaylists[activePlaylistIndex];
            }
            catch
            {
                activePlaylistIndex = 0;
                activePlaylist = allPlaylists[activePlaylistIndex];
            }
            UpdatePlaylistDisplay();
        }//Next Playlist Button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                activePlaylistIndex -= 1;
                activePlaylist = allPlaylists[activePlaylistIndex];
            }
            catch
            {
                activePlaylistIndex = 0;
                activePlaylist = allPlaylists[activePlaylistIndex];
            }
            UpdatePlaylistDisplay();
        }//Previous Playlist Button
        private void PlayPlaylist_Click(object sender, RoutedEventArgs e)
        {
            SelectPlaylist();
        }
        private void SkipTrack_Button_Click(object sender, RoutedEventArgs e)
        {
            SwitchTrack(true);
        }
        private void PreviousTrack_Button_Click(object sender, RoutedEventArgs e)
        {
            SwitchTrack(false);
        }
        private void PlayButton_AllSongs_Click(object sender, RoutedEventArgs e)
        {
            SelectPlaylist(true);
        }
    }
}
//BONK <3