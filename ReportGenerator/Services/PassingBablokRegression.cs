using RDotNet;
using ReportGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReportGenerator.Services
{
    public class Comparison
    {
        private readonly PassingBablokRegression pb;
        private List<double> _targetSampleList;
        private List<double> _matchSampleList;
        private List<double> _bias;
        private List<double> _yORn;

        public Comparison(string targetInstrumentName,
            List<double> targetSampleList,
            string matchInstrumentName,
            List<double> matchSampleList,
            string scriptPath)
        {
            _targetSampleList = targetSampleList;
            _matchSampleList = matchSampleList;
            pb = new PassingBablokRegression(targetInstrumentName, targetSampleList, matchInstrumentName, matchSampleList, scriptPath);
            pb.Run();
        }

        public string GetTargetSampleList()
        {
            return string.Join(",", _targetSampleList);
        }

        public string GetMatchSampleList()
        {
            return string.Join(",", _matchSampleList);
        }

        public bool NumberIsLargerThan19()
        {
            return _targetSampleList.Count >= 20 && _matchSampleList.Count >= 20;
        }

        public bool NumberIsEqual()
        {
            return _targetSampleList.Count == _matchSampleList.Count;
        }

        public double a { get => double.Parse(pb.a); }
        public double aUCI { get => double.Parse(pb.aUCI); }
        public double aLCI { get => double.Parse(pb.aLCI); }

        public double b { get => double.Parse(pb.b); }
        public double bUCI { get => double.Parse(pb.bUCI); }
        public double bLCI { get => double.Parse(pb.bLCI); }

        public double P { get => double.Parse(pb.P); }
    }

    public class PassingBablokRegression : IPassingBablokRegression
    {
        private REngine engine;

        private string _scriptPath;
        private string _imagePath;

        private List<double> _targetSampleList;
        private List<double> _matchSampleList;
        private string _targetInstrumentName;
        private string _matchInstrumentName;

        public PassingBablokRegression(string targetInstrumentName,
            List<double> targetSampleList,
            string matchInstrumentName,
            List<double> matchSampleList,
            string scriptPath)
        {
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();

            _scriptPath = Path.Combine(scriptPath, "仪器比对报告后端.R").Replace("\\", "/");

            var PictureDir = Path.Combine(scriptPath, "Pictures");
            if (!Directory.Exists(PictureDir))
            {
                Directory.CreateDirectory(PictureDir);
            }
            _imagePath = Path.Combine(PictureDir, DateTime.Now.ToString("yyyyMMdd") + "-" + System.Guid.NewGuid().ToString() + ".png").Replace("\\", "/");

            _targetSampleList = targetSampleList;
            _matchSampleList = matchSampleList;
            _targetInstrumentName = targetInstrumentName;
            _matchInstrumentName = matchInstrumentName;
        }

        public void Run()
        {
            engine.SetSymbol("target", engine.CreateCharacter(_targetInstrumentName));
            engine.SetSymbol("match", engine.CreateCharacter(_matchInstrumentName));
            engine.SetSymbol("filename", engine.CreateCharacter(_imagePath));

            engine.SetSymbol("检测系统A结果", engine.CreateNumericVector(_targetSampleList));
            engine.SetSymbol("检测系统B结果", engine.CreateNumericVector(_matchSampleList));

            engine.Evaluate("source('" + _scriptPath + "')");
        }

        public string PicPath
        {
            get => _imagePath;
        }

        public string P
        {
            get => engine.GetSymbol("p").AsNumeric()[0].ToString();
        }

        public string a
        {
            get => engine.GetSymbol("a").AsNumeric()[0].ToString("F4");
        }
        public string aUCI
        {
            get => engine.GetSymbol("a.upper").AsNumeric()[0].ToString("F4");
        }
        public string aLCI
        {
            get => engine.GetSymbol("a.lower").AsNumeric()[0].ToString("F4");
        }

        public string b
        {
            get => engine.GetSymbol("b").AsNumeric()[0].ToString("F4");
        }
        public string bUCI
        {
            get => engine.GetSymbol("b.upper").AsNumeric()[0].ToString("F4");
        }
        public string bLCI
        {
            get => engine.GetSymbol("b.lower").AsNumeric()[0].ToString("F4");
        }

        public int[] GetOutliersIndexList()
        {
            return engine.GetSymbol("ID").AsInteger().ToArray();
        }
    }
}
