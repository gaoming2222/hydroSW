using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Hydrology.DataMgr
{
    public class SystemTimeConfig
    {
        [DllImportAttribute("Kernel32.dll")]

        public static extern void GetLocalTime(SystemTime st);

        [DllImportAttribute("Kernel32.dll")]

        public static extern void SetLocalTime(SystemTime st);
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public class SystemTime
    {

        public ushort vYear;

        public ushort vMonth;

        public ushort vDayOfWeek;

        public ushort vDay;

        public ushort vHour;

        public ushort vMinute;

        public ushort vSecond;

    }
}
