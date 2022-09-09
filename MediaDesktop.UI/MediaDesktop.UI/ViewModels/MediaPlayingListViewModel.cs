using MediaDesktop.UI.Models;
using LibVLCSharp.Shared;
using System;
using MediaDesktop.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MediaDesktop.UI.ViewModels
{
    public class MediaPlayingListViewModel : INotifyPropertyChanged
    {
        #region Raw Data
        MediaPlayingList mediaPlayingList;
        #endregion
        #region Data Encapsulated
        public string Title
        {
            get { return mediaPlayingList.Title; }
            set
            {
                if (mediaPlayingList.Title != value)
                {
                    mediaPlayingList.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string Description
        {
            get { return mediaPlayingList.Description; }
            set
            {
                if (mediaPlayingList.Description != value)
                {
                    mediaPlayingList.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string CoverImagePath
        {
            get { return mediaPlayingList.CoverImagePath; }
            set
            {
                if (mediaPlayingList.CoverImagePath != value)
                {
                    mediaPlayingList.CoverImagePath = value;
                    OnPropertyChanged(nameof(CoverImagePath));
                }
            }
        }

        public DateTime CreatedTime
        {
            get { return mediaPlayingList.CreatedTime; }
            set
            {
                if (mediaPlayingList.CreatedTime != value)
                {
                    mediaPlayingList.CreatedTime = value;
                    OnPropertyChanged(nameof(CreatedTime));
                }
            }
        }

        public DateTime ModifiedTime
        {
            get { return mediaPlayingList.ModifiedTime; }
            set
            {
                if (mediaPlayingList.ModifiedTime != value)
                {
                    mediaPlayingList.ModifiedTime = value;
                    OnPropertyChanged(nameof(ModifiedTime));
                }
            }
        }

        public ObservableCollection<MediaDesktopItemViewModel> MediaItems
        {
            get { return mediaPlayingList.MediaItems; }
            private set
            {
                if (mediaPlayingList.MediaItems != value)
                {
                    mediaPlayingList.MediaItems = value;
                    OnPropertyChanged(nameof(MediaItems));
                }
            }
        }
        #endregion
        #region Methods
        public void PlayMediaList()
        {
            if (MediaItems.Any())
            {
                GlobalResources.ViewModelCollection.CurrentPlayingList.Clear();
                foreach(var item in MediaItems)
                {
                    GlobalResources.ViewModelCollection.CurrentPlayingList.Add(item);
                }
                MediaItems.First().MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
            }
        }
        #endregion
        #region Delegate Commands
        public DelegateCommand PlayMediaListCommand { get; set; }
        #endregion
        #region Inner Methods
        private void DelegateCommandStartup()
        {
            PlayMediaListCommand = new DelegateCommand((obj) => { PlayMediaList(); });
        }
        #endregion
        #region Ctor
        public MediaPlayingListViewModel()
        {
            mediaPlayingList = new MediaPlayingList();
            DelegateCommandStartup();
        }
        #endregion
        #region Notify Events&Methods

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
