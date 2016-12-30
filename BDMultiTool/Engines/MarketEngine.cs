using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BDMultiTool.Core;
using BDMultiTool.Engines;

namespace BDMultiTool.Engines
{
    public class MarketEngine : IEngine
    {
        private bool _running = false;
        private Thread _thread;

        private readonly IRegonizeArea _regonizeArea;
        private readonly IInputSender _inputSender;
        private readonly IScreenHelper _screenHelper;

        public MarketEngine(IRegonizeArea regonizeArea, IInputSender inputSender, IScreenHelper screenHelper)
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

        private void Execute()
        {
            while (_running)
            {
                if (CheckItemsState())
                {
                    TryBuy();
                }

                ClickRefresh();

                Thread.Sleep(500);
            }
        }

        private void TryBuy()
        {
            var rows = Enumerable.Range(1, 7).Select(i => i*62 + 338).ToList();

            foreach (var y in rows)
            {
                if (CheckState(y))
                {
                    var position = Enumerable.Range(1, 7).Select(i => i*62 + 309).FirstOrDefault(CheckButtonAvailable);

                    if (position != default(int))
                    {
                        _inputSender.MouseLeftClickTo(new Point() {X = 1330, Y = position + 20});
                        Thread.Sleep(100);

                        if (CheckSubWindowType() == SubWindow.InfoWindow)
                        {
                            _inputSender.SendKeys(Keys.Enter);
                        }
                        else
                        {
                            _inputSender.MouseLeftClickTo(new Point() {  });
                        }
                    }
                }
            }
        }

        private void ClickRefresh()
        {
            throw new NotImplementedException();
        }

        private bool CheckItemsState()
        {
            return Enumerable.Range(1, 7).Select(i => i * 62 + 338).Select(CheckState).Any();
        }

        private bool CheckState(int y)
        {
            var r = new Rectangle { X = 782, Y = y, Width = 30, Height = 17 };
            var color = Color.FromArgb(190, 190, 170);
            var seuil = 80;

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, new RegonizeEngine.ContourAcceptance
            {
                Height = 150,
                HeightOffset = 150,
                Width = 330,
                WidthOffset = 330,
                Size = 500,
                SizeOffset = 500
            });

            return rr.Any();
        }

        private bool CheckButtonAvailable(int y)
        {
            var r = new Rectangle { X = 1300, Y = y, Width = 62, Height = 46 };
            var color = Color.FromArgb(200, 200, 200);
            var seuil = 60;

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, new RegonizeEngine.ContourAcceptance
            {
                Height = 150,
                HeightOffset = 150,
                Width = 330,
                WidthOffset = 330,
                Size = 500,
                SizeOffset = 500
            });

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
            var r = new Rectangle { X = 700, Y = 45, Width = 500, Height = 600 };
            var color = Color.FromArgb(20, 20, 20);
            var seuil = 20;

            var tmp = _screenHelper.ScreenArea(r);

            var rr = _regonizeArea.GetAllRectangles(tmp, r, color, seuil, new RegonizeEngine.ContourAcceptance
            {
                Height = 150,
                HeightOffset = 150,
                Width = 330,
                WidthOffset = 100,
                Size = 500,
                SizeOffset = 500
            });

            var zz = rr.OrderByDescending(a => a.Height + a.Width).FirstOrDefault();

            if (zz == default(Rectangle))
            {
                return SubWindow.UnknownWindow;
            }

            if(zz.Height >= 300 && zz.Width >= 100)
                return SubWindow.InfoWindow;

            return SubWindow.QuantityBuyWindow;
        }
    }
}
