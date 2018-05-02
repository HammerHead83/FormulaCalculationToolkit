using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCT.Lib
{
    public class FXEngine
    {
        List<FXExpression> _exprs = new List<FXExpression>();
        string _formula = string.Empty;
        char[] _brasers = new char[2] { '[', ']' };
        double accumulator = default(double);
        readonly char decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
            .NumberDecimalSeparator.ToCharArray().LastOrDefault();
        readonly string[] hpFuncNames = new string[] { "sin", "cos", "tan", "sinh", "cosh", "tanh", "sqrt" };

        private bool CheckForHighPriorityFunctions()
        {
            bool result = false;
            ParallelLoopResult state;
            state = Parallel.ForEach<string>(hpFuncNames, (item) => { if (_formula.Contains(item)) result = true; });
            while (!state.IsCompleted) ;
            return result;
        }

        private int MinPredicate(params int[] values)
        {
            int min = values[0];
            for (int i = 1; i < values.Length; i++)
                if (values[i] < min)
                    min = values[i];
            return min;
        }

        private void CalculateHPFunc()
        {
            char[] cArray = _formula.ToCharArray();
            List<int> ilist = new List<int>();
            foreach (var item in hpFuncNames)
                ilist.Add(_formula.IndexOf(item));
            ilist.RemoveAll(i => i == -1);
            int idx = MinPredicate(ilist.ToArray());
            var fnBuilder = new StringBuilder();
            var valBuilder = new StringBuilder();
            for (int j = idx; j < cArray.Length; j++)
            {
                if (char.IsLetter(cArray[j]))
                {
                    fnBuilder.Append(cArray[j]);
                    continue;
                }
                else if (cArray[j].Equals('('))
                {
                    for (int k = j + 1; k < cArray.Length; k++)
                    {
                        if (char.IsNumber(cArray[k]) || cArray[k].Equals(decimalSeparator))
                        {
                            valBuilder.Append(cArray[k]);
                            continue;
                        }
                        if (cArray[k].Equals(')'))
                            break;
                    }
                    continue;
                }
                else if (cArray[j].Equals(')'))
                    break;

            }
            double value;
            if (!double.TryParse(valBuilder.ToString(), out value))
                return;
            var token = new FXExpression()
            {
                LeftValue = value,
                Operator = FXExpression.GetOperatorFromString(fnBuilder.ToString()),
                Priority = FXExpression.FXOperationPriority.High
            };
            accumulator = token.Calculate();
            _formula = _formula.Remove(idx, token.FullStrCount).Insert(idx, accumulator.ToString()); //See that String.Remove do not remove first needed character
        }

        private void Calculate(bool isHigh)
        {
            char[] cArray = _formula.ToCharArray();
            int idx1, idx2;
            if (isHigh)
            {
                idx1 = cArray.ToList().IndexOf('*');
                idx2 = cArray.ToList().IndexOf('/');
            }
            else
            {
                idx1 = cArray.ToList().IndexOf('+');
                idx2 = cArray.ToList().IndexOf('-');
            }
            int item;
            if (idx1 < 0)
                item = idx2;
            else if (idx2 < 0)
                item = idx1;
            else
                item = idx1 < idx2 ? idx1 : idx2; // Analog for Math.Min(idx1, idx2)
            idx1 = 0;
            var tk = new FXExpression();
            var builder1 = new StringBuilder();
            for (int j = item + 1; j < cArray.Length; j++)
                if (char.IsNumber(cArray[j]) || cArray[j].Equals(decimalSeparator))
                    builder1.Append(cArray[j]);
                else
                    break;
            double d;
            if (!double.TryParse(builder1.ToString(), out d))
                return;
            tk.RightValue = d;
            builder1.Clear();
            for (int k = item - 1; k >= 0; k--)
                if (char.IsNumber(cArray[k]) || cArray[k].Equals(decimalSeparator))
                    builder1.Insert(0, cArray[k]);
                else
                {
                    idx1 = item;
                    break;
                }
            if (!double.TryParse(builder1.ToString(), out d))
                return;
            tk.LeftValue = d;
            builder1.Clear();
            switch (cArray[item])
            {
                case '*':
                    tk.Operator = FXExpression.FXEOperation.Multiply;
                    tk.Priority = FXExpression.FXOperationPriority.Medium;
                    break;
                case '/':
                    tk.Operator = FXExpression.FXEOperation.Divide;
                    tk.Priority = FXExpression.FXOperationPriority.Medium;
                    break;
                case '-':
                    tk.Operator = FXExpression.FXEOperation.Minus;
                    tk.Priority = FXExpression.FXOperationPriority.Low;
                    break;
                default:
                    tk.Operator = FXExpression.FXEOperation.Plus;
                    tk.Priority = FXExpression.FXOperationPriority.Low;
                    break;
            }
            if (isHigh)
            {
                _formula = _formula.Remove(idx1 - tk.LeftStrCount, tk.FullStrCount);
                accumulator = tk.Calculate();
                _formula = _formula.Insert(idx1 - tk.LeftStrCount, accumulator.ToString());
            }
            else
            {
                _formula = _formula.Remove(0, tk.FullStrCount);
                accumulator = tk.Calculate();
                _formula = _formula.Insert(0, accumulator.ToString());
            }
        }

        public double Evaluate(string formula, char[] brSeparator = null, Dictionary<string, string> parameters = null)
        {
            if (string.IsNullOrEmpty(formula))
                throw new ArgumentNullException("formula");
            try
            {
                _formula = formula.ToLower();
                if (brSeparator != null && brSeparator.Length == 2)
                    brSeparator.CopyTo(_brasers, 0);
                _formula = _formula.Replace(_brasers[0], '[').Replace(_brasers[1], ']');
                int lParamsCount = _formula.Count(e => e.Equals('['));
                int rParamsCount = _formula.Count(e => e.Equals(']'));
                switch (lParamsCount.CompareTo(rParamsCount))
                {
                    case 1:
                        throw new Exception("Your formula missed right parenthesis");
                    case -1:
                        throw new Exception("Your formula missed left parenthesis");
                    default:
                        break;
                }
                if (parameters != null)
                    foreach (var item in parameters)
                        _formula = _formula.Replace(string.Format("[{0}]", item.Key), item.Value).Replace(
                            '.', decimalSeparator).Replace(',', decimalSeparator);
                if (_formula.Contains('[') || _formula.Contains(']'))
                    throw new Exception("Number of formula parameters not equal to parameters number in dictionary");
                _formula = _formula.Replace(" ", string.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable load formula", ex);
            }

            // Calculate high-priority functions
            while (CheckForHighPriorityFunctions())
            {
                CalculateHPFunc();
            }

            // Calculate medium-priority content
            while (_formula.Contains('*') || _formula.Contains('/'))
            {
                if (_formula.Split("*/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Count() < 2)
                    break;
                Calculate(true);
            }

            // Calculate low-priority content
            while (_formula.Contains('+') || _formula.Contains('-'))
            {
                if (_formula.Split("+-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Count() < 2)
                    break;
                Calculate(false);
            }
            return accumulator;
        }

    }
}
