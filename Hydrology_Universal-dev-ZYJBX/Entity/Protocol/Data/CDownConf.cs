using System;

namespace Hydrology.Entity
{
    public class CDownConf
    {
        //  测站信息
        public String StationID;
        public String Type;

        /// <summary>
        /// 03 时钟
        /// </summary>
        public Nullable<DateTime> Clock;

        /// <summary>
        /// 04 常规状态
        /// </summary>
        public Nullable<ENormalState> NormalState;

        /// <summary>
        /// 05 电压
        /// </summary>
        public Nullable<Decimal> Voltage;

        /// <summary>
        /// 08 站号
        /// </summary>
        public string StationCmdID;

        /// <summary>
        /// 14 对时选择
        /// </summary>
        public Nullable<ETimeChoice> TimeChoice;

        /// <summary>
        /// 24 定时段次
        /// </summary>
        public Nullable<ETimePeriod> TimePeriod;

        /// <summary>
        /// 20 工作状态
        /// </summary>
        public Nullable<EWorkStatus> WorkStatus;

        /// <summary>
        /// 19 版本号
        /// </summary>
        public string VersionNum;


        /// <summary>
        /// 27 主用信道
        /// </summary>
        public Nullable<EChannelType> MainChannel;
        /// <summary>
        /// 27 备用信道 
        /// </summary>
        public Nullable<EChannelType> ViceChannel;

        /// <summary>
        /// 28 SIM卡号
        /// </summary>
        public string TeleNum;

        /// <summary>
        /// 37 振铃次数
        /// </summary>
        public Nullable<Decimal> RingsNum;

        /// <summary>
        /// 49 目的地手机号码
        /// </summary>
        public string DestPhoneNum;

        /// <summary>
        /// 15 终端机号
        /// </summary>
        public string TerminalNum;

        /// <summary>
        /// 42 GSM控电模式
        /// </summary>
        public Nullable<GSMElec> GSMElec;

        /// <summary>
        /// 09 响应波束
        /// </summary>
        public string RespBeam;

        /// <summary>
        /// 16 平均时间
        /// </summary>
        public Nullable<Decimal> AvegTime;

        /// <summary>
        /// 10 雨量加报值
        /// </summary>
        public Nullable<Decimal> RainPlusReportedValue;

        /// <summary>
        /// 62 KC
        /// </summary>
        public string KC;

        /// <summary>
        /// 02 雨量
        /// </summary>
        public Nullable<Decimal> Rain;

        /// <summary>
        /// 12 存储水位
        /// </summary>
        public Nullable<Decimal> storeWater;

        /// <summary>
        /// 13 实测水位
        /// </summary>
        public Nullable<Decimal> realWater;

        /// <summary>
        /// 52 水位
        /// </summary>
        public Nullable<Decimal> WaterBase;

        /// <summary>
        /// 06 水位加报值
        /// </summary>
        public Nullable<Decimal> WaterPlusReportedValue;

        /// <summary>
        /// 11 采集段次选择
        /// </summary>
        public Nullable<ESelectCollectionParagraphs> SelectCollectionParagraphs;

        /// <summary>
        /// 07 测站类型
        /// </summary>
        public Nullable<EStationTypeProto> StationType;

        /// <summary>
        /// 54  用户名
        /// </summary>
        public String UserName;

        /// <summary>
        /// 55  测站名
        /// </summary>
        public String StationName;

        /// <summary>
        /// 清除缓存
        /// </summary>
        public String FlashClear;

        /// <summary>
        /// 传感器类型
        /// </summary>
        public Nullable<ESensorType> SensorType;
    }
}
