using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PureLib.Common;
using PureLib.WPF;

namespace PureLib.Web {
    [DataContract]
    public class DownloadItem : NotifyObject {
        private DownloadItemState _state;
        private Uri _uri;
        private string _referer;
        private string _userName;
        private string _password;
        private string _location;
        private string _fileName;
        private long _totalBytes;
        private long _receiveBytes;

        [DataMember]
        public DownloadItemState State {
            get {
                return _state;
            }
            private set {
                if (_state != value) {
                    DownloadItemState oldState = _state;
                    _state = value;
                    OnStateChanged(this, oldState, _state);
                    RaiseChange(() => State);
                }
            }
        }
        [DataMember]
        public Uri Uri {
            get {
                return _uri;
            }
            set {
                _uri = value;
                RaiseChange(() => Uri);
            }
        }
        [DataMember]
        public string Referer {
            get {
                return _referer;
            }
            set {
                _referer = value;
                RaiseChange(() => Referer);
            }
        }
        [DataMember]
        public string UserName {
            get {
                return _userName;
            }
            set {
                _userName = value;
                RaiseChange(() => UserName);
            }
        }
        [DataMember]
        public string Password {
            get {
                return _password;
            }
            set {
                _password = value;
                RaiseChange(() => Password);
            }
        }
        [DataMember]
        public string Directory {
            get {
                return _location;
            }
            set {
                _location = value;
                RaiseChange(() => Directory);
                RaiseChange(() => FilePath);
            }
        }
        [DataMember]
        public string FileName {
            get {
                return _fileName;
            }
            set {
                _fileName = value;
                RaiseChange(() => FileName);
                RaiseChange(() => FilePath);
            }
        }
        [DataMember]
        public string FilePath {
            get {
                return Path.Combine(Directory, FileName);
            }
            set {
                _location = Path.GetDirectoryName(value);
                _fileName = Path.GetFileName(value);
                RaiseChange(() => Directory);
                RaiseChange(() => FileName);
                RaiseChange(() => FilePath);
            }
        }
        [DataMember]
        public long TotalBytes {
            get {
                return _totalBytes;
            }
            set {
                _totalBytes = value;
                RaiseChange(() => TotalBytes);
                RaiseChange(() => Percentage);
            }
        }
        [DataMember]
        public long ReceivedBytes {
            get {
                return _receiveBytes;
            }
            set {
                _receiveBytes = value;
                RaiseChange(() => ReceivedBytes);
                RaiseChange(() => Percentage);
            }
        }
        public int Percentage {
            get { return TotalBytes == 0 ? 0 : (int)(100 * ReceivedBytes / TotalBytes); }

        }
        public bool IsReady {
            get { return State == DownloadItemState.Queued; }
        }
        public bool IsStopped {
            get { return State == DownloadItemState.Stopped; }
        }
        public bool IsDownloading {
            get { return State == DownloadItemState.Downloading; }
        }

        public event DownloadItemStateChangedEventHandler StateChanged;

        public DownloadItem() {
        }

        public DownloadItem(string url, string referer, string path,
            DownloadItemState state = DownloadItemState.Queued) : this(url, state, referer) {

            FilePath = path;
        }

        private DownloadItem(string url, DownloadItemState state, string referer) {
            if (!Uri.TryCreate(url, UriKind.Absolute, out _uri))
                throw new ArgumentException("Invalid url: {0}".FormatWith(url));

            _state = state;
            if (!IsReady && !IsStopped)
                throw new ApplicationException("{0} cannot be the inital state for download item.".FormatWith(state));

            _referer = referer;
        }

        public void Start() {
            if (State != DownloadItemState.Downloading)
                State = DownloadItemState.Queued;
        }

        public void Stop() {
            if ((State == DownloadItemState.Queued) || (State == DownloadItemState.Downloading))
                State = DownloadItemState.Stopped;
        }

        internal void Error() {
            if (State != DownloadItemState.Downloading)
                throw new ApplicationException("Cannot set {0} to error with {1} state.".FormatWith(FileName, State));
            State = DownloadItemState.Error;
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
