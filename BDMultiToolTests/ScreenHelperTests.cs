using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BDMultiTool;
using BDMultiTool.Core.Factory;
using BDMultiTool.Core.PInvoke;
using NUnit.Framework;

namespace BDMultiToolTests
{
    [TestFixture]
    public class ScreenHelperTests
    {
        [Test, Apartment(ApartmentState.STA)]
        public void Tests()
        {
            var aa = new TestGraphicsFactory();
            aa.LoadImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,@"..\..\ImageTest\a.jpg"));

            var a = new ScreenHelper(aa);
            var r = new Rectangle { X = 760, Y = 164, Width = 155, Height = 65 };
            var b = a.ScreenArea(r);

            Clipboard.SetImage(b);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void PinvokeTests()
        {
            var handle = WindowAttacher.GetHandleByWindowTitleBeginningWith("BLACK DESERT");

            var tmp = GraphicsFactory.CopyFromScreen(handle);

            Clipboard.SetImage(tmp);
        }
    }
}
