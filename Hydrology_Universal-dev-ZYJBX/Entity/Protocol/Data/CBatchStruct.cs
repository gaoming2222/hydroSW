using System;
using System.Collections.Generic;

namespace Hydrology.Entity
{
    public class CBatchStruct
    {
        public String StationID;
        public String Cmd;
        /// <summary>
        /// 站点类型
        ///     01为水位 
        ///     02为雨量
        /// </summary>
        public EStationType StationType;
        /// <summary>
        /// 传输类型
        /// 03为按小时传 02为按天传,
        /// 只有Flash传输时使用，U盘传输时不使用此字段
        /// </summary>
        public Nullable<ETrans> TransType;
        public List<CTimeAndData> Datas;
    }
    public class CTimeAndData
    {
        public DateTime Time;
        //public Int32 Data;
        public string Data;
    }
}
