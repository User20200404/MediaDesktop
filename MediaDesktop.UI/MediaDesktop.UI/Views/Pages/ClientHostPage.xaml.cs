using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.ViewModels;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientHostPage : Page
    {
        bool isSlidingTemp = false;
        bool isPlayingTemp = false;
        private MediaDesktopItemViewModel CurrentDesktopItemViewModel 
        {
            get
            {
                if (GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel;
                }
                else
                {
                    return null;
                }
            }
        }
        private MediaItemViewModel CurrentMediaItemViewModel
        {
            get
            {
                if (CurrentDesktopItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel.MediaItemViewModel;
                }
                else
                {
                    return null;
                }
            }
        }
        private MediaItemViewModel.RuntimeData CurrentMediaRuntimeDataSet
        {
            get
            {
                if (CurrentMediaItemViewModel != null)
                {
                    return GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel.MediaItemViewModel.RuntimeDataSet;
                }
                else
                {
                    return null;
                }
            }
        }

        public static ClientHostPage Instance { get; set; }
        public ClientHostPage()
        {
            Instance = this;
            this.InitializeComponent();
            contentFrame.Navigate(typeof(LibraryPage));

            GlobalResources.ViewModelCollection.PropertyChanged += ViewModelCollection_PropertyChanged;
            GlobalResources.MediaDesktopPlayer.ScreenSolutionChanged += System_ScreenSolutionChanged;
            progressSlider.AddHandler(PointerPressedEvent,new PointerEventHandler(ProgressSlider_PointerPressed), true);
            progressSlider.AddHandler(PointerReleasedEvent,new PointerEventHandler(ProgressSlider_PointerReleased), true);
        }

        /// <summary>
        /// Invoked when screen solution changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void System_ScreenSolutionChanged(object sender, EventArgs e)
        {
            //To ensure the playing media to fill the screen, reset its StrechMode to resize properly.
            if (CurrentMediaItemViewModel is not null)
                CurrentMediaItemViewModel.RuntimeDataSet.StrechMode = MediaItemViewModel.MediaStrechMode.UniformToFill;
        }

        private void ViewModelCollection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(GlobalResources.ViewModelCollection.CurrentDesktopItemViewModel):
                    if(CurrentMediaItemViewModel is not null)
                    CurrentMediaItemViewModel.RuntimeDataSet.StrechMode = MediaItemViewModel.MediaStrechMode.UniformToFill;
                    return;
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (isSlidingTemp && CurrentMediaRuntimeDataSet != null)
            {
                float value = (float)(sender as Slider).Value;
                if (value > 0.999f)
                    value = 0.999f;
                CurrentMediaRuntimeDataSet.MediaPlayedProgress = value;
            }
        }

        private void ProgressSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isSlidingTemp = true;
            isPlayingTemp = CurrentMediaRuntimeDataSet?.IsMediaPlaying ?? false;
            ProgressSlider_ValueChanged(progressSlider, null);
        }

        private void ProgressSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isSlidingTemp = false;
            if (isPlayingTemp)
            {
                CurrentMediaItemViewModel?.ToggleMediaStatusTo(GlobalResources.MediaDesktopPlayer, MediaItemViewModel.ToggleMediaStatusAction.Play);
            }
            else
            {
                CurrentMediaItemViewModel?.ToggleMediaStatusTo(GlobalResources.MediaDesktopPlayer, MediaItemViewModel.ToggleMediaStatusAction.Pause);
            }
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            switch ((args.InvokedItemContainer as NavigationViewItem).Name)
            {
                case nameof(navigationViewItem_Lib):
                    if (contentFrame.Content is not LibraryPage)
                        contentFrame.Navigate(typeof(LibraryPage));
                    break;
                case nameof(leftNavigationView.SettingsItem):
                    if (contentFrame.Content is not SettingsPage)
                        contentFrame.Navigate(typeof(SettingsPage));
                    break;
                case nameof(navigationViewItem_Play):
                    if(contentFrame.Content is not CurrentPlayingListPage)
                        contentFrame.Navigate(typeof(CurrentPlayingListPage));
                    break;
            }
        }
    }
}
