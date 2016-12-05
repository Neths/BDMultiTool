﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using BDMultiTool.Core;
using Point = System.Drawing.Point;

namespace BDMultiTool.Engines
{
    public class FishingEngine : IEngine
    {
        private readonly IRegonizeArea _regonizeArea;
        private readonly IInputSender _inputSender;
        private bool _running = false;
        private Thread _thread;

        //TODO
        private readonly Dictionary<int, Point> _shortcutsArea = new Dictionary<int, Point>
        {
            { 1, new Point { X = 10, Y = 10 } },
            { 2, new Point { X = 10, Y = 10 } },
            { 3, new Point { X = 10, Y = 10 } },
            { 4, new Point { X = 10, Y = 10 } },
            { 5, new Point { X = 10, Y = 10 } }
        };

        private readonly Rect _fishingMiniGameArea = new Rect();

        private readonly Point _cancelLootButtonPosition = new Point(10,10);

        private readonly Image _relicImage = null;

        public FishingEngine(IRegonizeArea regonizeArea, IInputSender inputSender)
        {
            _regonizeArea = regonizeArea;
            _inputSender = inputSender;
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

            WaitFishingStart();
        }

        private void WaitFishingStart()
        {
            //Search Space Rectangle for starting fishing cycle
            var startFishingArea = new Rect { X = 760, Y = 164, Width = 153, Height = 65 };
            //new Rectangle { X = 760, Y = 164, Width = 155, Height = 65 };

            //RGB : 164/136/26
            _regonizeArea.WaitRectangleColor(startFishingArea, Color.FromArgb(164,136,26), 80, WaitFishingStart_Callback, 5000);
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
            var fishingGauge = new Rect { X = 930, Y = 410, Width = 5, Height = 10 };

            //RGB: 93/142/172
            _regonizeArea.WaitRectangleColor(fishingGauge, Color.FromArgb(93,142,172), 20, WaitFishingGaugeInBlueArea_Callback, 100);
        }

        private void WaitFishingGaugeInBlueArea_Callback(object sender, RectEventArgs args)
        {
            Debug.Write("fish gauge in blue area ==> press space");
            _inputSender.SendKeys(Keys.Space);
            //WaitDisplayFishingMinigame();
        }

        //TODO
        private void WaitDisplayFishingMinigame()
        {
            //Wait fish mini game dispay with with characters on top
            var fishingMiniGameArea = new Rect { X = 10, Y = 10, Width = 80, Height = 40 };

            _regonizeArea.WaitRectangleColor(fishingMiniGameArea, Color.White, 20, WaitDisplayFishingMinigame_Callback, 100);
        }

        private void WaitDisplayFishingMinigame_Callback(object sender, RectEventArgs args)
        {
            Debug.Write("Fishing mini game displayed ==> resolve game");
            SolveFishingMiniGame();
        }

        //TODO
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

            Looting(true, WaitLooting_Callback);
        }

        private void WaitLooting_Callback(object sender, EventArgs args)
        {
            Debug.Write("Fishing mini game resolved ==> send keys");

            CheckFishingRod();
        }


        private void SolveMiniGame(EventHandler<KeysEventArgs> fishingStep4Callback)
        {
            var startPointOfTimeGauge = FindTimeGauge(_fishingMiniGameArea);

            var triangleAreas = GetTriangleArea(startPointOfTimeGauge).ToList();

            var colorFilter = GetColorNoise(triangleAreas.First());

            var orientedTriangles = CalculateOrientation(triangleAreas, colorFilter);

            var keys = orientedTriangles.Select(GetKeyFromOrientedTriangle).ToList();

            fishingStep4Callback(this,new KeysEventArgs(keys));
        }

        //TODO
        private IEnumerable<Rect> GetTriangleArea(Point startPointOfTimeGauge)
        {
            throw new NotImplementedException();
        }

        //TODO
        private Point FindTimeGauge(Rect fishingMiniGameArea)
        {
            throw new NotImplementedException();
        }

        //TODO
        private Keys GetKeyFromOrientedTriangle(OrientedTriangle orientedTriangle)
        {
            throw new NotImplementedException();
        }

        //TODO
        private IEnumerable<OrientedTriangle> CalculateOrientation(IEnumerable<Rect> triangleAreas , Color colorFilter)
        {
            throw new NotImplementedException();
        }

        //TODO
        private Color GetColorNoise(Rect triangleArea)
        {
            throw new NotImplementedException();
        }

        private void Looting(bool onlyRelic, EventHandler<EventArgs> waitLootingCallback)
        {
            if (!onlyRelic)
            {
                Debug.Write("All Looting");
                _inputSender.SendKeys(Keys.R);
                Thread.Sleep(2000);
            }
            else
            {
                var areas = _regonizeArea.GetAreasForImage(_relicImage);
                foreach (var area in areas)
                {
                    _inputSender.MouseRightClickTo(new Point(Convert.ToInt32(area.X + area.Width % 2), Convert.ToInt32(area.Y + area.Height % 2)));
                    Thread.Sleep(1000);
                }

                _inputSender.MouseLeftClickTo(_cancelLootButtonPosition);
                Thread.Sleep(1000);
            }

            waitLootingCallback(this, new EventArgs());
        }

        private void SwitchRod(EventHandler<EventArgs> switchFishingRodCallback)
        {
            for (var i = 0; i < 5; i++)
            {
                if (CheckDurabilityShortcut(i))
                {
                    Debug.Write($"Switch to fishing rod in {i} shortcut");
                    _inputSender.SendKeys(GetShortcutKey(i));
                }
            }

            Debug.Write("haven't fishing rod with durability ==> exit fishing macro");
            _running = false;
        }

        private bool CheckDurabilityShortcut(int i)
        {
            return _regonizeArea.GetColor(_shortcutsArea[i]) == Color.Red;
        }

        private Keys GetShortcutKey(int i)
        {
            switch (i)
            {
                case 1:
                    return Keys.Oem1;
                case 2:
                    return Keys.Oem2;
                case 3:
                    return Keys.Oem3;
                case 4:
                    return Keys.Oem4;
                case 5:
                    return Keys.Oem5;
            }

            return Keys.None;
        }

        private bool HaveUsefullFishingRod()
        {
            return MainWeaponDurabilityShortcut();
        }

        //TODO
        private bool MainWeaponDurabilityShortcut()
        {
            var mainWeaponDurabilityShortcutArea = new Point
            {
                X = 1290,
                Y = 138
            };

            return _regonizeArea.GetColor(mainWeaponDurabilityShortcutArea) == Color.Red;
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
