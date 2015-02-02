using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using PureLib.Native;

namespace PureLib.WPF {
    public abstract class MessageWindow : Window {
        protected abstract void ReceiveMessage(string message);

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
                hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            if (msg == Messaging.WM_COPYDATA) {
                CopyDataStruct cds = (CopyDataStruct)Marshal.PtrToStructure(lParam, typeof(CopyDataStruct));
                ReceiveMessage(cds.lpData);
            }
            return hWnd;
        }
    }
}
