using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    class Program
    {

        static int[] big_numbers = new [] { 50, 75, 100, 150, 175, 200, 250, 300, 400 };

        static Random rand = new Random();

        static int[] question(int big, int small)
        {
            var arr = new int[big + small];

            for(var i = 0; i < arr.Length; i++)
            {
                if(i < big)
                    arr[i] = big_numbers[rand.Next(big_numbers.Length)];
                else
                    arr[i] = rand.Next(1, 50);
            }

            return arr;
        }

        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("Please supply a comma-separated list of numbers, and the target number to compute!");
                Environment.Exit(-1);
            }
            int[] options = args[0].Split(',').Select(x => Int32.Parse(x)).ToArray();

            int target = Int32.Parse(args[1]);

            long? timeout = null;

            if(args.Length > 2)
            {
                timeout = Int64.Parse(args[2]);
            }

            Console.WriteLine("Attempting to find best match for {0} from {1} - Timeout is {2}", target, String.Join(",", options), timeout ?? Int64.MaxValue);
            Console.WriteLine("...");

            var question = new MathsQuestion(options, target);

            var timer = System.Diagnostics.Stopwatch.StartNew();
            var count = 0L;
            var solution = question.Solve(out count, timeout);

            timer.Stop();

            Console.WriteLine("The best match for a target of {0} from {1} was\n\n{2}", target, String.Join(",", options), solution.AsString());
            Console.WriteLine("It took {0}ms to find this result. {1} routes were explored", timer.ElapsedMilliseconds, count);

#if DEBUG
            Console.ReadLine();
#endif
        }

        static bool FirstNumberIsCloser(int target, int val1, int val2)
        {
            return Math.Abs(val1 - target) < Math.Abs(val2 - target);
        }
    }
}
