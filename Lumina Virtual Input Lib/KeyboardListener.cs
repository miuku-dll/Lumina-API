using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

namespace Lumina_Virtual_Input_Lib
{
    public class KeyboardListener
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private Dictionary<int, bool> keyStates = new Dictionary<int, bool>();
        private bool isListening = false;
        private Thread? listenerThread;
        private Stopwatch stopwatch = new Stopwatch();
        private long lastEventTime = 0;

        private static readonly int[] IgnoredKeys = { 0x01, 0x02, 0x04, 0x05, 0x06, 0x5B, 0x5C };

        private static readonly Dictionary<int, string> SpecialKeys = new Dictionary<int, string>
        {
            { 0x08, "BACKSPACE" },
            { 0x09, "TAB" },
            { 0x0D, "ENTER" },
            { 0x10, "SHIFT" },
            { 0x11, "CTRL" },
            { 0x12, "ALT" },
            { 0x13, "PAUSE" },
            { 0x14, "CAPSLOCK" },
            { 0x1B, "ESC" },
            { 0x20, "SPACE" },
            { 0x21, "PAGEUP" },
            { 0x22, "PAGEDOWN" },
            { 0x23, "END" },
            { 0x24, "HOME" },
            { 0x25, "LEFT" },
            { 0x26, "UP" },
            { 0x27, "RIGHT" },
            { 0x28, "DOWN" },
            { 0x2C, "PRINTSCREEN" },
            { 0x2D, "INSERT" },
            { 0x2E, "DELETE" },
            { 0x90, "NUMLOCK" },
            { 0x91, "SCROLLLOCK" }
        };

        public event EventHandler<string>? OutputGenerated;

        public void StartListening()
        {
            if (!isListening)
            {
                isListening = true;
                listenerThread = new Thread(ListenForKeys);
                listenerThread.Start();
                stopwatch.Start();
            }
        }

        public void StopListening()
        {
            isListening = false;
            listenerThread?.Join();
            stopwatch.Stop();
        }

        private void ListenForKeys()
        {
            while (isListening)
            {
                for (int i = 0; i < 256; i++)
                {
                    if (Array.IndexOf(IgnoredKeys, i) != -1)
                        continue;

                    int keyState = GetAsyncKeyState(i);
                    bool isKeyDown = (keyState & 0x8000) != 0;

                    if (keyStates.TryGetValue(i, out bool wasKeyDown))
                    {
                        if (isKeyDown && !wasKeyDown)
                        {
                            RecordEvent(i, "KeyDown");
                        }
                        else if (!isKeyDown && wasKeyDown)
                        {
                            RecordEvent(i, "KeyUp");
                        }
                    }

                    keyStates[i] = isKeyDown;
                }

                Thread.Sleep(10);
            }
        }

        private void RecordEvent(int keyCode, string eventType)
        {
            string keyName = GetKeyName(keyCode);
            if (string.IsNullOrEmpty(keyName))
            {
                return; // Don't record events for unknown keys
            }

            long currentTime = stopwatch.ElapsedMilliseconds;
            long delay = currentTime - lastEventTime;

            string output = "";
            if (lastEventTime > 0)
            {
                output += $"DELAY : {delay}\n";
            }

            output += $"Keyboard : {keyName} : {eventType}";

            OutputGenerated?.Invoke(this, output);

            lastEventTime = currentTime;
        }

        private string GetKeyName(int keyCode)
        {
            if (SpecialKeys.TryGetValue(keyCode, out string? specialKey))
            {
                return specialKey;
            }
            else if (keyCode >= 0x30 && keyCode <= 0x39 || keyCode >= 0x41 && keyCode <= 0x5A)
            {
                // Numbers and letters
                return ((char)keyCode).ToString();
            }

            // Return an empty string for any other keys
            return string.Empty;
        }
    }
}