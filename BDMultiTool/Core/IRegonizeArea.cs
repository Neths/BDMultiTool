using System;
using System.Collections.Generic;
using System.Drawing;
using BDMultiTool.Config;
using Emgu.CV;
using Emgu.CV.Structure;

namespace BDMultiTool.Core
{
    public interface IRegonizeArea
    {
        void WaitRectangleColor(Rectangle canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, RegonizeEngine.ContourAcceptance acceptance);
        Color GetColor(Point point);
        Color GetColor(Bitmap img, Point point);
        IEnumerable<Rectangle> GetAreasForImage(Image image);
        Rectangle GetRectangle(Rectangle canny, Color color, int colorThreshold,
            RegonizeEngine.ContourAcceptance acceptance);
        Rectangle GetRectangle(Bitmap img, Rectangle canny, Color color, int colorThreshold,
            RegonizeEngine.ContourAcceptance acceptance);
        IEnumerable<Rectangle> GetAllRectangles(Bitmap img, Rectangle canny, Color color, int colorThreshold,
            RegonizeEngine.ContourAcceptance acceptance);

        bool HaveRectangle(Bitmap img, Rectangle canny, Color color, int colorThreshold,
            RegonizeEngine.ContourAcceptance acceptance);
        bool HaveRectangle(Bitmap img, BasicAreaConfig config);
        Rectangle MatchPattern(Image<Bgr, byte> source, Image<Bgr, byte> pattern);
    }
}