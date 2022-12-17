using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
using MediaDesktop.UI.Views.Dialogs;
using MediaDesktop.UI.Views.Pages;
using Microsoft.UI.Xaml.Controls;

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
            AddToMediaPlayingListCommand = new DelegateCommand((obj) => { AddToMediaPlayingList(ClientHostPage.Instance.Content.XamlRoot); });
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

        public async void AddToMediaPlayingList(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            ContentDialog dialog = new ContentDialog()
            {
                XamlRoot = xamlRoot,
                Content = new AddToPlayingListDialogPage(),
                Title = "添加到播放列表",
                PrimaryButtonText = "添加",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary
            };
            (dialog.Content as AddToPlayingListDialogPage).Tag = this;

            await dialog.ShowAsync();

            /*
            if (result == ContentDialogResult.Primary)
            {
                ContentDialog errorDialog = new ContentDialog() { XamlRoot = xamlRoot, Title = "无法添加项目", PrimaryButtonText = "确定" };

                MediaPlayingListViewModel model = (dialog.Content as AddToPlayingListDialogPage).Tag as MediaPlayingListViewModel;
                if (model != null)
                {
                    if (!model.MediaItems.Contains(this))
                    {
                        model.MediaItems.Add(this);
                    }
                    else
                    {
                        errorDialog.Content = "播放列表中已经存在该项目。";
                        await errorDialog.ShowAsync();
                    }
                }
                else
                {
                    errorDialog.Content = "未选择项目。";
                    await errorDialog.ShowAsync();
                }
            }*/
        }
        #endregion

        #region Delegate Commands
        public DelegateCommand ToggleFavouriteCommand { get; set; }
        public DelegateCommand ResetHistoryLevelCommand { get; set; }
        public DelegateCommand AddToMediaPlayingListCommand { get; set; }
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
