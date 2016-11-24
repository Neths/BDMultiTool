using System;
using System.Drawing;
using System.Windows;

namespace BDMultiTool.Engines
{
    public interface IRegonizeArea
    {
        void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency);
        void GetTriangle(Rect canny);

    }
}