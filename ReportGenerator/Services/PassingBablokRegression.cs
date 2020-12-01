using RDotNet;
using ReportGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReportGenerator.Services
{
    /// <summary>
    /// 要将 "C:\R-3.5.3\bin\x64 添加到系统环境路径中
    /// The problem is actually not that stats.dll could not be found, 
    /// but that a.dll that stats.dll depends on could not be loaded - 
    /// I've found this out by inspecting the stats.dll file using a tool called Dependency Walker. 
    /// In fact, while loading the stats package, the files "R.dll", "Rblas.dll" and
    /// "Rlapack.dll" could not be found - they all lie in the "bin" directory of the R installation, e.g. "R-3.5.3\bin\x64".
    /// https://github.com/NetLogo/R-Extension/issues/5 @gunnardressler
    /// </summary>
    public class PassingBablokRegression : IPassingBablokRegression
    {

        private readonly REngine engine;

        private const string PictureFolder = "Pictures";
        private readonly string _scriptPath;
        private readonly string _imagePath;

        private string _targetInstrumentName;
        private List<double> _targetSampleList;
        private string _matchInstrumentName;
        private List<double> _matchSampleList;

        public PassingBablokRegression(string scriptPath)
        {
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();

            // 路径分隔符修改为 "/"
            _scriptPath = Path.Combine(scriptPath, "仪器比对报告后端.R").Replace("\\", "/");

            var PictureDir = Path.Combine(scriptPath, PictureFolder);
            if (!Directory.Exists(PictureDir))
            {
                Directory.CreateDirectory(PictureDir);
            }
            _imagePath = Path.Combine(PictureDir, DateTime.Now.ToString("yyyyMMdd") + "-" + System.Guid.NewGuid().ToString() + ".svg").Replace("\\", "/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scriptPath">脚本所在位置</param>
        /// <param name="targetInstrumentName">靶仪器编号</param>
        /// <param name="targetSampleList">已保留有效数字位数的靶仪器数据</param>
        /// <param name="matchInstrumentName">比对仪器编号</param>
        /// <param name="matchSampleList">已保留有效数字位数的比对仪器数据</param>
        public PassingBablokRegression(string scriptPath,
            string targetInstrumentName,
            List<double> targetSampleList,
            string matchInstrumentName,
            List<double> matchSampleList) : this(scriptPath)
        {
            _targetSampleList = targetSampleList;
            _matchSampleList = matchSampleList;
            _targetInstrumentName = targetInstrumentName;
            _matchInstrumentName = matchInstrumentName;
        }

        public void SetTargetInstrumentName(string name)
        {
            _targetInstrumentName = name;
        }

        public void SetMatchInstrumentName(string name)
        {
            _matchInstrumentName = name;
        }

        public void SetTargetSampleList(List<double> targetSampleList)
        {
            _targetSampleList = targetSampleList;
        }

        public void SetMatchSampleList(List<double> matchSampleList)
        {
            _matchSampleList = matchSampleList;
        }

        public void Run()
        {
            if (_targetInstrumentName == null ||
                _matchInstrumentName == null ||
                _targetSampleList == null ||
                _matchSampleList == null)
            {
                throw new ArgumentNullException("参数未设置");
            }

            engine.SetSymbol("target", engine.CreateCharacter(_targetInstrumentName));
            engine.SetSymbol("match", engine.CreateCharacter(_matchInstrumentName));
            engine.SetSymbol("filename", engine.CreateCharacter(_imagePath));

            engine.SetSymbol("检测系统A结果", engine.CreateNumericVector(_targetSampleList));
            engine.SetSymbol("检测系统B结果", engine.CreateNumericVector(_matchSampleList));

            engine.Evaluate("source('" + _scriptPath + "')");
        }

        public string PicPath
        {
            get => Path.Combine(@"\", PictureFolder, Path.GetFileName(_imagePath));
        }

        public string CompletePicPath
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
            var outliersIndexList = engine.GetSymbol("ID").AsInteger();
            if (outliersIndexList == null)
            {
                return new int[0];
            }
            else
            {
                return outliersIndexList.ToArray();
            }
        }
    }
}
