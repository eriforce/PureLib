using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PureLib.Native {
    public class Messaging {
        private const string user32Dll = "User32.dll";
        public const int WM_COPYDATA = 0x004A;

        public static void SendMessage(string message, Process process, bool sendAsync = false) {
            int length = Encoding.Default.GetBytes(message).Length;
            CopyDataStruct cds;
            cds.dwData = (IntPtr)100;
            cds.lpData = message;
            cds.cbData = length + 1;
            IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
            Marshal.StructureToPtr(cds, lParam, true);
            if (sendAsync)
                PostMessage(process.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, lParam);
            else
                SendMessage(process.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, lParam);
        }

        [DllImport(user32Dll)]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport(user32Dll)]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(user32Dll)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CopyDataStruct {
        /// <summary>
        /// User defined data
        /// </summary>
        public IntPtr dwData;
        /// <summary>
        /// Length of the string
        /// </summary>
        public int cbData;
        /// <summary>
        /// The string
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }
}
