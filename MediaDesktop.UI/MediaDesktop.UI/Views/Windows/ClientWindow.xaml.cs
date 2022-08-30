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
using MediaDesktop.UI.Views.Windows;
using System.ComponentModel;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.ViewModels;
using LibVLCSharp.Shared;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ClientWindow : Window
    {
        public static ClientWindow Instance { get; set; }
        public ClientWindow()
        {
            Instance = this;
            this.InitializeComponent();
            GlobalResources.InitializeAsync();
            mainContentFrame.Navigate(typeof(Views.Pages.ClientHostPage));
            this.Closed += ClientWindow_Closed;
        }

        private void ClientWindow_Closed(object sender, WindowEventArgs args)
        {
            GlobalResources.ViewModelCollection.SettingsItemViewModel.Save();
        }
    }
}
