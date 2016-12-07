﻿using System;
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

            var r = new Rectangle { X = 760, Y = 164, Width = 155, Height = 65 };

            engine.WaitRectangleColor(r, Color.FromArgb(164, 136, 26), 80, WaitFishingStart_Callback, 5000, new RegonizeEngine.ContourAcceptance { Size = 150, SizeOffset = 30, Width = 100, WidthOffset = 50, Height = 50, HeightOffset = 20 });
        }

        [Test, Apartment(ApartmentState.STA)]
        public void CheckGoodStateOfFishingGauge()
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\a.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rectangle { X = 900, Y = 400, Width = 30, Height = 30 };
            var r2 = new Rectangle { X = 0, Y = 0, Width = 60, Height = 60 };
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

            var r = new Rectangle { X = 950, Y = 400, Width = 30, Height = 30 };
            var r2 = new Rectangle { X = 0, Y = 0, Width = 60, Height = 60 };
            var args = new RectEventArgs(r2);

            GetValue(args, r);

            var color = engine.GetColor(new System.Drawing.Point(950, 415));

            var c = new RegonizeEngine.ColorAcceptance { BaseColor = Color.FromArgb(93, 142, 172), Offset = 30 };

            var zz = c.Validate(color);

            //RGB: 93/142/172
        }

        [Apartment(ApartmentState.STA)]
        [TestCase("b.jpg", 87, 127)]
        [TestCase("c.jpg", 79, 110)]
        [TestCase("d.jpg", 79, 110)]
        public void SearchFishingGameTimeGauge(string imageName, int x, int y)
        {
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"..\..\ImageTest\{imageName}"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rectangle { X = 620, Y = 310, Width = 180, Height = 170 };

            var result = engine.GetRectangle(r, Color.FromArgb(255, 255, 255), 80,
                new RegonizeEngine.ContourAcceptance
                {
                    Size = 10,
                    SizeOffset = 5,
                    Width = 70,
                    WidthOffset = 30,
                    Height = 4,
                    HeightOffset = 2
                });

            Assert.AreEqual(x, result.X);
            Assert.AreEqual(y, result.Y);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void SearchColorTriangleOfFishMiniGame()
        {
            //620-310
            //87-127
            var pointOfTimeGauge = new System.Drawing.Point(87 + 620, 127 + 310);

            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\b.jpg"));
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rectangle { X = pointOfTimeGauge.X - 35 + 8, Y = pointOfTimeGauge.Y - 45 + 8, Width = 12, Height = 12 };
            var r2 = new Rectangle { X = 0, Y = 0, Width = 60, Height = 60 };
            var args = new RectEventArgs(r2);

            GetValue(args, r);

            var color = engine.GetColor(new System.Drawing.Point(pointOfTimeGauge.X - 35 + 8, pointOfTimeGauge.Y - 45 + 8));

            Assert.IsTrue(ArroundColor(Color.FromArgb(0, 85, 255), color, 10));

            //RGB:45/66/61
        }

        public static bool ArroundColor(Color refColor, Color checkColor, int threshold)
        {
            if (checkColor.R > refColor.R + threshold || checkColor.R < refColor.R - 10)
                return false;

            if (checkColor.G > refColor.G + threshold || checkColor.G < refColor.G - 10)
                return false;

            if (checkColor.B > refColor.B + threshold || checkColor.B < refColor.B - 10)
                return false;

            return true;
        }

        [Apartment(ApartmentState.STA)]
        //78 - 113 - 148 - 183 - 218
        [TestCase(323, RegonizeEngine.FishTriangle.Orientation.None)]
        [TestCase(288, RegonizeEngine.FishTriangle.Orientation.None)]
        [TestCase(253, RegonizeEngine.FishTriangle.Orientation.Down)]
        [TestCase(218, RegonizeEngine.FishTriangle.Orientation.Left)]
        [TestCase(183, RegonizeEngine.FishTriangle.Orientation.Left)]
        [TestCase(148, RegonizeEngine.FishTriangle.Orientation.Left)]
        [TestCase(113, RegonizeEngine.FishTriangle.Orientation.Down)]
        [TestCase(78, RegonizeEngine.FishTriangle.Orientation.Left)]
        [TestCase(43, RegonizeEngine.FishTriangle.Orientation.Down)]
        [TestCase(8, RegonizeEngine.FishTriangle.Orientation.Down)]
        public void GetTrianglesOfFishingGame(int xCoord, RegonizeEngine.FishTriangle.Orientation orientation)
        {
            //620-310
            //87-127
            var pointOfTimeGauge = new System.Drawing.Point(87 + 620, 127 + 310);

            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\b.jpg"));

            var r = new Rect { X = pointOfTimeGauge.X - 35, Y = pointOfTimeGauge.Y - 45, Width = 380, Height = 17 };

            var filteredImage = RegonizeEngine.FilterCaptcha(new Image<Bgr, byte>(_screenHelper.ScreenArea(FromRect(r))),
                new RegonizeEngine.FilterParam(Color.FromArgb(0, 85, 255), 100));

            var t = new RegonizeEngine.FishTriangle(filteredImage,new System.Drawing.Point(xCoord,8));

            Clipboard.SetImage(ConvertBitmap(t.GetBitmap()));

            Assert.AreEqual(orientation, t.GetOrientation());
        }

        private void WaitTriangles_Callback(object sender, RectEventArgs args)
        {
            var r = new Rectangle { X = 760, Y = 164, Width = 155, Height = 65 };
            GetValue(args, r);
        }

        private void WaitFishingStart_Callback(object sender, RectEventArgs args)
        {
            var r = new Rectangle { X = 760, Y = 164, Width = 155, Height = 65 };
            GetValue(args, r);
        }



        private void GetValue(RectEventArgs args, Rectangle r)
        {
            var b = _screenHelper.ScreenArea(r);

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
