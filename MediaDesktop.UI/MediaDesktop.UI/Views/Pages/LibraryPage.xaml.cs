using MediaDesktop.UI.ViewModels;
using MediaDesktop.UI.Services;
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
using Microsoft.UI.Xaml.Shapes;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        private ViewModelCollection ViewModelCollection { get { return GlobalResources.ViewModelCollection; } }
        public LibraryPage()
        {
            this.InitializeComponent();
            if(!GlobalResources.IsInitialized)
            {
                GlobalResources.InitializeCompleted += GlobalResources_InitializeCompleted;
            }
            else
            {
                StopProgressRingIndicator();
            }

            playingListFrame.Navigate(typeof(MediaPlayingListPage));
        }
        
        private void StopProgressRingIndicator()
        {
            loadingProgressRing.IsActive = false;
            loadingProgressRingCover.Visibility = Visibility.Collapsed;
        }

        private void GlobalResources_InitializeCompleted(object sender, EventArgs e)
        {
            StopProgressRingIndicator();
        }

        private void LibraryItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as MediaDesktopItemViewModel).MediaItemViewModel.PlayMedia(GlobalResources.MediaDesktopPlayer);
        }
    }
}
