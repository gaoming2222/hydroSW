using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Hydrology.Entity
{
    /// <summary>
    /// 墒情站站点的实体类，附属于测站信息
    /// </summary>
    public class CEntitySoilStation
    {
        /// <summary>
        /// 主键，兼外键
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 所属分中心的ID
        /// </summary>
        public Nullable<int> SubCenterID { get; set; }

        public string StationName { get; set; }


        /// <summary>
        ///  站点类型
        /// </summary>
        public EStationType StationType { get; set; }


        /// <summary>
        /// 终端机器号
        /// </summary>
        //  public string StrDeviceNumber { get; set; }



        /// <summary>
        ///  10cm处的ABCD值
        /// </summary>
        public Nullable<decimal> A10 { get; set; }

        public Nullable<decimal> B10 { get; set; }

        public Nullable<decimal> C10 { get; set; }

        public Nullable<decimal> D10 { get; set; }

        public Nullable<decimal> M10 { get; set; }

        public Nullable<decimal> N10 { get; set; }
        /// <summary>
        /// 20cm处的ABCD值
        /// </summary>
        public Nullable<decimal> A20 { get; set; }

        public Nullable<decimal> B20 { get; set; }

        public Nullable<decimal> C20 { get; set; }

        public Nullable<decimal> D20 { get; set; }

        public Nullable<decimal> M20 { get; set; }

        public Nullable<decimal> N20 { get; set; }

        /// <summary>
        /// 30cm处的ABCD值
        /// </summary>
        public Nullable<decimal> A30 { get; set; }

        public Nullable<decimal> B30 { get; set; }

        public Nullable<decimal> C30 { get; set; }

        public Nullable<decimal> D30 { get; set; }

        public Nullable<decimal> M30 { get; set; }

        public Nullable<decimal> N30 { get; set; }
        /// <summary>
        /// 40cm处的ABCD值
        /// </summary>
        public Nullable<decimal> A40 { get; set; }

        public Nullable<decimal> B40 { get; set; }

        public Nullable<decimal> C40 { get; set; }

        public Nullable<decimal> D40 { get; set; }

        public Nullable<decimal> M40 { get; set; }

        public Nullable<decimal> N40 { get; set; }
        /// <summary>
        /// 60cm处的ABCD值
        /// </summary>
        public Nullable<decimal> A60 { get; set; }

        public Nullable<decimal> B60 { get; set; }

        public Nullable<decimal> C60 { get; set; }

        public Nullable<decimal> D60 { get; set; }

        public Nullable<decimal> M60 { get; set; }

        public Nullable<decimal> N60 { get; set; }

        //电压阈值
        public Nullable<decimal> VoltageMin { get; set; }

        /// <summary>
        /// GSM号码
        /// </summary>
        public string GSM { get; set; }

        /// <summary>
        /// GRPS号码
        /// </summary>
        public string GPRS { get; set; }

        /// <summary>
        /// 北斗卫星终端号码
        /// </summary>
        public string BDSatellite { get; set; }

        /// <summary>
        /// 北斗卫星成员号码
        /// </summary>
        public string BDMemberSatellite { get; set; }

        //主信道
        public string Maintran { get; set; }

        //备信道
        public string Subtran { get; set; }

        //数据协议
        public string Datapotocol { get; set; }

        //报讯段次
        public string Reportinterval { get; set; }


    }
}




