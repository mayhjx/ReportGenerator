using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportGenerator.Services;

namespace ReportGeneratorTest
{
    [TestClass]
    public class PassingBablokRegressionTest
    {
        private string targetName;
        private string matchName;
        private string picPath;

        public PassingBablokRegressionTest()
        {
            targetName = "target";
            matchName = "match";
            picPath = @"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGenerator\wwwroot";
        }

        [TestMethod]
        public void TestRun()
        {
            var filePath = @"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGeneratorTest\测试数据.csv";
            var data = DataReader(filePath);
            var pb = new PassingBablokRegression(targetName, data["target"], matchName, data["match"], picPath);

            pb.Run();

            Assert.IsTrue(File.Exists(pb.PicPath));
            if (File.Exists(pb.PicPath))
            {
                File.Delete(pb.PicPath);
            }

            Assert.AreEqual(data["a"][0].ToString("F4"), pb.a, "a is not equal");
            Assert.AreEqual(data["aLCI"][0].ToString("F4"), pb.aLCI, "aLCI is not equal");
            Assert.AreEqual(data["aUCI"][0].ToString("F4"), pb.aUCI, "aUCI is not equal");
            Assert.AreEqual(data["b"][0].ToString("F4"), pb.b, "b is not equal");
            Assert.AreEqual(data["bLCI"][0].ToString("F4"), pb.bLCI, "bLCI is not equal");
            Assert.AreEqual(data["bUCI"][0].ToString("F4"), pb.bUCI, "bUCI is not equal");
            Assert.AreEqual("0.11", pb.P, "p is not equal");
            Assert.AreEqual(data["OutLiersNumber"][0], pb.GetOutliersIndexList().Length);
        }

        [TestMethod]
        public void TestRun1()
        {
            var filePath = @"C:\Users\lihua\source\repos\ReportGeneratorV2\ReportGeneratorTest\测试数据1.csv";
            var data = DataReader(filePath);
            var pb = new PassingBablokRegression(targetName, data["target"], matchName, data["match"], picPath);

            pb.Run();

            Assert.IsTrue(File.Exists(pb.PicPath));
            if (File.Exists(pb.PicPath))
            {
                File.Delete(pb.PicPath);
            }

            Assert.AreEqual(data["a"][0].ToString("F4"), pb.a, "a is not equal");
            Assert.AreEqual(data["aLCI"][0].ToString("F4"), pb.aLCI, "aLCI is not equal");
            Assert.AreEqual(data["aUCI"][0].ToString("F4"), pb.aUCI, "aUCI is not equal");
            Assert.AreEqual(data["b"][0].ToString("F4"), pb.b, "b is not equal");
            Assert.AreEqual(data["bLCI"][0].ToString("F4"), pb.bLCI, "bLCI is not equal");
            Assert.AreEqual(data["bUCI"][0].ToString("F4"), pb.bUCI, "bUCI is not equal");
            Assert.AreEqual("0.11", pb.P, "p is not equal");
            Assert.AreEqual(data["OutLiersNumber"][0], pb.GetOutliersIndexList().Length);
        }

        /// <summary>
        /// 读取测试数据和正确结果
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Dictionary<string, List<double>> DataReader(string filePath)
        {
            Dictionary<string, List<double>> Data = new Dictionary<string, List<double>>() { };
            Data.Add("target", new List<double>());
            Data.Add("match", new List<double>());
            Data.Add("a", new List<double>());
            Data.Add("aLCI", new List<double>());
            Data.Add("aUCI", new List<double>());
            Data.Add("b", new List<double>());
            Data.Add("bLCI", new List<double>());
            Data.Add("bUCI", new List<double>());
            Data.Add("OutLiersNumber", new List<double>());

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    try
                    {
                        Data["target"].Add(double.Parse(fields[1]));
                        Data["match"].Add(double.Parse(fields[2]));
                    }
                    catch
                    {
                    }

                    if (fields[3] == "a")
                    {
                        Data["a"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "aUCI")
                    {
                        Data["aUCI"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "aLCI")
                    {
                        Data["aLCI"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "b")
                    {
                        Data["b"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "bUCI")
                    {
                        Data["bUCI"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "bLCI")
                    {
                        Data["bLCI"].Add(double.Parse(fields[4]));
                    }
                    else if (fields[3] == "OutLiersNumber")
                    {
                        Data["OutLiersNumber"].Add(double.Parse(fields[4]));
                    }
                }
            }

            return Data;
        }
    }
}
