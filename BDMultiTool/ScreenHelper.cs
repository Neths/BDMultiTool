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
            var bmp = _graphicFactory.CopyFromScreen(rect.Left, rect.Top, new Size(rect.Width, rect.Height), CopyPixelOperation.SourceCopy, PixelFormat.Format32bppArgb);
            
            return bmp;
        }
    }
}
