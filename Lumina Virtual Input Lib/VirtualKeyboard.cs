using System;
using System.Runtime.InteropServices;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualKeyboard
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const uint KEYEVENTF_KEYUP = 0x0002;

        public void SendKey(byte virtualKeyCode, bool keyUp = false)
        {
            uint flags = keyUp ? KEYEVENTF_KEYUP : 0;
            keybd_event(virtualKeyCode, 0, flags, UIntPtr.Zero);
        }
    }
}