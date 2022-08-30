using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
namespace MediaDesktop.UI.ViewModels
{
    public class MediaDesktopItemViewModel : INotifyPropertyChanged
    {
        #region Raw Data Source
        private MediaDesktopItem mediaDesktopItem;
        #endregion
        #region Raw Data Encapsulation

        public int HistoryLevel
        {
            get { return mediaDesktopItem.HistoryLevel; }
            set
            {
                if (mediaDesktopItem.HistoryLevel != value)
                {
                    mediaDesktopItem.HistoryLevel = value;
                    OnPropertyChanged(nameof(HistoryLevel));
                }
            }
        }

        public string MediaPath
        {
            get { return mediaDesktopItem.MediaPath; }
            set
            {
                if (mediaDesktopItem.MediaPath != value)
                {
                    mediaDesktopItem.MediaPath = value;
                    MediaItemViewModel.MediaPath = value;
                    OnPropertyChanged(nameof(MediaPath));
                }
            }
        }

        public string ImagePath
        {
            get { return mediaDesktopItem.ImagePath; }
            set
            {
                if (mediaDesktopItem.ImagePath != value)
                {
                    mediaDesktopItem.ImagePath = value; OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public string Title
        {
            get { return mediaDesktopItem.Title; }
            set { if(mediaDesktopItem.Title != value)
                {
                    mediaDesktopItem.Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string SubTitle
        {
            get { return mediaDesktopItem.SubTitle; }
            set
            {
                if (mediaDesktopItem.SubTitle != value)
                {
                    mediaDesktopItem.SubTitle = value;
                    OnPropertyChanged(nameof(SubTitle));
                }
            }
        }

        public bool IsFavourite
        {
            get { return mediaDesktopItem.IsFavourite; }
            set
            {
                if (mediaDesktopItem.IsFavourite != value)
                {
                    mediaDesktopItem.IsFavourite = value;
                    OnPropertyChanged(nameof(IsFavourite));
                }
            }
        }

        public MediaDesktopItemViewModel Self
        {
            get { return this; }
        }
        #endregion


        #region Sub-ViewModel
        public MediaItemViewModel MediaItemViewModel { get; private set; }
        #endregion

        #region Inner Methods
        private void DelegateCommandStartup()
        {
            ToggleFavouriteCommand = new DelegateCommand((obj) => { ToggleFavourite(); });
            ResetHistoryLevelCommand = new DelegateCommand((obj) => { ResetHistoryLevel(); });
        }
        #endregion

        #region Public Methods
        public void ToggleFavourite()
        {
            IsFavourite = !IsFavourite;
        }

        public void ResetHistoryLevel()
        {
            //We pass this request to ViewModelCollection.
            if (this.HistoryLevel != -1)
            {
                HistoryLevelResetRequired?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Delegate Commands
        public DelegateCommand ToggleFavouriteCommand { get; set; }
        public DelegateCommand ResetHistoryLevelCommand { get; set; }
        #endregion

        public MediaDesktopItemViewModel()
        {
            mediaDesktopItem = new MediaDesktopItem();
            MediaItemViewModel = new MediaItemViewModel();

            DelegateCommandStartup();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler HistoryLevelResetRequired;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
