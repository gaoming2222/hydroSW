using System;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    public class CUpReport: Entity.IReport
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
        /// 包序号
        /// </summary>
        public string PackageNum { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }
        /// <summary>
        /// 接收数据
        /// </summary>
        public CUpData Data { get; set; }


        public EChannelType ChannelType { get; set; }

        public string ListenPort { get; set; }
    }
}
