using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace BDMultiTool.Core
{
    public interface IRegonizeArea
    {
        void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency);
        void GetTriangle(Rect canny);
        Color GetColorArea(Rect rectangle);
        IEnumerable<Rect> GetAreasForImage(Image image);
    }
}