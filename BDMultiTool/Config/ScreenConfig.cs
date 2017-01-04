using System;
using System.Drawing;
using System.IO;
using BDMultiTool.Core;
using Newtonsoft.Json;

namespace BDMultiTool.Config
{
    public class ScreenConfig
    {
        public MarketConfig Market { get; set; }

        public static ScreenConfig LoadFromFile(string filename)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", $"{filename}.json");

            return JsonConvert.DeserializeObject<ScreenConfig>(new StreamReader(path).ReadToEnd());
        }
    }

    public class MarketConfig
    {
        public BasicAreaConfig CheckSubWindow { get; set; }
        public BasicAreaConfig CheckButtonAvailable { get; set; }
        public BasicAreaConfig MarketRowAvailable { get; set; }

        public ClicPosition Refresh { get; set; }
        public ClicPosition QuantityWindowBuy { get; set; }
        public ClicPosition MarketRow { get; set; }
    }

    public class ClicPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point ToPoint()
        {
            return new Point
            {
                X = X,
                Y = Y
            };
        }
    }

    public class BasicAreaConfig
    {
        public CaptureAreaConfig Area { get; set; }
        public ColorConfig Color { get; set; }
        public ContourAcceptanceConfig ContourAcceptance { get; set; }
    }

    public class ContourAcceptanceConfig
    {
        public int Height { get; set; }
        public int HeightOffset { get; set; }
        public int Width { get; set; }
        public int WidthOffset { get; set; }
        public int Size { get; set; }
        public int SizeOffset { get; set; }

        public RegonizeEngine.ContourAcceptance ToContourAcceptance()
        {
            return new RegonizeEngine.ContourAcceptance()
            {
                Size = Size,
                Width = Width,
                Height = Height,
                SizeOffset = SizeOffset,
                HeightOffset = HeightOffset,
                WidthOffset = WidthOffset
            };
        }
    }

    public class ColorConfig
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int Seuil { get; set; }

        public Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }

    public class CaptureAreaConfig
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle ToRectange()
        {
            return new Rectangle
            {
                X = X,
                Y = Y,
                Height = Height,
                Width = Width
            };
        }
    }
}
