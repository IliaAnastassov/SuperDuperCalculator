namespace Calculator.Factories
{
    using System.Collections.Generic;
    using Contracts;
    using Models;

    public abstract class OperationFactory
    {
        private static Dictionary<string, ICalculatable> operations = new Dictionary<string, ICalculatable>
        {
            { "+", new Addition() },
            { "-", new Subtraction() },
            { "x", new Multiplication() },
            { "/", new Division() }
        };

        /// <summary>
        /// Returns an operation based on the operator
        /// </summary>
        public static ICalculatable GetOperation(string operation)
            => operations[operation];
    }
}
