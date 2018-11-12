using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Hydrology.Entity
{
    /// <summary>
    /// 水位流量表中的每一条记录
    /// </summary>
    public class CEntityWaterFlowMap : IComparable
    {
        /// <summary>
        /// 唯一ID号码
        /// </summary>
        public long RecordId { get; set; }
        /// <summary>
        ///  测站中心的ID
        /// </summary>
        public string StationID { get; set; }

        //起止时间
        public DateTime BGTM { get; set; }

        //点序号
        public int PTNO { get; set; }
        /// <summary>
        /// 水位
        /// </summary>
        public Decimal ZR { get; set; }

        /// <summary>
        /// 流量
        /// </summary>
        public Decimal Q1 { get; set; }
        public Decimal Q2 { get; set; }
        public Decimal Q3 { get; set; }
        public Decimal Q4 { get; set; }
        public Decimal Q5 { get; set; }
        public Decimal Q6 { get; set; }
        public Decimal currQ { get; set; }
        /// <summary>
        /// 按照水位进行排序
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            int result = 0;
            try
            {
                CEntityWaterFlowMap waterflow = obj as CEntityWaterFlowMap;
                // 当前的大于被比较者
                //if (this.WaterStage > waterflow.WaterStage)
                //{
                return (int)((this.ZR - waterflow.ZR) * 100);
                //}
                //return (int)Math.Ceiling(this.WaterStage - waterflow.WaterStage);
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 水位
        /// </summary>
        public Decimal WaterStage { get; set; }

        /// <summary>
        /// 流量
        /// </summary>
        public Decimal WaterFlow { get; set; }

        ///// <summary>
        ///// 按照水位进行排序
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public int CompareTo(object obj)
        //{
        //    int result = 0;
        //    try
        //    {
        //        CEntityWaterFlowMap waterflow = obj as CEntityWaterFlowMap;
        //        // 当前的大于被比较者
        //        //if (this.WaterStage > waterflow.WaterStage)
        //        //{
        //        return (int)((this.WaterStage - waterflow.WaterStage) * 100);
        //        //}
        //        //return (int)Math.Ceiling(this.WaterStage - waterflow.WaterStage);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        Debug.WriteLine(ex.ToString());
        //    }
        //    return result;
        //}
    }
}
