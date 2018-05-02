using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FCT.Lib
{
    internal class FXExpression
    {
        internal enum FXEOperation
        {
            Plus,
            Minus,
            Divide,
            Multiply,
            Sin,
            Cos,
            Tan,
            SinH,
            CosH,
            TanH,
            SQRT
        }

        internal enum FXOperationPriority
        {
            High,
            Medium,
            Low
        }

        internal static FXEOperation GetOperatorFromString(string operation)
        {
            FXEOperation result;
            switch (operation)
            {
                case "-": result = FXEOperation.Minus;
                    break;
                case "*": result = FXEOperation.Multiply;
                    break;
                case "/": result = FXEOperation.Divide;
                    break;
                case "sin": result = FXEOperation.Sin;
                    break;
                case "cos": result = FXEOperation.Cos;
                    break;
                case "tan": result = FXEOperation.Tan;
                    break;
                case "tanh": result = FXEOperation.TanH;
                    break;
                case "sinh": result = FXEOperation.SinH;
                    break;
                case "cosh": result = FXEOperation.CosH;
                    break;
                default: result = FXEOperation.Plus;
                    break;
            }
            return result;
        }

        private double _leftValue = default(double);
        private double _rightValue = default(double);
        private FXEOperation _operator = FXEOperation.Plus;
        private FXOperationPriority _priority = FXOperationPriority.Low;

        internal double LeftValue { get { return _leftValue; } set { _leftValue = value; } }
        internal double RightValue { get { return _rightValue; } set { _rightValue = value; } }
        internal FXEOperation Operator { get { return this._operator; } set { _operator = value; } }
        internal FXOperationPriority Priority { get { return _priority; } set { _priority = value; } }

        internal int LeftStrCount { get { return _leftValue.ToString().Count(); } }
        internal int RightStrCount { get { return _rightValue.ToString().Count(); } }
        internal int FullStrCount { get { return ToString().Count(); } }

        public override string ToString()
        {
            if (_priority == FXOperationPriority.Low || _priority == FXOperationPriority.Medium)
            return string.Format("{0}{1}{2}", _leftValue,
                new Func<char>(() => {
                    char c;
                    switch (this._operator)
                    {
                        case FXEOperation.Minus: c = '-';
                            break;
                        case FXEOperation.Divide: c = '/';
                            break;
                        case FXEOperation.Multiply: c = '*';
                            break;
                        default: c = '+';
                            break;
                    }
                    return c;
                }).Invoke(),
                _rightValue);
            return string.Format("{0}({1})", Enum.GetName(typeof(FXEOperation), _operator).ToLower(), _leftValue);
        }

        internal double Calculate()
        {
            double d = default(double);
            switch (this._operator)
            {
                case FXEOperation.Plus:
                    d = _leftValue + _rightValue;
                    break;
                case FXEOperation.Minus:
                    d = _leftValue - _rightValue;
                    break;
                case FXEOperation.Multiply:
                    d = _leftValue * _rightValue;
                    break;
                case FXEOperation.Divide:
                    d = _rightValue == 0.0 ? _leftValue / double.Epsilon : _leftValue / _rightValue;
                    break;
                case FXEOperation.Cos:
                    d = _leftValue > 1 || _leftValue < -1 ? default(double) : Math.Cos(_leftValue);
                    break;
                case FXEOperation.CosH:
                    d = Math.Cosh(_leftValue);
                    break;
                case FXEOperation.Sin:
                    d = _leftValue > 1 || _leftValue < -1 ? default(double) : Math.Sin(_leftValue);
                    break;
                case FXEOperation.SinH:
                    d = Math.Sinh(_leftValue);
                    break;
                case FXEOperation.Tan:
                    d = Math.Tan(_leftValue);
                    break;
                case FXEOperation.TanH:
                    d = _leftValue > 1 || _leftValue < -1 ? default(double) : Math.Tanh(_leftValue);
                    break;
                case FXEOperation.SQRT:
                    d = _leftValue < 0 ? default(double) : Math.Sqrt(_leftValue);
                    break;
            }
            return d;
        }

    }
}
