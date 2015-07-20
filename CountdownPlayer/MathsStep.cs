using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public class MathsStep : IEnumerable<MathsStep>
    {
        private static int last_id = 0;
        private static int next_id()
        {
            return MathsStep.last_id++;
        }
        public MathsStep LeftOperand { get; private set; }
        public MathsStep RightOperand { get; private set; }
        public IOperation Operation { get; private set; }
        public int ID { get; private set; }

        public virtual int Result()
        {
            return this.Operation.Method(this.LeftOperand.Result(), this.RightOperand.Result());
        }

        public MathsStep(MathsStep left, MathsStep right, IOperation op)
        {
            this.LeftOperand = left;
            this.RightOperand = right;
            this.Operation = op;
            this.ID = MathsStep.next_id();

            //short curcuiting should prevent the reflection call expect when neccessary (i.e. when null)
            if ((left == null || right == null) && this.GetType() != typeof(MathsStartStep))
                throw new ArgumentNullException("Only MathStartStep types may have null operands");
        }

        public bool IsDependentOn(int ID)
        {
            return this.is_dependent_on_helper(ID, this);
        }

        private bool is_dependent_on_helper(int target_id, MathsStep current)
        {
            if (current == null)
                return false;

            if (current is MathsStartStep)
                return current.ID == target_id;

            if (current.LeftOperand.ID == target_id)
                return true;
            else if (current.RightOperand.ID == target_id)
                return true;
            else
                return this.is_dependent_on_helper(target_id, current.LeftOperand) || this.is_dependent_on_helper(target_id, current.RightOperand);
        }

        public IEnumerator<MathsStep> GetEnumerator()
        {
            return this.flatten_tree(this).GetEnumerator();
        }

        public override string ToString()
        {

            return String.Format("{0,-5} {1,-11} {2,-5} = {3,-5}", this.LeftOperand.Result(),
                                                                  this.Operation.OperationType,
                                                                  this.RightOperand.Result(),
                                                                  this.Result());
        }

        public string AsString()
        {
            return as_string_helper();
        }

        private string as_string_helper(StringBuilder sb = null)
        {
            if (sb == null)
                sb = new StringBuilder();

            sb.Append(this.ToString() + Environment.NewLine);

            if (this.LeftOperand != null)
            {
                if (!(this.LeftOperand is MathsStartStep))
                {
                    sb.AppendFormat("<<<<<Left Branch>>>>>>{0}{0}", Environment.NewLine);
                    sb.Append(this.LeftOperand.AsString() + Environment.NewLine);
                }
            }

            if (this.RightOperand != null)
            {
                if (!(this.RightOperand is MathsStartStep))
                {
                    sb.AppendFormat("<<<<<Right Branch>>>>>{0}{0}", Environment.NewLine);
                    sb.Append(this.RightOperand.AsString() + Environment.NewLine);
                }
            }

            return sb.ToString();

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IEnumerable<MathsStep> flatten_tree(MathsStep current, List<MathsStep> found = null)
        {
            if (found == null)
                found = new List<MathsStep>();

            if (current == null)
                return null;

            found.Add(current.LeftOperand);
            found.Add(current.RightOperand);

            flatten_tree(current.LeftOperand, found);
            flatten_tree(current.RightOperand, found);

            return found;
        }
    }

    public class MathsStartStep : MathsStep
    {
        public int Value { get; private set; }

        public MathsStartStep(int val)
            : base(null, null, null)
        {
            this.Value = val;
        }

        public override int Result()
        {
            return this.Value;
        }
    }
}
