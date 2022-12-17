using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaDesktop.UI.Models
{
    public class SettingsItem
    {
        public string MediaItemRecordINIPath { get; set; }
        public string MediaPlayingListINIDir { get; set; }
        public string ExceptionLogPath { get; set; }
        public int Volume { get; set; }
        public float SpeedRatio { get; set; }
        public PlayBackMode PlayBackMode { get; set; }  
        public int LastLibraryPagePivotIndex { get; set; }
    }
    public enum PlayBackMode
    {
        Shuffle = 0,
        RepeatOne = 1
    }
}
