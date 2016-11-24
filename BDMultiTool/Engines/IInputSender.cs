using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BDMultiTool.Engines
{
    public interface IInputSender
    {
        void SendKeys(Keys key);
        void SendKeys(IEnumerable<Keys> keys);
        void MouseRightClickTo(Point point);
        void MouseLeftClickTo(Point point);
    }
}