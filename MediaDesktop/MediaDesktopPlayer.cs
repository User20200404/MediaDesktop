﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
namespace MediaDesktop
{
    public class MediaDesktopPlayer
    {
        private VideoView mediaPresenter;
        private LibVLC libVLC;
        private MediaPlayer mediaPlayer;

        /// <summary>
        /// Get the instance of <see cref="LibVLCSharp.Shared.LibVLC"/> for current player.
        /// </summary>
        public LibVLC LibVLC
        {
            get { return libVLC; }
        }

        /// <summary>
        /// Get the instance of <see cref="VideoView"/> for current player (a system-managed window, you can control it using WinForms methods or Windows APIs).
        /// </summary>
        public VideoView MediaPresenter
        {
            get { return mediaPresenter; }
        }

        /// <summary>
        /// Get the instance of <see cref="LibVLCSharp.Shared.MediaPlayer"/> to control the media-play better.
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get { return mediaPlayer; }
        }


        private void PlayerStartup(int screenWidth, int screenHeight)
        {
            mediaPresenter = new VideoView();
            libVLC = new LibVLC("--input-repeat=999");
            mediaPlayer = new MediaPlayer(libVLC);
            mediaPresenter.MediaPlayer = mediaPlayer;
            mediaPresenter.Height = screenHeight;
            mediaPresenter.Width = screenWidth;
            mediaPresenter.Left = 0;
            mediaPresenter.Top = 0; 
        }

        /// <summary>
        /// Create an instance of <see cref="MediaDesktopPlayer"/> using default parameters.
        /// </summary>
        public MediaDesktopPlayer()
        {
            APIsPackaged.GetScreenResolution(out int width, out int height);
            PlayerStartup(width, height);
        }

        /// <summary>
        /// Set media to the player for playing.
        /// </summary>
        /// <param name="path">path of the media file.</param>
        public void SetMedia(string path)
        {
            Media media = new Media(libVLC, new Uri(path));
            mediaPlayer.Media = media;
        }

        /// <summary>
        /// Set media to the player for playing.
        /// </summary>
        /// <param name="path">stream of media data.</param>
        public void SetMedia(Stream stream)
        {
            Media media = new Media(libVLC, new StreamMediaInput(stream));
            mediaPlayer.Media = media;        
        }


        /// <summary>
        /// Start or resume the current media playing.
        /// </summary>
        /// <returns>A boolean value that indicates the success status of this operation.</returns>
        public bool Play()
        {
           return mediaPlayer.Play();
        }

        /// <summary>
        /// Pause the current media playing. Some media may not support this operation.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Pause()
        {
            if(mediaPlayer.CanPause)
            {
                mediaPlayer.Pause();
            }
            else
            {
                throw new InvalidOperationException("pausing is not supported for this media.");
            }
        }

        /// <summary>
        /// Stop the current media playing. the progress of playing will be reset.This method may block current thread.
        /// </summary>
        public void Stop()
        {
            mediaPlayer.Stop();
        }

        /// <summary>
        /// Attach this media player to a <see cref="MediaDesktopBase"/> instance.
        /// </summary>
        /// <param name="mediaDesktopBase"></param>
        public void AttachToMediaDesktopBase(MediaDesktopBase mediaDesktopBase)
        {
            SystemAPIs.SetParent(mediaPresenter.Handle, mediaDesktopBase.AttachmentHandlerForm.Handle);
            mediaPresenter.Show();
        }
    }
}