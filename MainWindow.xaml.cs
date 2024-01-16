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
        public int highestPlaylistID = 0;
        public int activePlaylistID = 0;
        public int activePlaylistIndex = 0;
        public Playlist activePlaylist = new Playlist();

        public List<Playlist> allPlaylists = new List<Playlist>();
        public List<Song> allSongs = new List<Song>();

        public string SONGSPATH = "/VS/Song Player/Files/Songs/";
        public string PLPATH = "/VS/Song Player/Files/Playlists/";

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
        private void LoadSongs()
        {
            

            foreach (string filename in Directory.GetFiles("/VS/Song Player/Files/Songs/", "*.mp3"))
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
                    bool done = false;
                    int j = 0;
                    string requiredSong = SONGSPATH + contents[i];
                    while (!done && !fail)
                    {
                        if (j == allSongs.Count() && !done)
                        {
                            fail = true;
                            tempPlaylist.playlist.Add(new Song("0", " ", "Missing File?", "404 - Failed to load"));
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

                allPlaylists.Add(tempPlaylist);
            }
        }
        private void UpdatePlaylistDisplay()
        {
            activePlaylist = allPlaylists[activePlaylistIndex];
            SongList_PlaylistPanel.Text = "";

            for (int i = 0; i < activePlaylist.playlist.Count; i++)
            {
                SongList_PlaylistPanel.Text = SongList_PlaylistPanel.Text + activePlaylist.playlist[i].title + " || " + activePlaylist.playlist[i].artist + " | " + (Math.Round(allSongs[i].length / 60, 2)).ToString() + ":00\n";
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

        //Main <3
        public MainWindow()
        {
            Song owo = new Song();
            allSongs.Add(owo);

            InitializeComponent();
            LoadSongs();
            LoadPlaylists();
            UpdatePlaylistList();
            UpdatePlaylistDisplay();
        }

        //UI Elements
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
    }
}
//BONK <3