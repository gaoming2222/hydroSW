using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Data;
using Hydrology.DBManager;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using Hydrology.DBManager.Interface;

namespace Hydrology.DBManager.DB.SQLServer
{
    public class CSQLCommunicationRate:CSQLBase, ICommunicationRateProxy
    {
        #region 静态常量
        private static readonly string CT_TableName = "CommunicateRate"; //数据库中畅通率表的名字
        private static readonly string CN_StationID = "StationID";
        private static readonly string CN_DataTime = "DataTime"; 
        #endregion 静态常量

        public CSQLCommunicationRate()
            : base()
        {
            m_tableDataAdded.Columns.Add(CN_StationID);
            m_tableDataAdded.Columns.Add(CN_DataTime);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_CommunicationRage;
        }
        /// <summary>
        /// 异步添加新的记录
        /// </summary>
        public void AddNewRow(CEntityCommunicationRate entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_DataTime] = entity.Time;
            row[CN_StationID] = entity.StationID;
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

        public void AddRange(List<CEntityCommunicationRate> listEntitys)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityCommunicationRate entity in listEntitys)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_DataTime] = entity.Time;
                row[CN_StationID] = entity.StationID;
                m_tableDataAdded.Rows.Add(row);
            }
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                NewTask(() => { AddDataToDB(); });
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer.Start();
            }
            m_mutexDataTable.ReleaseMutex();
        }

        public List<DateTime> QueryRecordByStationIdAndPeriod(string stationId, DateTime startTime,
            DateTime endTime)
        {
            List<DateTime> results = new List<DateTime>();
            try
            {
                string sql = string.Format("select {0} from {1} where {2} = '{3}' and {4} between {5} and {6}",
                    CN_DataTime,
                    CT_TableName,
                    CN_StationID, stationId,
                    CN_DataTime, DateTimeToDBStr(startTime), DateTimeToDBStr(endTime)
                    );
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                for (int i = 0; i < dataTableTmp.Rows.Count; ++i)
                {
                    DateTime time = DateTime.Parse(dataTableTmp.Rows[i][CN_DataTime].ToString());
                    results.Add(time);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
            return results;
        }

        public int GetCountOfRecordByStationIdAndPeriod(string stationId, DateTime startTime,
            DateTime endTime)
        {
            int iRecordCount = 0; 
            try
            {
                string sql = string.Format("select count(*) from {1} where {2} = '{3}' and {4} between {5} and {6}",
                    CN_DataTime,
                    CT_TableName,
                    CN_StationID, stationId,
                    CN_DataTime, DateTimeToDBStr(startTime), DateTimeToDBStr(endTime)
                    );
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                if (dataTableTmp.Rows.Count == 1)
                {
                    iRecordCount = int.Parse(dataTableTmp.Rows[0][0].ToString());
                }
                else
                {
                    // 不可能，应该有错误
                    Debug.WriteLine("GetCountOfRecordByStationIdAndPeriod Error");
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return -1;
            }
            return iRecordCount;
        }


        #region 帮助方法
        // 将当前所有数据写入数据库
        protected override bool AddDataToDB()
        {
            // 先获取内存表的访问权
            m_mutexDataTable.WaitOne();

            if (m_tableDataAdded.Rows.Count <= 0)
            {
                m_mutexDataTable.ReleaseMutex();
                return true;
            }
            //清空内存表的所有内容，把内容复制到临时表tmp中
            DataTable tmp = m_tableDataAdded.Copy();
            m_tableDataAdded.Rows.Clear();

            // 释放内存表的互斥量
            m_mutexDataTable.ReleaseMutex();

            // 再获取对数据库的唯一访问权
            m_mutexWriteToDB.WaitOne();

            try
            {
                //将临时表中的内容写入数据库
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(CDBManager.GetInstacne().GetConnectionString()))
                {
                    bulkCopy.DestinationTableName = CT_TableName;
                    bulkCopy.BatchSize = 1;
					bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.ColumnMappings.Add(CN_StationID, CN_StationID);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);

                    try {
                        bulkCopy.WriteToServer(tmp);
                    } catch(Exception e) {
                        Debug.WriteLine(e.ToString());
                    }
                }
                System.Diagnostics.Debug.WriteLine("###{0} :add {1} lines to commnunicationrate", DateTime.Now, tmp.Rows.Count);
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
