using System;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    public enum EBeidou
    {
        Type500,
        NormalTerminal
    };
    #region 500型指挥机类

    /// <summary>
    /// 3.1 用户信息
    ///     用于请求
    /// </summary>
    public class CBeiDouHQYH
    {
        public const String CMD_PREFIX = "HQYH";

        /// <summary>
        /// 卡序号
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 1.2 本地用户信息
    ///     用于接收
    /// </summary>
    public class CBeiDouBJXX
    {
        public const String CMD_PREFIX = "BJXX";
        /// <summary>
        /// 卡序号
        /// </summary>
        public string CardNum { get; set; }
        /// <summary>
        /// 本卡地址
        /// </summary>
        public string LocalAddr { get; set; }
        /// <summary>
        /// 通播地址
        /// </summary>
        public string BroadCastAddr { get; set; }
        /// <summary>
        /// 服务频度
        /// </summary>
        public string ServiceFrequency { get; set; }
        /// <summary>
        /// 保密标志
        /// </summary>
        public string ConfidentFlag { get; set; }
        /// <summary>
        /// 通信等级
        /// </summary>
        public string CommLevel { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public string ValidFlag { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }

    /// <summary>
    /// 2.1 状态检测
    ///     用于请求
    /// </summary>
    public class CBeiDouZTJC
    {
        public const String CMD_PREFIX = "ZTJC";
        /// <summary>
        /// 频度
        /// </summary>
        public string Frequency { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 2.2 状态输出
    ///     用于接收
    /// </summary>
    public class CBeiDouZTXX
    {
        public const String CMD_PREFIX = "ZTXX";
        /// <summary>
        /// 卡状态
        /// </summary>
        public string CardStatus { get; set; }
        /// <summary>
        /// 整机状态
        /// </summary>
        public string WholeMachineState { get; set; }
        /// <summary>
        /// 入站状态
        /// </summary>
        public string InStationStatus { get; set; }
        /// <summary>
        /// 电量
        /// </summary>
        public string Electricity { get; set; }
        /// <summary>
        /// 响应波束
        /// </summary>
        public string ResponseBeam { get; set; }
        /// <summary>
        /// 时差波束
        /// </summary>
        public string DifferenceBeam { get; set; }
        /// <summary>
        /// 信号强度1
        /// </summary>
        public string SignalStrength1 { get; set; }
        /// <summary>
        /// 信号强度2
        /// </summary>
        public string SignalStrength2 { get; set; }
        /// <summary>
        /// 信号强度3
        /// </summary>
        public string SignalStrength3 { get; set; }
        /// <summary>
        /// 信号强度4
        /// </summary>
        public string SignalStrength4 { get; set; }
        /// <summary>
        /// 信号强度5
        /// </summary>
        public string SignalStrength5 { get; set; }
        /// <summary>
        /// 信号强度6
        /// </summary>
        public string SignalStrength6 { get; set; }
        /// <summary>
        /// 回执状态
        /// </summary>
        public string ReceiptStatus { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }

    /// <summary>
    /// 3.1 请求时间
    ///     用于请求
    /// </summary>
    public class CBeiDouCXSJ
    {
        public const String CMD_PREFIX = "CXSJ";
        /// <summary>
        /// 频度
        /// </summary>
        public string Frequency { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 3.2 时间信息
    ///     用于接收
    /// </summary>
    public class CBeiDouSJXX
    {
        public const String CMD_PREFIX = "SJXX";
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Hour { get; set; }
        public string Minute { get; set; }
        public string Second { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }

    /// <summary>
    /// 4.1 终端到终端通信申请
    ///     用于请求
    /// </summary>
    public class CBeiDouTTCA
    {
        public const String CMD_PREFIX = "TTCA";
        /// <summary>
        /// 外设报文序号
        /// </summary>
        public string PeripheralReportNum { get; set; }
        /// <summary>
        /// 发信方ID
        /// </summary>
        public string SenderID { get; set; }
        /// <summary>
        /// 收信方地址
        /// </summary>
        public string ReceiverAddr { get; set; }
        /// <summary>
        /// 保密要求
        /// </summary>
        public string ConfidentialityRequirements { get; set; }
        /// <summary>
        /// 回执标志
        /// </summary>
        public string ReceiptFlag { get; set; }
        /// <summary>
        /// 电文长度
        /// </summary>
        public string MsgLength { get; set; }
        /// <summary>
        /// 电文内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }
    /// <summary>
    /// 4.2 通信输出
    ///     用于接收
    /// </summary>
    public class CBeiDouCOUT
    {
        public const String CMD_PREFIX = "COUT";
        /// <summary>
        /// CRC校验标志
        /// </summary>
        public string CRCFlag { get; set; }

        /// <summary>
        /// 通信类别
        /// </summary>
        public string CommType { get; set; }
        /// <summary>
        /// 发信方类型
        /// </summary>
        public string SenderType { get; set; }
        /// <summary>
        /// 发信方地址
        /// </summary>
        public string SenderAddr { get; set; }
        /// <summary>
        /// 回执标志
        /// </summary>
        public string ReceiptFlag { get; set; }
        /// <summary>
        /// 报文顺序号
        /// </summary>
        public string ReportNum { get; set; }
        /// <summary>
        /// 电文长度
        /// </summary>
        public string MsgLength { get; set; }
        /// <summary>
        /// 电文内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 收方ID 
        /// </summary>
        public string RecipientID { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public string ChannelNum { get; set; }
        /// <summary>
        /// 波束号
        /// </summary>
        public string BeamNum { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }
    }

    public class Beidou500BJXXEventArgs : EventArgs
    {
        public CBeiDouBJXX BJXXInfo { get; set; }

        public string RawMsg { get; set; }
    }
    public class Beidou500ZTXXEventArgs : EventArgs
    {
        public CBeiDouZTXX ZTXXInfo { get; set; }

        public string RawMsg { get; set; }
    }
    public class Beidou500SJXXEventArgs : EventArgs
    {
        public CBeiDouSJXX SJXXInfo { get; set; }

        public string RawMsg { get; set; }
    }
    #endregion

    #region 普通终端类
    public class CQSTAStruct
    {
        public const String CMD_PREFIX = "QSTA";
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 通信申请成功状态
    /// 
    /// CASS = Communication Application Success Status
    /// </summary>
    public class CCASSStruct
    {
        public const String CMD_PREFIX = "CASS";
        /// <summary>
        /// 成功状态
        /// </summary>
        public bool SuccessStatus { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 终端状态信息类
    /// 
    /// TSTA = Termial STAtus information
    /// </summary>
    public class CTSTAStruct
    {
        public const String CMD_PREFIX = "TSTA";
        /// <summary>
        /// 通道1接收信号功率电平
        /// </summary>
        public int Channel1RecvPowerLevel { get; set; }
        /// <summary>
        /// 通道2接收信号功率电平
        /// </summary>
        public int Channel2RecvPowerLevel { get; set; }
        /// <summary>
        /// 通道1锁定卫星波束
        /// </summary>
        public int Channel1LockingBeam { get; set; }
        /// <summary>
        /// 通道2锁定卫星波束
        /// </summary>
        public int Channel2LockingBeam { get; set; }
        /// <summary>
        /// 响应波束
        /// </summary>
        public int ResponseOfBeam { get; set; }
        /// <summary>
        /// 信号抑制
        /// </summary>
        public bool SignalSuppression { get; set; }
        /// <summary>
        /// 供电状态
        /// </summary>
        public bool PowerState { get; set; }
        /// <summary>
        /// 终端ID
        /// </summary>
        public String TerminalID { get; set; }
        /// <summary>
        /// 通播地址
        /// </summary>
        public String BroadcastAddr { get; set; }
        /// <summary>
        /// 服务频度
        /// </summary>
        public String ServiceFrequency { get; set; }
        /// <summary>
        /// 串口波特率
        /// </summary>
        public int SerialBaudRate { get; set; }
        /// <summary>
        /// 保密模块状态
        /// </summary>
        public int SecurityModuleState { get; set; }
        /// <summary>
        /// 气压测高模块状态
        /// </summary>
        public int BarometricAltimetryModuleState { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 设置终端状态
    /// 
    /// STST = Set Terminal STatus
    /// </summary>
    public class CSTSTStruct
    {
        public const String CMD_PREFIX = "STST";
        /// <summary>
        /// 服务频度
        /// </summary>
        public String ServiceFrequency { get; set; }
        /// <summary>
        /// 串口波特率
        /// </summary>
        public int SerialBaudRate { get; set; }
        /// <summary>
        /// 响应波束
        /// </summary>
        public int ResponseOfBeam { get; set; }
        /// <summary>
        /// 回执类型
        /// </summary>
        public bool AcknowledgmentType { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 自发自收参数类
    /// 
    /// TTCA = Terminal To Terminal Communication Application
    /// </summary>
    public class CTTCAStruct
    {
        public const String CMD_PREFIX = "TTCA";
        /// <summary>
        /// 发信方ID
        /// </summary>
        public String SenderID { get; set; }
        /// <summary>
        /// 收信方地址
        /// </summary>
        public String RecvAddr { get; set; }
        /// <summary>
        /// 保密要求
        /// </summary>
        public String Requirements { get; set; }
        /// <summary>
        /// 回执标志
        /// </summary>
        public bool ReceiptSign { get; set; }
        /// <summary>
        /// 电文长度
        /// </summary>
        public int MsgLength { get; set; }
        /// <summary>
        /// 电文内容
        /// </summary>
        public String MsgContent { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    //TAPP
    public class CTAPPStruct
    {
        public const String CMD_PREFIX = "TAPP";
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    //TINF
    public class CTINFStruct
    {
        public const String CMD_PREFIX = "TINF";
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 通信输出类
    /// 
    /// COUT = Communication OUTput
    /// </summary>
    public class CCOUTStruct
    {
        public const String CMD_PREFIX = "COUT";
        /// <summary>
        /// CRC校验标志
        /// </summary>
        public bool CRCCheckMark { get; set; }
        /// <summary>
        /// 通信类别
        /// </summary>
        public String CommType { get; set; }
        /// <summary>
        /// 发信方类型
        /// </summary>
        public String SenderType { get; set; }
        /// <summary>
        /// 发信方地址
        /// </summary>
        public String SenderAddr { get; set; }
        /// <summary>
        /// 回执标志
        /// </summary>
        public bool ReceiptSign { get; set; }
        /// <summary>
        /// 报文顺序号
        /// </summary>
        public String MsgSequenceNum { get; set; }
        /// <summary>
        /// 电文长度
        /// </summary>
        public int MsgLength { get; set; }
        /// <summary>
        /// 电文内容
        /// </summary>
        public String MsgContent { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }

    /// <summary>
    /// 通信输出成功状态类
    /// 
    /// COSS = Communication Output Success Status
    /// </summary>
    public class CCOSSStruct
    {
        public const String CMD_PREFIX = "COSS";
        /// <summary>
        /// 成功状态
        /// </summary>
        public bool SuccessStatus { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }
    /// <summary>
    /// 通信回执请求
    /// 
    /// CACA = Communication ACknowledgement Application
    /// </summary>
    public class CCACAStruct
    {
        public const String CMD_PREFIX = "CACA";
        /// <summary>
        /// 发信方ID
        /// </summary>
        public String SenderID { get; set; }
        /// <summary>
        /// 收信方类型
        /// </summary>
        public String RecvType { get; set; }
        /// <summary>
        /// 收信方地址
        /// </summary>
        public String RecvAddr { get; set; }
        /// <summary>
        /// 保密要求
        /// </summary>
        public String Requirements { get; set; }
        /// <summary>
        /// 回执的报文顺序号
        /// </summary>
        public String ReceiptMsgSequenceNum { get; set; }
        /// <summary>
        /// 回执内容
        /// </summary>
        public String ReceiptContent { get; set; }
        /// <summary>
        /// 校验和
        /// </summary>
        public String CheckSum { get; set; }
    }


    public class TSTAEventArgs : EventArgs
    {
        public CTSTAStruct TSTAInfo { get; set; }

        public string RawMsg { get; set; }
    }

    public class COUTEventArgs : EventArgs
    {
        public Dictionary<int, bool> BeidouStatus { get; set; }
    }

    public class SendOrRecvMsgEventArgs : EventArgs
    {
        public EChannelType ChannelType { get; set; }
        public String Msg { get; set; }
        public String Description { get; set; }
    }

    public class SendOrRecvMsgEventArgs1 : EventArgs
    {
      //  public EChannelType ChannelType { get; set; }
        public String Msg { get; set; }
      //  public String Description { get; set; }
    }

    #endregion
}
