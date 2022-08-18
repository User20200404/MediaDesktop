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
        public string MediaPath
        {
            get { return mediaDesktopItem.MediaPath; }
            set
            {
                if (mediaDesktopItem.MediaPath != value)
                {
                    mediaDesktopItem.MediaPath = value;
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
        #endregion
        #region Sub-ViewModel
        public MediaItemViewModel MediaItemViewModel { get; private set; }
        #endregion

        public MediaDesktopItemViewModel()
        {
            mediaDesktopItem = new MediaDesktopItem();
            MediaItemViewModel = new MediaItemViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
