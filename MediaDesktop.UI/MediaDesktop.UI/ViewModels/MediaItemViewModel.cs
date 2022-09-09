using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
using LibVLCSharp.Shared;
using System.Drawing;
using MediaDesktop.UI.Services;
using Microsoft.UI.Xaml.Controls;
using System.Threading;

namespace MediaDesktop.UI.ViewModels
{
    public class MediaItemViewModel : INotifyPropertyChanged
    {
        #region Raw Data Source
        private MediaItem mediaItem;
        #endregion

        #region Raw Data Encapsulation
        public string MediaPath
        {
            get { return mediaItem.MediaPath; }
            set
            {
                if (mediaItem.MediaPath != value)
                {
                    mediaItem.MediaPath = value;
                    OnPropertyChanged(nameof(MediaPath));
                }
            }
        }

        public long MediaSize
        {
            get { return new System.IO.FileInfo(MediaPath).Length; }
        }

        public bool IsMediaLoaded
        {
            get { return mediaItem.IsMediaLoaded; }
        }

        public Media Media
        {
            get { return mediaItem.Media; }
            private set
            {
                if (mediaItem.Media != value)
                {
                    mediaItem.Media = value;
                    RuntimeDataSet = new RuntimeData(mediaItem);
                    OnPropertyChanged(nameof(Media));
                }
            }
        }


        #endregion

        #region Runtime Data Encapsulation
        public RuntimeData RuntimeDataSet { get; private set; }
        #endregion

        #region Methods

        public void LoadMedia(LibVLC libVLC)
        {
            Media = new Media(libVLC, new Uri(MediaPath));
        }

        public void PlayMedia(MediaDesktopPlayer player)
        {

            var parentDesktopViewModel = GlobalResources.ViewModelCollection.ViewModelItems.First(i => i.MediaItemViewModel == this);
            if (!GlobalResources.ViewModelCollection.CurrentPlayingList.Contains(parentDesktopViewModel))
            {
                GlobalResources.ViewModelCollection.CurrentPlayingList.Add(parentDesktopViewModel);
            }
            player.MediaPlayer = RuntimeDataSet.MediaPlayer;
            player.Play();

        }

        public void PauseMedia(MediaDesktopPlayer player)
        {
            player.Pause();
        }

        public void TogglePlayStatus(MediaDesktopPlayer player)
        {
            if(RuntimeDataSet.MediaPlayer.IsPlaying)
            {
                PauseMedia(player);
            }
            else
            {
                PlayMedia(player);
            }
        }

        public void ToggleMediaStatusTo(MediaDesktopPlayer player, ToggleMediaStatusAction action)
        {
            switch (action)
            {
                case ToggleMediaStatusAction.Pause:
                    PauseMedia(player);
                    break;
                case ToggleMediaStatusAction.Play:
                    PlayMedia(player);
                    break;
                case ToggleMediaStatusAction.Stop:
                    StopMedia(player);
                    break;
            }
        }

        public void StopMedia(MediaDesktopPlayer player)
        {
            player.Stop();
        }

        public async void ShowMediaInfoDialog(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                PrimaryButtonText = "关闭",
                Content = new Views.Dialogs.MediaInfoDialogPage() { DataContext = this },
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            await contentDialog.ShowAsync();
        }

        public void PlayWithMediaList(MediaDesktopPlayer player,MediaPlayingListViewModel list)
        {
            var desktopItem = list.MediaItems.FirstOrDefault(i => i.MediaItemViewModel == this);
            if(desktopItem == default(MediaDesktopItemViewModel))
            {
                throw new Exception(nameof(PlayWithMediaList) + ": This method could only be invoked where current MediaItemViewModel is included in the given list.");
            }
            else
            {
                GlobalResources.ViewModelCollection.CurrentPlayingList.Clear();
                int index = list.MediaItems.IndexOf(desktopItem);
                
                //Refill CurrentPlayingList with the items in list, where ordering begins at desktopItem.
                foreach (var item in list.MediaItems.Where(i => list.MediaItems.IndexOf(i) >= index))
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }

                foreach (var item in list.MediaItems.Where(i => list.MediaItems.IndexOf(i) < index))
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }

                desktopItem.MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
            }
        }

        public void BrowseMediaFileInExplorer()
        {
            WinUI3Helper.SelectFileInExplorer(MediaPath);
        }

        #endregion

        #region DelegateCommands
        public DelegateCommand LoadMediaCommand { get; set; }
        public DelegateCommand PlayMediaCommand { get; set; }
        public DelegateCommand PauseMediaCommand { get; set; }
        public DelegateCommand StopMediaCommand { get; set; }
        public DelegateCommand PlayWithMediaListCommand { get; set; }
        public DelegateCommand TogglePlayStatusCommand { get; set; }
        public DelegateCommand ToggleMediaStatusToCommand { get; set; }
        public DelegateCommand ShowMediaInfoDialogCommand { get; set; }
        public DelegateCommand SelectFileInExplorerCommand { get; set; }

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor.
        /// </summary>
        public MediaItemViewModel()
        {
            mediaItem = new MediaItem();
            LoadMediaCommand = new DelegateCommand((obj) => { LoadMedia(obj as LibVLC); });
            PlayMediaCommand = new DelegateCommand((obj) => { PlayMedia(obj as MediaDesktopPlayer); });
            PauseMediaCommand = new DelegateCommand((obj) => { PauseMedia(obj as MediaDesktopPlayer); });
            StopMediaCommand = new DelegateCommand((obj) => { StopMedia(obj as MediaDesktopPlayer); });
            PlayWithMediaListCommand = new DelegateCommand((obj) => { PlayWithMediaList(GlobalResources.MediaDesktopPlayer, obj as MediaPlayingListViewModel); });
            TogglePlayStatusCommand = new DelegateCommand((obj) => { TogglePlayStatus(obj as MediaDesktopPlayer); });
            ToggleMediaStatusToCommand = new DelegateCommand((obj) => { var data = (ToggleMediaStatusData)obj; ToggleMediaStatusTo(data.Player, data.Action); });
            ShowMediaInfoDialogCommand = new DelegateCommand((obj) => { ShowMediaInfoDialog(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            SelectFileInExplorerCommand = new DelegateCommand((obj) => { BrowseMediaFileInExplorer(); });
        }
        #endregion

        #region Notify Event&Method

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// Represents and places the data produced on run time.
        /// </summary>
        public class RuntimeData : INotifyPropertyChanged
        {
            private MediaStrechMode strechMode;

            /// <summary>
            /// Used to get a media's Info.
            /// </summary>
            public MediaPlayer MediaPlayer { get; set; }
            private MediaItem MediaItem { get; set; }
            private Media Media { get { return MediaItem.Media; } }

            private void EventStartup()
            {
                MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                MediaPlayer.Media.StateChanged += Media_StateChanged;
            }

         

            private void EventLogOff()
            {
                MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
                MediaPlayer.Media.StateChanged -= Media_StateChanged;
            }
            public RuntimeData(MediaItem mediaItem)
            {
                MediaItem = mediaItem;
                MediaPlayer = new MediaPlayer(Media);
                Media.Parse(MediaParseOptions.ParseLocal);
                EventStartup();
            }
            
            ~RuntimeData()
            {
                EventLogOff();
            }

            #region Methods
            private void MediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
            {
                OnPropertyChanged(nameof(MediaCurrentTime));
                OnPropertyChanged(nameof(MediaPlayedProgress));
                OnPropertyChanged(nameof(MediaKbps));
            }
            private void Media_StateChanged(object sender, MediaStateChangedEventArgs e)
            {
                OnPropertyChanged(nameof(IsMediaPlaying));
            }

            private void ApplyStrechMode(MediaStrechMode mode)
            {
                Size scrSize = WinUI3Helper.GetScreenResolution();
                switch (mode)
                {
                    case MediaStrechMode.Uniform:
                        {
                            MediaPlayer.Scale = 1f;
                            MediaPlayer.AspectRatio = null;
                            break;
                        }
                    case MediaStrechMode.Strech:
                        {
                            MediaPlayer.Scale = 1f;
                            MediaPlayer.AspectRatio = scrSize.Width.ToString() + ":" + scrSize.ToString();
                            break;
                        }
                    case MediaStrechMode.UniformToFill:
                        {
                            MediaPlayer.AspectRatio = null;

                            Size mediaResolution = MediaResolution;
                            float scrRatio = (float)scrSize.Width / (float)scrSize.Height;   
                            float mediaRatio = (float)mediaResolution.Width / (float)mediaResolution.Height;

                            float scaleRatio;
                            if(scrRatio <= mediaRatio)
                            {
                                scaleRatio = (float)scrSize.Height / (float)MediaResolution.Height;
                            }
                            else
                            {
                                scaleRatio = (float)scrSize.Width / (float)MediaResolution.Width;
                            }

                            MediaPlayer.Scale = scaleRatio;

                            break;
                        }
                }
            }
            #endregion

            #region Public Properties
            public long MediaLength
            {
                get { return MediaPlayer.Length; }
            }

            public float MediaFps
            {
                get { return MediaPlayer.Fps; }
            }

            public Size MediaResolution
            {
                get
                {
                    uint width=0, height = 0;
                    MediaPlayer.Size(0, ref width, ref height);
                    return new Size((int)width,(int)height);
                }
            }

            public float MediaKbps
            {
                get { return MediaPlayer.Media.Statistics.DemuxBitrate; }
            }

            public long MediaSize
            {
                get { return new System.IO.FileInfo(MediaItem.MediaPath).Length; }
            }

            public float MediaKbpsAverage
            {
                get 
                {
                    long time_s = MediaLength / 1000;
                    float kbps = MediaSize / time_s / 125;
                    return kbps;
                }
            }

            public float MediaPlayedProgress
            {
                get { return MediaPlayer.Position; }
                set
                {
                    if (MediaPlayer.Position != value)
                    {
                        MediaPlayer.Position = value;
                        OnPropertyChanged(nameof(MediaPlayedProgress));
                    }
                }
            }
            
            public long MediaCurrentTime
            {
                get { return (long)(MediaPlayedProgress * MediaLength); }
                set
                {
                    float rate = (float)value / (float)MediaLength;
                    if (MediaPlayer.Position != rate)
                    {
                        MediaPlayer.Position = rate;
                        OnPropertyChanged(nameof(MediaCurrentTime));
                    }
                }
            }

            public bool IsMediaPlaying
            {
                get { return MediaPlayer.IsPlaying; }
            }

            public MediaStrechMode StrechMode
            {
                get { return strechMode; }
                set { if(strechMode != value)
                    {
                        strechMode = value;
                        ApplyStrechMode(value);
                        OnPropertyChanged(nameof(StrechMode));
                    }
                }
            }
            #endregion
            #region Notify Event&Method

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                //libVLC MediaPlayer is running in another thread, we have to call dispatcherQueue to do the task.
                Views.Windows.ClientWindow.Instance.DispatcherQueue.TryEnqueue(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); });
            }
            #endregion
        }

        public enum ToggleMediaStatusAction
        {
            Pause = 0,
            Play = 1,
            Stop = 2
        }

        public enum MediaStrechMode
        {
            Uniform = 0,
            UniformToFill = 1,
            Strech = 2
        }


        public struct ToggleMediaStatusData
        {
           public MediaDesktopPlayer Player { get; set; }
           public ToggleMediaStatusAction Action { get; set; }
            public ToggleMediaStatusData(MediaDesktopPlayer player,ToggleMediaStatusAction action)
            {
                Player = player;
                Action = action;
            }
        }
    }
}
