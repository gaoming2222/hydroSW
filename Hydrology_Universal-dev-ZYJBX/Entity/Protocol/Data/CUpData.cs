using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{
    public class CUpData
    {
        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 水位1
        /// </summary>
        public Nullable<Decimal> Water1 { get; set; }
        /// <summary>
        /// 水位2
        /// </summary>
        public Nullable<Decimal> Water2 { get; set; }
        /// <summary>
        /// 水位定时报
        /// </summary>
        public Dictionary<DateTime, Decimal> WaterList { get; set; }
        /// <summary>
        /// 水位2定时报
        /// </summary>
        public Dictionary<DateTime, Decimal> WaterList2 { get; set; }
        /// <summary>
        /// 雨量
        /// </summary>
        public Nullable<Decimal> Rain { get; set; }
        /// <summary>
        /// 电压
        /// </summary>
        public Decimal Voltage { get; set; }
        /// <summary>
        /// 多小时电压
        /// </summary>
        public Dictionary<DateTime, Decimal> VoltageList { get; set; }
        /// <summary>
        /// 水温
        /// </summary>
        public Nullable<Decimal> WaterTemp { get; set; }
        /// <summary>
        /// 多组水温
        /// </summary>
        public Dictionary<DateTime, Decimal> TempList { get; set; }
        /// <summary>
        /// 流量
        /// </summary>
        public Nullable<Decimal> WaterFlow { get; set; }
    }
}
