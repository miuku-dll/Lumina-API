using System;
using System.Runtime.InteropServices;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualMouse
    {

        #region Imports etc

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

        #endregion

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
                case MovementType.CubicEase:
                    CubicEaseMove(x, y);
                    break;
                case MovementType.EaseIn:
                    EaseInMove(x, y);
                    break;
                case MovementType.EaseOut:
                    EaseOutMove(x, y);
                    break;
                case MovementType.SineWave:
                    SineWaveMove(x, y);
                    break;
                case MovementType.CubicInterpolation:
                    CubicInterpolationMove(x, y);
                    break;
                case MovementType.CardinalSpline:
                    CardinalSplineMove(x, y);
                    break;
                case MovementType.HermiteSpline:
                    HermiteSplineMove(x, y);
                    break;
                case MovementType.CatmullRomSpline:
                    CatmullRomSplineMove(x, y);
                    break;
            }
        }

        #region Movement Types

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

        private void CubicEaseMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = t * t * (3 - 2 * t);
                int x = (int)(start.X + (targetX - start.X) * factor);
                int y = (int)(start.Y + (targetY - start.Y) * factor);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void EaseInMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = t * t;
                int x = (int)(start.X + (targetX - start.X) * factor);
                int y = (int)(start.Y + (targetY - start.Y) * factor);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void EaseOutMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = 1 - (1 - t) * (1 - t);
                int x = (int)(start.X + (targetX - start.X) * factor);
                int y = (int)(start.Y + (targetY - start.Y) * factor);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void SineWaveMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = (Math.Sin(t * Math.PI - Math.PI / 2) + 1) / 2;
                int x = (int)(start.X + (targetX - start.X) * factor);
                int y = (int)(start.Y + (targetY - start.Y) * factor);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void CubicInterpolationMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            POINT control1 = new POINT { X = (2 * start.X + targetX) / 3, Y = (2 * start.Y + targetY) / 3 };
            POINT control2 = new POINT { X = (start.X + 2 * targetX) / 3, Y = (start.Y + 2 * targetY) / 3 };
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double factor = t * t * (3 - 2 * t);
                int x = (int)((1 - factor) * start.X + factor * targetX);
                int y = (int)((1 - factor) * start.Y + factor * targetY);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void CardinalSplineMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            POINT control1 = new POINT { X = start.X + (targetX - start.X) / 3, Y = start.Y + (targetY - start.Y) / 3 };
            POINT control2 = new POINT { X = start.X + 2 * (targetX - start.X) / 3, Y = start.Y + 2 * (targetY - start.Y) / 3 };
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                int x = (int)(start.X * (2 * t3 - 3 * t2 + 1) + control1.X * (-2 * t3 + 3 * t2) + control2.X * (t3 - 2 * t2 + t) + targetX * (t3 - t2));
                int y = (int)(start.Y * (2 * t3 - 3 * t2 + 1) + control1.Y * (-2 * t3 + 3 * t2) + control2.Y * (t3 - 2 * t2 + t) + targetY * (t3 - t2));
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void HermiteSplineMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            POINT tangent1 = new POINT { X = (targetX - start.X) / 2, Y = (targetY - start.Y) / 2 };
            POINT tangent2 = new POINT { X = (start.X - targetX) / 2, Y = (start.Y - targetY) / 2 };
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                double h1 = 2 * t3 - 3 * t2 + 1;
                double h2 = -2 * t3 + 3 * t2;
                double h3 = t3 - 2 * t2 + t;
                double h4 = t3 - t2;
                int x = (int)(start.X * h1 + targetX * h2 + tangent1.X * h3 + tangent2.X * h4);
                int y = (int)(start.Y * h1 + targetY * h2 + tangent1.Y * h3 + tangent2.Y * h4);
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        private void CatmullRomSplineMove(int targetX, int targetY)
        {
            GetCursorPos(out POINT start);
            POINT p0 = new POINT { X = start.X - (targetX - start.X), Y = start.Y - (targetY - start.Y) };
            POINT p1 = start;
            POINT p2 = new POINT { X = targetX, Y = targetY };
            POINT p3 = new POINT { X = targetX + (targetX - start.X), Y = targetY + (targetY - start.Y) };
            int steps = 100;
            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / steps;
                double t2 = t * t;
                double t3 = t2 * t;
                int x = (int)(0.5 * ((2 * p1.X) + (-p0.X + p2.X) * t + (2 * p0.X - 5 * p1.X + 4 * p2.X - p3.X) * t2 + (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3));
                int y = (int)(0.5 * ((2 * p1.Y) + (-p0.Y + p2.Y) * t + (2 * p0.Y - 5 * p1.Y + 4 * p2.Y - p3.Y) * t2 + (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3));
                SetCursorPos(x, y);
                System.Threading.Thread.Sleep(1);
            }
        }

        #endregion

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