using System;
using System.Runtime.InteropServices;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualMouse
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        public void MoveTo(int x, int y, MovementType type)
        {
            switch (type)
            {
                case MovementType.Instant:
                    SetCursorPos(x, y);
                    break;
                case MovementType.Linear:
                    LinearMove(x, y);
                    break;
                case MovementType.Bezier:
                    BezierMove(x, y);
                    break;
            }
        }

        private void LinearMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                int x = start.X + (targetX - start.X) * i / steps;
                int y = start.Y + (targetY - start.Y) * i / steps;
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void BezierMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            POINT control = new POINT { X = (start.X + targetX) / 2, Y = start.Y };
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                int x = (int)((1 - t) * (1 - t) * start.X + 2 * (1 - t) * t * control.X + t * t * targetX);
                int y = (int)((1 - t) * (1 - t) * start.Y + 2 * (1 - t) * t * control.Y + t * t * targetY);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        public void Click(MouseButton button)
        {
            uint downFlag = button == MouseButton.Left ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
            uint upFlag = button == MouseButton.Left ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(10);
            mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
        }
    }
}