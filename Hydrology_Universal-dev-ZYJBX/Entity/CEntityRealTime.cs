using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    /// <summary>
    /// 实时最新数据实体
    /// </summary>
    [XmlRoot("RealTimeItem")]
    public class CEntityRealTime
    {
        public CEntityRealTime()
        {
            this.BIsReceivedValid = true; //默认都是True
            this.ERTDState = ERTDDataState.ENormal; //默认正常
            this.EIChannelType = EChannelType.GPRS;
            this.EIMessageType = EMessageType.EAdditional;
        }
        #region NO_USE
        // 测站名称，应该是根据测站ID查询出来的哦
        [XmlElement("sname")]
        public string StrStationName { get; set; }

        [XmlElement("stype")]
        public EStationType EIStationType { get; set; }

        #endregion ///<#region NO_USE

        // 测站编号
        [XmlElement("sid")]
        public string StrStationID { get; set; }

        /// <summary>
        /// 日雨量
        /// </summary>
        [XmlElement("rainfall")]
        public Nullable<Decimal> DDayRainFall { get; set; }

        /// <summary>
        /// 昨日雨量
        /// </summary>
        [XmlElement("lastdayrainfall")]
        public Nullable<Decimal> LastDayRainFall { get; set; }

        /// <summary>
        /// 时段雨量
        /// </summary>
        [XmlElement("periodrain")]
        public Nullable<Decimal> DPeriodRain { get; set; }

        /// <summary>
        /// 时段雨量
        /// </summary>
        [XmlElement("differencerain")]
        public Nullable<Decimal> DDifferenceRain { get; set; }

        /// <summary>
        /// 水位
        /// </summary>
        [XmlElement("wateryield")]
        public Nullable<Decimal> DWaterYield { get; set; }

        // 电压
        [XmlElement("voltage")]
        public Nullable<Decimal> Dvoltage { get; set; }

        // 接收时间
        [XmlElement("timerecv")]
        public DateTime TimeReceived { get; set; }

        // 采集时间
        [XmlElement("timedevicegained")]
        public DateTime TimeDeviceGained { get; set; }

        // 端口
        [XmlElement("port")]
        public string StrPort { get; set; }

        // 报文类型
        [XmlElement("msgtype")]
        public EMessageType EIMessageType { get; set; }

        // 信道类型
        [XmlElement("channeltype")]
        public EChannelType EIChannelType { get; set; }

        // 实测流量
        [XmlElement("waterflowactual")]
        public Nullable<Decimal> DWaterFlowActual { get; set; }

        // 相应流量
        [XmlElement("waterflowFindInTable")]
        public Nullable<Decimal> DWaterFlowFindInTable { get; set; }

        //////////////////////////////////////////////////////////////////////////
        ///////////////////////////便于计算时段雨量和日雨量,以及水位变化//////////
        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 上一次的雨量最大值
        /// </summary>
        [XmlElement("lastTotalRain")]
        public Nullable<Decimal> LastTotalRain { get; set; }

        /// <summary>
        /// 最近一天的的雨量记录
        /// </summary>
        [XmlElement("lastDayTotalRain")]
        public Nullable<Decimal> LastDayTotalRain { get; set; }

        [XmlElement("llastDayTotalRain")]
        public Nullable<Decimal> LLastDayTotalRain { get; set; }

        [XmlElement("LastClockSharpRain")]
        public Nullable<Decimal> LastClockSharpRain { get; set; }

        /// <summary>
        /// 最近一天有雨量记录的时间
        /// </summary>
        [XmlElement("lastDayTime")]
        public Nullable<DateTime> LastDayTime { get; set; }

        [XmlElement("LastClockSharpTime")]
        public Nullable<DateTime> LastClockSharpTime { get; set; }

        /// <summary>
        /// 上一次的水位
        /// </summary>
        [XmlElement("lastWaterStage")]
        public Nullable<Decimal> LastWaterStage { get; set; }

        /// <summary>
        /// 是否是收到实际数据生成的，如果是False的话，则只是记录了查询数据表的结果，不需要通知界面显示
        /// </summary>
        [XmlElement("isReceivedValid")]
        public bool BIsReceivedValid { get; set; }

        /// <summary>
        /// 实时数据的状态，用来显示颜色
        /// </summary>
        [XmlElement("state")]
        public ERTDDataState ERTDState { get; set; }

    }

    /// <summary>
    /// 实时最新数据实体集合类
    /// </summary>
    [XmlRoot("Root")]
    public class CEntityRealTimeCollection
    {
        [XmlArray("Items"), XmlArrayItem("RealTimeItem")]
        public List<CEntityRealTime> Items { get; set; }
    }
}
