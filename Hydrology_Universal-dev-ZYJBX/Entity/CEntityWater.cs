using System;

namespace Hydrology.Entity
{
    public class CEntityWater
    {
        #region PROPERTY

        /// <summary>
        /// 对应于每条水量记录的唯一索引，主键
        /// </summary>
        public long WaterID { get; set; }

        /// <summary>
        ///  测站中心的ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        ///  雨量值的采集时间
        /// </summary>
        public DateTime TimeCollect { get; set; }

        /// <summary>
        /// 系统接收数据的时间
        /// </summary>
        public DateTime TimeRecieved { get; set; }

        /// <summary>
        /// 水位
        /// </summary>
        public Decimal WaterStage { get; set; }

        /// <summary>
        /// 流量
        /// </summary>
        public Nullable<Decimal> WaterFlow { get; set; }

        /// <summary>
        /// 报文类型
        /// </summary>
        public EMessageType MessageType { get; set; }

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType ChannelType { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public int state { get; set; }

        #endregion
    }
}
