using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using BDMultiTool;
using BDMultiTool.Core;
using NUnit.Framework;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
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
            _graphicFactory.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\ImageTest\a.jpg"));

            _screenHelper = new ScreenHelper(_graphicFactory);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void Tests()
        {
            var engine = new RegonizeEngine(_screenHelper);

            var r = new Rect { X = 760, Y = 164, Width = 155, Height = 65 };

            engine.WaitRectangleColor(r, Color.FromArgb(164, 136, 26), 80, WaitFishingStart_Callback,5000);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void Test2()
        {
            var engine = new RegonizeEngine(_screenHelper);
            var color = engine.GetColor(new System.Drawing.Point(10, 10));
        }

        private void WaitFishingStart_Callback(object sender, RectEventArgs args)
        {
            var r = new Rect { X = 760, Y = 164, Width = 155, Height = 65 };
            var b = _screenHelper.ScreenArea(FromRect(r));

            var bmp = new Bitmap(Convert.ToInt32(args.Rect.Width), Convert.ToInt32(args.Rect.Height), PixelFormat.Format32bppArgb);

            using (var grD = Graphics.FromImage(bmp))
            {
                grD.DrawImage(b, new Rectangle(0, 0, Convert.ToInt32(args.Rect.Width), Convert.ToInt32(args.Rect.Height)), new Rectangle(Convert.ToInt32(args.Rect.X), Convert.ToInt32(args.Rect.Y), Convert.ToInt32(args.Rect.Width), Convert.ToInt32(args.Rect.Height)), GraphicsUnit.Pixel);
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
