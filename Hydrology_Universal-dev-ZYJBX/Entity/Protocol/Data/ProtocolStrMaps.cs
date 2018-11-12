using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{
    public class ProtocolMaps
    {
        public static CDictionary<EStationTypeProto, String> StationType4ProtoMap = new CDictionary<EStationTypeProto, String>()
        {
            //  需要修改
            { EStationTypeProto.ERainFall,           "01" },
            { EStationTypeProto.EParallelEHydrology, "03" },
            { EStationTypeProto.EParallelRiverWater, "02" },
            { EStationTypeProto.EParallelSpecial,    "17" },
            { EStationTypeProto.ESerialEHydrology,   "13" },
            { EStationTypeProto.ESerialRiverWater,   "12" },
            { EStationTypeProto.ESerialSpecial,      "07" }
        };
        public static CDictionary<EStationTypeProto, String> StationType4ProtoChineseMap = new CDictionary<EStationTypeProto, string>()
        {
            { EStationTypeProto.EParallelRiverWater, "并行水位站" },
            { EStationTypeProto.ERainFall,   "雨量站" },
            { EStationTypeProto.EParallelEHydrology,  "并行水文站" },
            { EStationTypeProto.ESerialRiverWater, "串行水位站" },
            { EStationTypeProto.ESerialEHydrology,  "串行行水文站" }
        };
        public static CDictionary<ENormalState, String> NormalState4ProtoMap = new CDictionary<ENormalState, String>()
        {
            { ENormalState.GPRS,   "01" },
            { ENormalState.GSM,    "02" }
        };
        public static CDictionary<ENormalState, string> NormalState4UI = new CDictionary<ENormalState, string>() 
        {
            { ENormalState.GPRS,   "GPRS" },
            { ENormalState.GSM,    "GSM" }
        };


        public static CDictionary<ETimeChoice, String> TimeChoice4ProtoMap = new CDictionary<ETimeChoice, String>()
        {
                        //  需要修改
            { ETimeChoice.AdjustTime,   "01" },
            { ETimeChoice.Two,          "02" }
        };
        public static CDictionary<ETimeChoice, String> TimeChoice4UI = new CDictionary<ETimeChoice, String>()
        {
                        //  需要修改
            { ETimeChoice.AdjustTime,   "对时" },
            { ETimeChoice.Two,          "不对时" }
        };

        public static CDictionary<GSMElec, String> GSMElec4ProtoMap = new CDictionary<GSMElec, String>()
        {
                        //  需要修改
            { GSMElec.AlwaysOL,     "01" },
            { GSMElec.TriggerOL,    "02" },
            { GSMElec.SelfReport,   "03" }
        };
        public static CDictionary<GSMElec, String> GSMElec4UI = new CDictionary<GSMElec, String>()
        {
                        //  需要修改
            { GSMElec.AlwaysOL,     "长期在线" },
            { GSMElec.TriggerOL,    "触发在线" },
            { GSMElec.SelfReport,   "自报" }
        };

        public static CDictionary<EWorkStatus, String> WorkStatus4ProtoMap = new CDictionary<EWorkStatus, String>()
        {
                        //  需要修改
            { EWorkStatus.Debug,   "01" },
            { EWorkStatus.Normal,  "02" },
            { EWorkStatus.DoubleAddress,"03" }
        };
        public static CDictionary<EWorkStatus, String> WorkStatus4UI = new CDictionary<EWorkStatus, String>()
        {
                        //  需要修改
            { EWorkStatus.Debug,   "调试状态" },
            { EWorkStatus.Normal,  "正常工作状态" },
            { EWorkStatus.DoubleAddress,"双目标地址" }
        };

        public static CDictionary<EDownParam, String> DownParamMap = new CDictionary<EDownParam, String>()
        {
            { EDownParam.Rain ,                     "02" },
            { EDownParam.Clock ,                    "03" },
            { EDownParam.NormalState ,              "04" },
            { EDownParam.Voltage ,                  "05" },
            { EDownParam.WaterPlusReportedValue ,   "06" },
            { EDownParam.StationType ,              "07" },
            { EDownParam.StationCmdID ,             "08" },
            { EDownParam.RespBeam ,                 "09" },
            { EDownParam.RainPlusReportedValue ,    "10" },
            { EDownParam.SelectCollectionParagraphs , "11" },
            { EDownParam.storeWater ,               "12" },
            { EDownParam.realWater ,               "13" },
            { EDownParam.TimeChoice ,               "14" },
            { EDownParam.TerminalNum ,              "15" },
            { EDownParam.AvegTime ,                 "16" },
            { EDownParam.VersionNum ,               "19" },
            { EDownParam.WorkStatus ,               "20" },
            { EDownParam.GSMElec,       "22" },
            { EDownParam.TimePeriod ,   "24" },
            { EDownParam.StandbyChannel,"27" },
            { EDownParam.TeleNum ,      "28" },
            { EDownParam.RingsNum ,     "37" },
            { EDownParam.DestPhoneNum , "49" },
            { EDownParam.WaterBase ,    "52" },
            { EDownParam.UserName , "54" },
            { EDownParam.StationName , "55" },
            { EDownParam.KC ,       "62" },
            { EDownParam.SensorType ,   "72" },
            { EDownParam.FlashClear ,   "63" }
        };
        /// <summary>
        /// -1 表示长度未定
        /// 正数表示相应字段对应字符串解析中的长度
        /// </summary>
        public static CDictionary<EDownParam, String> DownParamLengthMap = new CDictionary<EDownParam, String>()
        {
            { EDownParam.Clock,                     "12"},
            { EDownParam.NormalState,               "2" },
            { EDownParam.Voltage,                   "4" },
            { EDownParam.StationCmdID,              "4" },
            { EDownParam.TimeChoice,                "2" },
            { EDownParam.TimePeriod,                "2" },
            { EDownParam.WorkStatus,                "2" },
            { EDownParam.VersionNum,                "-1"},
            { EDownParam.StandbyChannel,            "4" },
            { EDownParam.TeleNum,                   "-1"},
            { EDownParam.RingsNum,                  "2" },
            { EDownParam.DestPhoneNum,              "11"},
            { EDownParam.TerminalNum,               "8" },
            { EDownParam.GSMElec,                   "2" },
            { EDownParam.RespBeam,                  "2" },
            { EDownParam.AvegTime,                  "2" },
            { EDownParam.RainPlusReportedValue,     "2" },
            { EDownParam.KC,                        "10"},
            { EDownParam.Rain,                      "4" },
            { EDownParam.storeWater,                "6" },
            { EDownParam.realWater,                 "6" },
            { EDownParam.WaterBase,                 "6" },
            { EDownParam.WaterPlusReportedValue,    "2" },
            { EDownParam.SelectCollectionParagraphs,"2" },
            { EDownParam.StationType,               "2" },
            { EDownParam.UserName,                  "-1"},
            { EDownParam.StationName,                "-1"},
            { EDownParam.FlashClear,                "-1"},
            { EDownParam.SensorType,                "2"}
        };
        public static CDictionary<EDownParam, String> DownParam4ChineseMap = new CDictionary<EDownParam, string>()
        {
            { EDownParam.Clock,                     "时钟"},
            { EDownParam.NormalState,               "常规状态" },
            { EDownParam.Voltage,                   "电压" },
            { EDownParam.StationCmdID,              "站号" },
            { EDownParam.TimeChoice,                "对时选择" },
            { EDownParam.TimePeriod,                "定时段次" },
            { EDownParam.WorkStatus,                "工作状态" },
            { EDownParam.VersionNum,                "版本号" },
            { EDownParam.StandbyChannel,            "主备信道" },
            { EDownParam.TeleNum,                   "SIM卡号"},
            { EDownParam.RingsNum,                  "振铃次数" },
            { EDownParam.DestPhoneNum,              "目的地手机号"},
            { EDownParam.TerminalNum,               "终端机号" },
            { EDownParam.GSMElec,                   "GSM控电模式" },
            { EDownParam.RespBeam,                  "响应波束" },
            { EDownParam.AvegTime,                  "平均时间" },
            { EDownParam.RainPlusReportedValue,     "雨量加报值" },
            { EDownParam.KC,                        "KC"},
            { EDownParam.Rain,                      "雨量" },
            { EDownParam.storeWater,                "存储水位" },
            { EDownParam.realWater,                "实测水位" },
            { EDownParam.WaterBase,                     "水位基值" },
            { EDownParam.WaterPlusReportedValue,    "水位加报值" },
            { EDownParam.SelectCollectionParagraphs,"采集段次选择"},
            { EDownParam.StationType,               "测站类型" } ,
            { EDownParam.UserName,                  "用户名" } ,
            { EDownParam.StationName,               "测站名" } ,
            { EDownParam.FlashClear,               "清除Flash" } ,
            { EDownParam.SensorType,               "传感器类型" }
        };
        public static CDictionary<EChannelType, String> ChannelType4ProtoMap = new CDictionary<EChannelType, String>()
        {
            { EChannelType.BeiDou,  "04" },
            { EChannelType.GPRS,    "06" },
            { EChannelType.GSM,     "05" },
            { EChannelType.None,    "00" },
            { EChannelType.PSTN,    "02" },
            { EChannelType.VHF,     "01" }
        };
        public static CDictionary<EChannelType, String> ChannelType4UIMap = new CDictionary<EChannelType, String>()
        {
            { EChannelType.BeiDou,  "北斗卫星" },
            { EChannelType.GPRS,    "GPRS" },
            { EChannelType.GSM,     "GSM" },
            { EChannelType.None,    "无" },
            { EChannelType.PSTN,    "PSTN" },
            { EChannelType.VHF,     "VHF" }
        };
        /// <summary>
        /// 不同类型的信道协议的起始字符
        /// 如 GSM信道 ： 起始字符为String.Empty
        ///    GPRS信道 ：  起始字符为 $
        ///   BeiDou,None,PSTV 暂时不清楚，所以用NOT_CLEAR代替
        /// </summary>
        public static CDictionary<EChannelType, String> ChannelProtocolStartCharMap = new CDictionary<EChannelType, string>()
        {
            { EChannelType.BeiDou,  "NOT_CLEAR" },
            { EChannelType.GPRS,     "$" },
            { EChannelType.GSM,     String.Empty },
            { EChannelType.None,    "NOT_CLEAR" },
            { EChannelType.PSTN,    "NOT_CLEAR" }
        };

        public static CDictionary<ETrans, String> TransMap = new CDictionary<ETrans, String>()
        {
            { ETrans.ByDay , "02" },
            { ETrans.ByHour, "03" }
        };

        public static CDictionary<EMessageType, String> MessageTypeMap = new CDictionary<EMessageType, String>()
        {
            { EMessageType.ETimed,       "22" },
            { EMessageType.EAdditional,  "21" },
            {EMessageType.Manual,        "11" }
        };

        public static CDictionary<EStationType, String> StationType4UIMap = new CDictionary<EStationType, string>()
        {
            //{ EStationType.ERiverWater, "01" },
            //{ EStationType.ERainFall,   "02" },
            //{ EStationType.EHydrology,  "03" }
            { EStationType.ERainFall,   "01" },
            { EStationType.ERiverWater, "02" },
            { EStationType.EHydrology,  "03" }
        };

        public static CDictionary<EStationType, String> StationType4ChineseMap = new CDictionary<EStationType, string>()
        {
            { EStationType.ERiverWater, "水位站" },
            { EStationType.ERainFall,   "雨量站" },
            { EStationType.EHydrology,  "水文站" }
        };

        public static CDictionary<ESelectCollectionParagraphs, String> SelectCollectionParagraphs4ProtoMap = new CDictionary<ESelectCollectionParagraphs, String>()
        {
            { ESelectCollectionParagraphs.FiveOrSix,    "01" },
            { ESelectCollectionParagraphs.TenOrTwelve,  "02" }
        };
        public static CDictionary<ESelectCollectionParagraphs, String> SelectCollectionParagraphs4UIMap = new CDictionary<ESelectCollectionParagraphs, string>() 
        {
            { ESelectCollectionParagraphs.FiveOrSix,   "5/6" },
            { ESelectCollectionParagraphs.TenOrTwelve, "10/12"}
        };

        public static CDictionary<ETimePeriod, String> TimePeriodMap = new CDictionary<ETimePeriod, string>()
        {
            { ETimePeriod.One,"01" },
            { ETimePeriod.Two,"02" },
            { ETimePeriod.Four,"04" },
            { ETimePeriod.Six,"06" },
            { ETimePeriod.Eight,"08" },
            { ETimePeriod.Twelve,"12" },
            { ETimePeriod.TwentyFour,"24" },
            { ETimePeriod.FourtyEight,"48" }
        };

        public static CDictionary<ESensorType, String> SensorType4ProtoMap = new CDictionary<ESensorType, String>()
        {
            { ESensorType.Standard,  "00" },
            { ESensorType.XZSDI,    "01" },
            { ESensorType.XZRS,     "02" },
            { ESensorType.OTT,    "03" },
            { ESensorType.PBSDI,    "05" },
            { ESensorType.MPM,     "06" },
            { ESensorType.TempSensor,     "07" },
            { ESensorType.NJRS,     "08" },
            { ESensorType.Unknown,     "09" }
        };
        public static CDictionary<EWaterType, String> WaterType4ProtoMap = new CDictionary<EWaterType, String>()
        {
            { EWaterType.storeWater,     "12" },
            { EWaterType.realWater,     "13" }
        };
        public static CDictionary<ESensorType, String> SensorType4UIMap = new CDictionary<ESensorType, String>()
        {
            { ESensorType.Standard,  "WL3100/HS-40 SDI-12" },
            { ESensorType.XZSDI,    "徐州浮子 SDI-12" },
            { ESensorType.XZRS,     "徐州浮子 RS485" },
            { ESensorType.OTT,    "OTT雷达 SDI-12" },
            { ESensorType.PBSDI,    "平板 SDI-12" },
            { ESensorType.MPM,     "MPM47（RS485）" },
            { ESensorType.TempSensor,     "水温传感器" },
            { ESensorType.NJRS,     "南京浮子485接口" },
            { ESensorType.Unknown,     "09" }
        };
        public static CDictionary<EWaterType, String> WaterType4UIMap = new CDictionary<EWaterType, String>()
        {
            { EWaterType.storeWater,     "存储水位" },
            { EWaterType.realWater,     "实测水位" }
        };
    }

    public class CDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TValue FindValue(TKey key)
        {
            if (this.ContainsKey(key))
            {
                return this[key];
            }
            throw new Exception(String.Format("{0}不在映射字典中！", key));
        }
        public TKey FindKey(TValue value)
        {
            foreach (var item in this)
            {
                if (item.Value.Equals(value))
                    return item.Key;
            }
            throw new Exception(String.Format("{0}不在映射字典中！", value));
        }
    }

    public class ProtocolHelpers
    {
        public static bool DeleteSpecialChar(string oldStr, out string newStr)
        {
            //  删除起始符 '$'
            if (oldStr.StartsWith(CSpecialChars.DOLLAR_CHAR.ToString()))
                oldStr = oldStr.Substring(1);
            //  删除结束符 '\r'
            if (oldStr.EndsWith(CSpecialChars.ENTER_CHAR.ToString()))
                oldStr = oldStr.Replace(CSpecialChars.ENTER_CHAR, CSpecialChars.BALNK_CHAR);
            //  删除起始位置的空字符 ' '
            oldStr = oldStr.Trim();
            newStr = oldStr;
            return newStr.Length > 0;
        }

        public static readonly string LEGAL_CHARACTERS = "abcdefghijklmnopqrstuvwxyz0123456789.@_-";
        public static bool isLegalCharacters(string sExt)
        {
            sExt = sExt.ToLower();
            for (int i = 0; i < sExt.Length; i++)
            {
                if (!LEGAL_CHARACTERS.Contains(sExt.Substring(i, 1)))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 处理BCD编码函数。
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static String dealBCD(string row)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in row)
            {
                string temp = string.Format("{0:D2}", (int)c);
                string res = Convert.ToString(int.Parse(temp), 2);
                string result = string.Format("{0:x2}", Convert.ToInt32(res, 2));
                if(result.Length  > 2)
                {
                    result = "" + result[0] + result[2];
                }
                sb.Append(result);
            }

            return sb.ToString();
        }
        public static string Dec2Hex(Int32 raw)
        {
            return String.Format("{ 0:X2}", raw);
        }
        public static Int32 Hex2Dec(string raw)
        {
            int current = 0;
            string map = "0123456789ABCDEF";
            foreach(char c in raw)
            {
                int value = map.IndexOf(c.ToString().ToUpper());
                current = current * 16 + value;
            }
            return current;
        }
        public static EStationType ProtoStr2StationType(String str)
        {
            var pStationType = ProtocolMaps.StationType4ProtoMap.FindKey(str);
            string type = string.Empty;
            switch (pStationType)
            {
                case EStationTypeProto.EParallelEHydrology:
                case EStationTypeProto.ESerialEHydrology:
                case EStationTypeProto.EParallelSpecial:
                case EStationTypeProto.ESerialSpecial:
                    type = "03"; break;
                case EStationTypeProto.EParallelRiverWater:
                case EStationTypeProto.ESerialRiverWater:
                    //type = "01"; break;
                   type = "02"; break;
                case EStationTypeProto.ERainFall:
                    //type = "02"; break;
                    type = "01"; break;
                default:
                    throw new Exception("站点类型转换错误");
            }
            return ProtocolMaps.StationType4UIMap.FindKey(type);
        }

        public static String StationType2ProtoStr(EStationType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case EStationType.EHydrology:
                case EStationType.ERiverWater:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERiverWater);
                    break;
                case EStationType.ERainFall:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERainFall);
                    break;
            }
            return str;
        }
        public static String StationType2ProtoStr_set(EStationType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case EStationType.EHydrology:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.EHydrology);
                    break;
                case EStationType.ERiverWater:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERiverWater);
                    break;
                case EStationType.ERainFall:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERainFall);
                    break;
            }
            return str;
        }

        public static String StationType2ProtoStr_set_proto(EStationTypeProto type)
        {
            string str = string.Empty;
            switch (type)
            {
                case EStationTypeProto.ESerialEHydrology:
                    str = ProtocolMaps.StationType4ProtoMap.FindValue(EStationTypeProto.ESerialEHydrology);
                    break;
                case EStationTypeProto.ESerialRiverWater:
                    str = ProtocolMaps.StationType4ProtoMap.FindValue(EStationTypeProto.ESerialRiverWater);
                    break;
                case EStationTypeProto.EParallelEHydrology:
                    str = ProtocolMaps.StationType4ProtoMap.FindValue(EStationTypeProto.EParallelEHydrology);
                    break;
                case EStationTypeProto.EParallelRiverWater:
                    str = ProtocolMaps.StationType4ProtoMap.FindValue(EStationTypeProto.EParallelRiverWater);
                    break;
                case EStationTypeProto.ERainFall:
                    str = ProtocolMaps.StationType4ProtoMap.FindValue(EStationTypeProto.ERainFall);
                    break;
            }
            return str;
        }

        public static String StationType2ProtoStr_1(EStationType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case EStationType.EHydrology:
                case EStationType.ERiverWater:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERainFall);
                    break;
                case EStationType.ERainFall:
                    str = ProtocolMaps.StationType4UIMap.FindValue(EStationType.ERiverWater);
                    break;
            }
            return str;
        }
    }
}
