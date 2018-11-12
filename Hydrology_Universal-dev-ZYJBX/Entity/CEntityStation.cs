using System;
using Hydrology.Entity;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    /// <summary>
    /// 站点数据实体
    /// </summary>
    [XmlRoot("StationItem")]
    public class CEntityStation : IComparable
    {
        #region PROPERTY

        /// <summary>
        /// 对应于每个站点的唯一索引，主键，四位,去除空格
        /// </summary>
        private string m_strStationID;
        [XmlElement("sid")]
        public string StationID
        {
            get { return m_strStationID; }
            set { m_strStationID = value.Trim(); }
        }

        /// <summary>
        /// 所属分中心的ID
        /// </summary>
        [XmlElement("subid")]
        public Nullable<int> SubCenterID { get; set; }

        /// <summary>
        ///  站点名字
        /// </summary>
        [XmlElement("sname")]
        public string StationName { get; set; }

        /// <summary>
        ///  站点类型
        /// </summary>
        [XmlElement("stype")]
        public EStationType StationType { get; set; }

        /// <summary>
        /// 站点的水位基值
        /// </summary>
        [XmlElement("wbase")]
        public Nullable<decimal> DWaterBase { get; set; }

        /// <summary>
        /// 站点的水位最大值
        /// </summary>
        [XmlElement("wmax")]
        public Nullable<decimal> DWaterMax { get; set; }

        /// <summary>
        /// 站点的水位最小值
        /// </summary>
        [XmlElement("wmin")]
        public Nullable<decimal> DWaterMin { get; set; }

        /// <summary>
        /// 允许的水位变化值,水位阀值
        /// </summary>
        [XmlElement("wchange")]
        public Nullable<decimal> DWaterChange { get; set; }

        /// <summary>
        /// 雨量精度( 0.1,0.5,1.0 ), 收到的雨量值如果是X，则雨量值是X*DRainAccuracy
        /// </summary>
        [XmlElement("racr")]
        public float DRainAccuracy { get; set; }

        /// <summary>
        /// 允许的雨量变化，雨量阀值
        /// </summary>
        [XmlElement("rchange")]
        public Nullable<decimal> DRainChange { get; set; }

        /// <summary>
        /// GSM号码
        /// </summary>
        [XmlElement("gsm")]
        public string GSM { get; set; }

        /// <summary>
        /// GRPS号码
        /// </summary>
        [XmlElement("gprs")]
        public string GPRS { get; set; }

        /// <summary>
        /// 北斗卫星终端号码
        /// </summary>
        [XmlElement("bdstl")]
        public string BDSatellite { get; set; }

        /// <summary>
        /// 北斗卫星成员号码
        /// </summary>
        [XmlElement("bdmbstl")]
        public string BDMemberSatellite { get; set; }


        /// <summary>
        /// 电压的最低值
        /// </summary>
        [XmlElement("vmin")]
        public Nullable<float> DVoltageMin { get; set; }


        //主信道
        [XmlElement("mtran")]
        public string Maintran { get; set; }

        //备信道
        [XmlElement("stran")]
        public string Subtran { get; set; }


        //数据协议
        [XmlElement("dprotocol")]
        public string Datapotocol { get; set; }

        //水位传感器
        [XmlElement("wsensor")]
        public string Watersensor { get; set; }

        //雨量传感器
        [XmlElement("rsensor")]
        public string Rainsensor { get; set; }

        //报讯段次
        [XmlElement("rinterval")]
        public string Reportinterval { get; set; }



        //////////////////////////////////////////////////////////////////////////
        ///////////////////////////便于计算时段雨量和日雨量,以及水位变化//////////
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 上一次的时段雨量
        /// </summary>
        [XmlElement("lprain")]
        public Nullable<Decimal> LastPeriodRain { get; set; }
        /// <summary>
        /// 上一次的雨量最大值
        /// </summary>
        [XmlElement("ltrain")]
        public Nullable<Decimal> LastTotalRain { get; set; }

        /// <summary>
        /// 上一次整点的雨量值
        /// </summary>
        [XmlElement("lcstrain")]
        public Nullable<Decimal> LastClockSharpTotalRain { get; set; }
        [XmlElement("lcst")]
        public Nullable<DateTime> LastClockSharpTime { get; set; }
        /// <summary>
        /// 最近一天的的雨量记录
        /// </summary>
        [XmlElement("ldtrain")]
        public Nullable<Decimal> LastDayTotalRain { get; set; }
        /// <summary>
        /// 前一天的的雨量记录
        /// </summary>
        [XmlElement("lldtrain")]
        public Nullable<Decimal> LLastDayTotalRain { get; set; }

        /// <summary>
        /// 最近一天有雨量记录的时间
        /// </summary>
        [XmlElement("ldaytime")]
        public Nullable<DateTime> LastDayTime { get; set; }

        /// <summary>
        /// 上一次的水位
        /// </summary>
        [XmlElement("lwstage")]
        public Nullable<Decimal> LastWaterStage { get; set; }

        /// <summary>
        /// 上一次的相应流量
        /// </summary>
        [XmlElement("lwflow")]
        public Nullable<Decimal> LastWaterFlow { get; set; }

        /// <summary>
        /// 上一次传输的信道类型
        /// </summary>
        [XmlElement("lctype")]
        public Nullable<EChannelType> LastChannelType { get; set; }

        /// <summary>
        /// 上一次的数据类型
        /// </summary>
        [XmlElement("lmtype")]
        public Nullable<EMessageType> LastMessageType { get; set; }

        /// <summary>
        /// 上一次的电压
        /// </summary>
        [XmlElement("lvoltage")]
        public Nullable<Decimal> LastVoltage { get; set; }

        /// <summary>
        /// 上一次数据的时间
        /// </summary>
        [XmlElement("ldatatime")]
        public Nullable<DateTime> LastDataTime { get; set; }

        #endregion

        public int CompareTo(object obj)
        {
            int result = 0;
            try
            {
                CEntityStation station = obj as CEntityStation;
                int idthis = int.Parse(this.StationID);
                int idobj = int.Parse(station.StationID);
                return idthis - idobj;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result;
        }

    }
    /// <summary>
    /// 实时最新数据实体集合类
    /// </summary>
    [XmlRoot("Root")]
    public class CEntityStationCollection
    {
        [XmlArray("Items"), XmlArrayItem("StationItem")]
        public List<CEntityStation> Items { get; set; }
    }
}
