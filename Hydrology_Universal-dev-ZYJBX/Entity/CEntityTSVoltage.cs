using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{
    public class CEntityTSVoltage
    {
        #region PROPERTY
        

        /// <summary>
        ///  测站中心的ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        ///  电压值的采集时间
        /// </summary>
        public DateTime TimeCollect { get; set; }

        /// <summary>
        /// 电压值
        /// </summary>
        public Decimal Voltage { get; set; }
        /// 系统接收数据的时间
        /// </summary>
        public DateTime TimeRecieved { get; set; }


        /// <summary>
        /// 报文类型
        /// </summary>
        public EMessageType MessageType { get; set; }

        /// <summary>
        /// 通讯方式类型
        /// </summary>
        public EChannelType ChannelType { get; set; }

        #endregion

    }
}
