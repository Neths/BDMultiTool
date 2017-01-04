using System;
using System.Drawing;
using BDMultiTool.Config;

namespace BDMultiTool.Core.PInvoke
{
    public interface IWindowAttacher
    {
        void Attach(IntPtr handleToAttach);
        IntPtr WindowHandle { get; }
        Size Size { get; }
        ScreenConfig Config { get; }
    }
}