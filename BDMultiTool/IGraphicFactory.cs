using System.Drawing;
using System.Drawing.Imaging;

namespace BDMultiTool
{
    public interface IGraphicFactory
    {
        Bitmap CopyFromScreen(int sourceX, int sourceY, Size blockRegionSize, CopyPixelOperation copyPixelOperation, PixelFormat pixelFormat);
    }
}