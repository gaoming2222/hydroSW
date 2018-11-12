using System;

namespace Hydrology.Entity
{
    /// <summary>
    /// 1G2217yymmddhhmm(年月日时分)123456（水位1234.56m）0001(累计雨量为1mm)1256(电压12.56V)0001（10cm处）0002（20cm处）0003（40cm处） 站类为04墒情站 05墒情雨量站 06，16墒情水位站 07，17墒情水文站
    /// </summary>
    public class CSoilStruct
    {
        public String StationID { get; set; }

        public String Type { get; set; }

        public EMessageType ReportType { get; set; }

        public EStationType StationType { get; set; }

        public DateTime RecvTime { get; set; }

        public EChannelType ChannelType { get; set; }

        public String ListenPort { get; set; }

        public DateTime DataTime { get; set; }

        public Nullable<Decimal> Water { get; set; }

        public Nullable<Decimal> Rain { get; set; }

        public Nullable<Decimal> Voltage { get; set; }

        public Nullable<Decimal> Voltage10 { get; set; }

        public Nullable<Decimal> Voltage20 { get; set; }

        public Nullable<Decimal> Voltage40 { get; set; }
    }

    /// <summary>
    /// 1G 25 1425（10cm处）2345（20cm处）3214（40cm处）读墒情
    /// </summary>
    public class CReadSoilStruct
    {
        public String StationID { get; set; }

        public String Type { get; set; }

        public EMessageType ReportType { get; set; }

        public EStationType StationType { get; set; }

        public DateTime RecvTime { get; set; }

        public EChannelType ChannelType { get; set; }

        public String ListenPort { get; set; }

        public DateTime DataTime { get; set; }

        public Nullable<Decimal> Voltage10 { get; set; }

        public Nullable<Decimal> Voltage20 { get; set; }

        public Nullable<Decimal> Voltage40 { get; set; }  
    }

    public class CSoilSensor
    {
        /// <summary>
        /// 测站编码    8位
        /// </summary>
        public String StationCode { get; set; }

        /// <summary>
        /// 站号  4位
        /// </summary>
        public String StationID { get; set; }

        /// <summary>
        /// 信道类型
        /// </summary>
        public EChannelType ChannelType { get; set; }

        /// <summary>
        /// 监听端口
        /// </summary>
        public String ListenPort { get; set; }

        /// <summary>
        /// 数据采集时间
        /// </summary>
        public DateTime DataTime { get; set; }

        /// <summary>
        /// 数据接收时间
        /// </summary>
        public DateTime RecvTime { get; set; }

        /// <summary>
        /// 土壤水分传感埋深设置类型
        /// </summary>
        public ESoilSensorType SensorType { get; set; }

        /// <summary>
        /// 10cm处电压值
        /// </summary>
        public Decimal Volegate10 { get; set; }

        /// <summary>
        /// 20cm处电压值
        /// </summary>
        public Decimal Volegate20 { get; set; }
        /// <summary>
        /// 30cm处电压值
        /// </summary>
        public Decimal Volegate30 { get; set; }

        /// <summary>
        /// 40cm处电压值
        /// </summary>
        public Decimal Volegate40 { get; set; }

        /// <summary>
        /// 60cm处电压值
        /// </summary>
        public Decimal Volegate60 { get; set; }
    }

    /// <summary>
    /// 土壤水分传感埋深设置类型
    /// </summary>
    public enum ESoilSensorType
    {
        /// <summary>
        /// 采集10cm,20cm,40cm处数据
        /// </summary>
        Three,
        /// <summary>
        /// 采集10cm,20cm,30cm,40cm,60cm处数据
        /// </summary>
        Five
    }

    public class YACSoilEventArg : EventArgs
    {
        public string RawData { get; set; }

        public CEntitySoilData Value { get; set; }
    }
}
