using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Data;
using Hydrology.DBManager;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using Hydrology.DBManager.Interface;

namespace Hydrology.DBManager.DB.SQLServer
{
    /// <summary>
    /// 墒情数据表操作的封装类
    /// </summary>
    public class CSQLSoilData : CSQLBase, ISoilDataProxy
    {
        #region 静态常量
        //public static readonly string CT_TableNameOld = "HydrologytestDB.dbo.SoilData"; 
        public static readonly string CT_TableName = "SoilData";      //数据库中墒情数据表的名字
        public static readonly string CN_StationId = "StationID";   // 站点唯一ID
        public static readonly string CN_DataTime = "DataTime";       // 字段日期时间
        public static readonly string CN_Voltage = "Voltage";         // 电压值
        public static readonly string CN_ChannelType = "TransType";       // 信道类型
        public static readonly string CN_MessageType = "MessageType";            // 保文类型
        public static readonly string CN_Voltage10 = "Voltage10";              // 10厘米处的电压值
        public static readonly string CN_Moisture10 = "Moisture10";             // 10厘米处的含水率
        public static readonly string CN_Voltage20 = "Voltage20";              // 20厘米处的电压值
        public static readonly string CN_Moisture20 = "Moisture20";             // 20厘米处的含水率
        public static readonly string CN_Voltage30 = "Voltage30";              // 30厘米处的电压值
        public static readonly string CN_Moisture30 = "Moistrue30";             // 30厘米处的含水率
        public static readonly string CN_Voltage40 = "Voltage40";              // 40厘米处的电压值
        public static readonly string CN_Moisture40 = "Moistrue40";             // 40厘米处的含水率
        public static readonly string CN_Voltage60 = "Voltage60";              // 60厘米处的电压值
        public static readonly string CN_Moisture60 = "Moistrue60";             // 160厘米处的含水率
        public static readonly string CN_RecvTime = "recvdatatime";
        public static readonly string CN_State = "state";
        #endregion

        #region 公共方法
        public CSQLSoilData()
            : base()
        {
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_StationId);
            m_tableDataAdded.Columns.Add(CN_DataTime);
            m_tableDataAdded.Columns.Add(CN_Voltage);
            m_tableDataAdded.Columns.Add(CN_ChannelType);
            m_tableDataAdded.Columns.Add(CN_MessageType);

            m_tableDataAdded.Columns.Add(CN_Voltage10);
            m_tableDataAdded.Columns.Add(CN_Moisture10);

            m_tableDataAdded.Columns.Add(CN_Voltage20);
            m_tableDataAdded.Columns.Add(CN_Moisture20);

            m_tableDataAdded.Columns.Add(CN_Voltage30);
            m_tableDataAdded.Columns.Add(CN_Moisture30);

            m_tableDataAdded.Columns.Add(CN_Voltage40);
            m_tableDataAdded.Columns.Add(CN_Moisture40);

            m_tableDataAdded.Columns.Add(CN_Voltage60);
            m_tableDataAdded.Columns.Add(CN_Moisture60);

            m_tableDataAdded.Columns.Add(CN_RecvTime);
            m_tableDataAdded.Columns.Add(CN_State);
            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_SoilStationData;

        }

        /// <summary>
        /// 异步添加一个数据记录
        /// </summary>
        /// <param name="entity"></param>
        public void AddNewRow(CEntitySoilData entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_StationId] = entity.StationID;
            row[CN_DataTime] = entity.DataTime.ToString(CDBParams.GetInstance().DBDateTimeFormat);
            row[CN_Voltage] = entity.DVoltage;
            row[CN_MessageType] = CEnumHelper.MessageTypeToDBStr(entity.MessageType);
            row[CN_ChannelType] = CEnumHelper.ChannelTypeToDBStr(entity.ChannelType);

            row[CN_Voltage10] = entity.Voltage10;
            row[CN_Moisture10] = entity.Moisture10;

            row[CN_Voltage20] = entity.Voltage20;
            row[CN_Moisture20] = entity.Moisture20;

            row[CN_Voltage30] = entity.Voltage30;
            row[CN_Moisture30] = entity.Moisture30;

            row[CN_Voltage40] = entity.Voltage40;
            row[CN_Moisture40] = entity.Moisture40;

            row[CN_Voltage60] = entity.Voltage60;
            row[CN_Moisture60] = entity.Moisture60;

            row[CN_RecvTime] = entity.reciveTime;
            row[CN_State] = entity.state;

            m_tableDataAdded.Rows.Add(row);
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                //Task task = new Task(() => { AddDataToDB(); });
                //task.Start();
                //m_mutexDataTable.ReleaseMutex();
                //Thread.Sleep(10 * 1000);
                NewTask(() => { AddDataToDB(); });
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer.Start();
            }
            m_mutexDataTable.ReleaseMutex();
        }

        /// <summary>
        /// 异步添加数据到数据库
        /// </summary>
        /// <param name="listData"></param>
        public void AddSoilDataRange(List<CEntitySoilData> listData)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntitySoilData entity in listData)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_StationId] = entity.StationID;
                row[CN_DataTime] = entity.DataTime.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_Voltage] = entity.DVoltage;
                row[CN_MessageType] = CEnumHelper.MessageTypeToDBStr(entity.MessageType);
                row[CN_ChannelType] = CEnumHelper.ChannelTypeToDBStr(entity.ChannelType);

                row[CN_Voltage10] = entity.Voltage10;
                row[CN_Moisture10] = entity.Moisture10;

                row[CN_Voltage20] = entity.Voltage20;
                row[CN_Moisture20] = entity.Moisture20;

                row[CN_Voltage30] = entity.Voltage30;
                row[CN_Moisture30] = entity.Moisture30;

                row[CN_Voltage40] = entity.Voltage40;
                row[CN_Moisture40] = entity.Moisture40;

                row[CN_Voltage60] = entity.Voltage60;
                row[CN_Moisture60] = entity.Moisture60;

                row[CN_RecvTime] = entity.reciveTime;
                row[CN_State] = entity.state;

                m_tableDataAdded.Rows.Add(row);
            }
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 超过最大缓存，开线程写入数据库
                NewTask(() => { AddDataToDB(); });
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer.Start();
            }
            m_mutexDataTable.ReleaseMutex();

        }

        /// <summary>
        /// 根据起始时间以及结束时间还有站点ID,查询墒情数据
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        public List<CEntitySoilData> QueryByStationAndTime(string stationId, DateTime timeStart, DateTime timeEnd)
        {
            List<CEntitySoilData> results = new List<CEntitySoilData>();
            try
            {
                string sql = string.Format("select * from {0} where {1} = {2} and {3} between {4} and {5} order by {6}",
                CT_TableName,
                CN_StationId, stationId,
                CN_DataTime, DateTimeToDBStr(timeStart), DateTimeToDBStr(timeEnd),
                CN_DataTime);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
                {
                    CEntitySoilData data = new CEntitySoilData();
                    data.StationID = dataTableTmp.Rows[rowid][CN_StationId].ToString();
                    data.DataTime = DateTime.Parse(dataTableTmp.Rows[rowid][CN_DataTime].ToString());
                    data.DVoltage = decimal.Parse(dataTableTmp.Rows[rowid][CN_Voltage].ToString());
                    data.MessageType = CEnumHelper.DBStrToMessageType(dataTableTmp.Rows[rowid][CN_MessageType].ToString());
                    data.ChannelType = CEnumHelper.DBStrToChannelType(dataTableTmp.Rows[rowid][CN_ChannelType].ToString());

                    data.Voltage10 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage10]);
                    data.Moisture10 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture10]);

                    data.Voltage20 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage20]);
                    data.Moisture20 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture20]);

                    data.Voltage30 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage30]);
                    data.Moisture30 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture30]);

                    data.Voltage40 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage40]);
                    data.Moisture40 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture40]);

                    data.Voltage60 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage60]);
                    data.Moisture60 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture60]);

                    data.reciveTime = DateTime.Parse(dataTableTmp.Rows[rowid][CN_DataTime].ToString());

                    try
                    {
                        if (dataTableTmp.Rows[rowid][CN_RecvTime].ToString() != "")
                        {
                            data.reciveTime = DateTime.Parse(dataTableTmp.Rows[rowid][CN_RecvTime].ToString());
                        }
                    }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                    catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                    {
                        data.reciveTime = DateTime.Now;
                    }
                    try
                    {
                        if (dataTableTmp.Rows[rowid][CN_State].ToString() != "")
                        {
                            data.state = int.Parse(dataTableTmp.Rows[rowid][CN_State].ToString());
                        }
                    }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                    catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                    {
                        data.state = 1;
                    }
                    results.Add(data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return results;
        }

        /// <summary>
        /// 获取某个站点最新的数据，如果没有，lastData = null
        /// </summary>
        /// <param name="lastData"></param>
        /// <returns></returns>
        public bool GetLastStationData(string stationId, out CEntitySoilData lastData)
        {
            lastData = null; 
            string sql = string.Format("select top 1 * from {0} where {1} = {2} order by {3} desc",
                CT_TableName,
                CN_StationId, stationId,
                CN_DataTime);
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            // 只有一行噻？不然怎么玩？？
            if( dataTableTmp.Rows.Count == 1 )
            {
                int rowid = 0;
                CEntitySoilData data = new CEntitySoilData();
                data.StationID = dataTableTmp.Rows[rowid][CN_StationId].ToString();
                data.DataTime = DateTime.Parse(dataTableTmp.Rows[rowid][CN_DataTime].ToString());
                data.DVoltage = decimal.Parse(dataTableTmp.Rows[rowid][CN_Voltage].ToString());
                data.MessageType = CEnumHelper.DBStrToMessageType(dataTableTmp.Rows[rowid][CN_MessageType].ToString());
                data.ChannelType = CEnumHelper.DBStrToChannelType(dataTableTmp.Rows[rowid][CN_ChannelType].ToString());

                data.Voltage10 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage10]);
                data.Moisture10 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture10]);

                data.Voltage20 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage20]);
                data.Moisture20 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture20]);

                data.Voltage30 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage30]);
                data.Moisture30 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture30]);

                data.Voltage40 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage40]);
                data.Moisture40 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture40]);

                data.Voltage60 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Voltage60]);
                data.Moisture60 = GetCellFloatValue(dataTableTmp.Rows[rowid][CN_Moisture60]);
                try
                {
                    if (dataTableTmp.Rows[rowid][CN_RecvTime].ToString() != "")
                    {
                        data.reciveTime = DateTime.Parse(dataTableTmp.Rows[rowid][CN_RecvTime].ToString());
                    }
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {
                    data.reciveTime = DateTime.Now;
                }
                try
                {
                    if (dataTableTmp.Rows[rowid][CN_State].ToString() != "")
                    {
                        data.state = int.Parse(dataTableTmp.Rows[rowid][CN_State].ToString());
                    }
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {
                    data.state = 1;
                }
                lastData = data;
                
            }
          //  Debug.WriteLine(string.Format("查询站点{0}最新墒情数据完成", stationId));
            return true;
        }

        public bool UpdateRows(List<CEntitySoilData> listData)
        {
            // 除主键外和站点外，其余信息随意修改
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < listData.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6},{7}='{8}',{9}={10},{11}={12},{13}={14},{15}={16},{17}={18}, {19}={20}, {21}={22} where {23}={24} and {25}='{26}';",
                    CT_TableName,
                    CN_Voltage, listData[i].DVoltage,
                    CN_ChannelType, CEnumHelper.ChannelTypeToDBStr(listData[i].ChannelType),
                    CN_MessageType, CEnumHelper.MessageTypeToDBStr(listData[i].MessageType),
                    CN_RecvTime, listData[i].reciveTime.ToString(),
                    CN_Moisture10,listData[i].Moisture10,
                    CN_Moisture20,listData[i].Moisture20,
                    CN_Moisture40,listData[i].Moisture40,
                    CN_Voltage10, listData[i].Voltage10,
                    CN_Voltage20, listData[i].Voltage20,
                    CN_Voltage40, listData[i].Voltage40,
                    CN_State, listData[i].state,
                    CN_StationId, listData[i].StationID,
                    CN_DataTime, listData[i].DataTime.ToString()
                    //    CN_VoltageID, voltages[i].VoltageID
                );
                //if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
                //{
                    // 更新数据库
                    //if (!this.ExecuteSQLCommand(sql.ToString()))
                    //{
                    //    // 保存失败
                    //    return false;
                    //}
                   
                    //currentBatchCount = 0;
                
            }
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql.ToString()))
            {
                return false;
            }
            sql.Clear(); //清除以前的所有命令
            ResetAll();
            return true;
        }

        public bool DeleteRows(List<String> soildatas_StationId, List<String> soildatas_StationDate)
        {
            // 删除某条雨量记录
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < soildatas_StationId.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("delete from {0} where {1}={2} and {3}='{4}';",
                    CT_TableName,
                    CN_StationId, soildatas_StationId[i].ToString(),
                    CN_DataTime, soildatas_StationDate[i].ToString()
                    // CN_VoltageID, voltages[i].ToString()
                );
                if (currentBatchCount >= CDBParams.GetInstance().UpdateBufferMax)
                {
                    // 更新数据库
                    if (!this.ExecuteSQLCommand(sql.ToString()))
                    {
                        // 保存失败
                        return false;
                    }
                    sql.Clear(); //清除以前的所有命令
                    currentBatchCount = 0;
                }
            }
            // 如何考虑线程同异步
            if (!ExecuteSQLCommand(sql.ToString()))
            {
                return false;
            }
            ResetAll();
            return true;
        }
        #endregion

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

            // 获取对数据库的唯一访问权
            m_mutexWriteToDB.WaitOne();

            try
            {
                //将临时表中的内容写入数据库
                SqlConnection conn = CDBManager.GetInstacne().GetConnection();
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = CT_TableName;
                    bulkCopy.BatchSize = 1;
					bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_Voltage, CN_Voltage);
                    bulkCopy.ColumnMappings.Add(CN_MessageType, CN_MessageType);
                    bulkCopy.ColumnMappings.Add(CN_ChannelType, CN_ChannelType);

                    bulkCopy.ColumnMappings.Add(CN_Voltage10, CN_Voltage10);
                    bulkCopy.ColumnMappings.Add(CN_Moisture10, CN_Moisture10);

                    bulkCopy.ColumnMappings.Add(CN_Voltage20, CN_Voltage20);
                    bulkCopy.ColumnMappings.Add(CN_Moisture20, CN_Moisture20);

                    bulkCopy.ColumnMappings.Add(CN_Voltage30, CN_Voltage30);
                    bulkCopy.ColumnMappings.Add(CN_Moisture30, CN_Moisture30);

                    bulkCopy.ColumnMappings.Add(CN_Voltage40, CN_Voltage40);
                    bulkCopy.ColumnMappings.Add(CN_Moisture40, CN_Moisture40);

                    bulkCopy.ColumnMappings.Add(CN_Voltage60, CN_Voltage60);
                    bulkCopy.ColumnMappings.Add(CN_Moisture60, CN_Moisture60);

                    bulkCopy.ColumnMappings.Add(CN_RecvTime, CN_RecvTime);
                    bulkCopy.ColumnMappings.Add(CN_State, CN_State);
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
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                m_mutexWriteToDB.ReleaseMutex();
                return false;
            }
            Debug.WriteLine("###{0} :add {1} lines to soil data db", DateTime.Now, tmp.Rows.Count);
            CDBLog.Instance.AddInfo(string.Format("添加{0}行到墒情记录表", tmp.Rows.Count));
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }
        #endregion ///< 帮助方法

        //1009gm CT_TableNameOld
        public List<CEntitySoilData> QueryForMonthTable(string stationId, DateTime date)
        {
            List<CEntitySoilData> results = new List<CEntitySoilData>();
            string sql = "select * from " + CT_TableName + " where Datatime in('";
            for (int j = 0; j < 23; j++)
            {
                sql = sql + date + "','";
                date = date.AddHours(1);
            }
            sql = sql + date + "') and StationID=" + stationId + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            int flag = dataTableTemp.Rows.Count;
            if (flag == 0)
            {
            }
            else
            {
                for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
                {
                    CEntitySoilData soilData = new CEntitySoilData();
                    object aa = dataTableTemp.Rows[rowid][CN_Moisture20].ToString();
                    if (dataTableTemp.Rows[rowid][CN_Moisture10].ToString() != "")
                    {
                        soilData.Moisture10 = float.Parse(dataTableTemp.Rows[rowid][CN_Moisture10].ToString());
                    }
                    else
                    {
                        soilData.Moisture10 = -1;
                    }
                    if (dataTableTemp.Rows[rowid][CN_Moisture20].ToString() != "")
                    {
                        soilData.Moisture20 = float.Parse(dataTableTemp.Rows[rowid][CN_Moisture20].ToString());
                    }
                    else
                    {
                        soilData.Moisture20 = -1;
                    }
                    if (dataTableTemp.Rows[rowid][CN_Moisture30].ToString() != "")
                    {
                        soilData.Moisture30 = float.Parse(dataTableTemp.Rows[rowid][CN_Moisture30].ToString());
                    }
                    else
                    {
                        soilData.Moisture30 = -1;
                    }
                    results.Add(soilData);
                }
            }
            return results;
        }

        // 恢复初始状态
        private void ResetAll()
        {
            m_mutexDataTable.WaitOne();
            m_iPageCount = -1;
            m_mapDataTable.Clear(); //清空所有记录
            m_mutexDataTable.ReleaseMutex();
        }
    }
}
