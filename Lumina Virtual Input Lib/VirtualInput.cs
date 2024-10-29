using System;

namespace Lumina_Virtual_Input_Lib
{
    public class VirtualInput
    {
        public static VirtualMouse Mouse { get; } = new VirtualMouse();
        public static VirtualKeyboard Keyboard { get; } = new VirtualKeyboard();
    }
}