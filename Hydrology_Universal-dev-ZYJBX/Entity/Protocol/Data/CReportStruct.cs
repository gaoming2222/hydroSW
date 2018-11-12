using System;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    public class CReportStruct:Entity.IReport
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string Stationid { get; set; }
        /// <summary>
        /// 通信类别
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 报文类别
        /// </summary>
        public EMessageType ReportType { get; set; }
        /// <summary>
        /// 站点类别
        /// </summary>
        public EStationType StationType { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        public List<CReportData> Datas { get; set; }

        public EChannelType ChannelType { get; set; }

        public string ListenPort { get; set; }

        /// <summary>
        /// 接收数据的通讯ID
        /// </summary>
        public string flagId { get; set; }
    }

    ///// <summary>
    ///// 接受的数据的类型 
    ///// </summary>
    //public enum EReportType
    //{
    //    /// <summary>
    //    /// 定时报
    //    /// </summary>
    //    ETimed = 22,

    //    /// <summary>
    //    /// 加报
    //    /// </summary>
    //    EAdditional = 21
    //};
    ///// <summary>
    ///// 测站类型
    ///// </summary>
    //public enum EStationType
    //{
    //    /// <summary>
    //    /// 水量站
    //    /// </summary>
    //    ERiverWater,

    //    /// <summary>
    //    /// 雨量站
    //    /// </summary>
    //    ERainFall,

    //    /// <summary>
    //    /// 水文站
    //    /// </summary>
    //    EHydrology
    //};

    public enum ETrans
    {
        /// <summary>
        /// 按小时传 03
        /// </summary>
        ByHour,
        /// <summary>
        /// 按天传   02
        /// </summary>
        ByDay
    }

    /// <summary>
    /// 人工水位
    /// </summary>
    public class CReportArtificalWater
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string Stationid { get; set; }

        /// <summary>
        /// 人工报采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }

        /// <summary>
        /// 通讯方式
        /// </summary>
        public EChannelType ChannelType { get; set; }

        /// <summary>
        /// 水位
        /// </summary>
        public string Water { get; set; }

        /// <summary>
        /// 水势
        /// </summary>
        public string WaterPotential { get; set; }
    }

    /// <summary>
    /// 人工流量
    /// </summary>
    public class CReportArtificalFlow
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string Stationid { get; set; }

        /// <summary>
        /// 人工报采集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }

        /// <summary>
        /// 通讯方式
        /// </summary>
        public EChannelType ChannelType { get; set; }

        /// <summary>
        /// 水位
        /// </summary>
        public string Water { get; set; }

        /// <summary>
        /// 水势
        /// </summary>
        public string WaterPotential { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public string Accuracy { get; set; }

        /// <summary>
        /// 有效数字
        /// </summary>
        public string EffectiveNumber { get; set; }

        /// <summary>
        /// 测流方法
        /// </summary>
        public string FlowMeasurementMethod { get; set; }
    }
}
