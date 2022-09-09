using MediaDesktop.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using IniParser;
using IniParser.Model;
using MediaDesktop.UI.Services;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using MediaDesktop.UI.Views.Windows;
using Windows.Storage.Pickers;
using System.Threading;

namespace MediaDesktop.UI.ViewModels
{
    public class SettingsItemViewModel : INotifyPropertyChanged
    {
        private SettingsItem settingsItem;
        private readonly string defaultKey = "Value";
        private readonly string defaultItemRecordPath = ApplicationData.Current.LocalFolder.Path + @"\items.ini";
        private readonly string defaultExceptionLogPath = ApplicationData.Current.LocalFolder.Path + @"\exceptionlog.ini";
        private readonly string defaultMediaPlayingListDir = ApplicationData.Current.LocalFolder.Path + @"\PlayingList";
        private readonly string basePath = ApplicationData.Current.LocalFolder.Path + @"\base.ini";
        private readonly string baseDirectory = ApplicationData.Current.LocalFolder.Path;
        private FileIniDataParser FileIniDataParser { get { return GlobalResources.FileIniDataParser; } }

        public string BasePath
        {
            get { return basePath; }
        }

        public string BaseDirectory
        {
            get { return baseDirectory; }
        }

        public string MediaItemRecordINIPath
        {
            get { return settingsItem.MediaItemRecordINIPath; }
            set
            {
                if (settingsItem.MediaItemRecordINIPath != value)
                {
                    settingsItem.MediaItemRecordINIPath = value;
                    OnPropertyChanged(nameof(MediaItemRecordINIPath));
                }
            }
        }

        public string MediaPlayingListINIDir
        {
            get { return settingsItem.MediaPlayingListINIDir; }
            set
            {
                if (settingsItem.MediaPlayingListINIDir != value)
                {
                    settingsItem.MediaPlayingListINIDir = value;
                    OnPropertyChanged(nameof(MediaPlayingListINIDir));
                }
            }
        }

        public string ExceptionLogPath
        {
            get { return settingsItem.ExceptionLogPath; }
            set
            {
                if (settingsItem.ExceptionLogPath != value)
                {
                    settingsItem.ExceptionLogPath = value;
                    OnPropertyChanged(nameof(ExceptionLogPath));
                }
            }
        }
        public int Volume
        {
            get { return settingsItem.Volume; }
            set { if(settingsItem.Volume!=value)
                {
                    settingsItem.Volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public PlayBackMode PlayBackMode
        {
            get { return settingsItem.PlayBackMode; }
            set
            {
                if (settingsItem.PlayBackMode != value)
                {
                    settingsItem.PlayBackMode = value;
                    OnPropertyChanged(nameof(PlayBackMode));
                }
            }
        }


        public void Save()
        {
            IniData iniData = EncodeIniData();
            FileIniDataParser.WriteFile(basePath, iniData);
        }
        public void SwitchPlayBackMode()
        {
            if (PlayBackMode == PlayBackMode.Shuffle)
                PlayBackMode = PlayBackMode.RepeatOne;
            else PlayBackMode = PlayBackMode.Shuffle;
        }

        public async void BrowseFileInExplorer(string path)
        {
            if (File.Exists(path))
            {
                WinUI3Helper.SelectFileInExplorer(path);
            }
            else
            {
                await new ContentDialog()
                {
                    Title = "无法定位配置文件",
                    PrimaryButtonText = "关闭",
                    DefaultButton = ContentDialogButton.Primary,
                    Content = "配置文件不存在。\n\n如果这是您第一次启动本程序，则属正常情况。\n配置文件将会在您退出程序时尝试生成。",
                    XamlRoot = ClientWindow.Instance.Content.XamlRoot
                }.ShowAsync();
            }
        }

        public async void SetMediaRecordPath()
        {
            FileSavePicker picker = new FileSavePicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "MediaDesktop.UI.Library";
            picker.FileTypeChoices.Add("配置文件", new List<string>() { ".ini" });
            StorageFile file = await picker.PickSaveFileAsync();
            if(file is not null)
            {
                MediaItemRecordINIPath = file.Path;
            }
        }

        public async void SetExceptionLogPath()
        {
            FileSavePicker picker = new FileSavePicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "MediaDesktop.UI.ExceptionLog";
            picker.FileTypeChoices.Add("日志文件", new List<string>() { ".ini" });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file is not null)
            {
                ExceptionLogPath = file.Path;
            }
        }

        public async void SetMediaPlayingListDir()
        {
            FolderPicker picker = new FolderPicker();
            WinUI3Helper.PickerInitializeWindow(Views.Windows.ClientWindow.Instance, picker);
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            if (folder is not null)
            {
                MediaPlayingListINIDir = folder.Path;
            }
        }

        #region Delegate Command
        public DelegateCommand SwitchPlayBackModeCommand { get; set; }
        public DelegateCommand BrowseFileInExplorerCommand { get; set; }
        public DelegateCommand SetMediaRecordPathCommand { get; set; }
        public DelegateCommand SetExceptionLogPathCommand { get; set; }
        public DelegateCommand SetMediaPlayingListDirCommand { get; set; }
        #endregion
        private void DelegateCommandStartup()
        {
            SwitchPlayBackModeCommand = new DelegateCommand((obj) => { SwitchPlayBackMode(); });
            BrowseFileInExplorerCommand = new DelegateCommand((obj) => { BrowseFileInExplorer(obj as string); });
            SetMediaRecordPathCommand = new DelegateCommand((obj) => { SetMediaRecordPath(); });
            SetExceptionLogPathCommand = new DelegateCommand((obj) => { SetExceptionLogPath(); });
            SetMediaPlayingListDirCommand = new DelegateCommand((obj) => { SetMediaPlayingListDir(); });
        }

        public void Initialize()
        {
            if (!File.Exists(basePath))
            {
                File.Create(basePath).Close();
            }
            IniData iniData = FileIniDataParser.ReadFile(basePath);
            MediaItemRecordINIPath = iniData.GetValueOrDefault(nameof(MediaItemRecordINIPath), defaultKey, defaultItemRecordPath);
            MediaPlayingListINIDir = iniData.GetValueOrDefault(nameof(MediaPlayingListINIDir), defaultKey, defaultMediaPlayingListDir);
            ExceptionLogPath = iniData.GetValueOrDefault(nameof(ExceptionLogPath), defaultKey, defaultExceptionLogPath);
            Volume = iniData.GetIntValueOrDefault(nameof(Volume), defaultKey, 100);
            PlayBackMode = (PlayBackMode)Enum.Parse(typeof(PlayBackMode), iniData.GetValueOrDefault(nameof(PlayBackMode), defaultKey, "RepeatOne"));
        }

        public SettingsItemViewModel()
        {
            settingsItem = new SettingsItem();
            EventStartup();
            DelegateCommandStartup();
            Initialize();
        }

        private IniData EncodeIniData()
        {
            
            IniData data = new IniData();
            data.Sections.AddSection(nameof(MediaItemRecordINIPath));
            data.Sections.AddSection(nameof(MediaPlayingListINIDir));
            data.Sections.AddSection(nameof(ExceptionLogPath));
            data.Sections.AddSection(nameof(Volume));
            data.Sections.AddSection(nameof(PlayBackMode));
            data.Sections[nameof(MediaItemRecordINIPath)].AddKey(defaultKey, MediaItemRecordINIPath);
            data.Sections[nameof(MediaPlayingListINIDir)].AddKey(defaultKey, MediaPlayingListINIDir);
            data.Sections[nameof(ExceptionLogPath)].AddKey(defaultKey, ExceptionLogPath);
            data.Sections[nameof(Volume)].AddKey(defaultKey, Volume.ToString());
            data.Sections[nameof(PlayBackMode)].AddKey(defaultKey, PlayBackMode.ToString());

            return data;
        }

        private void EventStartup()
        {
            PropertyChanged += This_PropertyChanged;
            GlobalResources.MediaDesktopPlayer.MediaPlayerPlaying += MediaDesktopPlayer_MediaPlayerPlaying;

        }

        private void MediaDesktopPlayer_MediaPlayerPlaying(object sender, LibVLCSharp.Shared.MediaPlayer args)
        {
            Task.Run(()=> { args.Mute = true; Thread.Sleep(150); args.Volume = Volume; args.Mute = false; });
        }

        private void This_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(MediaItemRecordINIPath):

                    break;

                case nameof(Volume):
                    if (GlobalResources.MediaDesktopPlayer.MediaPlayer != null)
                    {
                        GlobalResources.MediaDesktopPlayer.MediaPlayer.Volume = Volume;
                    }
                    break;
            }

        }
        #region Notify Events&Methods
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
        #endregion
    }
}
