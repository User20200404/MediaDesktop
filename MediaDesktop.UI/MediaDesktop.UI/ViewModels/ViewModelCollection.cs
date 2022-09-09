using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.Models;
using MediaDesktop.UI.Views.Pages;

namespace MediaDesktop.UI.ViewModels
{
    public class ViewModelCollection : INotifyPropertyChanged
    {

        private MediaDesktopItemViewModel currentDesktopItemViewModel;
        private SettingsItemViewModel settingsItemViewModel;
        private ObservableCollection<MediaDesktopItemViewModel> currentPlayingList;
        private ObservableCollection<MediaDesktopItemViewModel> viewModelItems_Favourite;
        private ObservableCollection<MediaDesktopItemViewModel> viewModelItems_History;
        private ObservableCollection<MediaPlayingListViewModel> mediaPlayingListViewModels;

        /// <summary>
        /// Do not modify this collection except using provided methods and delegate commands.
        /// </summary>
        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems { get; private set; }
        public ObservableCollection<SettingsNavigationItemViewModel> SettingsNavigationItemViewModels { get; private set; }
        public ObservableCollection<SettingsNavigationItemViewModel> SettingsNavigationItemViewModels_Bread { get; private set; }
        public MediaDesktopItemViewModelConfig MediaDesktopItemViewModelConfig { get; private set; }
        public MediaPlayingListConfig MediaPlayingListConfig { get; private set; }


        public ObservableCollection<MediaDesktopItemViewModel> CurrentPlayingList
        {
            get { return currentPlayingList; }
            set
            {
                if(currentPlayingList != value)
                {
                    currentPlayingList = value;
                    OnPropertyChanged(nameof(CurrentPlayingList));
                }
            }
        }

        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems_Favourite
        {
            get { return viewModelItems_Favourite; }
            private set
            {
                if (viewModelItems_Favourite != value)
                {
                    viewModelItems_Favourite = value;
                    OnPropertyChanged(nameof(ViewModelItems_Favourite));
                }
            }
        }
        public ObservableCollection<MediaDesktopItemViewModel> ViewModelItems_History
        {
            get { return viewModelItems_History; }
            private set
            {
                if (viewModelItems_History != value)
                {
                    viewModelItems_History = value;
                    OnPropertyChanged(nameof(ViewModelItems_History));

                }
            }
        }

        public ObservableCollection<MediaPlayingListViewModel> MediaPlayingListViewModels
        {
            get { return mediaPlayingListViewModels; }
            private set
            {
                if(mediaPlayingListViewModels != value)
                {
                    mediaPlayingListViewModels = value;
                    OnPropertyChanged(nameof(MediaPlayingListViewModels));
                }
            }
        }
        public MediaDesktopItemViewModel CurrentDesktopItemViewModel
        {
            get { return currentDesktopItemViewModel; }
            set
            {
                if (currentDesktopItemViewModel != value)
                {
                    currentDesktopItemViewModel = value;
                    OnPropertyChanged(nameof(CurrentDesktopItemViewModel));
                }
            }
        }
        public SettingsItemViewModel SettingsItemViewModel
        {
            get { return settingsItemViewModel; }
            set
            {
                if (settingsItemViewModel != value)
                {
                    settingsItemViewModel = value;
                    OnPropertyChanged(nameof(SettingsItemViewModel));
                }
            }
        }


        public ViewModelCollection()
        {
            ViewModelItems = new ObservableCollection<MediaDesktopItemViewModel>();
            MediaPlayingListViewModels = new ObservableCollection<MediaPlayingListViewModel>();
            CurrentPlayingList = new ObservableCollection<MediaDesktopItemViewModel>();
            MediaDesktopItemViewModelConfig = new MediaDesktopItemViewModelConfig(this);
            MediaPlayingListConfig = new MediaPlayingListConfig(this);

            SettingsItemViewModel = new SettingsItemViewModel();
            InitSettingsNavigationViewItems();
            EventStartup();
            DelegateCommandStartup();
        }

     


        #region Inner Methods

        private void InitSettingsNavigationViewItems()
        {
            SettingsNavigationItemViewModels = new ObservableCollection<SettingsNavigationItemViewModel>()
            {
               new SettingsNavigationItemViewModel() { Icon = "\xF8B0", Title = "设置", Introduction = "主页面",PageName = "设置",PageType = typeof(SettingsHostPage) },
               new SettingsNavigationItemViewModel() { Icon = "\xEA40", Title = "存储选项", Introduction = "配置文件位置",PageName = "存储选项",PageType = typeof(SettingsPage_Storage) },
               new SettingsNavigationItemViewModel() { Icon = "\xE946", Title = "关于", Introduction = "作者、软件版本",PageName = "关于",PageType = typeof(SettingsPage_About) }
            };

            SettingsNavigationItemViewModels_Bread = new ObservableCollection<SettingsNavigationItemViewModel>();
        }

        private void DelegateCommandStartup()
        {
            PlayNextCommand = new DelegateCommand((obj) => { PlayNext(); });
            PlayLastCommand = new DelegateCommand((obj) => { PlayLast(); });
            AddPlayingListViewModelCommand = new DelegateCommand((obj) => { AddPlayingListViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            AddViewModelCommand = new DelegateCommand((obj) => { AddViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot); });
            EditViewModelCommand = new DelegateCommand((obj) => { EditViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot, obj as MediaDesktopItemViewModel); });
            RemoveViewModelCommand = new DelegateCommand((obj) => { RemoveViewModel(Views.Pages.ClientHostPage.Instance.XamlRoot, obj as MediaDesktopItemViewModel); });
            ResetHistoryCommand = new DelegateCommand((obj) => { ResetHistory(Views.Pages.ClientHostPage.Instance.XamlRoot); });
        }

        private void EventStartup()
        {
            GlobalResources.InitializeCompleted += GlobalResources_InitializeCompleted;
            GlobalResources.MediaDesktopPlayer.MediaPlayerEndReached += MediaDesktopPlayer_MediaPlayerEndReached;
            SettingsNavigationItemViewModels_Bread.CollectionChanged += SettingsNavigationItemViewModels_Bread_CollectionChanged;
            ViewModelItems.CollectionChanged += ViewModelItems_CollectionChanged;
            CurrentPlayingList.CollectionChanged += CurrentPlayingList_CollectionChanged;
        }

    

        private void SettingsNavigationItemViewModels_Bread_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ClientHostPage.Instance.DispatcherQueue.TryEnqueue(() => { OnPropertyChanged(nameof(SettingsNavigationItemViewModels_Bread)); });

        }

        private void MediaDesktopPlayer_MediaPlayerEndReached(object sender, EventArgs e)
        {
            PlayBackMode mode = SettingsItemViewModel.PlayBackMode;

            switch (mode)
            {
                case PlayBackMode.Shuffle:
                    PlayNext();
                    break;
                case PlayBackMode.RepeatOne:
                    PlayRepeatOne();
                    break;
                default:
                    break;
            }
        }
        private void PlayRepeatOne()
        {
            Task.Run(() =>
            {
                CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.MediaPlayer.Media = CurrentDesktopItemViewModel.MediaItemViewModel.Media;
                CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.MediaPlayer.Play();
            });
        }

        private void PlayNext()
        {
            if (CurrentPlayingList is null)
                return;

            Task.Run(() =>
            {
                int index = CurrentPlayingList.IndexOf(CurrentDesktopItemViewModel);
                if (CurrentPlayingList.Last() == CurrentDesktopItemViewModel)
                {
                    //GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList.First().MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList.First().MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);

                    if(CurrentPlayingList.First() == CurrentPlayingList.Last())
                        GlobalResources.MediaDesktopPlayer.Stop();
                }
                else
                {
                    // GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList[index + 1].MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList[index + 1].MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                }

                GlobalResources.MediaDesktopPlayer.Play();
            });
        }

        private void PlayLast()
        {
            if (CurrentPlayingList is null)
                return;

            Task.Run(() =>
            {
                int index = CurrentPlayingList.IndexOf(CurrentDesktopItemViewModel);
                if (CurrentPlayingList.First() == CurrentDesktopItemViewModel)
                {
                    //GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList.First().MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList.Last().MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);

                    if (CurrentPlayingList.First() == CurrentPlayingList.Last())
                        GlobalResources.MediaDesktopPlayer.Stop();
                }
                else
                {
                    // GlobalResources.MediaDesktopPlayer.MediaPlayer = CurrentPlayingList[index + 1].MediaItemViewModel.RuntimeDataSet.MediaPlayer;
                    CurrentPlayingList[index - 1].MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                }

                GlobalResources.MediaDesktopPlayer.Play();
            });
        }

        private void CurrentPlayingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.OldItems != null)
            {
                bool isPlaying = CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet.IsMediaPlaying;
                if (e.OldItems.Contains(CurrentDesktopItemViewModel))
                {
                    CurrentDesktopItemViewModel.MediaItemViewModel.StopMedia(GlobalResources.MediaDesktopPlayer);
                    if (CurrentPlayingList.Count > e.OldStartingIndex)
                    {
                        CurrentDesktopItemViewModel = CurrentPlayingList[e.OldStartingIndex];
                    }
                    else
                    {
                        CurrentDesktopItemViewModel = CurrentPlayingList.LastOrDefault();
                    }

                    if(isPlaying && CurrentDesktopItemViewModel!=null)
                    {
                        CurrentDesktopItemViewModel.MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
                    }
                }
            }

            if(e.NewItems!=null)
            {
                if(CurrentDesktopItemViewModel == null)
                {
                    CurrentDesktopItemViewModel = e.NewItems[0] as MediaDesktopItemViewModel;
                }
            }
        }


        private void ViewModelItems_CollectionChanged(object sender,NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ViewModelItems));
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var model = item as MediaDesktopItemViewModel;
                    model.MediaItemViewModel.RuntimeDataSet.PropertyChanged += RuntimeDataViewModel_PropertyChanged;
                    model.HistoryLevelResetRequired += DesktopItemViewModel_HistoryLevelResetRequired;
                    model.PropertyChanged += DesktopItemViewModel_PropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var model = item as MediaDesktopItemViewModel;
                    model.MediaItemViewModel.RuntimeDataSet.PropertyChanged -= RuntimeDataViewModel_PropertyChanged;
                    model.HistoryLevelResetRequired -= DesktopItemViewModel_HistoryLevelResetRequired;
                    model.PropertyChanged -= DesktopItemViewModel_PropertyChanged;
                    UpdateHistoryLevel(model, HistoryLevelUpdateReason.ItemRemoved);
                }
            }
            MediaDesktopItemViewModelConfig.Save();
        }

        private void DesktopItemViewModel_HistoryLevelResetRequired(object sender, EventArgs e)
        {
            UpdateHistoryLevel(sender as MediaDesktopItemViewModel, HistoryLevelUpdateReason.HistoryLevelResetRequired);
        }

        /// <summary>
        /// Monitors and saves the changes of <see cref="ViewModelItems"/>
        /// </summary>
        /// <param name="sender">The instance of <see cref="MediaDesktopItemViewModel"/></param>
        /// <param name="e"></param>
        private void DesktopItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MediaDesktopItemViewModelConfig.Save();
            MediaDesktopItemViewModel model = sender as MediaDesktopItemViewModel; 
            switch(e.PropertyName)
            {
                case nameof(model.IsFavourite): //If the varied property name is "IsFavourite", updates favourite items collection.
                    if(model.IsFavourite)
                    {
                        ViewModelItems_Favourite.Add(model);
                    }
                    else
                    {
                        ViewModelItems_Favourite.Remove(model);
                    }
                    break;
            }
        }

        private void RuntimeDataViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsMediaPlaying")
            {
                if ((sender as MediaItemViewModel.RuntimeData).IsMediaPlaying) //Media is being played.
                {
                    CurrentDesktopItemViewModel = GetCurrentPlayingModel();
                    UpdateHistoryLevel(CurrentDesktopItemViewModel, HistoryLevelUpdateReason.ItemPlayed);
                    OnPropertyChanged(nameof(CurrentDesktopItemViewModel));
                }
            }
        }


        private void GlobalResources_InitializeCompleted(object sender, EventArgs e)
        {
            MediaDesktopItemViewModelConfig.InitViewModel(); //reads configs and restores the items last time.
            MediaPlayingListConfig.InitViewModel();
            ViewModelItems_Favourite = new ObservableCollection<MediaDesktopItemViewModel>( ViewModelItems.Where(item => item.IsFavourite is true)); //creates favourite item collection from basic item collection.
            InitHistoryCollection();
        }


        private MediaDesktopItemViewModel GetCurrentPlayingModel()
        {
            var collection = ViewModelItems.Where(model => model.MediaItemViewModel.RuntimeDataSet.IsMediaPlaying is true);
            if (collection.Any())
            {
                return collection.First();
            }
            else
            {
                return null;
            }
        }

        private void InitHistoryCollection()
        {
            List<MediaDesktopItemViewModel> orderList = ViewModelItems.Where(i => i.HistoryLevel != -1).ToList();
            orderList.Sort((m1, m2) =>
            {
                if (m1.HistoryLevel > m2.HistoryLevel)
                    return 1;
                else return -1;
            });
            ViewModelItems_History = new ObservableCollection<MediaDesktopItemViewModel>(orderList);
        }

        private void UpdateHistoryLevel(MediaDesktopItemViewModel model, HistoryLevelUpdateReason reason)
        {
            switch (reason)
            {
                case HistoryLevelUpdateReason.None:
                    break;
                case HistoryLevelUpdateReason.ItemRemoved:
                    UpdateHistoryLevel_ItemRemoved(model);
                    break;
                case HistoryLevelUpdateReason.ItemPlayed:
                    UpdateHistoryLevel_ItemPlayed(model);
                    break;
                case HistoryLevelUpdateReason.HistoryLevelResetRequired:
                    UpdateHistoryLevel_ResetRequired(model);
                    break;
                default:
                    break;
            }
        }

        private void UpdateHistoryLevel_ItemRemoved(MediaDesktopItemViewModel model)
        {
            #region Model Data Handling
            int oldLevel = model.HistoryLevel;
            if (oldLevel != -1)
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel > oldLevel)
                    {
                        i.HistoryLevel--;
                    }
                }
            }
            #endregion
            #region Collection Data Handling
            if (ViewModelItems_History.Contains(model))
            {
                ViewModelItems_History.Remove(model);
            }
            #endregion
        }

        private void UpdateHistoryLevel_ItemPlayed(MediaDesktopItemViewModel model)
        {
            #region Model Data Handling
            int oldLevel = model.HistoryLevel;
            if (oldLevel == -1) //First Play
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel != -1)
                    {
                        i.HistoryLevel++;
                    }
                }
            }
            else //Not First Play
            {
                foreach (var i in ViewModelItems)
                {
                    if (i.HistoryLevel < oldLevel && i.HistoryLevel != -1)
                    {
                        i.HistoryLevel++;
                    }
                }
            }
            CurrentDesktopItemViewModel.HistoryLevel = 0;
            #endregion

            #region Collection Data Handling
            if (ViewModelItems_History.FirstOrDefault() != model)
            {
                if (ViewModelItems_History.Contains(model))
                {
                    ViewModelItems_History.Remove(model);
                }
                ViewModelItems_History.Insert(0, model);
            }
            #endregion
        }

        private void UpdateHistoryLevel_ResetRequired(MediaDesktopItemViewModel model)
        {
            #region Data Handling
            int oldLevel = model.HistoryLevel;
            model.HistoryLevel = -1;

            foreach (var i in ViewModelItems)
            {
                if (i.HistoryLevel > oldLevel)
                {
                    i.HistoryLevel--;
                }
            }
            #endregion
            #region Collection Data Handling
            if(ViewModelItems_History.Contains(model))
            {
                ViewModelItems_History.Remove(model);
            }
            #endregion
        }

        private enum HistoryLevelUpdateReason
        {
            None = 0,
            ItemRemoved = 1,
            ItemPlayed = 2,
            HistoryLevelResetRequired = 3
        }
        #endregion

        #region Methods
        public async void AddPlayingListViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            var model = new MediaPlayingListViewModel();
            
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = new Views.Dialogs.ModifyPlayingListDialogPage() { DataContext = model, Tag = "新建播放列表" },
                PrimaryButtonText = "新建",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                MediaPlayingListViewModels.Add(model); 
            }

        }

        public async void AddViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            var model = new MediaDesktopItemViewModel();
            
            ContentDialog contentDialog = new ContentDialog()
            {
                Content = new Views.Dialogs.ModifyItemDialogPage() { DataContext = model, Tag = "添加项目" },
                PrimaryButtonText = "添加",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (File.Exists(model.MediaPath))
                {
                    model.MediaItemViewModel.LoadMedia(Services.GlobalResources.LibVLC);
                }
                ViewModelItems.Add(model);
            }
        }

        public async void EditViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot, MediaDesktopItemViewModel model)
        {
            MediaDesktopItemViewModel tempModel = new MediaDesktopItemViewModel()
            {
                Title = model.Title,
                SubTitle = model.SubTitle,
                MediaPath = model.MediaPath,
                ImagePath = model.ImagePath
            };

            ContentDialog contentDialog = new ContentDialog()
            {
                Content = new Views.Dialogs.ModifyItemDialogPage() { DataContext = tempModel, Tag = "编辑项目" },
                PrimaryButtonText = "保存",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                model.Title = tempModel.Title;
                model.SubTitle = tempModel.SubTitle;
                model.MediaPath = tempModel.MediaPath;
                model.ImagePath = tempModel.ImagePath;
            }
        }

        public async void RemoveViewModel(Microsoft.UI.Xaml.XamlRoot xamlRoot, MediaDesktopItemViewModel model)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "删除项目",
                Content = "确定要移除项目" + model.Title + "吗？",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Secondary,
                XamlRoot = xamlRoot
            };
            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ViewModelItems.Remove(model);
            }
        }

        public async void ResetHistory(Microsoft.UI.Xaml.XamlRoot xamlRoot)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "清空历史记录",
                Content = "所有项目的播放历史都将被重置，且无法撤销此操作。",
                PrimaryButtonText = "确定",
                SecondaryButtonText = "取消",
                DefaultButton = ContentDialogButton.Secondary,
                XamlRoot = xamlRoot
            };
            var result = await contentDialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                foreach(var i in ViewModelItems_History)
                {
                    i.HistoryLevel = -1;
                }
                ViewModelItems_History.Clear();
            }
        }
        #endregion

        #region Delegate Commands
        public DelegateCommand PlayNextCommand { get; set; }
        public DelegateCommand PlayLastCommand { get; set; }
        public DelegateCommand AddPlayingListViewModelCommand { get; set; }
        public DelegateCommand AddViewModelCommand { get; set; }
        public DelegateCommand EditViewModelCommand { get; set; }
        public DelegateCommand RemoveViewModelCommand { get; set; }
        public DelegateCommand ResetHistoryCommand { get; set; }
        #endregion

        #region Notify Event&Method

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
