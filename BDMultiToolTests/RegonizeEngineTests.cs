using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using BDMultiTool;
using BDMultiTool.Core;
using NUnit.Framework;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Clipboard = System.Windows.Clipboard;

namespace BDMultiToolTests
{
    [TestFixture]
    public class RegonizeEngineTests
    {
        private TestGraphicsFactory _graphicFactory;
        private ScreenHelper _screenHelper;

        [SetUp]
        public void SetUp()
        {
            _graphicFactory = new TestGraphicsFactory();

            _screenHelper = new ScreenHelper(_graphicFactory);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void SearchPressSpaceButton()
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\a.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = 760, Y = 164, Width = 155, Height = 65 };

            engine.WaitRectangleColor(r, Color.FromArgb(164, 136, 26), 80, WaitFishingStart_Callback, 5000, new RegonizeEngine.ContourAcceptance { Size = 150, SizeOffset = 30, Width = 100, WidthOffset = 50, Height = 50, HeightOffset = 20 });
        }

        [Test, Apartment(ApartmentState.STA)]
        public void CheckGoodStateOfFishingGauge()
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\a.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = 900, Y = 400, Width = 30, Height = 30 };
            var r2 = new Rect { X = 0, Y = 0, Width = 60, Height = 60 };
            var args = new RectEventArgs(r2);

            GetValue(args, r);

            var color = engine.GetColor(new System.Drawing.Point(930, 415));

            var c = new RegonizeEngine.ColorAcceptance {BaseColor = Color.FromArgb(93,142,172), Offset = 30};

            var zz = c.Validate(color);

            //RGB: 93/142/172
        }

        [Test, Apartment(ApartmentState.STA)]
        public void CheckBadStateOfFishingGauge()
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\a.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = 950, Y = 400, Width = 30, Height = 30 };
            var r2 = new Rect { X = 0, Y = 0, Width = 60, Height = 60 };
            var args = new RectEventArgs(r2);

            GetValue(args, r);

            var color = engine.GetColor(new System.Drawing.Point(950, 415));

            var c = new RegonizeEngine.ColorAcceptance { BaseColor = Color.FromArgb(93, 142, 172), Offset = 30 };

            var zz = c.Validate(color);

            //RGB: 93/142/172
        }

        [Test, Apartment(ApartmentState.STA)]
        public void SearchFishingGameTimeGauge()
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\b.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            //620-310 / 140-170
            var r = new Rect { X = 620, Y = 310, Width = 180, Height = 170 };

            engine.WaitRectangleColor(r, Color.FromArgb(255, 255, 255), 80, WaitGaugeBar_Callback, 5000, new RegonizeEngine.ContourAcceptance { Size = 10, SizeOffset = 5, Width = 70, WidthOffset = 30, Height = 4 , HeightOffset = 2});
        }

        [Test, Apartment(ApartmentState.STA)]
        public void SearchColorTriangleOfFishMiniGame()
        {
            //620-310
            //87-127
            var pointOfTimeGauge = new System.Drawing.Point(87 + 620, 127 + 310);

            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\b.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = pointOfTimeGauge.X - 34, Y = pointOfTimeGauge.Y - 43, Width = 15, Height = 15 };
            var r2 = new Rect { X = 0, Y = 0, Width = 60, Height = 60 };
            var args = new RectEventArgs(r2);

            

            GetValue(args, r);

            var color = engine.GetColor(new System.Drawing.Point(pointOfTimeGauge.X - 34, pointOfTimeGauge.Y - 43));

            //RGB:45/66/61
        }

        [Test, Apartment(ApartmentState.STA)]
        public void GetTrianglesOfFishingGame()
        {
            //620-310
            //87-127
            var pointOfTimeGauge = new System.Drawing.Point(87 + 620, 127 + 310);

            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\b.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = pointOfTimeGauge.X - 60, Y = pointOfTimeGauge.Y - 43, Width = 380, Height = 30 };
            var r2 = new Rect { X = 0, Y = 0, Width = 400, Height = 60 };
            var args = new RectEventArgs(r2);

            GetValue(args, r);

            engine.GetTriangles(r, Color.FromArgb(45, 66, 61), 10, WaitTriangles_Callback, 5000, new RegonizeEngine.ContourAcceptance { Size = 10, SizeOffset = 5, Width = 70, WidthOffset = 30, Height = 4, HeightOffset = 2 });

            //Clipboard.SetImage(ConvertBitmap(bmp));

            //var color = engine.GetColor(new System.Drawing.Point(pointOfTimeGauge.X - 34, pointOfTimeGauge.Y - 43));

            //620-310 / 140-170
            //var r = new Rect { X = 620, Y = 310, Width = 180, Height = 170 };

            
        }

        private void WaitTriangles_Callback(object sender, RectEventArgs args)
        {
            var r = new Rect { X = 760, Y = 164, Width = 155, Height = 65 };
            GetValue(args, r);
        }

        private void WaitFishingStart_Callback(object sender, RectEventArgs args)
        {
            var r = new Rect { X = 760, Y = 164, Width = 155, Height = 65 };
            GetValue(args, r);
        }

        private void WaitGaugeBar_Callback(object sender, RectEventArgs args)
        {
            var r = new Rect { X = 620, Y = 310, Width = 180, Height = 170 };
            GetValue(args, r);
        }

        private void GetValue(RectEventArgs args, Rect r)
        {
            var b = _screenHelper.ScreenArea(FromRect(r));

            var bmp = new Bitmap(Convert.ToInt32(args.Rect.Width), Convert.ToInt32(args.Rect.Height),
                PixelFormat.Format32bppArgb);

            using (var grD = Graphics.FromImage(bmp))
            {
                grD.DrawImage(b, new Rectangle(0, 0, Convert.ToInt32(args.Rect.Width), Convert.ToInt32(args.Rect.Height)),
                    new Rectangle(Convert.ToInt32(args.Rect.X), Convert.ToInt32(args.Rect.Y), Convert.ToInt32(args.Rect.Width),
                        Convert.ToInt32(args.Rect.Height)), GraphicsUnit.Pixel);
            }

            Clipboard.SetImage(ConvertBitmap(bmp));
        }

        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        private Rectangle FromRect(Rect rect)
        {
            return new Rectangle(Convert.ToInt32(rect.X), Convert.ToInt32(rect.Y), Convert.ToInt32(rect.Width), Convert.ToInt32(rect.Height));
        }
    }
}
