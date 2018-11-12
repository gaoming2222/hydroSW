using System;

namespace Hydrology.Entity
{
    public class CEntityRain
    {
        #region PROPERTY

        /// <summary>
        /// 对应于每条雨量记录的唯一索引，主键
        /// </summary>
        public long RainID { get; set; }

        /// <summary>
        ///  测站中心的ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        ///  雨量值的采集时间
        /// </summary>
        public DateTime TimeCollect { get; set; }

        /// <summary>
        /// 时段雨量
        /// </summary>
        public Nullable<Decimal> PeriodRain { get; set; }

        /// <summary>
        /// 差值雨量
        /// </summary>
        public Nullable<Decimal> DifferneceRain { get; set; }

        /// <summary>
        /// 累计雨量
        /// </summary>
        public Nullable<Decimal> TotalRain { get; set; }

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType ChannelType { get; set; }
        /// <summary>
        /// 报文类型
        /// </summary>
        public EMessageType MessageType { get; set; }

        /// <summary>
        /// 日雨量，只有8时才有值
        /// </summary>
        public Nullable<Decimal> DayRain { get; set; }

        /// <summary>
        /// 系统接收数据的时间
        /// </summary>
        public DateTime TimeRecieved { get; set; }

        /// 当前记录状态
        /// </summary>
        public int BState { get; set; }

        #endregion
    }
}
