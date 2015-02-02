using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PureLib.Native {
    internal class NativeMethods {
        private const string user32Dll = "user32.dll";

        [DllImport(user32Dll, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(user32Dll)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport(user32Dll)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport(user32Dll)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}
