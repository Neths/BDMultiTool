using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BDMultiTool.Core;
using BDMultiTool.Core.PInvoke;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace BDMultiTool.Engines
{
    public class MarketEngine : IEngine
    {
        private bool _running;
        private Thread _thread;

        private readonly IRegonizeArea _regonizeArea;
        private readonly IInputSender _inputSender;
        private readonly IScreenHelper _screenHelper;
        private readonly IWindowAttacher _windowAttacher;
        private Image<Bgr, byte> _imgBuy;
        private Image<Bgr, byte> _imgInfo;

        public MarketEngine(IRegonizeArea regonizeArea, IInputSender inputSender, IScreenHelper screenHelper, IWindowAttacher windowAttacher)
        {
            _regonizeArea = regonizeArea;
            _inputSender = inputSender;
            _screenHelper = screenHelper;
            _windowAttacher = windowAttacher;

            var patternsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Patterns");
            _imgInfo = new Image<Bgr, byte>(Path.Combine(patternsPath, "infoHeader.png"));
            _imgBuy = new Image<Bgr, byte>(Path.Combine(patternsPath, "buyHeader.png"));
        }

        public void Start()
        {
            User32.MoveWindow(_windowAttacher.WindowHandle, 0, 0, _windowAttacher.Size.Width, _windowAttacher.Size.Height, true);

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
            //var bidInProgress = false;
            //var i = 0;

            while (_running)
            {
                int refreshTime;
                ClickRefresh();

                if (MarketRowAvailable())
                {
                    //bidInProgress = true;
                    //i++;
                    refreshTime = 50;
                    TryBuy();
                }
                else
                {
                    //bidInProgress = false;
                    refreshTime = 500;
                }

                //if (!bidInProgress || i >= 10)
                //{
                //    ClickRefresh();
                //    i = 0;
                //}

                Thread.Sleep(refreshTime);
            }
        }

        private void TryBuy()
        {
            var config = _windowAttacher.Config.Market;
            var position = Enumerable.Range(1, 7).FirstOrDefault(CheckButtonAvailable);

            if (position == default(int))
                return;

            _inputSender.MouseLeftClickTo(new Point { X = config.MarketRow.X, Y = position * 62 + config.MarketRow.Y });

            Thread.Sleep(400);

            var windowType = CheckSubWindowType2();

            if (windowType == SubWindow.InfoWindow)
            {
                _inputSender.SendKeys(Keys.Enter);
                _inputSender.MouseMoveTo(new Point { X = config.MarketRow.X + 100, Y = position * 62 + config.MarketRow.Y });
                Thread.Sleep(300);
            }
            else
            {
                _inputSender.MouseLeftClickTo(config.QuantityWindowBuy.ToPoint());
                Thread.Sleep(300);
            }
        }

        private void ClickRefresh()
        {
            _inputSender.MouseLeftClickTo(_windowAttacher.Config.Market.Refresh.ToPoint());
        }

        private bool MarketRowAvailable()
        {
            return _regonizeArea
                    .HaveRectangle(_screenHelper.ScreenArea(_windowAttacher.Config.Market.MarketRowAvailable.Area.ToRectange()),
                                   _windowAttacher.Config.Market.MarketRowAvailable);
        }

        private bool CheckButtonAvailable(int slot)
        {
            var config = _windowAttacher.Config.Market.CheckButtonAvailable;

            var r = config.Area.ToRectange();
            r.Y = slot * 62 + r.Y;
            var color = config.Color.ToColor();
            var seuil = config.Color.Seuil;
            var acceptance = config.ContourAcceptance.ToContourAcceptance();

            return _regonizeArea.HaveRectangle(_screenHelper.ScreenArea(r), r, color, seuil, acceptance);
        }

        private enum SubWindow
        {
            InfoWindow,
            QuantityBuyWindow,
            UnknownWindow
        }

        private SubWindow CheckSubWindowType2()
        {
            var img = new Image<Bgr,byte>(_screenHelper.ScreenArea(_windowAttacher.Config.Market.CheckSubWindow.Area.ToRectange()));

            var result = _regonizeArea.MatchPattern(img, _imgInfo);
            if (result != Rectangle.Empty)
                return SubWindow.InfoWindow;

            result =_regonizeArea.MatchPattern(img, _imgBuy);
            if (result != Rectangle.Empty)
                return SubWindow.QuantityBuyWindow;

            return SubWindow.UnknownWindow;
        }

        private SubWindow CheckSubWindowType()
        {
            var config = _windowAttacher.Config.Market.CheckSubWindow;

            var r = config.Area.ToRectange();
            var color = config.Color.ToColor();
            var seuil = config.Color.Seuil;
            var acceptance = config.ContourAcceptance.ToContourAcceptance();

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, acceptance);

            var zz = rr.OrderByDescending(a => a.Height + a.Width).FirstOrDefault();

            if (zz == default(Rectangle))
            {
                return SubWindow.UnknownWindow;
            }

            if(zz.Height >= 100 && zz.Width >= 300)
                return SubWindow.InfoWindow;

            return SubWindow.QuantityBuyWindow;
        }
    }
}
