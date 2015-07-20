using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownPlayer
{
    public enum OperationType
    {
        Add,
        Subtract,
        Divide,
        Multiply
    }

    public delegate int MathsOperationDelegate(int a, int b);

    public interface IOperation
    {
        OperationType OperationType { get; }
        MathsOperationDelegate Method { get; }
    }

    public class Operation : IOperation
    {
        public OperationType OperationType { get; private set; }
        public MathsOperationDelegate Method { get; private set; }

        private Operation(MathsOperationDelegate method, OperationType type)
        {
            this.Method = method;
            this.OperationType = type;
        }

        public static readonly IOperation Addition = new Operation((x, y) => x + y, OperationType.Add);
        public static readonly IOperation Subtraction = new Operation((x, y) => x - y, OperationType.Subtract);
        public static readonly IOperation Division = new Operation((x, y) => { if (x % y != 0) throw new ArgumentException(); return x / y; }, OperationType.Divide);
        public static readonly IOperation Multiplication = new Operation((x, y) => x * y, OperationType.Multiply);

        private static Random random = new Random();

        public static IEnumerable<IOperation> Operations()
        {
            yield return Operation.Addition;
            yield return Operation.Subtraction;
            yield return Operation.Division;
            yield return Operation.Multiplication;
        }

        public static IOperation FromInt(int val)
        {
            switch (val)
            {
                case 0:
                    return Addition;
                case 1:
                    return Subtraction;
                case 2:
                    return Division;
                case 3:
                    return Multiplication;
                default:
                    throw new ArgumentOutOfRangeException("The operation values range is between 0 and 3");
            }
        }

        public static IOperation RandomOperation()
        {
            return Operation.Operations().ElementAt(Operation.random.Next(0, Operation.Operations().Count() - 1));
        }
    }
}
