using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportGenerator.Services
{
    public class Comparison
    {
        private const string splitChar = ",";
        private readonly ProjectParameter _project;
        private readonly PassingBablokRegression pb;

        private List<string> _sampleNameList;
        private List<double> _targetResultList;
        private List<double> _matchResultList;
        private List<string> _biasList;
        private List<string> _yORnList;

        public Comparison(ProjectParameter project,
            List<string> sampleNameList,
            string targetInstrumentName,
            List<double> targetSampleList,
            string matchInstrumentName,
            List<double> matchSampleList,
            string scriptPath)
        {
            _project = project;

            _sampleNameList = sampleNameList;
            _targetResultList = targetSampleList;
            _matchResultList = matchSampleList;
            _biasList = new List<string>();
            _yORnList = new List<string>();

            pb = new PassingBablokRegression(scriptPath,
                targetInstrumentName,
                _targetResultList,
                matchInstrumentName,
                _matchResultList);
        }

        public void Run()
        {
            Remark = "";
            try
            {
                pb.SetTargetSampleList(ConvertSampleListToListDouble(GetTargetSampleList()));
                pb.SetMatchSampleList(ConvertSampleListToListDouble(GetMatchSampleList()));
                pb.Run();
            }
            catch (Exception e)
            {
                throw new ApplicationException($"调用R语言模块计算时出错（{e.Message}），程序退出！");
            }
            SetRemark();
        }

        private List<double> ConvertSampleListToListDouble(string SampleList)
        {
            return SampleList.Split(splitChar).Select(i => double.Parse(i)).ToList();
        }

        public string GetSampleNameList()
        {
            return string.Join(splitChar, _sampleNameList);
        }

        /// <summary>
        /// 返回保留有效数字位数后的靶仪器数据
        /// </summary>
        /// <returns>string</returns>
        public string GetTargetSampleList()
        {
            return string.Join(splitChar,
                _targetResultList.Select(value =>
                SignificantDigits.Reserved(value, SignificantDigit))
                .ToArray());
        }

        /// <summary>
        /// 返回保留有效数字位数后的比对数据数据
        /// </summary>
        /// <returns>string</returns>
        public string GetMatchSampleList()
        {
            return string.Join(splitChar,
                _matchResultList.Select(value =>
                SignificantDigits.Reserved(value, SignificantDigit))
                .ToArray());
        }

        /// <summary>
        /// 判断靶仪器数据和比对仪器数据的个数是不是都大于20个
        /// </summary>
        /// <returns></returns>
        public bool NumberIsLargerThan19()
        {
            return NumberIsLargerThan(_targetResultList, 19) &&
                NumberIsLargerThan(_matchResultList, 19);
        }

        private bool NumberIsLargerThan<T>(List<T> SampleList, int target)
        {
            return SampleList.Count > target;
        }

        /// <summary>
        /// 判断靶仪器数据和比对仪器数据的个数是不是一样
        /// </summary>
        /// <returns></returns>
        public bool NumberIsEqual()
        {
            return _targetResultList.Count == _matchResultList.Count;
        }

        /// <summary>
        /// 判断靶仪器数据和比对仪器数据是不是相同，相同说明原始数据粘贴错误
        /// </summary>
        /// <returns></returns>
        public bool VerifySampleResultIsDifferent()
        {
            var SameResultCount = 0;
            for (int i = 0; i < _targetResultList.Count; i++)
            {
                if (_targetResultList[i] == _matchResultList[i])
                {
                    SameResultCount++;
                }
            }
            return SameResultCount != _targetResultList.Count;
        }

        public string GetBias()
        {
            // ToDo 去除离群值后需要重新计算
            if (_biasList.Count == 0)
            {
                CalculateBias();
            }
            return string.Join(splitChar, _biasList);
        }

        public string GetYorN()
        {
            // ToDo 去除离群值后需要重新计算
            if (_yORnList.Count == 0)
            {
                CalculateBias();
            }
            return string.Join(splitChar, _yORnList);
        }

        private void CalculateBias()
        {
            var significantDigit = SignificantDigit;
            var cutOffValue = _project.SpecificationOneConcRange;
            var specificationOne = _project.SpecificationOne;
            var specificationTwo = _project.SpecificationTwo / 100;

            for (int i = 0; i < _targetResultList.Count; i++)
            {
                var targetValue = _targetResultList[i];
                var matchValue = _matchResultList[i];

                if (targetValue <= cutOffValue)
                {
                    string diff = (matchValue - targetValue).ToString("F" + significantDigit);
                    _biasList.Add(diff);
                    if (Math.Abs(double.Parse(diff)) < specificationOne)
                    {
                        _yORnList.Add("Y");
                    }
                    else
                    {
                        _yORnList.Add("N");
                    }
                }
                else
                {
                    string bias = ((matchValue - targetValue) / targetValue).ToString("F4");
                    _biasList.Add(double.Parse(bias).ToString("P2"));
                    if (Math.Abs(double.Parse(bias)) < specificationTwo)
                    {
                        _yORnList.Add("Y");
                    }
                    else
                    {
                        _yORnList.Add("N");
                    }
                }
            }
        }

        // 跟RunPB可能形成死循环************************************
        //public void RemoveSingleOutliersAndReRun(int outLiersIndex)
        //{
        //    /* 
        //     * 如果样品数量*大于*20个
        //     *      保留targetSampleList和matchSampleList的原始副本
        //     *      在targetSampleList和matchSampleList中去除离群值
        //     *      重新传入targetSampleList和matchSampleList到pb
        //     *      调用pb
        //     *      如果此时又出现离群值
        //     *          更新备注
        //     *          退出
        //     *      如果此时无离群值
        //     *          更新SampleNameList
        //     *          更新备注
        //     * 否则
        //     *      更新备注
        //     *      退出                  
        //     */
        //    if (NumberIsEqual() &&
        //        NumberIsLargerThan(_targetSampleList, 20) &&
        //        NumberIsLargerThan(_matchSampleList, 20))
        //    {
        //        var outliers = _sampleNameList.ElementAt(outLiersIndex - 1);
        //        var originTargetSampleList = _targetSampleList;
        //        _targetSampleList.RemoveAt(outLiersIndex - 1);
        //        var originMatchSampleList = _matchSampleList;
        //        _matchSampleList.RemoveAt(outLiersIndex - 1);

        //        pb.Run();

        //        var outliersIndexList = GetOutliers();
        //        if (outliersIndexList.Count == 0)
        //        {
        //            _sampleNameList.RemoveAt(outLiersIndex - 1);
        //            CalculateBias();
        //            SetRemark($"结果已剔除离群值：{outliers}");
        //        }
        //        else
        //        {
        //            _targetSampleList = originTargetSampleList;
        //            _matchSampleList = originMatchSampleList;

        //            pb.Run();

        //            var newOutliersSampleName = outliersIndexList.Select(i => _sampleNameList.ElementAt(i - 1));
        //            SetRemark($"去除离群值：{outliers}后又出现新的离群值：{string.Join(splitChar, newOutliersSampleName)}" +
        //                $"，请挑选数据后重新提交");
        //        }
        //        return;
        //    }
        //    else
        //    {
        //        SetRemark("若去除离群值，数据量将小于20个，不满足文件要求，请补充数据后重新提交");
        //        return;
        //    }
        //}

        private List<string> GetOutliersSampleName(int[] outliersIndexList)
        {
            //int[] OutliersIndexList = pb.GetOutliersIndexList();
            List<string> sampleNames = new List<string>();
            for (int i = 0; i < outliersIndexList.Length; i++)
            {
                sampleNames.Add(_sampleNameList.ElementAt(outliersIndexList[i] - 1));
            }
            return sampleNames;
        }

        private void SetRemark()
        {
            /*
             * 一致性判断
             * P值 <= 0.1
             * 最大SE/Xc%未通过
             * 离群值
             */
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < _yORnList.Count; i++)
            {
                var yn = _biasList[i];
                if (yn == "N")
                {
                    var samplename = _sampleNameList[i];
                    stringBuilder.Append($"{samplename}一致性判断未通过。");
                    Status = "未通过";
                }
            }

            var outliersSampleList = GetOutliersSampleName(pb.GetOutliersIndexList());
            if (outliersSampleList.Count == 1)
            {
                stringBuilder.Append($"{string.Join(splitChar, outliersSampleList)}为离群值，请剔除数据后加做比对补充。");
            }
            else if (outliersSampleList.Count > 1)
            {
                stringBuilder.Append($"{string.Join(splitChar, outliersSampleList)}为离群值，请查找比对差异较大的原因，重新比对。");
            }

            if (P <= 0.1)
            {
                stringBuilder.Append("两组数据线性相关性未通过: P<=0.10。");
                Status = "未通过";
            }

            if (GetMaxSEXc(Xc1) * 2 > ALE)
            {
                stringBuilder.Append($"医学决定水平处({Xc1})最大SE/Xc%={GetMaxSEXc(Xc1):P1} > 1/2ALE。");
                Status = "未通过";
            }

            if (GetMaxSEXc(Xc2) * 2 > ALE)
            {
                stringBuilder.Append($"医学决定水平处({Xc2})最大SE/Xc%={GetMaxSEXc(Xc2):P1} > 1/2ALE。");
                Status = "未通过";
            }

            Remark = stringBuilder.ToString();
        }

        /// <summary>
        /// 计算最大SE/Xc%，保留3位小数
        /// </summary>
        /// <param name="Xc">医学决定水平</param>
        /// <returns></returns>
        private double GetMaxSEXc(double Xc)
        {
            return Math.Round(Math.Max(Math.Abs((bLCI * Xc + aLCI) / Xc - 1),
                            Math.Abs((bUCI * Xc + aUCI) / Xc - 1)), 3);
        }

        public double ALE { get => _project.GetALE(); }
        public double Xc1 { get => _project.Xc1; }
        public double Xc2 { get => _project.Xc2; }
        public string Unit { get => _project.Unit; }
        public int SignificantDigit { get => _project.SignificantDigits; }

        public double a { get => double.Parse(pb.a); }
        public double aUCI { get => double.Parse(pb.aUCI); }
        public double aLCI { get => double.Parse(pb.aLCI); }

        public double b { get => double.Parse(pb.b); }
        public double bUCI { get => double.Parse(pb.bUCI); }
        public double bLCI { get => double.Parse(pb.bLCI); }

        public double P { get => double.Parse(pb.P); }

        public string PicturePath { get => pb.PicPath; }
        public string Remark { get; private set; }

        public string Status { get; private set; } = "待审核";
    }
}
