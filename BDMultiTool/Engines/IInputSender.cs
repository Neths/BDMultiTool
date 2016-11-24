using System.Collections.Generic;
using System.Windows.Forms;

namespace BDMultiTool.Engines
{
    public interface IInputSender
    {
        void SendKeys(Keys key);
        void SendKeys(IEnumerable<Keys> keys);
    }
}