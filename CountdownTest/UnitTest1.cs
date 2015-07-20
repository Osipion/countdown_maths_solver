using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using CountdownPlayer;

namespace CountdownTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PathProducesSequentialSteps()
        {
            var p = new Frame(6);

            var r = p.AllPossibleSteps();

            int i = 0;

            foreach(var u in r)
            {
                if (i++ > 10)
                    break;

                Console.WriteLine(u);
            }

        }

        [TestMethod]
        public void PerfOfPathMap()
        {
            var g = new PathGrid(6);

            var count = 0L;

            var timer = System.Diagnostics.Stopwatch.StartNew();

            foreach (var step in g.Steps())
                count++;

            timer.Stop();

            Console.WriteLine("It took {0}ms to generate all {1} steps", timer.ElapsedMilliseconds, count);
        }
    }
}
