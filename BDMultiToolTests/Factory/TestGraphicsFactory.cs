using System.Drawing;
using System.Drawing.Imaging;
using BDMultiTool;

namespace BDMultiToolTests.Factory
{
    public class TestGraphicsFactory : IGraphicFactory
    {
        private Image _currentImage;

        public void LoadImage(string imagePath)
        {
            _currentImage = Image.FromFile(imagePath);
        }

        public Bitmap CopyFromScreen(int sourceX, int sourceY, Size blockRegionSize, CopyPixelOperation copyPixelOperation,
            PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(_currentImage, 0, 0, new Rectangle(sourceX, sourceY, blockRegionSize.Width,blockRegionSize.Height), GraphicsUnit.Pixel);

            return bmp;
        }
    }
}