namespace Calculator.Models.Abstract
{
    using Contracts;

    public abstract class ArithmeticOperation : ICalculatable
    {
        public abstract double Calculate(double valueOne, double valueTwo);
    }
}
