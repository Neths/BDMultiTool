using System;
using System.Windows.Forms;

namespace BDMultiTool.Core.PInvoke
{
    public interface IWindowAttacher
    {
        void Attach(IntPtr handleToAttach);
        void SendKeypress(Keys currentKey);
    }
}