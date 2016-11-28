using System.Drawing;
using System.Drawing.Imaging;

namespace BDMultiTool
{
    public class ScreenHelper : IScreenHelper
    {
        private readonly IGraphicFactory _graphicFactory;

        public ScreenHelper(IGraphicFactory graphicFactory)
        {
            _graphicFactory = graphicFactory;
        }

        public Bitmap ScreenArea(Rectangle rect)
        {
            var bmp = _graphicFactory.CopyFromScreen(rect.Left, rect.Top, new Size(rect.Width, rect.Width), CopyPixelOperation.SourceCopy, PixelFormat.Format32bppArgb);
            
            return bmp;
        }
    }

    public interface IScreenHelper
    {
        Bitmap ScreenArea(Rectangle rect);
    }

    public interface IGraphicFactory
    {
        Bitmap CopyFromScreen(int sourceX, int sourceY, Size blockRegionSize, CopyPixelOperation copyPixelOperation, PixelFormat pixelFormat);
    }
}
