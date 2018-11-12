using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Utils
{
    class CStringUtil
    {
        public static bool IsDigit(string str)
        {
            if (String.IsNullOrEmpty(str))
                return false;
            foreach (var ch in str)
            {
                if (!char.IsDigit(ch))
                    return false;
            }
            return true;
        }
        public static bool IsDigitStrWithSpecifyLength(string str, int length)
        {
            if (str == null)
                return false;
            bool isSpecifyLength = str.Length == length;
            if (!isSpecifyLength)
                return false;
            bool isDigit = IsDigit(str);
            return (isSpecifyLength && isDigit);
        }
        public static bool IsDigitOrAlpha(string str)
        {
            if (String.IsNullOrEmpty(str))
                return false;
            foreach (var ch in str)
            {
                if (!((ch >= 'a' && ch <= 'z')
                    || (ch >= 'A' && ch <= 'Z')
                    || char.IsDigit(ch)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
