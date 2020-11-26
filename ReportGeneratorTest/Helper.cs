using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Linq;

namespace ReportGeneratorTest
{
    static class Helper
    {
        /// <summary>
        /// 读取测试数据和正确结果
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> DataReader(string filePath)
        {
            Dictionary<string, List<string>> Data = new Dictionary<string, List<string>>() { };
            Data.Add("sampleName", new List<string>());
            Data.Add("target", new List<string>());
            Data.Add("match", new List<string>());
            Data.Add("a", new List<string>());
            Data.Add("aLCI", new List<string>());
            Data.Add("aUCI", new List<string>());
            Data.Add("b", new List<string>());
            Data.Add("bLCI", new List<string>());
            Data.Add("bUCI", new List<string>());
            Data.Add("OutLiersNumber", new List<string>());
            Data.Add("OutLiers", new List<string>());

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    try
                    {
                        double value;
                        if (double.TryParse(fields[1], out value))
                        {
                            Data["sampleName"].Add(fields[0]);
                            Data["target"].Add(fields[1]);
                            Data["match"].Add(fields[2]);
                        }
                    }
                    catch
                    {
                    }

                    if (fields[3] == "a")
                    {
                        Data["a"].Add(fields[4]);
                    }
                    else if (fields[3] == "aUCI")
                    {
                        Data["aUCI"].Add(fields[4]);
                    }
                    else if (fields[3] == "aLCI")
                    {
                        Data["aLCI"].Add(fields[4]);
                    }
                    else if (fields[3] == "b")
                    {
                        Data["b"].Add(fields[4]);
                    }
                    else if (fields[3] == "bUCI")
                    {
                        Data["bUCI"].Add(fields[4]);
                    }
                    else if (fields[3] == "bLCI")
                    {
                        Data["bLCI"].Add(fields[4]);
                    }
                    else if (fields[3] == "OutLiersNumber")
                    {
                        Data["OutLiersNumber"].Add(fields[4]);
                    }
                    else if (fields[3] == "OutLiers")
                    {
                        var name = fields[4].Split(",");
                        foreach (var n in name)
                        {
                            Data["OutLiers"].Add(n);
                        }
                    }
                }
            }

            return Data;
        }

        public static List<double> ConvertSampleListToListDouble(string SampleList)
        {
            return SampleList.Split(",").Select(i => double.Parse(i)).ToList();
        }
    }
}
