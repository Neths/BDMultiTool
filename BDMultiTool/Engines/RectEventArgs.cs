using System;
using System.Windows;

namespace BDMultiTool.Engines
{
    public class RectEventArgs : EventArgs
    {
        public Rect Rect { get; set; }

        public RectEventArgs(Rect rect)
        {
            Rect = rect;
        }
    }
}