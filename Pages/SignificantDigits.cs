using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReportGenerator.Pages
{
    /// <summary>
    /// 将num保留n位有效数字
    /// 规则：四舍六入五考虑，五后非空就进一，五后为空看奇偶，五前为偶应舍去，五前为奇要进一
    /// </summary>
    public class SignificantDigits
    {
        public static string Reserved(double num, int n)
        {
            if (num == 0) return "0";

            if (n <= 0 || n > 28)
                throw new ArgumentOutOfRangeException("有效数字位数不能小于等于0或大于28");

            decimal result, number;
            try
            {
                number = decimal.Parse(num.ToString());
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("非法输入");
            }

            int time = 0;   // number除以10或乘以10的次数
            int pointIndex = number.ToString().IndexOf('.');  // 小数点位置(正数：-1, 含小数：>=1, 负数：2）
            int negative = number.ToString().StartsWith('-') ? 1 : 0;

            if (Math.Abs(number) > 1)
            {
                if (n >= pointIndex && pointIndex > 0)
                {
                    result = Math.Round(number, n - pointIndex + negative, MidpointRounding.ToEven);
                    return result.ToString($"F{n - pointIndex + negative}");
                }
                else
                {
                    while (Math.Abs(number) > 1)
                    {
                        number *= (decimal)0.1;
                        time++;
                    }
                    result = Math.Round(number, n, MidpointRounding.ToEven) * (decimal)Math.Pow(10, time);
                    return result.ToString($"G{n}");
                }
            }
            else if (Math.Abs(number) == 1)
            {
                // num传入为1.000时，被自动转换为1（原因？），导致pointIndex=-1
                return number.ToString($"F{n + pointIndex}");
            }
            else if (Math.Abs(number) >= (decimal)0.1)
            {
                result = Math.Round(number, n, MidpointRounding.ToEven);
                return result.ToString($"F{n}");
            }
            else
            {
                while (Math.Abs(number) < (decimal)0.1)
                {
                    number *= 10;
                    time--;
                }
                result = Math.Round(number, n, MidpointRounding.ToEven) * (decimal)Math.Pow(10, time);
                return result.ToString($"F{n - time}");
            }
        }
    }
}
