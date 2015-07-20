using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public class MathsSolution
    {
        private List<MathsStep> numbers_left;
        public List<MathsStep> NumbersLeft { get { return this.numbers_left; } }

        public MathsSolution(IEnumerable<MathsStartStep> initial_steps)
        {
            this.numbers_left = initial_steps.Cast<MathsStep>().ToList();
        }

        public void Add(int left_operand_index, int right_operand_index, IOperation operation)
        {
            if (left_operand_index < 0 || right_operand_index < 0)
                throw new ArgumentException("left_operand|right_operand must be greater than -1");

            var s1 = this.numbers_left[left_operand_index];
            this.numbers_left.RemoveAt(left_operand_index);

            var s2 = this.numbers_left[right_operand_index];
            this.numbers_left.RemoveAt(right_operand_index);

            this.numbers_left.Add(new MathsStep(s1, s2, operation));
        }

        public T Test<T>(int left_operand_index, int right_operand_index, Func<MathsStep, MathsStep, T> test)
        {
            if (right_operand_index > this.numbers_left.Count - 2 || left_operand_index > this.numbers_left.Count - 1)
                throw new ArgumentOutOfRangeException("The right operand should be no greater than frame size - 2, and the left, frame size - 1");

            if (left_operand_index > right_operand_index)
                return test(this.numbers_left[left_operand_index], this.numbers_left[right_operand_index]);

            return test(this.numbers_left[left_operand_index], this.numbers_left[right_operand_index + 1]);
        }

        public MathsStep ClosestTo(int value)
        {
            var best = this.numbers_left[0];

            foreach (var step in this.numbers_left)
            {
                if (Math.Abs(step.Result() - value) < Math.Abs(best.Result() - value))
                    best = step;
            }

            return best;
        }

        public bool Remove(MathsStep step)
        {
            if (step is MathsStartStep)
                return false; //Cannot remove a start step except by adding a new step

            if (step == null)
                return false;

            if (!this.numbers_left.Contains(step))
                return false;

            remove_helper(step);

            return true;
        }

        private void remove_helper(MathsStep step)
        {
            if (step is MathsStartStep)
                return;

            this.numbers_left.Remove(step);
            this.numbers_left.Add(step.LeftOperand);
            this.numbers_left.Add(step.RightOperand);
            this.remove_helper(step.LeftOperand);
            this.remove_helper(step.RightOperand);

        }
    }
}
