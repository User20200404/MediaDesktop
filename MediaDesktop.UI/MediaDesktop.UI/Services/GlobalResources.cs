using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibVLCSharp.Shared;
using MediaDesktop.UI.ViewModels;
using IniParser;
using IniParser.Model;
using IniParser.Parser;
using Microsoft.UI.Dispatching;
using MediaDesktop;
using MediaDesktop.UI.Models;
using System.ComponentModel;

namespace MediaDesktop.UI.Services
{
    public class GlobalResources
    {
        #region Events
        public static event EventHandler InitializeLibVLCCompeleted;
        public static event EventHandler InitializeCompleted;
        #endregion

        #region Global Resources
        public static List<ImageCache> ImageCaches { get; private set; }
        public static MediaDesktopBase MediaDesktopBase { get; private set; }
        public static MediaDesktopPlayer MediaDesktopPlayer { get; private set; }

        public static ViewModelCollection ViewModelCollection { get; private set; }
        public static FileIniDataParser FileIniDataParser { get; private set; }


        public static LibVLC LibVLC { get; private set; }

        #endregion

        #region Status
        public static bool IsInitialized
        {
            get { return LibVLC is not null; }
        }

        #endregion

        #region Methods
        public static async void InitializeAsync()
        {
            InitializeLibVLCCompeleted += GlobalResources_InitializeLibVLCCompeleted;
            InitIniParser();
            InitMediaDesktopHelper();
            InitViewModel();
            InitModel();
            await InitLibVLC();
            InitializeCompleted?.Invoke(typeof(GlobalResources), EventArgs.Empty);
        }

        private static void GlobalResources_InitializeLibVLCCompeleted(object sender, EventArgs e)
        {
            MediaDesktopPlayer.LibVLC = LibVLC;
            
        }
        #endregion

        #region Inner Methods

        private static void InitIniParser()
        {
            FileIniDataParser = new FileIniDataParser();
        }

        private static void InitViewModel()
        {
            ViewModelCollection = new ViewModelCollection();
        }

        private static void InitModel()
        {
            ImageCaches = new List<ImageCache>();
        }

        private static void InitMediaDesktopHelper()
        {
            MediaDesktopBase = new MediaDesktopBase();
            MediaDesktopPlayer = new MediaDesktopPlayer();
            MediaDesktopPlayer.AttachToMediaDesktopBase(MediaDesktopBase);
            MediaDesktopBase.MediaAttachedPosition = MediaAttachedPosition.AttachToWorkerW;
            MediaDesktopBase.AttachToDesktop();
        }

        private static async Task InitLibVLC()
        {
            await Task.Run(() =>
            {
                LibVLC = new LibVLC();
            });
            InitializeLibVLCCompeleted?.Invoke(LibVLC, EventArgs.Empty);
        }
        #endregion
    }
}
