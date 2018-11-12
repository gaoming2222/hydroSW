using System;
using System.Text;

namespace Hydrology.Entity
{
    public class CReportData
    {
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 水量
        /// </summary>
        public Nullable<Decimal> Water { get; set; }
        /// <summary>
        /// 雨量
        /// </summary>
        public Nullable<Decimal> Rain { get; set; }
        /// <summary>
        /// 电压
        /// </summary>
        public Nullable<Decimal> Voltge { get; set; }
    }
}
