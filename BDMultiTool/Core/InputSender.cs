using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BDMultiTool.Core.Hook;
using InputManager;

namespace BDMultiTool.Core
{
    public class InputSender : IInputSender
    {
        public InputSender()
        {
            
        }

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
            Thread.Sleep(100);
            Mouse.PressButton(Mouse.MouseKeys.Right);
        }

        public void MouseLeftClickTo(Point point)
        {
            Mouse.Move(point.X, point.Y);
            Thread.Sleep(100);
            Mouse.ButtonDown(Mouse.MouseKeys.Left);
            Thread.Sleep(500);
            Mouse.ButtonUp(Mouse.MouseKeys.Left);
        }

        public void MouseMoveTo(Point point)
        {
            Mouse.Move(point.X, point.Y);
        }
    }
}
