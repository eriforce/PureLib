using System;
using System.Runtime.InteropServices;

namespace PureLib.MediaInfo {
    public sealed class MediaInfo : IDisposable {
        private IntPtr _handle;

        public bool Loaded {
            get { return _handle != IntPtr.Zero; }
        }

        public MediaInfo() {
            try {
                _handle = NativeMethods.MediaInfo_New();
            }
            catch { }
        }

        ~MediaInfo() {
            Dispose();
        }

        public int Open(string fileName) {
            return NativeMethods.MediaInfo_Open(_handle, fileName).ToInt32();
        }

        public int OpenBufferInit(long fileSize, long fileOffset) {
            return NativeMethods.MediaInfo_Open_Buffer_Init(_handle, fileSize, fileOffset).ToInt32();
        }

        public int OpenBufferContinue(IntPtr buffer, IntPtr bufferSize) {
            return NativeMethods.MediaInfo_Open_Buffer_Continue(_handle, buffer, bufferSize).ToInt32();
        }

        public long OpenBufferContinueGoToGet() {
            return NativeMethods.MediaInfo_Open_Buffer_Continue_GoTo_Get(_handle);
        }

        public int OpenBufferFinalize() {
            return NativeMethods.MediaInfo_Open_Buffer_Finalize(_handle).ToInt32();
        }

        public void Close() {
            NativeMethods.MediaInfo_Close(_handle);
        }

        public string Inform() {
            return Marshal.PtrToStringUni(NativeMethods.MediaInfo_Inform(_handle, (IntPtr)0));
        }

        public string Get(StreamKind streamKind, int streamNumber, string parameter, InfoKind kindOfInfo = InfoKind.Text, InfoKind kindOfSearch = InfoKind.Name) {
            return Marshal.PtrToStringUni(NativeMethods.MediaInfo_Get(_handle, (IntPtr)streamKind, (IntPtr)streamNumber, parameter, (IntPtr)kindOfInfo, (IntPtr)kindOfSearch));
        }

        public string Get(StreamKind streamKind, int streamNumber, int parameter, InfoKind kindOfInfo = InfoKind.Text) {
            return Marshal.PtrToStringUni(NativeMethods.MediaInfo_GetI(_handle, (IntPtr)streamKind, (IntPtr)streamNumber, (IntPtr)parameter, (IntPtr)kindOfInfo));
        }

        public string Option(string option, string value = "") {
            return Marshal.PtrToStringUni(NativeMethods.MediaInfo_Option(_handle, option, value));
        }

        public int StateGet() {
            return NativeMethods.MediaInfo_State_Get(_handle).ToInt32();
        }

        public int CountGet(StreamKind streamKind, int streamNumber = -1) {
            return NativeMethods.MediaInfo_Count_Get(_handle, (IntPtr)streamKind, (IntPtr)streamNumber).ToInt32();
        }

        public void Dispose() {
            if (Loaded)
                NativeMethods.MediaInfo_Delete(_handle);
        }
    }

    public enum StreamKind {
        General,
        Video,
        Audio,
        Text,
        Chapters,
        Image,
        Menu
    }

    public enum InfoKind {
        Name,
        Text,
        Measure,
        Options,
        NameText,
        MeasureText,
        Info,
        HowTo
    }

    public enum InfoOptions {
        ShowInInform,
        Support,
        ShowInSupported,
        TypeOfValue
    }

    public enum InfoFileOptions {
        FileOption_Nothing = 0x00,
        FileOption_NoRecursive = 0x01,
        FileOption_CloseAll = 0x02,
        FileOption_Max = 0x04
    }
}
