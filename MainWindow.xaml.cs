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
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

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

        #region Utilities
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
        private void FixFilePath(ref string path)
        {
            List<char> newPath = new List<char>();
            foreach (char i in path)
            {
                if (i == '\\') { newPath.Add('/'); }
                else { newPath.Add(i); }
            }
            path = "";
            for(int i = 0; i < newPath.Count; i++)
            {
                path += newPath[i];
            }
            
        }
        #endregion

        #region Song Stuffs
        private void LoadSongs(string path, bool clear = true)
        {
            if (clear) { allSongs.Clear(); }
            FixFilePath(ref path);
            try {
                foreach (string filename in Directory.GetFiles(path, "*.mp3"))
                {
                    var mp3 = TagLib.File.Create(filename);
                    string tempTitle = (mp3.Tag.Title != null) ? mp3.Tag.Title : removeStrFromStart(filename, path);
                    string tempAuthor = mp3.Tag.FirstPerformer;
                    string tempLength = mp3.Tag.Length;

                    tempTitle = (tempTitle[0] == '\\') ? removeStrFromStart(tempTitle, "\\"): tempTitle;

                    allSongs.Add(new Song(tempLength, filename, tempAuthor, tempTitle));
                }
            }catch { }
        }
        #endregion

        #region Playlist Stuffs
        private void LoadPlaylists()
        {
            allPlaylists.Clear();

            foreach (string file in Directory.GetFiles(PLPATH))
            {
                string[] content = System.IO.File.ReadAllText(file).Split("~");
                Playlist tempPlaylist = new Playlist(file, content[0], content[1]); //Make a temporary playlist to build with.

                for (int i = 2; i < content.Length; i++)
                {
                    int j = 0;
                    bool done = false;
                    while (j < allSongs.Count && !done)
                    {
                        if (content[i] == allSongs[j].title)
                        {
                            done = true;
                            tempPlaylist.addSong(allSongs[j]);
                        }//Song found in allSongs
                        j++;
                    }
                    if (!done && content[i] != "#") { tempPlaylist.addSong(new Song("0", "", " - File Missing ?", "404")); }
                }//One loop per song, takes full filepath and tries to locate in allSongs. If not there, placeholder song added.

                allPlaylists.Add(tempPlaylist);
                
            }//One loop for each playlist file.
        }
        private void UpdatePlaylistDisplay()
        {
            LoadPlaylists();
            activePlaylist = allPlaylists[activePlaylistIndex];
            SongList_PlaylistPanel.Text = "";

            for (int i = 0; i < activePlaylist.playlist.Count; i++)
            {
                SongList_PlaylistPanel.Text = SongList_PlaylistPanel.Text + activePlaylist.playlist[i].title + " || " + activePlaylist.playlist[i].artist + " | " + (activePlaylist.playlist[i].length).ToString() + "\n";
            }
            PlaylistName_Panel.Text = activePlaylist.name;
            PlaylistAuthor_Panel.Text = activePlaylist.author;
        }
        private void UpdatePlaylistList()
        {
            LoadPlaylists();
            PlaylistList_Panel.Text = "\n";
            for (int i = 0; i < allPlaylists.Count; i++)
            {
                PlaylistList_Panel.Text += allPlaylists[i].name + "\n\n";
            }
        }
        #endregion

        #region Player controls
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
            try
            {
                Song test = player.activeSong;
                player.TrackSwitch(forward);
                LoadActiveSongDetails();
            }
            catch { }//Doesn't run if no playlist is currently loaded
        }
        #endregion

        #region Song information
        private void LoadActiveSongDetails()
        {
            SongName_Small.Text = player.activeSong.title;
            SongArtist_Small.Text = player.activeSong.artist;

            SongInfo_Name.Text = player.activeSong.title;
            SongInfo_Author.Text = player.activeSong.artist;
        }
        private void EditSongTags(string value, string tagField)
        {

        }
        private void ShowEditFieldInputs(string tagField)
        {
            //is a toggle
            switch (tagField)
            {
                case "Title": 
                    if (SongInfo_Edit_Title_Input.Visibility == Visibility.Visible)
                    {
                        SongInfo_Edit_Title_Input.Visibility = Visibility.Hidden;
                        SongInfo_Edit_Title_Confirmation.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        SongInfo_Edit_Title_Input.Visibility = Visibility.Visible;
                        SongInfo_Edit_Title_Confirmation.Visibility = Visibility.Visible;
                    }
                    break;
                case "Artist":
                    if (SongInfo_Edit_Artist_Input.Visibility == Visibility.Visible)
                    {
                        SongInfo_Edit_Artist_Input.Visibility = Visibility.Hidden;
                        SongInfo_Edit_Artist_Confirmation.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        SongInfo_Edit_Artist_Input.Visibility = Visibility.Visible;
                        SongInfo_Edit_Artist_Confirmation.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }
        #endregion

        //Main
        public MainWindow()
        {
            InitializeComponent();
            RefreshPress();
            LoadPlaylists();
            UpdatePlaylistList();
            UpdatePlaylistDisplay();
        }

        #region UI Elements/Buttons
        private void RefreshPress(bool onlySongs = false)
        {
            LoadSongs(SONGSPATH);
            LoadSongs(SongFolder1.Text, false);
            LoadSongs(SongFolder2.Text, false);
            LoadSongs(SongFolder3.Text, false);
            LoadSongs(SongFolder4.Text, false);
            if (!onlySongs)
            {
                AllSongs_Panel.Text = "";
                for (int i = 0; i < allSongs.Count; i++)
                {
                    AllSongs_Panel.Text = AllSongs_Panel.Text + allSongs[i].title + " || " + allSongs[i].artist + " | " + (allSongs[i].length).ToString() + "\n";
                }
                UpdatePlaylistDisplay();
            }
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
            RefreshPress();
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
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            switch (player.isPlaying)
            {
                case false:
                    try { Play(true); }
                    catch { try { SelectPlaylist(); } catch { SelectPlaylist(true); } }//If no song selected, load active playlist and try again, if no active playlist, load all songs and try again.
                    break;
                case true:
                    try { Play(false); } catch { };
                    break;
            }
            
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPress();
        }
        private void SongInfo_Edit_Title_Click(object sender, RoutedEventArgs e)
        {
            ShowEditFieldInputs("Title");
        }
        private void SongInfo_Edit_Artist_Click(object sender, RoutedEventArgs e)
        {
            ShowEditFieldInputs("Artist");
        }
        #endregion
    }
}
//BONK <3