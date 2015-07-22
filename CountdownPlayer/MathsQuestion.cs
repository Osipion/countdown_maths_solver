using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public class MathsQuestion
    {
        public readonly int[] Numbers;
        public readonly int Target;

        public MathsQuestion(IEnumerable<int> numbers, int target)
        {
            if (numbers == null)
                throw new ArgumentNullException("numbers");

            this.Numbers = numbers.ToArray();
            this.Target = target;
        }

        public MathsStep Solve(out long count_of_routes_tried, long? timeout_ms = null)
        {
            var g = new PathGrid(this.Numbers.Length);

            var steps = this.Numbers.Select(x => new MathsStartStep(x)).ToArray();

            var invalid_division_flag = false;

            count_of_routes_tried = 0L;

            MathsSolution solution = null;

            MathsStep best_step = new MathsStartStep(0);

            System.Diagnostics.Stopwatch watch = null;

            if (timeout_ms != null)
                watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var step in g.Steps())
            {
                if (step.FrameSize == this.Numbers.Length)
                {
                    solution = new MathsSolution(steps);
                    invalid_division_flag = false;
                    count_of_routes_tried++;
                }
                else if (invalid_division_flag)
                    continue;

                var curr_best = solution.ClosestTo(this.Target);

                if (first_number_is_closer(this.Target, curr_best.Result(), best_step.Result()))
                {
                    best_step = curr_best;

                    if (curr_best.Result() == this.Target)
                        break;
                }

                if (watch != null)
                {
                    if (watch.ElapsedMilliseconds > timeout_ms)
                        break;
                }

                if (Operation.FromInt(step.Operation) == Operation.Division)
                {

                    if (solution.Test(step.Index1, step.Index2, (x, y) => { if (y.Result() == 0 || x.Result() == 0) return true; return x.Result() % y.Result() != 0; }))
                    {
                        invalid_division_flag = true;
                        continue;
                    }
                }

                if (!invalid_division_flag)
                    solution.Add(step.Index1, step.Index2, Operation.FromInt(step.Operation));
            }

            return best_step;
        }

        private static bool first_number_is_closer(int target, int val1, int val2)
        {
            return Math.Abs(val1 - target) < Math.Abs(val2 - target);
        }


        public long CountOfPossibleRoutes()
        {
            long c = this.Numbers.Length;
            long op_count = Operation.Operations().Count();

            var sum = 1L;
            
            /*****************************
             * 
             * Let n = the count of numbers in a frame
             * Let t = the number of operations
             * 
             * At each frame, the number of possible choices is
             *  (n * (n - 1)) * t
             *  
             * Because:
             *  1. The first choice is one in n, and the second, 1 in n - 1, because we've just used one option.
             *  
             *  2. Then there are t number of different ways we could (possibly) combine those numbers - although many 
             *     numbers will not divide equally, and so are invalid
             * 
             * Each frame is one option smaller than the last, so
             *  ((n * (n - 1)) * t) * (((n - 1) * (n - 2)) * t) = number of choices in two succesive frames (where n > 3)
             * 
             */

            while (c > 1)
                sum *= ((c-- * c) * op_count);

            return sum;

        }

        public int NumberOfPossibleFrames()
        {
            return this.Numbers.Length - 1;
        }
        

    }

}
