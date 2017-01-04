using System;
using System.IO;
using BDMultiTool.Config;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BDMultiToolTests
{
    [TestFixture]
    public class ScreenConfigTests
    {
        [Test]
        public void test()
        {
            var config = new ScreenConfig
            {
                Market = new MarketConfig
                {
                    Refresh = new ClicPosition { X = 0,Y = 0},
                    MarketRow = new ClicPosition { X = 0, Y = 0 },
                    QuantityWindowBuy = new ClicPosition { X = 0, Y = 0 },

                    CheckButtonAvailable = new BasicAreaConfig
                    {
                        Color = new ColorConfig { R = 0, G = 0, B = 0, Seuil = 0},
                        Area = new CaptureAreaConfig { X = 0, Y = 0, Width = 0, Height = 0},
                        ContourAcceptance = new ContourAcceptanceConfig
                        {
                            Height = 0, HeightOffset = 0,
                            Width = 0, WidthOffset = 0,
                            Size = 0, SizeOffset = 0
                        }
                    },
                    CheckSubWindow = new BasicAreaConfig
                    {
                        Color = new ColorConfig { R = 0, G = 0, B = 0, Seuil = 0 },
                        Area = new CaptureAreaConfig { X = 0, Y = 0, Width = 0, Height = 0 },
                        ContourAcceptance = new ContourAcceptanceConfig
                        {
                            Height = 0,
                            HeightOffset = 0,
                            Width = 0,
                            WidthOffset = 0,
                            Size = 0,
                            SizeOffset = 0
                        }
                    },
                    MarketRowAvailable = new BasicAreaConfig
                    {
                        Color = new ColorConfig { R = 0, G = 0, B = 0, Seuil = 0 },
                        Area = new CaptureAreaConfig { X = 0, Y = 0, Width = 0, Height = 0 },
                        ContourAcceptance = new ContourAcceptanceConfig
                        {
                            Height = 0,
                            HeightOffset = 0,
                            Width = 0,
                            WidthOffset = 0,
                            Size = 0,
                            SizeOffset = 0
                        }
                    }
                }
            };

            var w = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test.config"));
            w.Write(JsonConvert.SerializeObject(config));
            w.Close();
        }
    }
}
