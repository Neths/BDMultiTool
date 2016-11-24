using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BDMultiTool.Engines
{
    public class FishingEngine : IEngine
    {
        private readonly IRegonizeArea _regonizeArea;
        private readonly IInputSender _inputSender;
        private bool _running = false;
        private Thread _thread;

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
            var startFishingArea = new Rect { X = 10, Y = 10, Width = 80, Height = 40 };

            _regonizeArea.WaitRectangleColor(startFishingArea, Color.Gold, 20, WaitFishingStart_Callback, 5000);
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
            var fishingGauge = new Rect { X = 10, Y = 10, Width = 80, Height = 40 };

            _regonizeArea.WaitRectangleColor(fishingGauge, Color.CornflowerBlue, 20, WaitFishingGaugeInBlueArea_Callback, 100);
        }

        private void WaitFishingGaugeInBlueArea_Callback(object sender, RectEventArgs args)
        {
            Debug.Write("fish gauge in blue area ==> press space");
            _inputSender.SendKeys(Keys.Space);
            WaitDisplayFishingMinigame();
        }

        private void WaitDisplayFishingMinigame()
        {
            //Wait fish mini game dispay with with characters on top
            var fishingMiniGameArea = new Rect { X = 10, Y = 10, Width = 80, Height = 40 };

            _regonizeArea.WaitRectangleColor(fishingMiniGameArea, Color.White, 20, WaitDisplayFishingMinigame_Callback, 100);
        }

        private void WaitDisplayFishingMinigame_Callback(object sender, RectEventArgs args)
        {
            Debug.Write("Fishing mini game displayed ==> resolve game");
            SolveFishingMiniGame(args.Rect);
        }

        private void SolveFishingMiniGame(Rect refRect)
        {
            //Resolve Mini game
            var fishingMiniGameArea = new Rect
            {
                X = refRect.X + 10,
                Y = refRect.Y + 10,
                Width = refRect.Width,
                Height = refRect.Height
            };

            ResolveMiniGame(fishingMiniGameArea, SolveFishingMiniGame_Callback);
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


        private void ResolveMiniGame(Rect fishingMiniGameArea, EventHandler<KeysEventArgs> fishingStep4Callback)
        {
            throw new NotImplementedException();
        }

        private void Looting(bool onlyRelic, EventHandler<EventArgs> waitLootingCallback)
        {
            throw new NotImplementedException();
        }

        private void SwitchRod(EventHandler<EventArgs> switchFishingRodCallback)
        {
            throw new NotImplementedException();
        }

        private bool HaveUsefullFishingRod()
        {
            throw new NotImplementedException();
        }
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
