﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PureLib.Native {
    public class Messaging {
        public const int WM_COPYDATA = 0x004A;

        public static void SendMessage(string message, Process targetProcess, bool sendAsync = false) {
            int length = Encoding.Default.GetBytes(message).Length;
            CopyDataStruct cds;
            cds.dwData = (IntPtr)100;
            cds.lpData = message;
            cds.cbData = length + 1;
            IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(cds));
            Marshal.StructureToPtr(cds, lParam, true);
            if (sendAsync)
                NativeMethods.PostMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, lParam);
            else
                NativeMethods.SendMessage(targetProcess.MainWindowHandle, WM_COPYDATA, IntPtr.Zero, lParam);
        }
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
