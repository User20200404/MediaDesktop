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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using MediaDesktop.UI.Services;
using MediaDesktop.UI.Views.Windows;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MediaDesktop.UI.Views.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyItemDialogPage : Page
    {
        public ModifyItemDialogPage()
        {
            this.InitializeComponent();
        }


        
        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var targetTextBox = (sender as Control).Tag as TextBox;

            FileOpenPicker picker = new FileOpenPicker();

            if (targetTextBox == mediaPathTextBox)
            {
                picker.FileTypeFilter.Add(".avi");
                picker.FileTypeFilter.Add(".mp4");
                picker.FileTypeFilter.Add(".mkv");
                picker.FileTypeFilter.Add(".flv");
            }
            if(targetTextBox == imagePathTextBox)
            {
                picker.FileTypeFilter.Add(".png");
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".gif");
                picker.FileTypeFilter.Add(".bmp");
            }

            WinUI3Helper.PickerInitializeWindow(ClientWindow.Instance, picker);

            StorageFile storageFile = await picker.PickSingleFileAsync();

            if (storageFile is null)
                return;

            targetTextBox.Text = storageFile.Path;
        }
    }
}
