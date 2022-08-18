using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDesktop.UI.Models;
using LibVLCSharp.Shared;
namespace MediaDesktop.UI.ViewModels
{
    public class MediaItemViewModel : INotifyPropertyChanged
    {
        #region Raw Data Source
        private MediaItem mediaItem;
        #endregion

        #region Raw Data Encapsulation

        #endregion

        #region Runtime Data Encapsulation
        #endregion


        public MediaItemViewModel()
        {
            mediaItem = new MediaItem();
        }






        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Represents and places the data produced on run time.
        /// </summary>
        public class RuntimeData
        {
            private Media media;

            public RuntimeData(Media media)
            {
                this.media = media;
            }
        }
    }
}
