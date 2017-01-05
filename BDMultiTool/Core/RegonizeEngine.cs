using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using BDMultiTool.Config;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using OpenTK.Graphics.ES11;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

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

        public Rectangle GetRectangle(Bitmap img, Rectangle canny, Color color, int colorThreshold, ContourAcceptance acceptance)
        {
            DebugWindow.SetImageStartFishingRaw((Bitmap)img.Clone());

            var imgFiltered = FilterImage(new Image<Bgr, byte>(img), new FilterParam(color, colorThreshold));

            DebugWindow.SetImageStartFishingFiltered(imgFiltered.ToBitmap());

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
                        return new Rectangle(new Point(area.X, area.Y), new Size(area.Width, area.Height));
                    }
                }
            }

            return Rectangle.Empty;
        }

        public IEnumerable<Rectangle> GetAllRectangles(Bitmap img, Rectangle canny, Color color, int colorThreshold,
            ContourAcceptance acceptance)
        {
            var imgFiltered = FilterImage(new Image<Bgr, byte>(img), new FilterParam(color, colorThreshold));

            var result = new List<Rectangle>();

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
                        result.Add(new Rectangle(new Point(area.X, area.Y), new Size(area.Width, area.Height)));
                    }
                }
            }

            return result;
        }

        public bool HaveRectangle(Bitmap img, Rectangle canny, Color color, int colorThreshold,
            ContourAcceptance acceptance)
        {
            var config = new BasicAreaConfig
            {
                Area = new CaptureAreaConfig { X = canny.X, Y = canny.Y, Width = canny.Width, Height = canny.Height},
                ContourAcceptance = new ContourAcceptanceConfig
                {
                    Height = acceptance.Height, HeightOffset = acceptance.HeightOffset,
                    Width = acceptance.Width, WidthOffset = acceptance.WidthOffset,
                    Size = acceptance.Size, SizeOffset = acceptance.SizeOffset
                },
                Color = new ColorConfig { R = color.R, G = color.G, B = color.B, Seuil = colorThreshold }
            };

            return HaveRectangle(img, config);
        }

        public bool HaveRectangle(Bitmap img, BasicAreaConfig config)
        {
            var acceptance = config.ContourAcceptance.ToContourAcceptance();
            var imgFiltered = FilterImage(new Image<Bgr, byte>(img), new FilterParam(config.Color.ToColor(), config.Color.Seuil));

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
                        return true;
                    }
                }
            }

            return false;
        }

        public Rectangle GetRectangle(Rectangle canny, Color color, int colorThreshold, ContourAcceptance acceptance)
        {
            return GetRectangle(_screenHelper.ScreenArea(canny), canny, color, colorThreshold, acceptance);
        }

        public void WaitRectangleColor(Rectangle canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency, ContourAcceptance acceptance)
        {
            while (true)
            {
                var rectangle = GetRectangle(canny, color, colorThreshold, acceptance);

                if (rectangle != Rectangle.Empty)
                {
                    callback(this, new RectEventArgs(rectangle));
                    return;
                }

                Thread.Sleep(checkFrequency);
            }
        }

        public Color GetColor(Point point)
        {
            var img = _screenHelper.ScreenArea(new Rectangle(point.X, point.Y, 1, 1));
            return GetColor(img, point);
        }

        public Color GetColor(Bitmap img, Point point)
        {
            return img.GetPixel(0, 0);
        }

        public static Image<Gray, byte> FilterImage(Image<Bgr, byte> imgSrc, FilterParam filter)
        {
            return imgSrc.InRange(filter.GetMinus(), filter.GetMaxi());
        }

        public IEnumerable<Rectangle> GetAreasForImage(Image image)
        {
            throw new NotImplementedException();
        }

        public Rectangle MatchPattern(Image<Bgr, byte> source, Image<Bgr, byte> pattern)
        {
            using (var result = source.MatchTemplate(pattern, TemplateMatchingType.CcoeffNormed))
            {
                double[] minValues, maxValues;
                Point[] minLocations, maxLocations;
                result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                if (maxValues[0] > 0.9)
                {
                    return new Rectangle() { Location = maxLocations[0], Size = pattern.Size};
                }
            }

            return Rectangle.Empty;
        }

        public class FishTriangle
        {
            readonly byte[,] _rawData = new byte[12,12];

            public FishTriangle(Image<Gray, byte> img, Point coord)
            {
                for (var x = 0; x < 12; x++)
                {
                    for (var y = 0; y < 12; y++)
                    {
                        _rawData[y, x] = img.Data[(int)coord.Y - 6 + y, (int)coord.X - 6 + x , 0];
                    }
                }
            }

            public Orientation GetOrientation()
            {
                if(!HaveData)
                    return Orientation.None;

                if (TopLeftAvailable)
                {
                    return TopRightAvailable ? Orientation.Down : Orientation.Right;
                }

                return TopRightAvailable ? Orientation.Left : Orientation.Up;
            }

            public bool HaveData => TopLeftAvailable || TopRightAvailable || BottomLeftAvailable || BottomRightAvailable;

            public bool TopLeftAvailable => GetAvailable(0, 0);

            public bool TopRightAvailable => GetAvailable(0, 8);

            public bool BottomLeftAvailable => GetAvailable(8, 0);

            public bool BottomRightAvailable => GetAvailable(8, 8);

            private bool GetAvailable(int y, int x)
            {
                //var d = _rawData[x, y] + _rawData[x + 1, y] + _rawData[x, y - 1] + _rawData[x + 1, y - 1];

                int s = 0;

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        s += _rawData[y + i, x + j] > 0 ? 1 : 0;
                    }
                }

                return s > 0;
            }

            public Bitmap GetBitmap()
            {
                var img = new Image<Gray,byte>(new System.Drawing.Size(12,12));

                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        img.Data[i, j, 0] = Convert.ToByte(_rawData[i, j]);
                    }
                }

                return img.ToBitmap();
            }

            public enum Orientation
            {
                Up,
                Down,
                Left,
                Right,
                None
            }
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
