using System;
using System.Drawing;
using System.Drawing.Imaging;
using BDMultiTool.Core.PInvoke;

namespace BDMultiTool.Core.Factory
{
    public class GraphicsFactory : IGraphicFactory
    {
        public Bitmap CopyFromScreen(int sourceX, int sourceY, Size blockRegionSize,
            CopyPixelOperation copyPixelOperation, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(blockRegionSize.Width, blockRegionSize.Height, pixelFormat);

            Graphics.FromImage(bmp).CopyFromScreen(sourceX, sourceY, 0, 0, new Size(blockRegionSize.Width, blockRegionSize.Width), copyPixelOperation);

            return bmp;
        }

        public static Bitmap CopyFromScreen(IntPtr bdoHandle)
        {
            User32.SIZE size;
            var hDc = User32.GetDC(bdoHandle);
            var hMemDc = GDI32.CreateCompatibleDC(hDc);

            size.cx = User32.GetSystemMetrics(User32.SM_CXSCREEN);
            size.cy = User32.GetSystemMetrics(User32.SM_CYSCREEN);

            var hBitmap = GDI32.CreateCompatibleBitmap(hDc, size.cx, size.cy);

            if (hBitmap == IntPtr.Zero)
                return null;

            var hOld = GDI32.SelectObject(hMemDc, hBitmap);

            GDI32.BitBlt(hMemDc, 0, 0, size.cx, size.cy, hDc, 0, 0, GDI32.SRCCOPY);
            GDI32.SelectObject(hMemDc, hOld);
            GDI32.DeleteDC(hMemDc);
            User32.ReleaseDC(bdoHandle, hDc);
            var bmp = Image.FromHbitmap(hBitmap);

            GDI32.DeleteObject(hBitmap);
            GC.Collect();
            return bmp;
        }
    }
}
