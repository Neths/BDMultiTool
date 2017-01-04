using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using BDMultiTool.Core;
using Emgu.CV;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;

namespace BDMultiTool.Engines
{
    public class FishingEngine : IEngine
    {
        private readonly IRegonizeArea _regonizeArea;
        private readonly IInputSender _inputSender;
        private readonly IScreenHelper _screenHelper;
        private bool _running = false;
        private Thread _thread;

        private readonly Rect _fishingMiniGameArea = new Rect();

        private readonly Image _relicImage = null;

        private readonly List<Keys> _shortcutList = new List<Keys> { Keys.Oem2, Keys.Oem3 };

        private readonly  List<int> _xCoordOfFishingKeyIndicators = new List<int> { 8, 43, 78, 113, 148, 183, 218, 253, 288, 323 };

        public FishingEngine(IRegonizeArea regonizeArea, IInputSender inputSender, IScreenHelper screenHelper)
        {
            _regonizeArea = regonizeArea;
            _inputSender = inputSender;
            _screenHelper = screenHelper;
        }

        public void Start()
        {
            _running = true;
            _thread = new Thread(Execute);
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _thread.Abort();
        }

        public bool Running => _running;

        private void Execute()
        {
            while (_running)
            {
                CheckFishingRod();
                Thread.Sleep(50);
            }
        }

        private void CheckFishingRod()
        {
            Debug.Write("Check Fishing Rod");
            if (HaveUsefullFishingRod())
            {
                Debug.Write("Have usefull fishing rod => start fishing");
                StartFishing();
            }
            else
            {
                Debug.Write("Have usefless fishing rod => switch fishing");
                SwitchFishingRod();
            }
        }

        private void SwitchFishingRod()
        {
            Debug.Write("Switch Fishing Rod");
            SwitchRod(SwitchFishingRod_Callback);
        }

        private void SwitchFishingRod_Callback(object sender, EventArgs args)
        {
            Debug.Write("Fishing Rod switched ==> Start Fishing");
            StartFishing();
        }

        private void StartFishing()
        {
            Debug.Write("Start Fishing");
            Thread.Sleep(2000);
            _inputSender.SendKeys(Keys.Space);

            Thread.Sleep(5000);

            WaitFishingStart();
        }

        private void WaitFishingStart()
        {
            var startFishingArea = new Rectangle { X = 800, Y = 130, Width = 400, Height = 300 };

            _regonizeArea.WaitRectangleColor(startFishingArea, Color.FromArgb(140,110,10), 80, WaitFishingStart_Callback, 5000, new RegonizeEngine.ContourAcceptance { Size = 150, SizeOffset = 30, Width = 100, WidthOffset = 50, Height = 50, HeightOffset = 20 });
        }

        private void WaitFishingStart_Callback(object sender, RectEventArgs args)
        {
            Debug.Write("Yellow rectangle found ==> Fishing Start");
            _inputSender.SendKeys(Keys.Space);
            WaitFishingGaugeInBlueArea();
        }

        private void WaitFishingGaugeInBlueArea()
        {
            //Search when fishing gauge are in blue area
            //var fishingGauge = new Rect { X = 930, Y = 410, Width = 5, Height = 10 };
            var refColor = new RegonizeEngine.ColorAcceptance { BaseColor = Color.FromArgb(93, 142, 172), Offset = 30 };
            var gaugePoint = new Point(1060, 415);

            while (true)
            {
                var color = _regonizeArea.GetColor(gaugePoint);
                if (refColor.Validate(color))
                {
                    WaitFishingGaugeInBlueArea_Callback(this,EventArgs.Empty);
                    break;
                }

                Thread.Sleep(10);
            }
        }

        private void WaitFishingGaugeInBlueArea_Callback(object sender, EventArgs args)
        {
            DebugWindow.SetPointFishingGauge(true);
            Debug.Write("fish gauge in blue area ==> press space");
            _inputSender.SendKeys(Keys.Space);
            WaitDisplayFishingMinigame();
        }

        private void WaitDisplayFishingMinigame()
        {
            Thread.Sleep(2000);
            Debug.Write("Fishing mini game displayed ==> resolve game");
            SolveFishingMiniGame();
        }

        private void SolveFishingMiniGame()
        {
            //Resolve Mini game
            SolveMiniGame(SolveFishingMiniGame_Callback);
        }

        private void SolveFishingMiniGame_Callback(object sender, KeysEventArgs args)
        {
            Debug.Write("Fishing mini game resolved ==> send keys");
            _inputSender.SendKeys(args.Keys);
            WaitLooting();
        }

        private void WaitLooting()
        {
            Debug.Write("Fishing mini game finished ==> wait for looting");
            Thread.Sleep(5000);

            Debug.Write("Looting");

            Looting(WaitLooting_Callback);
        }

        private void WaitLooting_Callback(object sender, EventArgs args)
        {
            Debug.Write("Fishing mini game resolved ==> send keys");

            CheckFishingRod();
        }


        private void SolveMiniGame(EventHandler<KeysEventArgs> fishingStep4Callback)
        {
            var gameArea = new Rectangle { X = 620, Y = 310, Width = 180, Height = 170 };
            var img = _screenHelper.ScreenArea(gameArea);

            DebugWindow.SetImageFishingGameRaw(img);

            var startPointOfTimeGauge = FindTimeGauge(img);

            var colorFilter = GetColorNoise(img, startPointOfTimeGauge);

            var filteredImage = RegonizeEngine.FilterImage(new Image<Bgr, byte>(img),
                new RegonizeEngine.FilterParam(colorFilter, 100));

            DebugWindow.SetImageFishingGameFiltered(filteredImage.Bitmap);

            var keys = _xCoordOfFishingKeyIndicators
                .Select(x => new RegonizeEngine.FishTriangle(filteredImage, new Point(x, 8)))
                .Select(t => t.GetOrientation())
                .Where(o => o != RegonizeEngine.FishTriangle.Orientation.None)
                .Select(GetKeyFromOrientedTriangle)
                .ToList();

            fishingStep4Callback(this,new KeysEventArgs(keys));
        }

        private Point FindTimeGauge(Bitmap img)
        {
            //620-310 / 140-170
            var r = new Rectangle { X = 620, Y = 310, Width = 180, Height = 170 };

            var timeGaugeArea = _regonizeArea.GetRectangle(img, r, Color.FromArgb(255, 255, 255), 80,
                new RegonizeEngine.ContourAcceptance
                {
                    Size = 10,
                    SizeOffset = 5,
                    Width = 70,
                    WidthOffset = 30,
                    Height = 4,
                    HeightOffset = 2
                });

            return new Point(timeGaugeArea.X,timeGaugeArea.Y);
        }

        private static Keys GetKeyFromOrientedTriangle(RegonizeEngine.FishTriangle.Orientation orientation)
        {
            switch (orientation)
            {
                case RegonizeEngine.FishTriangle.Orientation.Up:
                    return Keys.Z;
                case RegonizeEngine.FishTriangle.Orientation.Down:
                    return Keys.S;
                case RegonizeEngine.FishTriangle.Orientation.Left:
                    return Keys.Q;
                case RegonizeEngine.FishTriangle.Orientation.Right:
                default:
                    return Keys.D;
            }
        }

        private Color GetColorNoise(Bitmap img, Point pointOfTimeGauge)
        {
            return _regonizeArea.GetColor(img, new Point(pointOfTimeGauge.X - 35 + 8, pointOfTimeGauge.Y - 45 + 8));
        }

        private void Looting(EventHandler<EventArgs> waitLootingCallback)
        {
            //if (!onlyRelic)
            //{
            //    Debug.Write("All Looting");
            //    _inputSender.SendKeys(Keys.R);
                
            //}
            //else
            //{
            //    var areas = _regonizeArea.GetAreasForImage(_relicImage);
            //    foreach (var area in areas)
            //    {
            //        _inputSender.MouseRightClickTo(new Point(Convert.ToInt32(area.X + area.Width % 2), Convert.ToInt32(area.Y + area.Height % 2)));
            //        Thread.Sleep(1000);
            //    }

            //    _inputSender.MouseLeftClickTo(_cancelLootButtonPosition);
            //    Thread.Sleep(1000);
            //}

            Thread.Sleep(2000);

            waitLootingCallback(this, new EventArgs());
        }

        private void SwitchRod(EventHandler<EventArgs> switchFishingRodCallback)
        {
            foreach (var key in _shortcutList)
            {
                Debug.Write($"Switch to fishing rod in {key} shortcut");
                _inputSender.SendKeys(key);
                Thread.Sleep(2000);

                if (HaveUsefullFishingRod())
                {
                    switchFishingRodCallback(this, EventArgs.Empty);
                    return;
                }
            }

            Debug.Write("haven't fishing rod with durability ==> exit fishing macro");
            _running = false;
        }

        private bool HaveUsefullFishingRod()
        {
            return MainWeaponDurabilityShortcut();
        }

        private bool MainWeaponDurabilityShortcut()
        {
            var mainWeaponDurabilityShortcutArea = new Point
            {
                X = 1290,
                Y = 138
            };

            return _regonizeArea.GetColor(mainWeaponDurabilityShortcutArea) != Color.FromArgb(147,6,15);
        }
    }

    public class OrientedTriangle
    {
    }

    public class KeysEventArgs : EventArgs
    {
        public IEnumerable<Keys> Keys { get; set; }

        public KeysEventArgs(IEnumerable<Keys> keys)
        {
            Keys = keys;
        }
    }
}
