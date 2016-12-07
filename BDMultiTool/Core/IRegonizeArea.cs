using System;
using System.Collections.Generic;
using System.Drawing;

namespace BDMultiTool.Core
{
    public interface IRegonizeArea
    {
        void WaitRectangleColor(Rectangle canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, RegonizeEngine.ContourAcceptance acceptance);
        Color GetColor(Point point);
        IEnumerable<Rectangle> GetAreasForImage(Image image);
    }
}