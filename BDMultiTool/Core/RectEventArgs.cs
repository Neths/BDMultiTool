using System;
using System.Drawing;
using System.Windows;

namespace BDMultiTool.Core
{
    public class RectEventArgs : EventArgs
    {
        public Rectangle Rect { get; set; }

        public RectEventArgs(Rectangle rect)
        {
            Rect = rect;
        }
    }
}