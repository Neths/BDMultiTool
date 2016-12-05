using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Color = System.Drawing.Color;
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

        public void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, ContourAcceptance acceptance)
        {
            while (true)
            {
                var img = _screenHelper.ScreenArea(new Rectangle(Convert.ToInt32(canny.X), Convert.ToInt32(canny.Y), Convert.ToInt32(canny.Width), Convert.ToInt32(canny.Height)));

                var imgFiltered = FilterCaptcha(new Image<Bgr, byte>(img), new FilterParam(color, colorThreshold));

                Clipboard.SetImage(ConvertBitmap(imgFiltered.Bitmap));

                using (var contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(imgFiltered, contours, null, RetrType.List, ChainApproxMethod.LinkRuns);

                    for (var i = 0; i < contours.Size; i++)
                    {
                        var contour = contours[i];

                        if (!acceptance.ValideSize(contour.Size))
                            continue;

                        var area = CvInvoke.MinAreaRect(contour).MinAreaRect();

                        if (acceptance.ValideHeight(area.Height) && acceptance.ValideWidth(area.Width))
                        {
                            var aa = img.Clone(new Rectangle(area.X, area.Y, area.Width, area.Height), img.PixelFormat);
                            Clipboard.SetImage(ConvertBitmap(aa));
                            callback(this, new RectEventArgs(new Rect(new Point(area.X,area.Y),new Size(area.Width,area.Height))));
                        }
                    }
                }

                Thread.Sleep(checkFrequency);
            }
        }

        public void GetTriangles(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, ContourAcceptance acceptance)
        {
            var img = _screenHelper.ScreenArea(new Rectangle(Convert.ToInt32(canny.X), Convert.ToInt32(canny.Y), Convert.ToInt32(canny.Width), Convert.ToInt32(canny.Height)));

            var a = new Image<Rgb, byte>(img);
            var b = new Image<Bgr,byte>(img);

            CvInvoke.CvtColor(a,b,ColorConversion.Rgb2Bgr);

            Clipboard.SetImage(ConvertBitmap(a.Bitmap));

            var imgFiltered = FilterCaptcha(b, new FilterParam(color, colorThreshold));

            Clipboard.SetImage(ConvertBitmap(imgFiltered.Bitmap));

            using (var contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(imgFiltered, contours, null, RetrType.List, ChainApproxMethod.LinkRuns);

                for (var i = 0; i < contours.Size; i++)
                {
                    var contour = contours[i];

                    if (!acceptance.ValideSize(contour.Size))
                        continue;

                    var area = CvInvoke.MinAreaRect(contour).MinAreaRect();

                    if (acceptance.ValideHeight(area.Height) && acceptance.ValideWidth(area.Width))
                    {
                        var aa = img.Clone(new Rectangle(area.X, area.Y, area.Width, area.Height), img.PixelFormat);
                        Clipboard.SetImage(ConvertBitmap(aa));
                        //callback(this, new RectEventArgs(new Rect(new Point(area.X, area.Y), new Size(area.Width, area.Height))));
                    }
                }
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

        public class ContourAcceptance
        {
            public int Size;
            public int Width;
            public int Height;

            public int SizeOffset;
            public int WidthOffset;
            public int HeightOffset;

            public bool ValideSize(int size) => size >= GetSizeMinus() && size <= GetSizeMaxi();
            public bool ValideWidth(int width) => width >= GetWidthMinus() && width <= GetWidthMaxi();
            public bool ValideHeight(int height) => height >= GetHeightMinus() && height <= GetHeightMaxi();

            public int GetSizeMinus() => Size - SizeOffset;
            public int GetSizeMaxi() => Size + SizeOffset;

            public int GetWidthMinus() => Width - WidthOffset;
            public int GetWidthMaxi() => Width + WidthOffset;

            public int GetHeightMinus() => Height - HeightOffset;
            public int GetHeightMaxi() => Height + HeightOffset;
        }

        public class ColorAcceptance
        {
            public Color BaseColor;
            public int Offset;

            public Color GetMinus() => Color.FromArgb(BaseColor.R - Offset, BaseColor.G - Offset, BaseColor.B - Offset);
            public Color GetMaxi() => Color.FromArgb(BaseColor.R + Offset, BaseColor.G + Offset, BaseColor.B + Offset);

            public bool Validate(Color color)
            {
                if (color.R < BaseColor.R - Offset || color.R > BaseColor.R + Offset)
                    return false;

                if (color.G < BaseColor.G - Offset || color.G > BaseColor.G + Offset)
                    return false;

                if (color.B < BaseColor.B - Offset || color.B > BaseColor.B + Offset)
                    return false;

                return true;
            }
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
