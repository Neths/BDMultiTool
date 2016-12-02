using System.Drawing;
using System.Drawing.Imaging;

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
    }
}
