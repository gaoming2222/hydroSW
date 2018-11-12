using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Hydrology.DBManager;
using System.Threading.Tasks;
using Hydrology.DBManager.DB.SQLServer;
using Hydrology.DBManager.Interface;

namespace Hydrology.DBManager.DB.SQLServer
{
    public class CSQLWaterFlowMap : CSQLBase, IWaterFlowMapProxy
    {
        #region 静态常量
        //private static readonly string CT_TableName = "ST_ZQ_B";      //数据库水位流量表的名字
        //private static readonly string CN_RecordID = "ID";                 //记录的唯一ID号码
        //private static readonly string CN_StationID = "StationID";      //站点ID的列名
        //private static readonly string CN_WaterStage = "ZR";        //水位的列名
        //private static readonly string CN_WaterFlow = "Q";     //流量的列名
        private static readonly string CT_TableName = "ST_ZQ_B";      //数据库水位流量表的名字
#pragma warning disable CS0414 // 字段“CSQLWaterFlowMap.CN_RecordID”已被赋值，但从未使用过它的值
        private static readonly string CN_RecordID = "ID";                 //记录的唯一ID号码
#pragma warning restore CS0414 // 字段“CSQLWaterFlowMap.CN_RecordID”已被赋值，但从未使用过它的值
        private static readonly string CN_StationID = "stationid";      //站点ID的列名
        private static readonly string CN_BGTM = "BGTM"; // 起止时间
        private static readonly string CN_PTNO = "PTNO"; //点序号
        private static readonly string CN_ZR = "ZR";        //水位的列名

        private static readonly string CN_Q1 = "Q1";     //流量1的列名
        private static readonly string CN_Q2 = "Q2";
        private static readonly string CN_Q3 = "Q3";     //流量3的列名
        private static readonly string CN_Q4 = "Q4";
        private static readonly string CN_Q5 = "Q5";     //流量5的列名
        private static readonly string CN_Q6 = "Q6";
        private static readonly string CN_currQ = "currQ";
        #endregion

        #region 公共方法
        public CSQLWaterFlowMap()
            : base()
        {
            // 为数据表添加列
            //m_tableDataAdded.Columns.Add(CN_StationID);
            //m_tableDataAdded.Columns.Add(CN_WaterStage);
            //m_tableDataAdded.Columns.Add(CN_WaterFlow);
            m_tableDataAdded.Columns.Add(CN_StationID);
            m_tableDataAdded.Columns.Add(CN_BGTM);
            m_tableDataAdded.Columns.Add(CN_PTNO);
            m_tableDataAdded.Columns.Add(CN_ZR);
            m_tableDataAdded.Columns.Add(CN_Q1);
            m_tableDataAdded.Columns.Add(CN_Q2);
            m_tableDataAdded.Columns.Add(CN_Q3);
            m_tableDataAdded.Columns.Add(CN_Q4);
            m_tableDataAdded.Columns.Add(CN_Q5);
            m_tableDataAdded.Columns.Add(CN_Q6);
            m_tableDataAdded.Columns.Add(CN_currQ);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_WaterFlowMap;
        }

        // 添加新列
        public void AddNewRow(CEntityWaterFlowMap entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            //row[CN_StationID] = entity.StationID;
            //row[CN_WaterStage] = entity.WaterStage;
            //row[CN_WaterFlow] = entity.WaterFlow;
            row[CN_StationID] = entity.StationID;
            row[CN_BGTM] = entity.BGTM.ToString(CDBParams.GetInstance().DBDateTimeFormat);
            row[CN_PTNO] = entity.PTNO;
            row[CN_ZR] = entity.ZR;
            row[CN_Q1] = entity.Q1;
            row[CN_Q2] = entity.Q2;
            row[CN_Q3] = entity.Q3;
            row[CN_Q4] = entity.Q4;
            row[CN_Q5] = entity.Q5;
            row[CN_Q6] = entity.Q6;
            row[CN_currQ] = entity.currQ;
            m_tableDataAdded.Rows.Add(row);
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                Task task = new Task(() => { AddDataToDB(); });
                task.Start();
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer.Start();
            }
            m_mutexDataTable.ReleaseMutex();
        }

        public bool AddRange(List<CEntityWaterFlowMap> listEntitys)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityWaterFlowMap entity in listEntitys)
            {
                DataRow row = m_tableDataAdded.NewRow();
                //row[CN_StationID] = entity.StationID;
                //row[]=
                //row[CN_WaterStage] = entity.WaterStage;
                //row[CN_WaterFlow] = entity.WaterFlow;
                row[CN_StationID] = entity.StationID;
                // entity.BGTM = Convert.ToDateTime("2010-06-06");
                row[CN_BGTM] = entity.BGTM.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_PTNO] = entity.PTNO;
                row[CN_ZR] = entity.ZR;
                row[CN_Q1] = entity.Q1;
                row[CN_Q2] = entity.Q2;
                row[CN_Q3] = entity.Q3;
                row[CN_Q4] = entity.Q4;
                row[CN_Q5] = entity.Q5;
                row[CN_Q6] = entity.Q6;
                row[CN_currQ] = entity.currQ;

                m_tableDataAdded.Rows.Add(row);
                if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
                {
                    // 如果超过最大值，写入数据库
                    m_mutexDataTable.ReleaseMutex();
                    AddDataToDB();
                    m_mutexDataTable.WaitOne();
                }
            }
            m_mutexDataTable.ReleaseMutex();
            return AddDataToDB();
        }

        public List<CEntityWaterFlowMap> QueryMapsByStationId(string stationId)
        {
            // 根据站点ID,查询当前站点的水位流量关系
            List<CEntityWaterFlowMap> results = new List<CEntityWaterFlowMap>();
            string sql = string.Format("select * from {0} where {1} = '{2}' order by BGTM , PTNO", CT_TableName, CN_StationID, stationId);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                for (int i = 0; i < dataTableTmp.Rows.Count; ++i)
                {
                    CEntityWaterFlowMap entity = new CEntityWaterFlowMap();
                    //entity.StationID = dataTableTmp.Rows[i][CN_StationID].ToString().Trim();
                    //entity.WaterFlow = decimal.Parse(dataTableTmp.Rows[i][CN_WaterFlow].ToString());
                    //entity.WaterStage = decimal.Parse(dataTableTmp.Rows[i][CN_WaterStage].ToString());
                    //entity.RecordId = long.Parse(dataTableTmp.Rows[i][CN_RecordID].ToString());
                    entity.StationID = dataTableTmp.Rows[i][CN_StationID].ToString().Trim();
                    entity.BGTM = Convert.ToDateTime(dataTableTmp.Rows[i][CN_BGTM].ToString());
                    entity.PTNO = int.Parse(dataTableTmp.Rows[i][CN_PTNO].ToString());
                    entity.ZR = Decimal.Parse(dataTableTmp.Rows[i][CN_ZR].ToString());

                    entity.Q1 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q1].ToString());
                    entity.Q2 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q2].ToString());
                    entity.Q3 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q3].ToString());
                    entity.Q4 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q4].ToString());
                    entity.Q5 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q5].ToString());
                    entity.Q6 = Decimal.Parse(dataTableTmp.Rows[i][CN_Q6].ToString());

                    entity.currQ = Decimal.Parse(dataTableTmp.Rows[i][CN_currQ].ToString());
                    // entity.RecordId = long.Parse(dataTableTmp.Rows[i][CN_RecordID].ToString());
                    results.Add(entity);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return results;
        }

        public bool UpdateRange(List<CEntityWaterFlowMap> listEntity)
        {
            // 站点应该是不允许更新的，SQL这边除了主键外，所有的都可以更新
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0; // 当前累积的最大值
            for (int i = 0; i < listEntity.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6}, {7}={8},{9}={10},{11}={12},{13}={14},{15}={16} where {17}={18} and {19}='{20}' and {21}={22};",
                    //CT_TableName,
                    //CN_StationID, listEntity[i].StationID,
                    //CN_WaterStage, listEntity[i].WaterStage,
                    //CN_WaterFlow, listEntity[i].WaterFlow,
                    //CN_RecordID, listEntity[i].RecordId
                      CT_TableName,
                    //CN_StationID, listEntity[i].StationID,
                    //CN_BGTM, listEntity[i].BGTM,
                    //CN_PTNO, listEntity[i].PTNO,
                    CN_ZR, listEntity[i].ZR,
                    CN_Q1, listEntity[i].Q1,
                    CN_Q2, listEntity[i].Q2,
                    CN_Q3, listEntity[i].Q3,
                    CN_Q4, listEntity[i].Q4,
                    CN_Q5, listEntity[i].Q5,
                    CN_Q6, listEntity[i].Q6,
                    CN_currQ, listEntity[i].currQ,
                    CN_StationID, listEntity[i].StationID,
                    CN_BGTM, listEntity[i].BGTM,
                    CN_PTNO, listEntity[i].PTNO
                );
                if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
                {
                    // 更新数据库
                    if (!this.ExecuteSQLCommand(sql.ToString()))
                    {
                        // 如果某一条失败的话， 直接报错
                        return false;
                    }
                    sql.Clear(); //清除以前的所有命令
                    currentBatchCount = 0;
                }
            } // end of for
            return this.ExecuteSQLCommand(sql.ToString());
        }

        //public bool DeleteRange(List<long> listIds)
        //{
        //// 删除多行数据
        //StringBuilder sql = new StringBuilder();
        //int currentBatchCount = 0;
        //for (int i = 0; i < listIds.Count; i++)
        //{
        //    ++currentBatchCount;
        //    sql.AppendFormat("delete from {0} where {1}={2};",
        //        CT_TableName,
        //        CN_RecordID, listIds[i].ToString()
        //    );
        //    if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
        //    {
        //        // 更新数据库
        //        if (!this.ExecuteSQLCommand(sql.ToString()))
        //        {
        //            return false;
        //        }
        //        sql.Clear(); //清除以前的所有命令
        //        currentBatchCount = 0;
        //    }
        //} // end of for
        //// 如何考虑线程同异步
        //return ExecuteSQLCommand(sql.ToString());
        //}

        public bool DeleteRange(List<CEntityWaterFlowMap> listEntity)
        {
            // 删除多行数据
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < listEntity.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("delete from {0} where {1}={2} and {3}='{4}' and {5}={6};",
                    CT_TableName,
                    CN_StationID, listEntity[i].StationID,
                    CN_BGTM, listEntity[i].BGTM,
                    CN_PTNO, listEntity[i].PTNO
                );
                if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
                {
                    // 更新数据库
                    if (!this.ExecuteSQLCommand(sql.ToString()))
                    {
                        return false;
                    }
                    sql.Clear(); //清除以前的所有命令
                    currentBatchCount = 0;
                }
            } // end of for
            // 如何考虑线程同异步
            return ExecuteSQLCommand(sql.ToString());
        }

        public bool DeleteLine(CEntityWaterFlowMap Entity)
        {
            // 删除多行数据
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            //for (int i = 0; i < Entity.Count; i++)
            //{
            ++currentBatchCount;
            //   sql.AppendFormat("delete from {0} where {1}={2} and {3}='{4}';",
            sql.AppendFormat("delete from {0} where {1}={2};",
                    CT_TableName,
                    CN_StationID, Entity.StationID
                //CN_BGTM, Entity.BGTM
                );
            if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
            {
                // 更新数据库
                if (!this.ExecuteSQLCommand(sql.ToString()))
                {
                    return false;
                }
                sql.Clear(); //清除以前的所有命令
                currentBatchCount = 0;
            }
            //} // end of for
            // 如何考虑线程同异步
            return ExecuteSQLCommand(sql.ToString());
        }
        #endregion 公共方法

        #region 帮助方法
        // 将当前所有数据写入数据库
        protected override bool AddDataToDB()
        {
            // 先获取对数据库的唯一访问权
            m_mutexWriteToDB.WaitOne();

            // 然后获取内存表的访问权
            m_mutexDataTable.WaitOne();

            if (m_tableDataAdded.Rows.Count <= 0)
            {
                m_mutexDataTable.ReleaseMutex();
                m_mutexWriteToDB.ReleaseMutex();
                return true; // 没有任何东西需要写入数据库
            }
            //清空内存表的所有内容，把内容复制到临时表tmp中
            DataTable tmp = m_tableDataAdded.Copy();
            m_tableDataAdded.Rows.Clear();

            // 释放内存表的互斥量
            m_mutexDataTable.ReleaseMutex();

            try
            {
                //将临时表中的内容写入数据库
                SqlConnection conn = CDBManager.Instance.GetConnection();
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.DestinationTableName = CT_TableName;
                    //bulkCopy.ColumnMappings.Add(CN_StationID, CN_StationID);
                    //bulkCopy.ColumnMappings.Add(CN_WaterStage, CN_WaterStage);
                    //bulkCopy.ColumnMappings.Add(CN_WaterFlow, CN_WaterFlow);
                    bulkCopy.ColumnMappings.Add(CN_StationID, CN_StationID);//映射字段名 DataTable列名 ,数据库 对应的列名  
                    bulkCopy.ColumnMappings.Add(CN_BGTM, CN_BGTM);
                    bulkCopy.ColumnMappings.Add(CN_PTNO, CN_PTNO);
                    bulkCopy.ColumnMappings.Add(CN_ZR, CN_ZR);
                    bulkCopy.ColumnMappings.Add(CN_Q1, CN_Q1);
                    bulkCopy.ColumnMappings.Add(CN_Q2, CN_Q2);
                    bulkCopy.ColumnMappings.Add(CN_Q3, CN_Q3);
                    bulkCopy.ColumnMappings.Add(CN_Q4, CN_Q4);
                    bulkCopy.ColumnMappings.Add(CN_Q5, CN_Q5);
                    bulkCopy.ColumnMappings.Add(CN_Q6, CN_Q6);
                    bulkCopy.ColumnMappings.Add(CN_currQ, CN_currQ);
                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
                conn.Close();   //关闭连接
                Debug.WriteLine("###{0} :add {1} lines to waterflowmap db", DateTime.Now, tmp.Rows.Count);
                return true; // 写入数据库成功
            }
            catch (System.Exception ex)
            {
                // 遇到异常，写入数据库失败
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                //  总是释放互斥量
                m_mutexWriteToDB.ReleaseMutex();
            }

        }

        #endregion  帮助方法


    }
}
