using System;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualInput
    {
        public static VirtualMouse Mouse { get; } = new VirtualMouse();
        public static VirtualKeyboard Keyboard { get; } = new VirtualKeyboard();
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