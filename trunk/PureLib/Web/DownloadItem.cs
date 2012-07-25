using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PureLib.Common;

namespace PureLib.Web {
    public class DownloadItem {
        private DownloadItemState _state;

        public string Url { get; private set; }
        public string Referer { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public CookieContainer Cookies { get; private set; }
        public string FilePath { get; private set; }
        public DownloadItemState State {
            get {
                return _state;
            }
            private set {
                if (_state != value) {
                    DownloadItemState oldState = _state;
                    _state = value;
                    OnStateChanged(this, oldState, _state);
                }
            }
        }
        public bool IsReady {
            get {
                return State == DownloadItemState.Queued;
            }
        }
        public bool IsStopped {
            get {
                return State == DownloadItemState.Stopped;
            }
        }
        public string FileName {
            get {
                return Path.GetFileName(FilePath);
            }
        }

        public virtual long TotalBytes { get; set; }
        public virtual long ReceivedBytes { get; set; }
        public virtual int Percentage { get; set; }

        public event DownloadItemStateChangedEventHandler StateChanged;

        public DownloadItem(string url, string referer, CookieContainer cookies, string path, DownloadItemState state = DownloadItemState.Queued) {
            if ((state != DownloadItemState.Queued) && (state != DownloadItemState.Stopped))
                throw new ApplicationException("{0} cannot be the inital state for download item.".FormatWith(state));
            _state = state;

            Url = url;
            Referer = referer;
            Cookies = cookies;
            FilePath = path;
        }

        public void Start() {
            if (State != DownloadItemState.Downloading)
                State = DownloadItemState.Queued;
        }

        public void Stop() {
            if ((State == DownloadItemState.Queued) || (State == DownloadItemState.Downloading))
                State = DownloadItemState.Stopped;
        }

        internal void Download() {
            if (State != DownloadItemState.Queued)
                throw new ApplicationException("Cannot download {0} with {1} state.".FormatWith(FileName, State));
     
            State = DownloadItemState.Downloading;
        }

        internal void Complete() {
            if (State != DownloadItemState.Downloading)
                throw new ApplicationException("Cannot complete {0} with {1} state.".FormatWith(FileName, State));

            State = DownloadItemState.Completed;
            if (TotalBytes == 0)
                TotalBytes = new FileInfo(FilePath).Length;
            ReceivedBytes = TotalBytes;
            Percentage = 100;
        }

        protected virtual void OnStateChanged(DownloadItem item, DownloadItemState oldState, DownloadItemState newState) {
            if (StateChanged != null)
                StateChanged(this, new DownloadItemStateChangedEventArgs(item, oldState, newState));
        }
    }

    public class DownloadItemStateChangedEventArgs : EventArgs {
        public DownloadItem DownloadItem { get; private set; }
        public DownloadItemState OldState { get; private set; }
        public DownloadItemState NewState { get; private set; }

        public DownloadItemStateChangedEventArgs(DownloadItem item, DownloadItemState oldState, DownloadItemState newState) {
            DownloadItem = item;
            OldState = oldState;
            NewState = newState;
        }
    }
    public delegate void DownloadItemStateChangedEventHandler(object sender, DownloadItemStateChangedEventArgs e);
}
