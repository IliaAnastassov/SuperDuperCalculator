namespace Calculator.Models
{
    using Abstract;
    using Contracts;

    public class Multiplication : ArithmeticOperation, ICalculatable
    {
        public override double Calculate(double valueOne, double valueTwo)
            => valueOne * valueTwo;
    }
}
