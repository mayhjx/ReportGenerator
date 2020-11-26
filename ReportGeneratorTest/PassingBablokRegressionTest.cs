using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportGenerator.Services;
using System.IO;
using System.Linq;

namespace ReportGeneratorTest
{
    [TestClass]
    public class PassingBablokRegressionTest
    {
        private string targetName;
        private string matchName;
        private string scriptPath;

        public PassingBablokRegressionTest()
        {
            targetName = "target";
            matchName = "match";
            scriptPath = @"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGenerator\wwwroot";
        }

        [DataTestMethod]
        [DataRow(@"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGeneratorTest\测试数据.csv")]
        [DataRow(@"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGeneratorTest\测试数据1.csv")]
        public void TestRun(string filePath)
        {
            var data = Helper.DataReader(filePath);
            var targetSampleList = data["target"].Select(x => double.Parse(x)).ToList();
            var matchSampleList = data["match"].Select(x => double.Parse(x)).ToList();
            var pb = new PassingBablokRegression(scriptPath, targetName, targetSampleList, matchName, matchSampleList);

            pb.Run();

            Assert.IsTrue(File.Exists(pb.PicPath));
            if (File.Exists(pb.PicPath))
            {
                File.Delete(pb.PicPath);
            }

            Assert.AreEqual(double.Parse(data["a"][0]).ToString("F4"), pb.a, "a is not equal");
            Assert.AreEqual(double.Parse(data["aLCI"][0]).ToString("F4"), pb.aLCI, "aLCI is not equal");
            Assert.AreEqual(double.Parse(data["aUCI"][0]).ToString("F4"), pb.aUCI, "aUCI is not equal");
            Assert.AreEqual(double.Parse(data["b"][0]).ToString("F4"), pb.b, "b is not equal");
            Assert.AreEqual(double.Parse(data["bLCI"][0]).ToString("F4"), pb.bLCI, "bLCI is not equal");
            Assert.AreEqual(double.Parse(data["bUCI"][0]).ToString("F4"), pb.bUCI, "bUCI is not equal");
            Assert.AreEqual("0.11", pb.P, "p is not equal");
            Assert.AreEqual(int.Parse(data["OutLiersNumber"][0]), pb.GetOutliersIndexList().Length, "outliers number is not equal");
            Assert.AreEqual(string.Join(",", data["OutLiers"]), string.Join(",", pb.GetOutliersIndexList()), "outliers is not equal");
        }
    }
}
