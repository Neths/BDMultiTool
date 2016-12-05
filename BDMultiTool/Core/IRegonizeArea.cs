using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using Point = System.Drawing.Point;

namespace BDMultiTool.Core
{
    public interface IRegonizeArea
    {
        void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, RegonizeEngine.ContourAcceptance acceptance);
        Color GetColor(Point point);
        IEnumerable<Rect> GetAreasForImage(Image image);
    }
}