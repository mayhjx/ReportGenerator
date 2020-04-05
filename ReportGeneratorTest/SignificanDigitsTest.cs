using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReportGenerator.Pages;

namespace ReportGeneratorTests
{
    [TestClass]
    public class SignificantDigitsTest
    {
        // 四舍六入五考虑，五后非空就进一，五后为空看奇偶，五前为偶应舍去，五前为奇要进一

        [TestMethod]
        public void Reversed()
        {

            Assert.AreEqual("0", SignificantDigits.Reserved(0, 3));

            // 0 < n < 29
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => SignificantDigits.Reserved(1, 29));
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => SignificantDigits.Reserved(1, 0));

            Assert.AreEqual("1.235E+08", SignificantDigits.Reserved(123456789, 4));
            Assert.AreEqual("0.1235", SignificantDigits.Reserved(0.123456789, 4));
            Assert.AreEqual("0.001235", SignificantDigits.Reserved(0.00123456789, 4));

            // number >= 1
            Assert.AreEqual("1.0", SignificantDigits.Reserved(1, 2));
            Assert.AreEqual("1.2340", SignificantDigits.Reserved(1.234, 5));
            Assert.AreEqual("25.0", SignificantDigits.Reserved(25, 3)); 
            Assert.AreEqual("1000.00", SignificantDigits.Reserved(1000, 6)); 
            Assert.AreEqual("1.1", SignificantDigits.Reserved(1.14, 2));
            Assert.AreEqual("1.2", SignificantDigits.Reserved(1.16, 2));
            Assert.AreEqual("1.3", SignificantDigits.Reserved(1.251, 2));
            Assert.AreEqual("1.2", SignificantDigits.Reserved(1.25, 2));
            Assert.AreEqual("1.2", SignificantDigits.Reserved(1.15, 2));

            // 0.1 <= number < 1
            Assert.AreEqual("0.10", SignificantDigits.Reserved(0.100, 2));
            Assert.AreEqual("0.11", SignificantDigits.Reserved(0.114, 2));
            Assert.AreEqual("0.12", SignificantDigits.Reserved(0.116, 2));
            Assert.AreEqual("0.13", SignificantDigits.Reserved(0.1251, 2));
            Assert.AreEqual("0.12", SignificantDigits.Reserved(0.125, 2));
            Assert.AreEqual("0.12", SignificantDigits.Reserved(0.115, 2));

            // 0 < number < 0.1
            Assert.AreEqual("0.011", SignificantDigits.Reserved(0.0114, 2));
            Assert.AreEqual("0.012", SignificantDigits.Reserved(0.0116, 2));
            Assert.AreEqual("0.013", SignificantDigits.Reserved(0.01251, 2));
            Assert.AreEqual("0.012", SignificantDigits.Reserved(0.0125, 2));
            Assert.AreEqual("0.012", SignificantDigits.Reserved(0.0115, 2));

            Assert.AreEqual("0.01234506", SignificantDigits.Reserved(0.012345055, 7));
            Assert.AreEqual("0.01234506", SignificantDigits.Reserved(0.012345065, 7));

            // minus
            Assert.AreEqual("-1.235", SignificantDigits.Reserved(-1.23456789, 4));
            Assert.AreEqual("-0.1235", SignificantDigits.Reserved(-0.123456789, 4));
            Assert.AreEqual("-0.01235", SignificantDigits.Reserved(-0.0123456789, 4));

            // tailling zores
            Assert.AreEqual("0.1250", SignificantDigits.Reserved(0.125000, 4));
            Assert.AreEqual("0.012500", SignificantDigits.Reserved(0.0125000, 5));

        }
    }
}
