using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Hydrology.Entity
{
    /// <summary>
    /// 墒情数据
    /// </summary>
    [XmlRoot("RealTimeSoilItem")]
    public class CEntitySoilData
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        [XmlElement("StationID")]
        public string StationID { get; set; }


        /// <summary>
        /// 终端机器号
        /// </summary>
        public string StrDeviceNumber { get; set; }

        /// <summary>
        /// 单条记录的时间
        /// </summary>
        [XmlElement("DataTime")]
        public DateTime DataTime { get; set; }


        /// <summary>
        /// 监听的端口或者串口号
        /// </summary>
        //public String ListenPort { get; set; }

        /// <summary>
        /// 电压值
        /// </summary>
        [XmlElement("DVoltage")]
        public Decimal DVoltage { get; set; }

        /// <summary>
        /// 通讯方式
        /// </summary>
        [XmlElement("ChannelType")]
        public EChannelType ChannelType { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [XmlElement("MessageType")]
        public EMessageType MessageType { get; set; }

        /// <summary>
        /// 10厘米处的电压值和含水率
        /// </summary>
        [XmlElement("Voltage10")]
        public Nullable<float> Voltage10 { get; set; }
        [XmlElement("Moisture10")]
        public Nullable<float> Moisture10 { get; set; }

        /// <summary>
        /// 20厘米处的电压值和含水率
        /// </summary>
        [XmlElement("Voltage20")]
        public Nullable<float> Voltage20 { get; set; }
        [XmlElement("Moisture20")]
        public Nullable<float> Moisture20 { get; set; }

        /// <summary>
        /// 30厘米处的电压值和含水率
        /// </summary>
        [XmlElement("Voltage30")]
        public Nullable<float> Voltage30 { get; set; }
        [XmlElement("Moisture30")]
        public Nullable<float> Moisture30 { get; set; }

        /// <summary>
        /// 40厘米处的电压值和含水率
        /// </summary>
        [XmlElement("Voltage40")]
        public Nullable<float> Voltage40 { get; set; }
        [XmlElement("Moisture40")]
        public Nullable<float> Moisture40 { get; set; }

        /// <summary>
        /// 60厘米处的电压值和含水率
        /// </summary>
        [XmlElement("Voltage60")]
        public Nullable<float> Voltage60 { get; set; }
        [XmlElement("Moisture60")]
        public Nullable<float> Moisture60 { get; set; }

        //1008
        public DateTime reciveTime { get; set; }

        public int state { get; set; }
    }

    /// <summary>
    /// 实时最新墒情数据实体集合类
    /// </summary>
    [XmlRoot("Root")]
    public class CEntityRealTimeSoilCollection
    {
        [XmlArray("Items"), XmlArrayItem("RealTimeSoilItem")]
        public List<CEntitySoilData> Items { get; set; }
    }
}
