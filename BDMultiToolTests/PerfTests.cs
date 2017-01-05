using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDMultiTool;
using BDMultiTool.Core.Factory;
using BDMultiTool.Core.PInvoke;
using NUnit.Framework;

namespace BDMultiToolTests
{
    [TestFixture]
    public class PerfTests
    {
        [Test]
        public void ScreenHelperPerf()
        {
            var g = new GraphicsFactory();
            var a = new ScreenHelper(g);
            var r = new Rectangle() { X = 0, Y = 0, Width = 500, Height = 500 };

            Debug.WriteLine(@"Start Perftest with 100 step");
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < 100; i++)
            {
                var bmp = a.ScreenArea(r);
                bmp.Dispose();
            }
            st.Stop();
            Debug.WriteLine(@"End Perftest with 100 step");
            Debug.WriteLine($@"Total time {st.ElapsedMilliseconds} ms, average {st.ElapsedMilliseconds / 100}");

            Debug.WriteLine(@"Start Perftest with 1000 step");
            var st2 = new Stopwatch();
            st2.Start();
            for (var i = 0; i < 1000; i++)
            {
                var bmp = a.ScreenArea(r);
                bmp.Dispose();
            }
            st2.Stop();
            Debug.WriteLine(@"End Perftest with 1000 step");
            Debug.WriteLine($@"Total time {st2.ElapsedMilliseconds} ms, average {st2.ElapsedMilliseconds / 1000}");

            Debug.WriteLine(@"Start Perftest with 10000 step");
            var st3 = new Stopwatch();
            st3.Start();
            for (var i = 0; i < 10000; i++)
            {
                var bmp = a.ScreenArea(r);
                bmp.Dispose();
            }
            st3.Stop();
            Debug.WriteLine(@"End Perftest with 10000 step");
            Debug.WriteLine($@"Total time {st3.ElapsedMilliseconds} ms, average {st3.ElapsedMilliseconds / 10000}");
        }

        [Test]
        public void PinvokeCapturePerf()
        {
            var handle = WindowAttacher.GetHandleByWindowTitleBeginningWith("BLACK DESERT");

            Debug.WriteLine(@"Start Perftest with 100 step");
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < 100; i++)
            {
                var bmp = GraphicsFactory.CopyFromScreen(handle);
                bmp.Dispose();
            }
            st.Stop();
            Debug.WriteLine(@"End Perftest with 100 step");
            Debug.WriteLine($@"Total time {st.ElapsedMilliseconds} ms, average {st.ElapsedMilliseconds / 100}");

            Debug.WriteLine(@"Start Perftest with 1000 step");
            var st2 = new Stopwatch();
            st2.Start();
            for (var i = 0; i < 1000; i++)
            {
                var bmp = GraphicsFactory.CopyFromScreen(handle);
                bmp.Dispose();
            }
            st2.Stop();
            Debug.WriteLine(@"End Perftest with 1000 step");
            Debug.WriteLine($@"Total time {st2.ElapsedMilliseconds} ms, average {st2.ElapsedMilliseconds / 1000}");

            Debug.WriteLine(@"Start Perftest with 10000 step");
            var st3 = new Stopwatch();
            st3.Start();
            for (var i = 0; i < 10000; i++)
            {
                var bmp = GraphicsFactory.CopyFromScreen(handle);
                bmp.Dispose();
            }
            st3.Stop();
            Debug.WriteLine(@"End Perftest with 10000 step");
            Debug.WriteLine($@"Total time {st3.ElapsedMilliseconds} ms, average {st3.ElapsedMilliseconds / 10000}");
        }
    }
}
