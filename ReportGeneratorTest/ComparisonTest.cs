using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportGenerator.Models;
using ReportGenerator.Services;
using System.Collections.Generic;

namespace ReportGeneratorTest
{
    [TestClass]
    public class ComparisonTest
    {
        private readonly string targetName;
        private readonly string matchName;
        private readonly string scriptPath;
        private readonly ProjectParameter project;

        public ComparisonTest()
        {
            targetName = "target";
            matchName = "match";
            scriptPath = @"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGenerator\wwwroot";

            project = new ProjectParameter
            {
                ID = 1,
                Name = "Test",
                ALE = 25,
                LOQ = 0.08,
                SignificantDigits = 1,
                SpecificationOneConcRange = 0,
                SpecificationOne = 0.01,
                SpecificationTwoConcRange = 0,
                SpecificationTwo = 10,
                Unit = "nmol/L",
                Xc1 = 1,
                Xc2 = 10
            };
        }

        [TestMethod]
        public void TestGetSampleNameList()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", };
            List<double> targetSampleList = new List<double> { };
            List<double> matchSampleList = new List<double> { };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            var result = comparison.GetSampleNameList();

            Assert.AreEqual(string.Join(",", sampleNameList), result);
        }

        [TestMethod]
        public void TestGetTargetSampleList()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", };
            List<double> targetSampleList = new List<double> { 1, 2, 3, };
            List<double> matchSampleList = new List<double> { };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            var result = comparison.GetTargetSampleList();

            Assert.AreEqual(string.Join(",", targetSampleList), result);
        }

        [TestMethod]
        public void TestGetMatchSampleList()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", };
            List<double> targetSampleList = new List<double> { };
            List<double> matchSampleList = new List<double> { 1, 2, 3, };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            var result = comparison.GetMatchSampleList();

            Assert.AreEqual(string.Join(",", matchSampleList), result);
        }

        [TestMethod]
        public void TestNumberIsEqual_NotEqual()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", };
            List<double> targetSampleList = new List<double> { 1, 2, 3, 4 };
            List<double> matchSampleList = new List<double> { 1, 2, 3 };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsFalse(comparison.NumberIsEqual());
        }

        [TestMethod]
        public void TestNumberIsEqual_Equal()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", };
            List<double> targetSampleList = new List<double> { 1, 2, 3 };
            List<double> matchSampleList = new List<double> { 1, 2, 3 };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsTrue(comparison.NumberIsEqual());
        }

        [TestMethod]
        public void TestNumberIsLargerThan19_True()
        {
            List<string> sampleNameList = new List<string> { };
            List<double> targetSampleList = new List<double> { };
            List<double> matchSampleList = new List<double> { };

            for (int i = 0; i < 20; i++)
            {
                targetSampleList.Add(i);
                matchSampleList.Add(i);
            }

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsTrue(comparison.NumberIsLargerThan19());
        }

        [TestMethod]
        public void TestNumberIsLargerThan19_False()
        {
            List<string> sampleNameList = new List<string> { };
            List<double> targetSampleList = new List<double> { };
            List<double> matchSampleList = new List<double> { };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsFalse(comparison.NumberIsLargerThan19());
        }

        [TestMethod]
        public void TestVerifySampleResultIsDifferent_True()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", "4" };
            List<double> targetSampleList = new List<double> { 1, 2, 3, 5 };
            List<double> matchSampleList = new List<double> { 1, 2, 3, 4 };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsTrue(comparison.VerifySampleResultIsDifferent());
        }

        [TestMethod]
        public void TestVerifySampleResultIsDifferent_False()
        {
            List<string> sampleNameList = new List<string> { "1", "2", "3", "4" };
            List<double> targetSampleList = new List<double> { 1, 2, 3, 4 };
            List<double> matchSampleList = new List<double> { 1, 2, 3, 4 };

            var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.IsFalse(comparison.VerifySampleResultIsDifferent());
        }

        [TestMethod]
        public void TestGetBias()
        {
            var customProject = new ProjectParameter
            {
                SignificantDigits = 2,
                SpecificationOneConcRange = 7,
                SpecificationTwoConcRange = 7,
            };
            List<string> sampleNameList = new List<string> { "1", "2", "3", "4" };
            List<double> targetSampleList = new List<double> { 1, 5, 10, 15 };
            List<double> matchSampleList = new List<double> { 1.1, 4.91, 12.5, 13.5 };

            var comparison = new Comparison(customProject, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

            Assert.AreEqual("0.10,-0.09,25.00%,-10.00%", comparison.GetBias());
        }

        [TestMethod]
        public void TestGetYorN()
        {
            var customProject = new ProjectParameter
            {
                SignificantDigits = 2,
                SpecificationOneConcRange = 7,
                SpecificationOne = 0.1,
                SpecificationTwoConcRange = 7,
                SpecificationTwo = 25
            };
            List<string> sampleNameList = new List<string> { "1", "2", "3", "4" };
            List<double> targetSampleList = new List<double> { 1, 5, 10, 15 };
            List<double> matchSampleList = new List<double> { 1.2, 4.91, 12.5, 13.5 };

            var comparison = new Comparison(customProject, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);
            comparison.GetYorN();
            Assert.AreEqual("N,Y,N,Y", comparison.GetYorN());
        }

        //该方法已声明为private
        //[TestMethod]
        //public void TestGetOutliersSampleName_ExitOne()
        //{
        //    List<string> sampleNameList = new List<string> { "A", "B", "C", "D", "E" };
        //    List<double> targetSampleList = new List<double>() { 1, 2, 3, 4, 5 };
        //    List<double> matchSampleList = new List<double>() { 1, 2, 3, 4, 5 };
        //    var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

        //    int[] outliersIndexList = new int[] { 1 };

        //    var outlierSampleName = comparison.GetOutliersSampleName(outliersIndexList);

        //    Assert.AreEqual("A", string.Join(",", outlierSampleName));
        //}

        //[TestMethod]
        //public void TestGetOutliersSampleName_NotExit()
        //{
        //    List<string> sampleNameList = new List<string> { "A", "B", "C", "D", "E" };
        //    List<double> targetSampleList = new List<double>() { 1, 2, 3, 4, 5 };
        //    List<double> matchSampleList = new List<double>() { 1, 2, 3, 4, 5 };
        //    var comparison = new Comparison(project, sampleNameList, targetName, targetSampleList, matchName, matchSampleList, scriptPath);

        //    int[] outliersIndexList = new int[] { };

        //    var outlierSampleName = comparison.GetOutliersSampleName(outliersIndexList);

        //    Assert.AreEqual("", string.Join(",", outlierSampleName));
        //}
    }
}
