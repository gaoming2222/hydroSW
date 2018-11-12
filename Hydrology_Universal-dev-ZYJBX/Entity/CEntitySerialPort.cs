using System;

namespace Hydrology.Entity
{
    public class CEntitySerialPort
    {
        #region PROPERTY

        /// <summary>
        /// 串口数字编号，一般最多也就16个串口
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        ///  通讯方式，如超短波，电话等等
        /// </summary>
        public ESerialPortTransType TransType { get; set; }

        /// <summary>
        ///  波特率
        /// </summary>
        public int Baudrate { get; set; }

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBit { get; set; }

        /// <summary>
        /// 停止位
        /// </summary>
        public int StopBit { get; set; }

        /// <summary>
        /// 校验方式
        /// </summary>
        public EPortParityType ParityType { get; set; }

        /// <summary>
        /// 流控制方式
        /// </summary>
        public ESerialPortStreamType Stream { get; set; }

        /// <summary>
        /// 中断开关
        /// </summary>
        public Nullable<bool> Break { get; set; }

        /// <summary>
        /// 开关状态，用来控制是否打开串口
        /// </summary>
        public Nullable<bool> SwitchSatus { get; set; }

        #endregion
    }
}
