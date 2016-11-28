using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace BDMultiTool.Core
{
    public class RegonizeEngine : IRegonizeArea
    {
        private readonly IScreenHelper _screenHelper;

        public RegonizeEngine(IScreenHelper screenHelper)
        {
            _screenHelper = screenHelper;
        }

        public void WaitRectangleColor(Rect canny, Color color, int colorThreshold, EventHandler<RectEventArgs> callback, int checkFrequency)
        {
            while (true)
            {
                var img = _screenHelper.ScreenArea(new Rectangle(Convert.ToInt32(canny.X), Convert.ToInt32(canny.Y), Convert.ToInt32(canny.Width), Convert.ToInt32(canny.Height)));

                var imgFiltered = FilterCaptcha(new Image<Bgr, byte>(img), new FilterParam(color, colorThreshold));

                var lstC = new List<Rectangle>();
                using (var contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(imgFiltered, contours, null, RetrType.List, ChainApproxMethod.ChainApproxNone);

                    for (var i = 0; i < contours.Size; i++)
                    {
                        var contour = contours[i];

                        if (contour.Size < 10)
                            continue;

                        var area = CvInvoke.MinAreaRect(contour).MinAreaRect();

                        if (area.Height >= 200)
                        {
                            //var angle = CvInvoke.MinAreaRect(contour).Angle;

                            lstC.Add(area);
                        }
                    }
                }

                Thread.Sleep(checkFrequency);
            }
        }

        public static Image<Gray, byte> FilterCaptcha(Image<Bgr, byte> imgSrc, int filterB, int filterG, int filterR, int filterOffset)
        {
            var filter = new FilterParam { B = filterB, G = filterG, R = filterR, Offset = filterOffset };

            return FilterCaptcha(imgSrc, filter);
        }

        public static Image<Gray, byte> FilterCaptcha(Image<Bgr, byte> imgSrc, FilterParam filter)
        {
            return imgSrc.InRange(filter.GetMinus(), filter.GetMaxi());
        }

        public void GetTriangle(Rect canny)
        {
            throw new NotImplementedException();
        }

        public Color GetColorArea(Rect rectangle)
        {
            throw new NotImplementedException();
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
