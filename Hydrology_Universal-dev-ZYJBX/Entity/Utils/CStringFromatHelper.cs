using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Hydrology.Entity
{
    /// <summary>
    /// 字符串分析类
    /// </summary>
    public static class CStringFromatHelper
    {
        /// <summary>
        /// 判断字符串都是由字母组成
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigit(String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                // 不能是空的字符串
                return false;
            }
            for (int i = 0; i < str.Length; ++i)
            {
                if (!Char.IsDigit(str[i]))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 字符串转换成可控Decimal
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Nullable<Decimal> ConvertToNullableDecimal(String str)
        {
            Nullable<Decimal> result = null;
            try
            {
                result = Decimal.Parse(str);
            }
            catch (System.Exception)
            {
                Debug.WriteLine("ConvertToNullableDecimal error");
            }
            return result;
        }

        public static Nullable<float> ConvertToNullableFloat(String str)
        {
            Nullable<float> result = null;
            try
            {
                result = float.Parse(str);
            }
            catch (System.Exception)
            {
                Debug.WriteLine("ConvertToNullableFloat error");
            }
            return result;
        }

        private static readonly string CS_NULL_UI = "---";
        /// <summary>
        /// 转换成界面字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUIString(Nullable<Decimal> value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString();
            }
            else
            {
                return CS_NULL_UI;
            }
        }
        public static string ToUIString(Nullable<float> value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString();
            }
            else
            {
                return CS_NULL_UI;
            }
        }

        public static string ToUIString2Number(Nullable<float> value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString("0.00");
            }
            else
            {
                return CS_NULL_UI;
            }
        }

        public static string ToUIString3Number(Nullable<float> value)
        {
            if (value.HasValue)
            {
                return value.Value.ToString("0.000");
            }
            else
            {
                return CS_NULL_UI;
            }
        }
    }


}
