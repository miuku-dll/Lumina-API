using System;
using System.Runtime.InteropServices;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualMouse
    {
        #region Imports and Structures

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_MOVE = 0x0001;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        #endregion

        public void MoveTo(int x, int y, MovementType type)
        {
            GetCursorPos(out POINT start);
            int steps = 100;

            switch (type)
            {
                case MovementType.Instant:
                    SendMouseInput(x, y);
                    break;
                case MovementType.Linear:
                case MovementType.Bezier:
                case MovementType.CubicEase:
                case MovementType.EaseIn:
                case MovementType.EaseOut:
                case MovementType.SineWave:
                    for (int i = 0; i <= steps; i++)
                    {
                        double factor = GetFactor(type, (double)i / steps);
                        int currentX = (int)(start.X + (x - start.X) * factor);
                        int currentY = (int)(start.Y + (y - start.Y) * factor);
                        SendMouseInput(currentX, currentY);
                        System.Threading.Thread.Sleep(1);
                    }
                    break;
                case MovementType.CubicInterpolation:
                    CubicInterpolationMove(start.X, start.Y, x, y, steps);
                    break;
                case MovementType.CardinalSpline:
                    CardinalSplineMove(start.X, start.Y, x, y, steps);
                    break;
                case MovementType.HermiteSpline:
                    HermiteSplineMove(start.X, start.Y, x, y, steps);
                    break;
                case MovementType.CatmullRomSpline:
                    CatmullRomSplineMove(start.X, start.Y, x, y, steps);
                    break;
            }
        }

        private void SendMouseInput(int x, int y)
        {
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dx = x * (65535 / screenWidth);
            inputs[0].mi.dy = y * (65535 / screenHeight);
            inputs[0].mi.mouseData = 0;
            inputs[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
            inputs[0].mi.time = 0;
            inputs[0].mi.dwExtraInfo = IntPtr.Zero;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        private double GetFactor(MovementType type, double t)
        {
            switch (type)
            {
                case MovementType.Linear:
                    return t;
                case MovementType.Bezier:
                    return t * t * (3 - 2 * t);
                case MovementType.CubicEase:
                    return t * t * (3 - 2 * t);
                case MovementType.EaseIn:
                    return t * t;
                case MovementType.EaseOut:
                    return 1 - (1 - t) * (1 - t);
                case MovementType.SineWave:
                    return (Math.Sin(t * Math.PI - Math.PI / 2) + 1) / 2;
                default:
                    return t;
            }
        }

        private void CubicInterpolationMove(int startX, int startY, int targetX, int targetY, int steps)
        {
            POINT control1 = new POINT { X = (2 * startX + targetX) / 3, Y = (2 * startY + targetY) / 3 };
            POINT control2 = new POINT { X = (startX + 2 * targetX) / 3, Y = (startY + 2 * targetY) / 3 };

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = t * t * (3 - 2 * t);
                int x = (int)((1 - factor) * startX + factor * targetX);
                int y = (int)((1 - factor) * startY + factor * targetY);
                SendMouseInput(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void CardinalSplineMove(int startX, int startY, int targetX, int targetY, int steps)
        {
            POINT control1 = new POINT { X = startX + (targetX - startX) / 3, Y = startY + (targetY - startY) / 3 };
            POINT control2 = new POINT { X = startX + 2 * (targetX - startX) / 3, Y = startY + 2 * (targetY - startY) / 3 };

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                int x = (int)(startX * (2 * t3 - 3 * t2 + 1) + control1.X * (-2 * t3 + 3 * t2) + control2.X * (t3 - 2 * t2 + t) + targetX * (t3 - t2));
                int y = (int)(startY * (2 * t3 - 3 * t2 + 1) + control1.Y * (-2 * t3 + 3 * t2) + control2.Y * (t3 - 2 * t2 + t) + targetY * (t3 - t2));
                SendMouseInput(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void HermiteSplineMove(int startX, int startY, int targetX, int targetY, int steps)
        {
            POINT tangent1 = new POINT { X = (targetX - startX) / 2, Y = (targetY - startY) / 2 };
            POINT tangent2 = new POINT { X = (startX - targetX) / 2, Y = (startY - targetY) / 2 };

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                double h1 = 2 * t3 - 3 * t2 + 1;
                double h2 = -2 * t3 + 3 * t2;
                double h3 = t3 - 2 * t2 + t;
                double h4 = t3 - t2;
                int x = (int)(startX * h1 + targetX * h2 + tangent1.X * h3 + tangent2.X * h4);
                int y = (int)(startY * h1 + targetY * h2 + tangent1.Y * h3 + tangent2.Y * h4);
                SendMouseInput(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void CatmullRomSplineMove(int startX, int startY, int targetX, int targetY, int steps)
        {
            POINT p0 = new POINT { X = startX - (targetX - startX), Y = startY - (targetY - startY) };
            POINT p1 = new POINT { X = startX, Y = startY };
            POINT p2 = new POINT { X = targetX, Y = targetY };
            POINT p3 = new POINT { X = targetX + (targetX - startX), Y = targetY + (targetY - startY) };

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                int x = (int)(0.5 * ((2 * p1.X) + (-p0.X + p2.X) * t + (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 + (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3));
                int y = (int)(0.5 * ((2 * p1.Y) + (-p0.Y + p2.Y) * t + (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 + (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3));
                SendMouseInput(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        public void Click(MouseButton button)
        {
            uint downFlag = button == MouseButton.Left ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;
            uint upFlag = button == MouseButton.Left ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            INPUT[] inputs = new INPUT[2];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = downFlag;
            inputs[1].type = INPUT_MOUSE;
            inputs[1].mi.dwFlags = upFlag;

            SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void DoubleClick(MouseButton button)
        {
            Click(button);
            System.Threading.Thread.Sleep(10);
            Click(button);
        }

        public void Press(MouseButton button)
        {
            uint flag = button == MouseButton.Left ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = flag;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void Release(MouseButton button)
        {
            uint flag = button == MouseButton.Left ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = flag;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void Scroll(int amount)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = 0x0800; // MOUSEEVENTF_WHEEL
            inputs[0].mi.mouseData = (uint)amount;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public POINT GetCursorPosition()
        {
            GetCursorPos(out POINT point);
            return point;
        }
    }

    public enum MovementType
    {
        Instant,
        Linear,
        Bezier,
        CubicEase,
        EaseIn,
        EaseOut,
        SineWave,
        CubicInterpolation,
        CardinalSpline,
        HermiteSpline,
        CatmullRomSpline
    }

    public enum MouseButton
    {
        Left,
        Right
    }
}