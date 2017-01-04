using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BDMultiTool.Core;
using BDMultiTool.Core.PInvoke;

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

        public MarketEngine(IRegonizeArea regonizeArea, IInputSender inputSender, IScreenHelper screenHelper, IWindowAttacher windowAttacher)
        {
            _regonizeArea = regonizeArea;
            _inputSender = inputSender;
            _screenHelper = screenHelper;
            _windowAttacher = windowAttacher;
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
            while (_running)
            {
                int refreshTime;
                if (CheckItemsState())
                {
                    refreshTime = 50;
                    TryBuy();
                }
                else
                {
                    refreshTime = 500;
                    ClickRefresh();
                }

                Thread.Sleep(refreshTime);
            }
        }

        private void TryBuy()
        {
            var config = _windowAttacher.Config.Market;
            var position = Enumerable.Range(1, 7).FirstOrDefault(CheckButtonAvailable);

            if (position == default(int))
                return;

            //new Point { X = 1330, Y = position * 62 + 309 + 20 + 26 - 62 }
            _inputSender.MouseLeftClickTo(new Point { X = config.MarketRow.X, Y = position * 62 + config.MarketRow.Y });

            Thread.Sleep(400);

            var windowType = CheckSubWindowType();

            if (windowType == SubWindow.InfoWindow)
            {
                _inputSender.SendKeys(Keys.Enter);
                //_inputSender.MouseMoveTo(new Point { X = 1330 + 100, Y = position * 62 + 309 + 20 + 26 - 62 });
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

        private bool CheckItemsState()
        {
            return Enumerable.Range(1, 7).Any(MarketRowAvailable);
        }

        private bool MarketRowAvailable(int slot)
        {
            var config = _windowAttacher.Config.Market.MarketRowAvailable;

            //var y = slot * 62 + 338 + 26 - 62;
            //var r = new Rectangle { X = 782, Y = y, Width = 30, Height = 17 };
            //var color = Color.FromArgb(190, 190, 170);
            //var seuil = 80;
            //var acceptance = new RegonizeEngine.ContourAcceptance
            //{
            //    Height = 150,
            //    HeightOffset = 150,
            //    Width = 330,
            //    WidthOffset = 330,
            //    Size = 500,
            //    SizeOffset = 500
            //};

            var r = config.Area.ToRectange();
            r.Y = slot*62 + r.Y;
            var color = config.Color.ToColor();
            var seuil = config.Color.Seuil;
            var acceptance = config.ContourAcceptance.ToContourAcceptance();

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, acceptance);

            return rr.Any();
        }

        private bool CheckButtonAvailable(int slot)
        {
            var config = _windowAttacher.Config.Market.CheckButtonAvailable;
            //var y = slot * 62 + 309 + 26 - 62;
            //var r = new Rectangle { X = 1300, Y = y, Width = 62, Height = 46 };
            //var color = Color.FromArgb(200, 200, 200);
            //var seuil = 60;
            //var acceptance = new RegonizeEngine.ContourAcceptance
            //{
            //    Height = 150,
            //    HeightOffset = 150,
            //    Width = 330,
            //    WidthOffset = 330,
            //    Size = 500,
            //    SizeOffset = 500
            //};

            var r = config.Area.ToRectange();
            r.Y = slot * 62 + r.Y;
            var color = config.Color.ToColor();
            var seuil = config.Color.Seuil;
            var acceptance = config.ContourAcceptance.ToContourAcceptance();

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, acceptance);

            return rr.Any();
        }

        private enum SubWindow
        {
            InfoWindow,
            QuantityBuyWindow,
            UnknownWindow
        }

        private SubWindow CheckSubWindowType()
        {
            var config = _windowAttacher.Config.Market.CheckSubWindow;

            //var r = new Rectangle { X = 700, Y = 45, Width = 500, Height = 600 };
            //var color = Color.FromArgb(20, 20, 20);
            //var seuil = 20;
            //var acceptance = new RegonizeEngine.ContourAcceptance
            //{
            //    Height = 150,
            //    HeightOffset = 150,
            //    Width = 330,
            //    WidthOffset = 100,
            //    Size = 500,
            //    SizeOffset = 500
            //};

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
