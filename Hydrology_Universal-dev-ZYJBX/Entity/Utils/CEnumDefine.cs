using System;

namespace Hydrology.Entity
{
    #region 枚举定义

    /// <summary>
    /// 系统中信道协议监听的串口，端口类型
    /// </summary>
    public enum EListeningProtType
    {
        /// <summary>
        /// 串口
        /// </summary>
        SerialPort,

        /// <summary>
        /// 端口
        /// </summary>
        Port
    }

    /// <summary>
    /// 测站类型
    /// </summary>
    public enum EStationType
    {
        ERainFall =0,//雨量站
        ERiverWater = 1,    //水位站
        EHydrology = 2,     /*水文站*/
        ESoil = 3,         //  04墒情站
        ESoilRain = 4,     //  05墒情雨量站
        ESoilWater = 5,     //  06，16墒情水位站
        ESoilHydrology = 6  //  07，17墒情水文站
    }

    public enum EStationTypeProto
    {
        ERainFall = 1,          //  雨量站   01

        EParallelEHydrology,    //  并行水文  03
        EParallelRiverWater,    //  并行水位  02
        EParallelSpecial,         //  并口特殊类型      07   

        ESerialEHydrology,      //  串行水文  13
        ESerialRiverWater,      //  串行水位  12
        ESerialSpecial            //  串口特殊类型     17  
    }

    /// <summary>
    /// 接受的数据的类型 
    /// </summary>
    public enum EMessageType
    {
        
        Batch,  //批量
        ETimed,     /*定时报*/
        EAdditional,   /*加报*/
        Manual,
        EMannual


    };

    /// <summary>
    /// 串口通讯方式
    /// </summary>
    public enum ESerialPortTransType
    {
        Nothing = 0,
        EUSM = 1,/*超短波*/
        ETelephone = 2,/*电话*/
        EBDSatelite = 3,/*北斗卫星*/
        EMarisat = 4,/*海事卫星*/
        ESMS = 5/*短信息*/
    };

    /// <summary>
    /// 串口通信的校验方式
    /// </summary>
    public enum EPortParityType
    {
        ENone,/*无*/
        EOdd,/*奇校验*/
        EEven /*偶校验*/
    }

    /// <summary>
    /// 通讯方式的枚举定义
    /// </summary>
    public enum EChannelType
    {
        GPRS = 15,
        BeiDou = 3,
        GSM,
        PSTN,
        None,
        VHF,
        //  仅用于记录日志
        BeidouNormal,
        Beidou500
    };

    /// <summary>
    /// 串口流控制方式
    /// </summary>
    public enum ESerialPortStreamType
    {
        ENone, /*无*/
        ERts, /*rts/cts*/
        EDtr  /*dtr/dsr*/
    }

    /// <summary>
    /// 站点批量传输类型，要么是Upan,要么是Flash，要么没有
    /// </summary>
    public enum EStationBatchType
    {
        ENone, /*无*/
        EFlash, /*Flash传输*/
        EUPan /*U盘传输*/
    }
    /// <summary>
    /// 实时数据状态
    /// </summary>
    public enum ERTDDataState { ENormal, EWarning, EError };

    /// <summary>
    /// 信息的颜色
    /// </summary>
    public enum ETextMsgState { ENormal, EWarning, EError };

    /// <summary>
    /// 告警信息编码
    /// </summary>
    public enum EWarningInfoCodeType
    {
        ERain,  /*雨量报警*/
        EWater, /*水位报警*/
        EVlotage /*电压报警*/
    }

    #endregion 枚举定义

    /// <summary>
    /// 将自定义枚举类型转换成字符串或者数据库中的对应字段的帮助类
    /// </summary>
    public static class CEnumHelper
    {
        #region SerailPortTransType
        private static readonly string CS_ESerialPortTransType_Nothing_DBStr = "0";
        private static readonly string CS_ESerialPortTransType_Nothing_UIStr = "无";

        private static readonly string CS_ESerialPortTransType_USM_DBStr = "1";
        private static readonly string CS_ESerialPortTransType_USM_UIStr = "超短波";

        private static readonly string CS_ESerialPortTransType_Telephone_DBStr = "2";
        private static readonly string CS_ESerialPortTransType_Telephone_UIStr = "电话";

        private static readonly string CS_ESerialPortTransType_BDSatelite_DBStr = "3";
        private static readonly string CS_ESerialPortTransType_BDSatelite_UIStr = "北斗卫星";

        private static readonly string CS_ESerialPortTransType_Marisat_DBStr = "4";
        private static readonly string CS_ESerialPortTransType_Marisat_UIStr = "海事卫星";

        private static readonly string CS_ESerialPortTransType_SMS_DBStr = "5";
        private static readonly string CS_ESerialPortTransType_SMS_UIStr = "短信息";


        public static ESerialPortTransType UIStrToTransType(string type)
        {
            if (type == CS_ESerialPortTransType_Nothing_UIStr)
                return ESerialPortTransType.Nothing;
            else if (type == CS_ESerialPortTransType_USM_UIStr)
                return ESerialPortTransType.EUSM;
            else if (type == CS_ESerialPortTransType_Telephone_UIStr)
                return ESerialPortTransType.ETelephone;
            else if (type == CS_ESerialPortTransType_BDSatelite_UIStr)
                return ESerialPortTransType.EBDSatelite;
            else if (type == CS_ESerialPortTransType_Marisat_UIStr)
                return ESerialPortTransType.EMarisat;
            else if (type == CS_ESerialPortTransType_SMS_UIStr)
                return ESerialPortTransType.ESMS;
            throw new Exception("UIStrToTransType Error");
        }
        public static ESerialPortTransType DBStrToSerialTransType(string type)
        {
            type = type.Trim();
            if (type == CS_ESerialPortTransType_Nothing_DBStr)
                return ESerialPortTransType.Nothing;
            else if (type == CS_ESerialPortTransType_USM_DBStr)
                return ESerialPortTransType.EUSM;
            else if (type == CS_ESerialPortTransType_Telephone_DBStr)
                return ESerialPortTransType.ETelephone;
            else if (type == CS_ESerialPortTransType_BDSatelite_DBStr)
                return ESerialPortTransType.EBDSatelite;
            else if (type == CS_ESerialPortTransType_Marisat_DBStr)
                return ESerialPortTransType.EMarisat;
            else if (type == CS_ESerialPortTransType_SMS_DBStr)
                return ESerialPortTransType.ESMS;
            throw new Exception("DBStrToSerialTransType Error");
        }
        public static string SerialTransTypeToDBStr(ESerialPortTransType type)
        {
            switch (type)
            {
                case ESerialPortTransType.Nothing: return CS_ESerialPortTransType_Nothing_DBStr;
                case ESerialPortTransType.EUSM: return CS_ESerialPortTransType_USM_DBStr;
                case ESerialPortTransType.ETelephone: return CS_ESerialPortTransType_Telephone_DBStr;
                case ESerialPortTransType.EBDSatelite: return CS_ESerialPortTransType_BDSatelite_DBStr;
                case ESerialPortTransType.EMarisat: return CS_ESerialPortTransType_Marisat_DBStr;
                case ESerialPortTransType.ESMS: return CS_ESerialPortTransType_SMS_DBStr;
            }
            throw new Exception("SerialTransTypeToDBStr Error");
        }

        public static string SerialTransTypeToUIStr(ESerialPortTransType type)
        {
            switch (type)
            {
                case ESerialPortTransType.Nothing: return CS_ESerialPortTransType_Nothing_UIStr;
                case ESerialPortTransType.EUSM: return CS_ESerialPortTransType_USM_UIStr;
                case ESerialPortTransType.ETelephone: return CS_ESerialPortTransType_Telephone_UIStr;
                case ESerialPortTransType.EBDSatelite: return CS_ESerialPortTransType_BDSatelite_UIStr;
                case ESerialPortTransType.EMarisat: return CS_ESerialPortTransType_Marisat_UIStr;
                case ESerialPortTransType.ESMS: return CS_ESerialPortTransType_SMS_UIStr;
            }
            throw new Exception("SerialTransTypeToUIStr Error");
        }

        #endregion ///< SerailPortTransType

        #region MessageType
        private static readonly string CS_EMessageType_Batch_DBStr = "3";
        private static readonly string CS_EMessageType_ETimed_DBStr = "8";
        private static readonly string CS_EMessageType_EAdditional_DBStr = "9";
        private static readonly string CS_EMessageType_Manual_DBStr = "6";
        private static readonly string CS_EMessageType_Batch_UIStr = "批量";
        private static readonly string CS_EMessageType_EDTimed_UIStr = "定时报";
        private static readonly string CS_EMessageType_EAdditional_UIStr = "加报";
        private static readonly string CS_EMessageType_Manual_UIStr = "人工报";
        public static string MessageTypeToDBStr(EMessageType type)
        {
            string result = "";
            switch (type)
            {
                case EMessageType.Batch: { result = CS_EMessageType_Batch_DBStr; } break;
                case EMessageType.ETimed: { result = CS_EMessageType_ETimed_DBStr; } break;
                case EMessageType.EAdditional: { result = CS_EMessageType_EAdditional_DBStr; } break;
                case EMessageType.Manual: { result = CS_EMessageType_Manual_DBStr; } break;
            }
            return result;
        }

        public static string MessageTypeToUIStr(EMessageType type)
        {
            string result = "";
            switch (type)
            {
                case EMessageType.Batch: { result = CS_EMessageType_Batch_UIStr; } break;
                case EMessageType.ETimed: { result = CS_EMessageType_EDTimed_UIStr; } break;
                case EMessageType.EAdditional: { result = CS_EMessageType_EAdditional_UIStr; } break;
                case EMessageType.Manual: { result = CS_EMessageType_Manual_UIStr; } break;
            }
            return result;
        }

        public static EMessageType DBStrToMessageType(string type)
        {
            if (type.Equals(CS_EMessageType_EAdditional_DBStr))
            {
                return EMessageType.EAdditional;
            }
            else if (type.Equals(CS_EMessageType_ETimed_DBStr))
            {
                return EMessageType.ETimed;
            }
            else if (type.Equals(CS_EMessageType_Batch_DBStr))
            {
                return EMessageType.Batch;
            }
            else if (type.Equals(CS_EMessageType_Manual_DBStr))
            {
                return EMessageType.Manual;
            }
            throw new Exception("DBStrToMessageType ERROR");
        }

        public static EMessageType UIStrToMesssageType(string str)
        {
            if (str.Equals(CS_EMessageType_EAdditional_UIStr))
            {
                return EMessageType.EAdditional;
            }
            else if (str.Equals(CS_EMessageType_EDTimed_UIStr))
            {
                return EMessageType.ETimed;
            }
            else if (str.Equals(CS_EMessageType_Batch_UIStr))
            {
                return EMessageType.Batch;
            }
            else if (str.Equals(CS_EMessageType_Manual_UIStr))
            {
                return EMessageType.Manual;
            }

            throw new Exception("UIStrToMesssageType ERROR");
        }
        #endregion ///<MessageType

        #region ChannelType
        public static readonly string CS_EChannelType_None_DBStr = "0";
        public static readonly string CS_EChannelType_None_UIStr = "无";
        public static readonly string CS_EChannelType_GPRS_DBStr = "15";
        public static readonly string CS_EChannelType_Beidou_DBStr = "3";
        public static readonly string CS_EChannelType_GSM_DBStr = "16";
        public static readonly string CS_EChannelType_PSTN_DBStr = "17";
        public static readonly string CS_EChannelType_GPRS_UIStr = "GPRS";
        public static readonly string CS_EChannelType_Beidou_UIStr = "北斗";
        public static readonly string CS_EChannelType_GSM_UIStr = "GSM";
        public static readonly string CS_EChannelType_PSTN_UIStr = "PSTN";
        public static string ChannelTypeToDBStr(EChannelType type)
        {
            switch (type)
            {
                case EChannelType.GPRS: { return CS_EChannelType_GPRS_DBStr; }
                case EChannelType.BeiDou: { return CS_EChannelType_Beidou_DBStr; }
                case EChannelType.GSM: { return CS_EChannelType_GSM_DBStr; }
                case EChannelType.PSTN: { return CS_EChannelType_PSTN_DBStr; }
                case EChannelType.None: { return CS_EChannelType_None_DBStr; }
            }
            throw new Exception("ChannelTypeToDBStr Error");
        }

        public static string ChannelTypeToUIStr(EChannelType type)
        {
            switch (type)
            {
                case EChannelType.GPRS: { return CS_EChannelType_GPRS_UIStr; }
                case EChannelType.BeiDou: { return CS_EChannelType_Beidou_UIStr; }
                case EChannelType.GSM: { return CS_EChannelType_GSM_UIStr; }
                case EChannelType.PSTN: { return CS_EChannelType_PSTN_UIStr; }
                case EChannelType.None: { return CS_EChannelType_None_UIStr; }
            }
            throw new Exception("ChannelTypeToUIStr Error");
        }

        public static EChannelType DBStrToChannelType(string type)
        {
            type = type.Trim();
            if (type.Equals(CS_EChannelType_GPRS_DBStr))
            {
                return EChannelType.GPRS;
            }
            else if (type.Equals(CS_EChannelType_Beidou_DBStr))
            {
                return EChannelType.BeiDou;
            }
            else if (type.Equals(CS_EChannelType_PSTN_DBStr))
            {
                return EChannelType.PSTN;
            }
            else if (type.Equals(CS_EChannelType_GSM_DBStr))
            {
                return EChannelType.GSM;
            }
            else if (type.Equals(CS_EChannelType_None_DBStr))
            {
                return EChannelType.None;
            }
            throw new Exception("DBStrToChannelType ERROR");
        }

        public static EChannelType UIStrToChannelType(string str)
        {
            if (str.Equals(CS_EChannelType_GPRS_UIStr))
            {
                return EChannelType.GPRS;
            }
            else if (str.Equals(CS_EChannelType_Beidou_UIStr))
            {
                return EChannelType.BeiDou;
            }
            else if (str.Equals(CS_EChannelType_PSTN_UIStr))
            {
                return EChannelType.PSTN;
            }
            else if (str.Equals(CS_EChannelType_GSM_UIStr))
            {
                return EChannelType.GSM;
            }
            else if (str.Equals(CS_EChannelType_None_UIStr))
            {
                return EChannelType.None;
            }
            throw new Exception("UIStrToChannelType ERROR");
        }
        #endregion ///< ChannelType

        #region 站点类型
        public static readonly string CS_EStationType_ERainFall_UIStr = "雨量站";
        public static readonly string CS_EStationType_ERainFall_DBStr = "0";

        public static readonly string CS_EStationType_ERiverWater_UIStr = "水位站";
        public static readonly string CS_EStationType_ERiverWater_DBStr = "1";

        public static readonly string CS_EStationType_EHydrology_UIStr = "水文站";
        public static readonly string CS_EStationType_EHydrology_DBStr = "2";

        public static readonly string CS_EStationType_ESoil_UIStr = "墒情站";
        public static readonly string CS_EStationType_ESoil_DBStr = "3";

        public static readonly string CS_EStationType_ESoilRain_UIStr = "墒情雨量站";
        public static readonly string CS_EStationType_ESoilRain_DBStr = "4";

        public static readonly string CS_EStationType_ESoilWater_UIStr = "墒情水位站";
        public static readonly string CS_EStationType_ESoilWater_DBStr = "5";

        public static readonly string CS_EStationType_ESoilHydrology_UIStr = "墒情水文站";
        public static readonly string CS_EStationType_ESoilHydrology_DBStr = "6";

        //        ESoil = 3,         //  04墒情站
        //ESoilRain = 4,     //  05墒情雨量站
        //ESoilWater = 5,     //  06，16墒情水位站
        //ESoilHydrology = 6  //  07，17墒情水文站

        public static string StationTypeToUIStr(EStationType type)
        {
            string result = "";
            switch (type)
            {
                case EStationType.ERainFall: { result = CS_EStationType_ERainFall_UIStr; } break;
                case EStationType.ERiverWater: { result = CS_EStationType_ERiverWater_UIStr; } break;
                case EStationType.EHydrology: { result = CS_EStationType_EHydrology_UIStr; } break;
                case EStationType.ESoil: { result = CS_EStationType_ESoil_UIStr; } break;
                case EStationType.ESoilRain: { result = CS_EStationType_ESoilRain_UIStr; } break;
                case EStationType.ESoilWater: { result = CS_EStationType_ESoilWater_UIStr; } break;
                case EStationType.ESoilHydrology: { result = CS_EStationType_ESoilHydrology_UIStr; } break;
                default: { result = "未知站点类型"; } break;
            }
            return result;
        }

        public static string WaterSensorTypeToUIStr(int type)
        {
            string result = "";
            switch (type)
            {
                case 0: { result = "浮子水位"; } break;
                case 1: { result = "气泡水位"; } break;
                case 2: { result = "压阻水位"; } break;
                case 3: { result = "雷达水位"; } break;
                //default: { result = "未知水位传感器类型"; } break;
                default: { result = "无"; } break;
            }
            return result;
        }

        public static string RainSensorTypeToUIStr(int type)
        {
            string result = "";
            switch (type)
            {
                case 0: { result = "翻斗雨量"; } break;
                case 1: { result = "雨雪量计"; } break;
                //default: { result = "未知雨量传感器类型"; } break;
                default: { result = "无"; } break;
            }
            return result;
        }

        public static EStationType UIStrToStationType(string type)
        {
            type = type.Trim();
            if (type.Equals(CS_EStationType_ERainFall_UIStr))
                return EStationType.ERainFall;
            else if (type.Equals(CS_EStationType_ERiverWater_UIStr))
                return EStationType.ERiverWater;
            else if (type.Equals(CS_EStationType_EHydrology_UIStr))
                return EStationType.EHydrology;
            else if (type.Equals(CS_EStationType_ESoil_UIStr))
                return EStationType.ESoil;
            else if (type.Equals(CS_EStationType_ESoilRain_UIStr))
                return EStationType.ESoilRain;
            else if (type.Equals(CS_EStationType_ESoilWater_UIStr))
                return EStationType.ESoilWater;
            else if (type.Equals(CS_EStationType_ESoilHydrology_UIStr))
                return EStationType.ESoilHydrology;
            throw new Exception("UIStrToStationType ERROR");
        }
        public static EStationType DBStrToStationType(string type)
        {
            type = type.Trim();
            if (type.Equals(CS_EStationType_ERainFall_DBStr))
                return EStationType.ERainFall;
            else if (type.Equals(CS_EStationType_ERiverWater_DBStr))
                return EStationType.ERiverWater;
            else if (type.Equals(CS_EStationType_EHydrology_DBStr))
                return EStationType.EHydrology;
            else if (type.Equals(CS_EStationType_ESoil_DBStr))
                return EStationType.ESoil;
            else if (type.Equals(CS_EStationType_ESoilRain_DBStr))
                return EStationType.ESoilRain;
            else if (type.Equals(CS_EStationType_ESoilWater_DBStr))
                return EStationType.ESoilWater;
            else if (type.Equals(CS_EStationType_ESoilHydrology_DBStr))
                return EStationType.ESoilHydrology;
            throw new Exception("DBStrToStationType ERROR");
        }
        public static string StationTypeToDBStr(EStationType type)
        {
            switch (type)
            {
                case EStationType.ERainFall: return CS_EStationType_ERainFall_DBStr;
                case EStationType.ERiverWater: return CS_EStationType_ERiverWater_DBStr;
                case EStationType.EHydrology: return CS_EStationType_EHydrology_DBStr;
                case EStationType.ESoil: return CS_EStationType_ESoil_DBStr;
                case EStationType.ESoilRain: return CS_EStationType_ESoilRain_DBStr;
                case EStationType.ESoilWater: return CS_EStationType_ESoilWater_DBStr;
                case EStationType.ESoilHydrology: return CS_EStationType_ESoilHydrology_DBStr;
            }
            throw new Exception("StationTypeToDBStr ERROR");
        }

        public static int WaterSensorTypeToDBStr(string type)
        {
            switch (type)
            {
                case "浮子水位": return 0;
                case "气泡水位": return 1;
                case "压阻水位": return 2;
                case "雷达水位": return 3;
                case "无": return 4;

            }
            throw new Exception("WaterSensorTypeToDBStr ERROR");
        }

        public static int RainSensorTypeToDBStr(string type)
        {
            switch (type)
            {
                case "翻斗雨量": return 0;
                case "雨雪量计": return 1;
                case "无": return 1;
            }
            throw new Exception("RainSensorTypeToDBStr ERROR");
        }
        #endregion ///<站点类型

        #region 串口数据校验方式

        private static readonly char CS_PortParityType_None_DBChar = '0';
        private static readonly string CS_PortParityType_None_UIStr = "无";

        private static readonly char CS_PortParityType_Odd_DBChar = '1';
        private static readonly string CS_PortParityType_Odd_UIStr = "奇校验";

        private static readonly char CS_PortParityType_Even_DBChar = '2';
        private static readonly string CS_PortParityType_Even_UIStr = "偶校验";

        public static EPortParityType UIStrToParityType(string type)
        {
            if (type == CS_PortParityType_None_UIStr)
                return EPortParityType.ENone;
            else if (type == CS_PortParityType_Even_UIStr)
                return EPortParityType.EEven;
            else if (type == CS_PortParityType_Odd_UIStr)
                return EPortParityType.EOdd;
            throw new Exception("UIStrToParityType Error");
        }

        public static EPortParityType DBCharToPortParityType(char ch)
        {
            if (ch == CS_PortParityType_None_DBChar)
                return EPortParityType.ENone;
            else if (ch == CS_PortParityType_Even_DBChar)
                return EPortParityType.EEven;
            else if (ch == CS_PortParityType_Odd_DBChar)
                return EPortParityType.EOdd;
            throw new Exception("DBCharToPortParityType Error");
        }

        public static char PortParityTypeToDBChar(EPortParityType type)
        {
            switch (type)
            {
                case EPortParityType.ENone: return CS_PortParityType_None_DBChar;
                case EPortParityType.EOdd: return CS_PortParityType_Odd_DBChar;
                case EPortParityType.EEven: return CS_PortParityType_Even_DBChar;
            }
            throw new Exception("PortParityTypeToDBChar Error");
        }

        public static string PortParityTypeToUIStr(EPortParityType type)
        {
            switch (type)
            {
                case EPortParityType.ENone: return CS_PortParityType_None_UIStr;
                case EPortParityType.EOdd: return CS_PortParityType_Odd_UIStr;
                case EPortParityType.EEven: return CS_PortParityType_Even_UIStr;
            }
            throw new Exception("PortParityTypeToUIStr Error");
        }
        #endregion ///<串口数据校验方式

        #region 串口流控制方式
        private static string CS_ESerialPortStreamType_None_DBStr = "0";
        private static string CS_ESerialPortStreamType_None_UIStr = "无";

        private static string CS_ESerialPortStreamType_RTS_DBStr = "1";
        private static string CS_ESerialPortStreamType_RTS_UIStr = "rts/cts";

        private static string CS_ESerialPortStreamType_DTR_DBStr = "2";
        private static string CS_ESerialPortStreamType_DTR_UIStr = "dtr/dsr";


        public static ESerialPortStreamType DBStrToSerialPortStreamType(string type)
        {
            if (type == CS_ESerialPortStreamType_None_DBStr)
                return ESerialPortStreamType.ENone;
            else if (type == CS_ESerialPortStreamType_RTS_DBStr)
                return ESerialPortStreamType.ERts;
            else if (type == CS_ESerialPortStreamType_DTR_DBStr)
                return ESerialPortStreamType.EDtr;
            throw new Exception("DBStrToSerialPortStreamType Error");
        }
        public static ESerialPortStreamType UIStrToSerialPortStreamType(string type)
        {
            if (type == CS_ESerialPortStreamType_None_UIStr)
                return ESerialPortStreamType.ENone;
            else if (type == CS_ESerialPortStreamType_RTS_UIStr)
                return ESerialPortStreamType.ERts;
            else if (type == CS_ESerialPortStreamType_DTR_UIStr)
                return ESerialPortStreamType.EDtr;
            throw new Exception("UIStrToSerialPortStreamType Error");
        }
        public static string SerialPortStreamTypeToDBStr(ESerialPortStreamType type)
        {
            switch (type)
            {
                case ESerialPortStreamType.ENone: return CS_ESerialPortStreamType_None_DBStr;
                case ESerialPortStreamType.ERts: return CS_ESerialPortStreamType_RTS_DBStr;
                case ESerialPortStreamType.EDtr: return CS_ESerialPortStreamType_DTR_DBStr;
            }
            throw new Exception("SerialPortStreamTypeToDBStr Error");
        }
        public static string SerialPortStreamTypeToUIStr(ESerialPortStreamType type)
        {
            switch (type)
            {
                case ESerialPortStreamType.ENone: return CS_ESerialPortStreamType_None_UIStr;
                case ESerialPortStreamType.ERts: return CS_ESerialPortStreamType_RTS_UIStr;
                case ESerialPortStreamType.EDtr: return CS_ESerialPortStreamType_DTR_UIStr;
            }
            throw new Exception("SerialPortStreamTypeToDBStr Error");
        }
        #endregion ///< 串口流控制方式

        #region 站点批量传输类型
        private static readonly string CS_EStationBatchType_None_DBStr = "0";
        private static readonly string CS_EStationBatchType_Flash_DBStr = "1";
        private static readonly string CS_EStationBatchType_UPan_DBStr = "2";

        public static EStationBatchType DBStrToStationBatchType(string str)
        {
            if (str.Equals(CS_EStationBatchType_None_DBStr))
            {
                return EStationBatchType.ENone;
            }
            else if (str.Equals(CS_EStationBatchType_UPan_DBStr))
            {
                return EStationBatchType.EUPan;
            }
            else if (str.Equals(CS_EStationBatchType_Flash_DBStr))
            {
                return EStationBatchType.EFlash;
            }
            throw new Exception("DBStrToStationBatchType Error");
        }
        public static string StationBatchTypeToDBStr(EStationBatchType type)
        {
            switch (type)
            {
                case EStationBatchType.ENone: return CS_EStationBatchType_None_DBStr;
                case EStationBatchType.EFlash: return CS_EStationBatchType_Flash_DBStr;
                case EStationBatchType.EUPan: return CS_EStationBatchType_UPan_DBStr;
            }
            throw new Exception("StationBatchTypeToDBStr Error");
        }
        #endregion 站点批量传输类型

        #region 告警信息错误码
        private static readonly string CS_EWarningCodeType_Rain = "111";
        private static readonly string CS_EWarningCodeType_Water = "112";
        private static readonly string CS_EWarningCodeType_Voltate = "113";
        public static string WarningCodeTypeToDBStr(EWarningInfoCodeType type)
        {
            switch (type)
            {
                case EWarningInfoCodeType.ERain: return CS_EWarningCodeType_Rain;
                case EWarningInfoCodeType.EWater: return CS_EWarningCodeType_Water;
                case EWarningInfoCodeType.EVlotage: return CS_EWarningCodeType_Voltate;
            }
            throw new Exception("WarningCodeTypeToDBStr Error");
        }
        #endregion 告警信息错误码


    }
}
