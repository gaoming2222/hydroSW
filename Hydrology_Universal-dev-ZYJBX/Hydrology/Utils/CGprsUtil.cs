using System;
using System.Text;

namespace Hydrology.Utils
{
    public static class CGprsUtil
    {
        private const double YEAR_1970 = 25569.3333333333;
        private const double SEC_PER_DAY = 86400;

        public static string ByteArrayToHexString(byte[] bts)
        {
            if (bts == null || bts.Length == 0) return string.Empty;
            return ByteArrayToHexString(bts, 0, bts.Length);
        }
        public static string ByteArrayToHexString(byte[] bts, int startIndex, int lenth)
        {
            if (bts == null || lenth == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int ii = startIndex; ii < bts.Length && ii < startIndex + lenth; ii++)
            {
                string sh = bts[ii].ToString("X").PadLeft(2, '0');
                sb.Append(sh + " ");
            }
            return sb.ToString();
        }
        public static bool ByteArrayFromHexString(string hexStr, out byte[] bts)
        {
            try
            {
                hexStr = hexStr.Trim();
                string[] ss = hexStr.Split(new char[] { ' ', ',', ';', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);
                bts = new byte[ss.Length];
                for (int ii = 0; ii < ss.Length; ii++)
                {
                    bts[ii] = byte.Parse(ss[ii], System.Globalization.NumberStyles.HexNumber);
                }
                return true;
            }
            catch
            {
                bts = null;
                return false;
            }
        }
        public static string DTUIDToString(uint dtuID)
        {
            return dtuID.ToString("x").PadLeft(8, '0');
        }
        public static UInt32 Byte4ToDTUID(byte[] bts, int startIndex)
        {
            UInt32 id = (uint)bts[startIndex]
                + (uint)0x100 * bts[startIndex + 1]
                + (uint)0x10000 * bts[startIndex + 2]
                + (uint)0x1000000 * bts[startIndex + 3];
            return id;
        }
        public static string Byte4ToIP(byte[] bts, int startIndex)
        {
            string ips = string.Empty;
            for (int ii = startIndex; ii < startIndex + 4; ii++)
            {
                if (ips.Length != 0) ips = ips + ".";
                ips = ips + bts[ii].ToString();
            }
            return ips;
        }
        public static string Byte11ToPhoneNO(byte[] bts, int startIndex)
        {
            string phn = string.Empty;
            for (int ii = startIndex; ii < startIndex + 11; ii++)
            {
                phn = phn + Convert.ToChar(bts[ii]);
            }
            return phn;
        }
        public static DateTime ULongToDatetime(ulong value)
        {
            DateTime dt = DateTime.FromOADate(YEAR_1970 + value / SEC_PER_DAY);
            return dt;
        }
        public static void Wait(uint milsecond)
        {
            long waittick = (long)10000 * milsecond;
            long tick = DateTime.Now.Ticks;
            while (tick + waittick > DateTime.Now.Ticks)
            {
                System.Threading.Thread.Sleep(100);
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
