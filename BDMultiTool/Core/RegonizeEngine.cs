using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace BDMultiTool.Core
{
    public class RegonizeEngine : IRegonizeArea
    {
        private readonly IScreenHelper _screenHelper;

        public RegonizeEngine(IScreenHelper screenHelper)
        {
            _screenHelper = screenHelper;
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency)
        {
            while (true)
            {
                var img = _screenHelper.ScreenArea(new Rectangle(Convert.ToInt32(canny.X), Convert.ToInt32(canny.Y), Convert.ToInt32(canny.Width), Convert.ToInt32(canny.Height)));

                var imgFiltered = FilterCaptcha(new Image<Bgr, byte>(img), new FilterParam(color, colorThreshold));

                //Clipboard.SetImage(ConvertBitmap(imgFiltered.Bitmap));

                using (var contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(imgFiltered, contours, null, RetrType.List, ChainApproxMethod.LinkRuns);

                    for (var i = 0; i < contours.Size; i++)
                    {
                        var contour = contours[i];

                        if (contour.Size < 10)
                            continue;

                        var area = CvInvoke.MinAreaRect(contour).MinAreaRect();

                        if (area.Height >= 50 && area.Width >= 100)
                        {
                            callback(this, new RectEventArgs(new Rect(new Point(area.X,area.Y),new Size(area.Width,area.Height))));
                        }
                    }
                }

                Thread.Sleep(checkFrequency);
            }
        }

        public Color GetColor(System.Drawing.Point point)
        {
            var img = _screenHelper.ScreenArea(new Rectangle(Convert.ToInt32(point.X), Convert.ToInt32(point.Y), 1, 1));
            return img.GetPixel(0,0);
        }

        public static Image<Gray, byte> FilterCaptcha(Image<Bgr, byte> imgSrc, FilterParam filter)
        {
            return imgSrc.InRange(filter.GetMinus(), filter.GetMaxi());
        }

        public IEnumerable<Rect> GetAreasForImage(Image image)
        {
            throw new NotImplementedException();
        }

        public class FilterParam
        {
            public int B;
            public int G;
            public int R;
            public int Offset;

            public FilterParam(Color color, int offset)
            {
                R = Convert.ToInt32(color.R);
                G = Convert.ToInt32(color.G);
                B = Convert.ToInt32(color.B);
                Offset = offset;
            }

            public FilterParam()
            {
            }

            public Bgr GetMinus()
            {
                return new Bgr(B - Offset, G - Offset, R - Offset);
            }

            public Bgr GetMaxi()
            {
                return new Bgr(B + Offset, G + Offset, R + Offset);
            }
        }
    }
}
