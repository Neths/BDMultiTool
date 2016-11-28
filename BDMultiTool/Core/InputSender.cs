using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using InputManager;

namespace BDMultiTool.Core
{
    public class InputSender : IInputSender
    {
        public void SendKeys(Keys key)
        {
            Keyboard.KeyPress(key);
        }

        public void SendKeys(IEnumerable<Keys> keys)
        {
            keys.ToList().ForEach(SendKeys);
        }

        public void MouseRightClickTo(Point point)
        {
            Mouse.Move(point.X, point.Y);
            Thread.Sleep(5);
            Mouse.PressButton(Mouse.MouseKeys.Right);
        }

        public void MouseLeftClickTo(Point point)
        {
            Mouse.Move(point.X, point.Y);
            Thread.Sleep(5);
            Mouse.PressButton(Mouse.MouseKeys.Right);
        }
    }
}
