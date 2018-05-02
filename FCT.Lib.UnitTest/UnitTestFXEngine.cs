using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FCT.Lib.UnitTest
{
    [TestClass]
    public class UnitTestFXEngine
    {
        [TestMethod]
        public void TestCalculateFormula()
        {
            var fxEngine = new Lib.FXEngine();
            var dict = new Dictionary<string, string>();
            var random = new Random(int.MaxValue);
            dict.Add("age", random.NextDouble().ToString());
            dict.Add("stage", random.NextDouble().ToString());
            dict.Add("price", random.NextDouble().ToString());
            dict.Add("salary", random.NextDouble().ToString());
            try
            {
                var result = fxEngine.Evaluate(formula: "[age] + 0.5 * 15 - sin([stage]) + [price] * 0.1 - cos([salary]) * 0.1 * [stage]",
                    brSeparator: null, parameters: dict);
                Assert.AreNotEqual<double>(default(double), result);
            }
            catch (Exception ex)
            { throw; }
        }
    }
}
