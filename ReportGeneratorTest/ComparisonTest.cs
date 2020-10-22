using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportGenerator.Services;

namespace ReportGeneratorTest
{
    [TestClass]
    public class ComparisonTest
    {
        [TestMethod]
        public void TestGetSampleListMethod()
        {
            List<double> target = new List<double> { 1, 2, 3 };
            List<double> match = new List<double> { 1, 2, 3 };

            var comparison = new Comparison(target, match);

            Assert.AreEqual<string>(string.Join(",", target), comparison.GetTargetSampleList());
            Assert.AreEqual<string>(string.Join(",", match), comparison.GetMatchSampleList());
        }

        [TestMethod]
        public void TestNumberIsNoEqual_ReturnFalse()
        {
            List<double> target = new List<double> { 1, 2, 3, 4 };
            List<double> match = new List<double> { 1, 2, 3 };

            var comparison = new Comparison(target, match);

            Assert.IsFalse(comparison.NumberIsEqual());
        }

        [TestMethod]
        public void TestNumberIsEqual_ReturnTrue()
        {
            List<double> target = new List<double> { 1, 2, 3 };
            List<double> match = new List<double> { 1, 2, 3 };

            var comparison = new Comparison(target, match);

            Assert.IsTrue(comparison.NumberIsEqual());
        }

        [TestMethod]
        public void TestNumberIsLargerThan19_ReturnTrue()
        {
            List<double> target = new List<double> { };
            List<double> match = new List<double> { };

            for (int i = 0; i < 20; i++)
            {
                target.Add(i);
                match.Add(i);
            }

            var comparison = new Comparison(target, match);

            Assert.IsTrue(comparison.NumberIsLargerThan19());
        }

        [TestMethod]
        public void TestNumberIsFewerThan19_ReturnTrue()
        {
            List<double> target = new List<double> { };
            List<double> match = new List<double> { };

            var comparison = new Comparison(target, match);

            Assert.IsFalse(comparison.NumberIsLargerThan19());
        }
    }
}
