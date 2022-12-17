using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
namespace MediaDesktop
{
    public delegate int WndProcCallBack(int nCode, uint wParam, IntPtr lParam);
    public class SystemAPIs
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendMessageA([In] IntPtr handleOfWindow, [In] UInt32 Message, [In] IntPtr wParam, [In] IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean PostMessageA([In] IntPtr handleOfWindow, [In] UInt32 Message, [In] IntPtr wParam, [In] IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent([In] IntPtr handleOfChildWindow, [In, Optional] IntPtr handleOfNewParentWindow);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowExA([In, Optional] IntPtr handleOfParentWindow, [In, Optional] IntPtr handleOfChildAfter, [In, Optional] string className, [In, Optional] string windowText);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr FindWindowA([In, Optional] string className, [In, Optional] string windowText);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 GetWindowThreadProcessId([In] IntPtr windowHandle, [Out, Optional]out Int32 processId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean IsWindowEnabled([In] IntPtr windowHandle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean IsWindowVisible([In] IntPtr windowHandle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern Boolean ShowWindow([In] IntPtr windowHandle ,[In] int command);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfoA(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookExA(int idHook, WndProcCallBack LRESULT_CODE_WPARAM_LPARAM, IntPtr hMod, int threadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookExA(int idHook, IntPtr lpFunc, IntPtr hMod, int threadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandleA(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibraryA(string lpModulePath);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hDll);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    }
}
