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
                    // Skippaa ignoratut nappaimet inshallah
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

                Thread.Sleep(10); // prossun saastaminen ong
            }
        }

        private void RecordEvent(int keyCode, string eventType)
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            long delay = currentTime - lastEventTime;

            string output = "";
            if (lastEventTime > 0)
            {
                output += $"DELAY : {delay}\n";
            }

            string keyName = GetKeyName(keyCode);
            output += $"Keyboard : {keyName} : {eventType}";

            OutputGenerated?.Invoke(this, output);

            lastEventTime = currentTime;
        }

        private string GetKeyName(int keyCode)
        {
            if (keyCode == 0x20) // Space key
            {
                return "SPACE";
            }
            else
            {
                return ((char)keyCode).ToString();
            }
        }
    }
}