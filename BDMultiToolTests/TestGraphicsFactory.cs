using System.Drawing;
using System.Drawing.Imaging;
using BDMultiTool;

namespace BDMultiToolTests
{
    public class TestGraphicsFactory : IGraphicFactory
    {
        private Image _image;

        public void LoadImage(string imagePath)
        {
            _image = Image.FromFile(imagePath);
        }

        public Bitmap CopyFromScreen(int sourceX, int sourceY, Size blockRegionSize, CopyPixelOperation copyPixelOperation,
            PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(blockRegionSize.Width, blockRegionSize.Height, pixelFormat);

            using (var grD = Graphics.FromImage(bmp))
            {
                grD.DrawImage(_image, new Rectangle(0,0,blockRegionSize.Width,blockRegionSize.Height), new Rectangle(sourceX,sourceY,blockRegionSize.Width,blockRegionSize.Height), GraphicsUnit.Pixel);
            }

            return bmp;
        }
    }
}