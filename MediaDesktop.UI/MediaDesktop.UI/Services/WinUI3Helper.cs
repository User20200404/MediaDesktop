using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Services
{

    class WinUI3Helper
    {
        /// <summary>
        /// Initialize target picker for using.
        /// </summary>
        /// <param name="window">An instance of <see cref="Window"/> in WinUI3.</param>
        /// <param name="targetPicker">The picker that is initialized with window.</param>
        public static void PickerInitializeWindow(Window window, object targetPicker)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(targetPicker, hwnd);
        }

        public static Size GetScreenResolution()
        {
            int width = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            int height = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            return new Size(width, height);
        }

        public static Process SelectFileInExplorer(string filePath)
        {
            string cmd = "explorer.exe";
            string arg = "/select," + filePath;
            return Process.Start(cmd, arg);
        }
    }

}
