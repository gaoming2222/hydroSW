using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using System.Text;

namespace Hydrology.DBManager.DB.SQLServer
{
    public class CSQLWarningInfo : CSQLBase, IWarningInfoProxy
    {
        #region 静态常量
        private static readonly string CT_TableName = "WarningInfo";    //数据库警告信息表的名字
        private static readonly string CN_DataTime = "DataTime";        //警告信息的时间
        private static readonly string CN_InfoDetail = "InfoDetail";    //警告信息的详细内容
        private static readonly string CN_InfoID = "InfoID";            //警告信息的唯一ID
        private static readonly string CN_ErroCode = "ErroCode";        //错误代码列
        private static readonly string CN_StationID = "StationID";      //站点id,4位字符串
        #endregion

        public CSQLWarningInfo()
            : base()
        {
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_DataTime);
            m_tableDataAdded.Columns.Add(CN_InfoDetail);
            m_tableDataAdded.Columns.Add(CN_InfoID);
            m_tableDataAdded.Columns.Add(CN_ErroCode);
            m_tableDataAdded.Columns.Add(CN_StationID);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_WarningInfo;
        }

        // 添加新列
        public void AddNewRow(CEntityWarningInfo entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_DataTime] = entity.DataTime;
            row[CN_InfoDetail] = entity.InfoDetail;
            row[CN_InfoDetail] = entity.InfoDetail;
            if (entity.WarningInfoCodeType.HasValue)
            {
                row[CN_ErroCode] = CEnumHelper.WarningCodeTypeToDBStr(entity.WarningInfoCodeType.Value);
            }
            else
            {
                row[CN_ErroCode] = null;
            }
            row[CN_StationID] = entity.StrStationId.ToString();
            m_tableDataAdded.Rows.Add(row);
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                base.NewTask(() => { AddDataToDB(); });
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer.Start();
            }
            m_mutexDataTable.ReleaseMutex();
        }

        public bool Add(CEntityWarningInfo entity)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_DataTime] = entity.DataTime;
            row[CN_InfoDetail] = entity.InfoDetail;
            m_tableDataAdded.Rows.Add(row);
            m_mutexDataTable.ReleaseMutex();
            return AddDataToDB();
        }

        public bool AddRange(List<CEntityWarningInfo> listEntitys)
        {
            m_mutexDataTable.WaitOne();
            foreach (CEntityWarningInfo entity in listEntitys)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_DataTime] = entity.DataTime;
                row[CN_InfoDetail] = entity.InfoDetail;
                m_tableDataAdded.Rows.Add(row);
            }
            m_mutexDataTable.ReleaseMutex();
            return AddDataToDB();
        }

        public List<CEntityWarningInfo> QueryWarningInfo(DateTime startTime, DateTime endTime)
        {
            string sql = string.Format("select * from {0} where {1} between {2} and {3};",
                CT_TableName,
                CN_DataTime,
                base.DateTimeToDBStr(startTime), base.DateTimeToDBStr(endTime));
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                List<CEntityWarningInfo> results = new List<CEntityWarningInfo>();
                for (int i = 0; i < dataTableTmp.Rows.Count; ++i)
                {
                    CEntityWarningInfo entity = new CEntityWarningInfo();
                    entity.DataTime = DateTime.Parse(dataTableTmp.Rows[i][CN_DataTime].ToString());
                    entity.InfoDetail = dataTableTmp.Rows[i][CN_InfoDetail].ToString();
                    entity.WarningInfoID = long.Parse(dataTableTmp.Rows[i][CN_InfoID].ToString());
                    results.Add(entity);
                }
                return results;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine("QueryWarningInfo Exception\n{0}",ex.ToString());
                return null;
            }
        }

        public bool DeleteWarningInfo(long id)
        {
            //删除告警信息
            string sql = string.Format("delete from {0} where {1} = {2};", CT_TableName,
                CN_InfoID, id);
            return base.ExecuteSQLCommand(sql);
        }
        public bool DeleteRange(List<long> listId)
        {
            if (listId.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < listId.Count; ++i)
            {
                sql.AppendFormat("delete from {0} where {1} = {2};",
                    CT_TableName,
                    CN_InfoID, listId[i]);
            }
            return base.ExecuteSQLCommand(sql.ToString());
        }
        /// <summary>
        /// 删除时间段内的所有报警信息
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public bool DeleteRange(DateTime timeStart, DateTime timeEnd)
        {
            string sql = string.Format("delete from {0} where {1} between {2} and {3};",
                CT_TableName, CN_DataTime,
                DateTimeToDBStr(timeStart), DateTimeToDBStr(timeEnd));
            return base.ExecuteSQLCommand(sql);
        }

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
                return true;
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
                    //bulkCopy.ColumnMappings.Add(CN_RainID, CN_RainID);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_InfoDetail, CN_InfoDetail);
                    bulkCopy.ColumnMappings.Add(CN_StationID, CN_StationID);
                    bulkCopy.ColumnMappings.Add(CN_ErroCode, CN_ErroCode);
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
                Debug.WriteLine("###{0} :add {1} lines to warning info db", DateTime.Now, tmp.Rows.Count);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                m_mutexWriteToDB.ReleaseMutex();
            }
        }
        #endregion ///< 帮助方法

        
    }
}
