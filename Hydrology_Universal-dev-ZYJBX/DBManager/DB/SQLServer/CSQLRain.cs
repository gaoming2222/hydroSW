using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;

namespace Hydrology.DBManager.DB.SQLServer
{
    /// <summary>
    /// 提供SQL Server 数据库中对雨量表的操作
    /// </summary>
    public class CSQLRain : CSQLBase, IRainProxy
    {
        #region 数据成员
        private const string CT_EntityName = "CEntityRain";   //  数据库表Rain实体类
        //public static readonly string CT_TableName = "rain"; //数据库中雨量表的名字
        //0504
        //public static readonly string CT_TableName = "rain" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString(); //数据库中雨量表的名字
        public static readonly string CT_TableName = "rain"; //数据库中雨量表的名字
        //  public static readonly string CN_RainID = "RID";            //雨量表的唯一ID
        public static readonly string CN_StationId = "stationid"; //站点ID
        public static readonly string CN_DataTime = "datatime";    //数据的采集时间

        public static readonly string CN_PeriodRain = "periodrain";  //雨量差值,即时段雨量
        public static readonly string CN_DifferenceRain = "differencerain";  //雨量差值,即时段雨量
        public static readonly string CN_TotalRain = "totalrain";     //累积雨量
        public static readonly string CN_DayRain = "dayrain";       //日雨量

        public static readonly string CN_DataState = "state";       //状态，True or False
        public static readonly string CN_TransType = "transtype";  //通讯方式
        public static readonly string CN_MsgType = "messagetype";  //报送类型
        public static readonly string CN_RecvDataTime = "recvdatatime";    //接收到数据的时间

        private const int CN_FIELD_COUNT = 9;
        private List<long> m_listDelRows;            // 删除雨量记录的链表
        private List<CEntityRain> m_listUpdateRows; // 更新雨量录的链表

        private string m_strStaionId;       //需要查询的测站
        private DateTime m_startTime;  //查询起始时间
        private DateTime m_endTime;    //查询结束时间
        private bool m_TimeSelect;
        private string TimeSelectString
        {
            get
            {
                if (m_TimeSelect == false)
                {
                    return "";
                }
                else
                {
                    return "convert(VARCHAR," + CN_DataTime + ",120) LIKE '%00:00%' and ";
                }
            }
        }


        public System.Timers.Timer m_addTimer_1;

        #endregion


        public CSQLRain()
            : base()
        {
            m_listDelRows = new List<long>();
            m_listUpdateRows = new List<CEntityRain>();
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_StationId);
            // m_tableDataAdded.Columns.Add(CN_RainID);
            m_tableDataAdded.Columns.Add(CN_DataTime);

            m_tableDataAdded.Columns.Add(CN_PeriodRain);
            m_tableDataAdded.Columns.Add(CN_DifferenceRain);
            //  m_tableDataAdded.Columns.Add(CN_RecvDataTime);
            m_tableDataAdded.Columns.Add(CN_TotalRain);
            m_tableDataAdded.Columns.Add(CN_DayRain);

            m_tableDataAdded.Columns.Add(CN_TransType);
            m_tableDataAdded.Columns.Add(CN_MsgType);

            //  m_tableDataAdded.Columns.Add(CN_DataState);
            m_tableDataAdded.Columns.Add(CN_RecvDataTime);
            m_tableDataAdded.Columns.Add(CN_DataState);
            // 分页查询相关
            m_strStaionId = null;

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_Rain;

            m_addTimer_1 = new System.Timers.Timer();
            m_addTimer_1.Elapsed += new System.Timers.ElapsedEventHandler(EHTimer_1);
            m_addTimer_1.Enabled = false;
            m_addTimer_1.Interval = CDBParams.GetInstance().AddToDbDelay;
        }

        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void EHTimer_1(object source, System.Timers.ElapsedEventArgs e)
        {
            //定时器事件，将所有的记录都写入数据库
            m_addTimer_1.Stop();  //停止定时器
            m_dateTimePreAddTime = DateTime.Now;
            //将数据写入数据库
            NewTask(() => { InsertSqlBulk(m_tableDataAdded); });
        }

        public void AddNewRow(CEntityRain rain)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_StationId] = rain.StationID;
            row[CN_DataTime] = rain.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
            row[CN_PeriodRain] = rain.PeriodRain;
            row[CN_DifferenceRain] = rain.DifferneceRain;
            row[CN_TotalRain] = rain.TotalRain;
            row[CN_DayRain] = rain.DayRain;

            row[CN_TransType] = CEnumHelper.ChannelTypeToDBStr(rain.ChannelType);
            row[CN_MsgType] = CEnumHelper.MessageTypeToDBStr(rain.MessageType);

            row[CN_RecvDataTime] = rain.TimeRecieved.ToString(CDBParams.GetInstance().DBDateTimeFormat);

            row[CN_DataState] = rain.BState;
            m_tableDataAdded.Rows.Add(row);
            m_mutexDataTable.ReleaseMutex();

            // 判断是否需要创建新分区
            //CSQLPartitionMgr.Instance.MaintainRain(rain.TimeCollect);
            // 可能被多线程抢断，不过Count值总是最新的
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
        }

        public void AddNewRows(List<Hydrology.Entity.CEntityRain> rains)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityRain rain in rains)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_StationId] = rain.StationID;
                row[CN_DataTime] = rain.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_PeriodRain] = rain.PeriodRain;
                row[CN_DifferenceRain] = rain.DifferneceRain;
                row[CN_TotalRain] = rain.TotalRain;
                row[CN_DayRain] = rain.DayRain;

                row[CN_TransType] = CEnumHelper.ChannelTypeToDBStr(rain.ChannelType);
                row[CN_MsgType] = CEnumHelper.MessageTypeToDBStr(rain.MessageType);

                row[CN_RecvDataTime] = rain.TimeRecieved.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                //row[CN_StationId] = rain.StationID;
                //row[CN_DataTime] = rain.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                //row[CN_PeriodRain] = rain.PeriodRain;
                //row[CN_TotalRain] = rain.TotalRain;
                //row[CN_MsgType] = CEnumHelper.MessageTypeToDBStr(rain.MessageType);
                //row[CN_TransType] = CEnumHelper.ChannelTypeToDBStr(rain.ChannelType);
                //row[CN_DayRain] = rain.DayRain;
                //row[CN_RecvDataTime] = rain.TimeRecieved.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_DataState] = rain.BState;
                m_tableDataAdded.Rows.Add(row);

                // 判断是否需要创建新分区
                //CSQLPartitionMgr.Instance.MaintainRain(rain.TimeCollect);
            }
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                //   NewTask(() => { AddDataToDB(); });
                NewTask(() => { InsertSqlBulk(m_tableDataAdded); });
            }
            else
            {
                // 没有超过缓存最大值，开启定时器进行检测,多次调用Start()会导致重新计数
                m_addTimer_1.Start();
            }
            // AddDataToDB(); 
            m_mutexDataTable.ReleaseMutex();
        }

        public bool DeleteRows(List<String> rains_StationId, List<String> rains_StationDate)
        {
            // 删除某条雨量记录
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < rains_StationId.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("delete from {0} where {1}={2} and {3}='{4}';",
                    CT_TableName,
                    CN_StationId, rains_StationId[i].ToString(),
                    CN_DataTime, rains_StationDate[i].ToString()
                // CN_RainID, rains[i].ToString()
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
            if (!ExecuteSQLCommand(sql.ToString()))
            {
                ExecuteSQLCommand(sql.ToString());
                //return false;
            }
            ResetAll();
            // 如何考虑线程同异步
            return true;

        }

        public bool UpdateRows(List<Hydrology.Entity.CEntityRain> rains)
        {
            // 除主键外和站点外，其余信息随意修改
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < rains.Count; i++)
            {

                ++currentBatchCount;
                sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6},{7}={8},{9}={10},{11}={12},{13}='{14}',{15} ='{16}' where {17}={18} and {19}='{20}';",
                    CT_TableName,
                    //  CN_DataTime, DateTimeToDBStr(rains[i].TimeCollect),
                    CN_PeriodRain, rains[i].PeriodRain.HasValue ? rains[i].PeriodRain.Value.ToString() : "null",
                    CN_TotalRain, rains[i].TotalRain.HasValue ? rains[i].TotalRain.Value.ToString() : "null",
                    CN_DifferenceRain, rains[i].DifferneceRain.HasValue ? rains[i].DifferneceRain.Value.ToString() : "null",
                    CN_DayRain, rains[i].DayRain.HasValue ? rains[i].DayRain.Value.ToString() : "null",
                    CN_TransType, CEnumHelper.ChannelTypeToDBStr(rains[i].ChannelType),
                    CN_MsgType, CEnumHelper.MessageTypeToDBStr(rains[i].MessageType),
                    CN_RecvDataTime, rains[i].TimeRecieved.ToString(),
                    CN_DataState, rains[i].BState,
                    //CN_RainID, rains[i].RainID
                    CN_StationId, rains[i].StationID,
                    CN_DataTime, rains[i].TimeCollect.ToString()
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
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql.ToString()))
            {
                return false;
            }
            ResetAll();
            return true;
        }

        public void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            // 设置查询条件
            if (null == m_strStaionId)
            {
                // 第一次查询
                m_iRowCount = -1;
                m_iPageCount = -1;
                m_strStaionId = stationId;
                m_startTime = timeStart;
                m_endTime = timeEnd;
                m_TimeSelect = TimeSelect;
            }
            else
            {
                // 不是第一次查询
                if (stationId != m_strStaionId || timeStart != m_startTime || timeEnd != m_endTime || m_TimeSelect != TimeSelect)
                {
                    m_iRowCount = -1;
                    m_iPageCount = -1;
                    m_mapDataTable.Clear(); //清空上次查询缓存
                }
                m_strStaionId = stationId;
                m_startTime = timeStart;
                m_endTime = timeEnd;
                m_TimeSelect = TimeSelect;
            }
        }

        public int GetPageCount()
        {
            //select top 300 ROW_NUMBER() over(order by datatime) as rowid,* from Tsrain where datatime between CAST('2009-12-22 09:09:09' as datetime) and CAST('2010-01-01 09:09:09' as datetime);
            if (-1 == m_iPageCount)
            {
                DoCountQuery();
            }
            return m_iPageCount;
        }

        public int GetRowCount()
        {
            if (-1 == m_iPageCount)
            {
                DoCountQuery();
            }
            return m_iRowCount;
        }

        public List<Hydrology.Entity.CEntityRain> GetPageData(int pageIndex)
        {
            if (pageIndex <= 0 || m_startTime == null || m_endTime == null || m_strStaionId == null)
            {
                return new List<CEntityRain>();
            }
            // 获取某一页的数据，判断所需页面是否在内存中有值
            int startIndex = (pageIndex - 1) * CDBParams.GetInstance().UIPageRowCount + 1;
            int key = (int)(startIndex / CDBParams.GetInstance().DBPageRowCount) + 1; //对应于数据库中的索引
            int startRow = startIndex - (key - 1) * CDBParams.GetInstance().DBPageRowCount - 1;
            Debug.WriteLine("startIndex;{0} key:{1} startRow:{2}", startIndex, key, startRow);
            // 判断MAP中是否有值
            if (m_mapDataTable.ContainsKey(key))
            {
                // 从内存中读取
                return CopyDataToList(key, startRow);
            }
            else
            {
                // 从数据库中查询
                int topcount = key * CDBParams.GetInstance().DBPageRowCount;
                int rowidmim = topcount - CDBParams.GetInstance().DBPageRowCount;
                string sql = " select * from ( " +
                    "select top " + topcount.ToString() + " row_number() over( order by " + CN_DataTime + " ) as " + CN_RowId + ",* " +
                    "from " + CT_TableName + " " +
                    "where " + CN_StationId + "=" + m_strStaionId.ToString() + " " +
                    "and " + TimeSelectString + CN_DataTime + " between " + DateTimeToDBStr(m_startTime) +
                    "and " + DateTimeToDBStr(m_endTime) +
                    ") as tmp1 " +
                    "where " + CN_RowId + ">" + rowidmim.ToString() +
                    " order by " + CN_DataTime + " DESC";
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                m_mapDataTable.Add(key, dataTableTmp);
                return CopyDataToList(key, startRow);
            }
        }

        public bool GetMinDataTime(ref DateTime time)
        {
            // 获取数据表中最早的记录时间
            string sql = string.Format("select top 1 {0} from {1} order by {2};",
                CN_DataTime,
                CT_TableName, CN_DataTime);
            //m_mutexWriteToDB.WaitOne();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                if (dataTableTmp.Rows.Count > 0)
                {
                    time = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                //m_mutexWriteToDB.ReleaseMutex();
            }
            return false;

        }

        public bool GetLastData(ref Nullable<Decimal> lastTotalRain, ref Nullable<DateTime> lastDataTime, ref Nullable<Decimal> lastDayTotalRain, ref Nullable<Decimal> llastDayTotalRain, ref Nullable<Decimal> lastSharpClockTotalRain, ref Nullable<DateTime> latSharpClockTime, ref Nullable<DateTime> lastDayTime, ref Nullable<EChannelType> lastChannelType, ref Nullable<EMessageType> lastMessageType, string stationId)
        {
            // 获取计算雨量值所需的数据
            try
            {
                // 获取最近一条的雨量值
                string sql = string.Format("select top 1 * from {0} where {1} = '{2}' order by {3} desc;",
                    CT_TableName,
                    CN_StationId, stationId,
                    CN_DataTime);
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                if (dataTableTmp.Rows.Count > 0)
                {
                    lastDataTime = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                    lastTotalRain = Decimal.Parse(dataTableTmp.Rows[0][CN_TotalRain].ToString());
                    //lastPeriodRain = Decimal.Parse(dataTableTmp.Rows[0][CN_PeriodRain].ToString());
                    //lastPeriodRain = base.GetCellDecimalValue(dataTableTmp.Rows[0][CN_PeriodRain]);
                    lastChannelType = CEnumHelper.DBStrToChannelType(dataTableTmp.Rows[0][CN_TransType].ToString());
                    lastMessageType = CEnumHelper.DBStrToMessageType(dataTableTmp.Rows[0][CN_MsgType].ToString());
                }
                else
                {
                    //       Debug.WriteLine(string.Format("查询雨量表为空,站点{0}", stationId));
                }

                // 获取最近一天的雨量记录
                sql = string.Format("select top 2 {0},{1} from {2} where {3} is not null and {4}='{5}'order by {6} desc;",
                    CN_TotalRain, CN_DataTime,
                    CT_TableName,
                    CN_DayRain,
                    CN_StationId, stationId,
                    CN_DataTime);
                adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);

                ////start
                //SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                //DataTable dataTableTemp = new DataTable();
                //adapter.Fill(dataTableTemp);
                //int flag = dataTableTemp.Rows.Count;
                //if (flag == 0)
                //{
                //}
                //else
                //{
                //    for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
                //    {
                //        CEntityRain rain = new CEntityRain();
                //        rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                //        rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                //        if (dataTableTemp.Rows[rowid][CN_PeriodRain].ToString() != "")
                //        {
                //            rain.PeriodRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_PeriodRain].ToString());
                //        }
                //        else
                //        {
                //            rain.PeriodRain = -9999;
                //        }
                //        if (dataTableTemp.Rows[rowid][CN_TotalRain].ToString() != "")
                //        {
                //            rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                //        }
                //        else
                //        {
                //            rain.TotalRain = -9999;
                //        }
                //        results.Add(rain);
                //    }
                //}
                //return results;
                ////end
                if (dataTableTmp.Rows.Count > 0)
                {
                    for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
                    {
                        if (rowid == 0)
                        {
                            lastDayTotalRain = Decimal.Parse(dataTableTmp.Rows[0][CN_TotalRain].ToString());
                            lastDayTime = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                        }
                        if (rowid == 1)
                        {

                            llastDayTotalRain = Decimal.Parse(dataTableTmp.Rows[1][CN_TotalRain].ToString());
                        }

                    }

                }
                else
                {
                    //        Debug.WriteLine(string.Format("查询日雨量为空,站点{0}", stationId));
                }
                //最近正点
                sql = string.Format("select top 1 {0},{1} from {2} where {3} is not null and {4}='{5}'order by {6} desc;",
                    CN_TotalRain, CN_DataTime,
                    CT_TableName,
                    CN_PeriodRain,
                    CN_StationId, stationId,
                    CN_DataTime);
                adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                if (dataTableTmp.Rows.Count > 0)
                {
                    lastSharpClockTotalRain = Decimal.Parse(dataTableTmp.Rows[0][CN_TotalRain].ToString());
                    latSharpClockTime = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                }
                else
                {
                    //       Debug.WriteLine(string.Format("查询日雨量为空,站点{0}", stationId));
                }
                //结束
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
            }
        }

        //public decimal GetLastDayTotalRain(string stationId, DateTime lastDay)
        //{
        //    //decimal result = 0;
        //    //string sql = "select totalrain from " + CT_TableName + " where stationid= " + stationId + " and datatime= '" + lastDay + "';";
        //    //SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
        //    //DataTable dataTableTmp = new DataTable();
        //    //adapter.Fill(dataTableTmp);
        //    //result = decimal.Parse((dataTableTmp.Rows[0])[0].ToString());
        //    decimal result = 0;
        //    return result;
        //}

        //public List<CEntityRain> GetLastClockSharp(string stationId, DateTime TodayTime)
        //{
        //    List<CEntityRain> listRain=new List<CEntityRain>();
        //    //DateTime PreTime = TodayTime.Subtract(new TimeSpan(5,0,0));
        //    //string sql = "select * from " + CT_TableName + " where stationid= " + stationId + " and datatime between  '" + PreTime + " ' and  '",+";";
        //    return listRain;
        //}
        // 会出错吗  1026
        public decimal GetLastDayTotalRain(string stationId, DateTime lastDay)
        {
            decimal result = 0;
            string sql = "select top(1) totalrain from " + CT_TableName + " where stationid= " + stationId + " and datatime< '" + lastDay + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result = decimal.Parse((dataTableTmp.Rows[0])[0].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = 0;
            }

            return result;
        }

        public CEntityRain GetLastDayRain(string stationId, DateTime lastDay)
        {
            CEntityRain result = new CEntityRain();
            result.TimeRecieved = DateTime.Now;
            string sql = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime<= '" + lastDay + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                try
                {
                    result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                    result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {

                }

            }
            if (dataTableTmp.Rows.Count == 0)
            {

            }

            return result;
        }

        public CEntityRain GetLastRain(string stationId, DateTime dt)
        {
            CEntityRain result = new CEntityRain();
            result.TimeRecieved = DateTime.Now;
            string sql = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime< '" + dt + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                try
                {
                    result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                    result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                }
                catch
                {

                }

            }
            if (dataTableTmp.Rows.Count == 0)
            {

            }

            return result;
        }

        public CEntityRain GetLastSharpRain(string stationId, DateTime dt)
        {
            CEntityRain result = new CEntityRain();
            result.TimeRecieved = DateTime.Now;
            DateTime tmp = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            string sql = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime= '" + tmp + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                try
                {
                    result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                    result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {

                }

            }
            else
            {
                string sql2 = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime< '" + tmp + "' order by datatime desc;";
                SqlDataAdapter adapter2 = new SqlDataAdapter(sql2, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp2 = new DataTable();
                adapter2.Fill(dataTableTmp2);
                if (dataTableTmp2.Rows.Count == 1)
                {
                    try
                    {
                        result.TotalRain = decimal.Parse((dataTableTmp2.Rows[0])[CN_TotalRain].ToString());
                        result.TimeCollect = DateTime.Parse(dataTableTmp2.Rows[0][CN_DataTime].ToString());
                    }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                    catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                    {

                    }

                }
            }
            return result;
        }

        public decimal GetLastClockSharpTotalRain(string stationId, DateTime TodayTime)
        {
            decimal result = 0;
            DateTime tmp = TodayTime.Subtract(new TimeSpan(1, 0, 0));
            string sql = "select top(1) * from " + CT_TableName + " where stationid= " + stationId + " and datatime< '" + tmp + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                if (dataTableTmp.Rows[0][CN_TotalRain].ToString() != null)
                {
                    try
                    {
                        result = decimal.Parse(dataTableTmp.Rows[0][CN_TotalRain].ToString());
                    }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                    catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                    {
                        result = 0;
                    }

                }
            }
            else
            {
                result = 0;
            }
            return result;
        }


        #region 帮助方法
        // 根据当前条件查询统计数据
        private void DoCountQuery()
        {
            string sql = "select count(*) count from " + CT_TableName + " " +
                "where " + CN_StationId + " = " + m_strStaionId + " " +
                "and " + TimeSelectString + CN_DataTime + "  between " + DateTimeToDBStr(m_startTime) +
                 "and " + DateTimeToDBStr(m_endTime);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                m_iRowCount = Int32.Parse((dataTableTmp.Rows[0])[0].ToString());
                m_iPageCount = (int)Math.Ceiling((double)m_iRowCount / CDBParams.GetInstance().UIPageRowCount); //向上取整
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        // 将Map中由key指定的DataTable,从startRow开始返回界面最大行数的集合
        private List<CEntityRain> CopyDataToList(int key, int startRow)
        {
            List<CEntityRain> result = new List<CEntityRain>();
            try
            {
                // 取最小值 ，保证不越界
                int endRow = Math.Min(m_mapDataTable[key].Rows.Count, startRow + CDBParams.GetInstance().UIPageRowCount);
                DataTable table = m_mapDataTable[key];
                for (; startRow < endRow; ++startRow)
                {
                    CEntityRain rain = new CEntityRain();
                    //  rain.RainID = long.Parse(table.Rows[startRow][CN_RainID].ToString());
                    rain.StationID = table.Rows[startRow][CN_StationId].ToString();
                    rain.TimeCollect = DateTime.Parse(table.Rows[startRow][CN_DataTime].ToString());
                    if (!table.Rows[startRow][CN_PeriodRain].ToString().Equals(""))
                    {
                        rain.PeriodRain = Decimal.Parse(table.Rows[startRow][CN_PeriodRain].ToString());
                    }
                    if (!table.Rows[startRow][CN_DifferenceRain].ToString().Equals(""))
                    {
                        rain.DifferneceRain = Decimal.Parse(table.Rows[startRow][CN_DifferenceRain].ToString());
                    }
                    if (!table.Rows[startRow][CN_TotalRain].ToString().Equals(""))
                    {
                        rain.TotalRain = Decimal.Parse(table.Rows[startRow][CN_TotalRain].ToString());
                    }
                    rain.TimeRecieved = DateTime.Parse(table.Rows[startRow][CN_RecvDataTime].ToString());
                    rain.ChannelType = CEnumHelper.DBStrToChannelType(table.Rows[startRow][CN_TransType].ToString());
                    rain.MessageType = CEnumHelper.DBStrToMessageType(table.Rows[startRow][CN_MsgType].ToString());
                    // 日雨量，与前一日的差值
                    if (!table.Rows[startRow][CN_DayRain].ToString().Equals(""))
                    {
                        rain.DayRain = Decimal.Parse(table.Rows[startRow][CN_DayRain].ToString());
                    }
                    if (!table.Rows[startRow][CN_DataState].ToString().Equals(""))
                    {
                        // 数据状态
                        rain.BState = int.Parse(table.Rows[startRow][CN_DataState].ToString());
                    }
                    else
                    {
                        rain.BState = 1;
                    }
                    result.Add(rain);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("" + e.ToString());
            }
            return result;
        }

        // 将当前所有数据写入数据库
        protected override bool AddDataToDB()
        {
            // 然后获取内存表的访问权
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

            // 先获取对数据库的唯一访问权
            m_mutexWriteToDB.WaitOne();

            try
            {
                //将临时表中的内容写入数据库
                //SqlConnection conn = CDBManager.GetInstacne().GetConnection();
                //conn.Open();
                string connstr = CDBManager.Instance.GetConnectionString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connstr, SqlBulkCopyOptions.FireTriggers))
                {
                    // 雨量表有插入触发器，如果遇到重复记录，则更新为当前的最新记录
                    bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.DestinationTableName = CSQLRain.CT_TableName;
                    //bulkCopy.ColumnMappings.Add(CN_RainID, CN_RainID);
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_PeriodRain, CN_PeriodRain);
                    bulkCopy.ColumnMappings.Add(CN_DifferenceRain, CN_DifferenceRain);
                    bulkCopy.ColumnMappings.Add(CN_TotalRain, CN_TotalRain);
                    bulkCopy.ColumnMappings.Add(CN_DayRain, CN_DayRain);
                    bulkCopy.ColumnMappings.Add(CN_TransType, CN_TransType);
                    bulkCopy.ColumnMappings.Add(CN_MsgType, CN_MsgType);

                    bulkCopy.ColumnMappings.Add(CN_RecvDataTime, CN_RecvDataTime);
                    bulkCopy.ColumnMappings.Add(CN_DataState, CN_DataState);

                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }

                }
                //conn.Close();   //关闭连接
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                m_mutexWriteToDB.ReleaseMutex();
                return false;
            }
            Debug.WriteLine("###{0} :add {1} lines to rain db", DateTime.Now, tmp.Rows.Count);
            CDBLog.Instance.AddInfo(string.Format("添加{0}行到雨量表", tmp.Rows.Count));
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }


        protected void InsertSqlBulk(DataTable dt)
        {
            // 然后获取内存表的访问权
            m_mutexDataTable.WaitOne();

            if (dt.Rows.Count <= 0)
            {
                m_mutexDataTable.ReleaseMutex();
                return;
            }
            //清空内存表的所有内容，把内容复制到临时表tmp中
            DataTable tmp = dt.Copy();
            m_tableDataAdded.Rows.Clear();

            // 释放内存表的互斥量
            m_mutexDataTable.ReleaseMutex();

            // 先获取对数据库的唯一访问权
            // m_mutexWriteToDB.WaitOne();

            try
            {
                //将临时表中的内容写入数据库
                //SqlConnection conn = CDBManager.GetInstacne().GetConnection();
                //conn.Open();
                string connstr = CDBManager.Instance.GetConnectionString();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connstr, SqlBulkCopyOptions.FireTriggers))
                {
                    //1126
                    // 雨量表有插入触发器，如果遇到重复记录，则更新为当前的最新记录
                    //bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.DestinationTableName = CSQLRain.CT_TableName;
                    //bulkCopy.ColumnMappings.Add(CN_RainID, CN_RainID);
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_DataTime, CN_DataTime);
                    bulkCopy.ColumnMappings.Add(CN_PeriodRain, CN_PeriodRain);
                    bulkCopy.ColumnMappings.Add(CN_DifferenceRain, CN_DifferenceRain);
                    bulkCopy.ColumnMappings.Add(CN_TotalRain, CN_TotalRain);
                    bulkCopy.ColumnMappings.Add(CN_DayRain, CN_DayRain);
                    bulkCopy.ColumnMappings.Add(CN_TransType, CN_TransType);
                    bulkCopy.ColumnMappings.Add(CN_MsgType, CN_MsgType);

                    bulkCopy.ColumnMappings.Add(CN_RecvDataTime, CN_RecvDataTime);
                    bulkCopy.ColumnMappings.Add(CN_DataState, CN_DataState);

                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                        Debug.WriteLine("###{0} :add {1} lines to rain db", DateTime.Now, tmp.Rows.Count);
                        CDBLog.Instance.AddInfo(string.Format("添加{0}行到雨量表", tmp.Rows.Count));
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        //如果出现异常，SqlBulkCopy 会使数据库回滚，所有Table中的记录都不会插入到数据库中，
                        //此时，把Table折半插入，先插入一半，再插入一半。如此递归，直到只有一行时，如果插入异常，则返回。
                        if (tmp.Rows.Count == 1)
                            return;
                        int middle = tmp.Rows.Count / 2;
                        DataTable table = tmp.Clone();
                        for (int i = 0; i < middle; i++)
                            table.ImportRow(tmp.Rows[i]);

                        InsertSqlBulk(table);

                        table.Clear();
                        for (int i = middle; i < tmp.Rows.Count; i++)
                            table.ImportRow(tmp.Rows[i]);
                        InsertSqlBulk(table);
                    }
                }
                //conn.Close();   //关闭连接
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //   m_mutexWriteToDB.ReleaseMutex();
                return;
            }

            //  m_mutexWriteToDB.ReleaseMutex();
            return;
        }


        // 恢复初始状态
        private void ResetAll()
        {
            m_mutexDataTable.WaitOne();
            m_iPageCount = -1;
            m_mapDataTable.Clear(); //清空所有记录
            m_mutexDataTable.ReleaseMutex();
        }

        #endregion ///<HELP_METHOD

        //1009gm
        public List<CEntityRain> QueryAccTime(DateTime time)
        {
            DateTime startTime = time;
            DateTime endTime1 = time.AddMinutes(59).AddSeconds(59);
            DateTime endTime2 = time.AddHours(24);
            List<CEntityRain> results = new List<CEntityRain>();
            //string sqlAcc = "select * from HydrologytestDB.dbo.Rain where Datatime between '2010-01-05 01:00:00.000' and '2010-01-05 01:59:59.000'; ";
            //string sql = "select * from " + CT_TableName + " where Datetime between startTime and endTime;";
            string sql = "select * from " + CT_TableName + " where DataTime between '2010-01-05 16:40:56' and '2010-01-05 17:40:56';";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            int i = dataTableTemp.Columns.Count;
            foreach (DataColumn dc in dataTableTemp.Columns)
            {
                string temp = dc.ColumnName;
            }

            for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
            {
                CEntityRain rain = new CEntityRain();
                rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                if (dataTableTemp.Rows[rowid][CN_PeriodRain].ToString() != "")
                {
                    rain.PeriodRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_PeriodRain].ToString());
                }
                else
                {
                    rain.PeriodRain = -9999;
                }
                if (dataTableTemp.Rows[rowid][CN_TotalRain].ToString() != "")
                {
                    rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                }
                else
                {
                    rain.TotalRain = -9999;
                }

                results.Add(rain);
            }
            return results;
        }
        //gm
        public List<CEntityRain> QueryAccTimeAndStation(string StationId, DateTime time)
        {
            string sql = "select * from " + CT_TableName + " where Datatime in('";
            for (int j = 0; j < 23; j++)
            {
                sql = sql + time + "','";
                time = time.AddHours(1);
            }
            sql = sql + time + "') and StationID=" + StationId + ";";
            //DateTime startTime = time;
            //DateTime endTime1 = time.AddMinutes(59).AddSeconds(59);
            List<CEntityRain> results = new List<CEntityRain>();
            //string sql = "select * from HydrologytestDB.dbo.Rain where DataTime= '" + startTime + "' and StationID="  + StationId + ";";
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
                    CEntityRain rain = new CEntityRain();
                    rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                    rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                    if (dataTableTemp.Rows[rowid][CN_PeriodRain].ToString() != "")
                    {
                        rain.PeriodRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_PeriodRain].ToString());
                    }
                    else
                    {
                        rain.PeriodRain = -9999;
                    }
                    if (dataTableTemp.Rows[rowid][CN_TotalRain].ToString() != "")
                    {
                        rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                    }
                    else
                    {
                        rain.TotalRain = -9999;
                    }
                    results.Add(rain);
                }
            }
            return results;

        }

        public List<CEntityRain> QueryForMonthTable(string StationId, DateTime time)
        {
            string sql = "select * from " + CT_TableName + " where Datatime in('";
            for (int j = 0; j < 23; j++)
            {
                sql = sql + time + "','";
                time = time.AddHours(1);
            }
            sql = sql + time + "') and StationID=" + StationId + ";";
            //DateTime startTime = time;
            //DateTime endTime1 = time.AddMinutes(59).AddSeconds(59);
            List<CEntityRain> results = new List<CEntityRain>();
            //string sql = "select * from HydrologytestDB.dbo.Rain where DataTime= '" + startTime + "' and StationID="  + StationId + ";";
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
                    CEntityRain rain = new CEntityRain();
                    rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                    rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                    if (dataTableTemp.Rows[rowid][CN_PeriodRain].ToString() != "")
                    {
                        rain.PeriodRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_PeriodRain].ToString());
                    }
                    else
                    {
                        rain.PeriodRain = -9999;
                    }

                    if (dataTableTemp.Rows[rowid][CN_TotalRain].ToString() != "")
                    {
                        rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                    }
                    else
                    {
                        rain.TotalRain = -9999;
                    }

                    results.Add(rain);
                }
            }
            return results;
        }

        public List<CEntityRain> QueryForYearTable(string StationId, DateTime time)
        {
            List<CEntityRain> results = new List<CEntityRain>();
            DateTime start = time;
            DateTime end = time.AddMonths(1);
            string sql = "select * from " + CT_TableName + " where Datatime between '" + start + "' and '" + end + "' and messagetype = 8 and stationid = " + StationId + ";";
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
                    CEntityRain rain = new CEntityRain();
                    rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                    rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                    if (dataTableTemp.Rows[rowid][CN_PeriodRain].ToString() != "")
                    {
                        rain.PeriodRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_PeriodRain].ToString());
                    }
                    else
                    {
                        rain.PeriodRain = -9999;
                    }
                    if (dataTableTemp.Rows[rowid][CN_TotalRain].ToString() != "")
                    {
                        rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                    }
                    else
                    {
                        rain.TotalRain = -9999;
                    }
                    results.Add(rain);
                }
            }

            return results;
        }

        public bool checkRainIsExists(string stationid, DateTime dt)
        {
            bool flag = false;
            string sql = "select count(totalrain) from " + CT_TableName + "where stationid= " + stationid + "and datatime= '" + dt + "';";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
            {
                int count = int.Parse(dataTableTemp.Rows[rowid][0].ToString());
                if (count > 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool UpdateRows_1(List<Hydrology.Entity.CEntityRain> rains, int index)
        {
            // 除主键外和站点外，其余信息随意修改
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = index; i < rains.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6},{7}={8}  where {9}={10} and {11}='{12}';",
                    CT_TableName,
                    CN_PeriodRain, rains[i].PeriodRain.HasValue ? rains[i].PeriodRain.Value.ToString() : "null",
                    CN_DifferenceRain, rains[i].DifferneceRain.HasValue ? rains[i].DifferneceRain.Value.ToString() : "null",
                    CN_DayRain, rains[i].DayRain.HasValue ? rains[i].DayRain.Value.ToString() : "null",
                    CN_DataState, rains[i].BState,
                    CN_StationId, rains[i].StationID,
                    CN_DataTime, rains[i].TimeCollect.ToString()
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
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql.ToString()))
            {
                return false;
            }
            ResetAll();
            return true;
        }

        public bool createTable(string name)
        {
            StringBuilder sqlcreate = new StringBuilder();
            StringBuilder sqlAlter = new StringBuilder();
            StringBuilder sqlcreate2 = new StringBuilder();
            StringBuilder sqlAlter2 = new StringBuilder();
            for (int i = 1; i <= 12; i++)
            {
                string tableName = name + i.ToString() + "A";
                sqlcreate.Clear();
                sqlAlter.Clear();
                sqlcreate.AppendFormat("if not exists (select * from sysobjects where id = object_id('{0}') and OBJECTPROPERTY(id, 'IsUserTable') = 1) CREATE TABLE [{1}] ([stationid][char](4) NOT NULL,[datatime][datetime] NOT NULL,[periodrain][numeric](18, 1) NULL,[differencerain][numeric](18, 1) NULL,[totalrain][numeric](18, 1) NULL,[dayrain][numeric](18, 1) NULL,[transtype][char](2) NULL,[messagetype][char](1) NULL,[recvdatatime][datetime] NULL,[state][int] NULL,CONSTRAINT [{2}] PRIMARY KEY CLUSTERED ( [stationid] ASC, [datatime] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]; ",
                      tableName,
                      tableName,
                      "PK_" + tableName
                    );
                if (this.ExecuteSQLCommand(sqlcreate.ToString()))
                {
                    sqlAlter.AppendFormat("ALTER TABLE [{0}]  WITH NOCHECK ADD  CONSTRAINT [{1}] FOREIGN KEY([stationid]) REFERENCES [dbo].[hydlstation]([StationID]) ALTER TABLE [{2}] CHECK CONSTRAINT[{3}];",
                        tableName,
                        "FK_" + tableName + "_hydlstation",
                        tableName,
                        "FK_" + tableName + "_hydlstation"
                        );
                    this.ExecuteSQLCommand(sqlAlter.ToString());
                }

                string tableName2 = name + i.ToString() + "B";
                sqlAlter2.Clear();
                sqlcreate2.Clear();
                sqlcreate2.AppendFormat("if not exists (select * from sysobjects where id = object_id('{0}') and OBJECTPROPERTY(id, 'IsUserTable') = 1) CREATE TABLE [{1}] ([stationid][char](4) NOT NULL,[datatime][datetime] NOT NULL,[periodrain][numeric](18, 1) NULL,[differencerain][numeric](18, 1) NULL,[totalrain][numeric](18, 1) NULL,[dayrain][numeric](18, 1) NULL,[transtype][char](2) NULL,[messagetype][char](1) NULL,[recvdatatime][datetime] NULL,[state][int] NULL,CONSTRAINT [{2}] PRIMARY KEY CLUSTERED ( [stationid] ASC, [datatime] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]; ",
                      tableName2,
                      tableName2,
                      "PK_" + tableName2
                    );
                if (this.ExecuteSQLCommand(sqlcreate2.ToString()))
                {
                    sqlAlter2.AppendFormat("ALTER TABLE [{0}]  WITH NOCHECK ADD  CONSTRAINT [{1}] FOREIGN KEY([stationid]) REFERENCES [dbo].[hydlstation]([StationID]) ALTER TABLE [{2}] CHECK CONSTRAINT[{3}];",
                        tableName2,
                        "FK_" + tableName2 + "_hydlstation",
                        tableName2,
                        "FK_" + tableName2 + "_hydlstation"
                        );
                    this.ExecuteSQLCommand(sqlAlter2.ToString());
                }

            }
            return true;
        }

        public void AddOrUpdate(List<Hydrology.Entity.CEntityRain> rains)
        {
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityRain rain in rains)
            {
                try
                {
                    DataRow row = m_tableDataAdded.NewRow();
                    row[CN_StationId] = rain.StationID;
                    row[CN_DataTime] = rain.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                    row[CN_PeriodRain] = rain.PeriodRain;
                    row[CN_DifferenceRain] = rain.DifferneceRain;
                    row[CN_TotalRain] = rain.TotalRain;
                    row[CN_DayRain] = rain.DayRain;
                    row[CN_TransType] = CEnumHelper.ChannelTypeToDBStr(rain.ChannelType);
                    row[CN_MsgType] = CEnumHelper.MessageTypeToDBStr(rain.MessageType);
                    row[CN_RecvDataTime] = rain.TimeRecieved.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                    row[CN_DataState] = rain.BState;
                    m_tableDataAdded.Rows.Add(row);
                    // 判断是否需要创建新分区
                    //CSQLPartitionMgr.Instance.MaintainRain(rain.TimeCollect);

                    NewTask(() => { AddDataToDB(); });

                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
                catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
                {
                    StringBuilder sql = new StringBuilder();
                    sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6},{7}={8},{9}={10},{11}={12},{13}='{14}',{15} ='{16}' where {17}={18} and {19}='{20}';",
                            CT_TableName,
                            //  CN_DataTime, DateTimeToDBStr(rains[i].TimeCollect),
                            CN_PeriodRain, rain.PeriodRain.HasValue ? rain.PeriodRain.Value.ToString() : "null",
                            CN_TotalRain, rain.TotalRain.HasValue ? rain.TotalRain.Value.ToString() : "null",
                            CN_DifferenceRain, rain.DifferneceRain.HasValue ? rain.DifferneceRain.Value.ToString() : "null",
                            CN_DayRain, rain.DayRain.HasValue ? rain.DayRain.Value.ToString() : "null",
                            CN_TransType, CEnumHelper.ChannelTypeToDBStr(rain.ChannelType),
                            CN_MsgType, CEnumHelper.MessageTypeToDBStr(rain.MessageType),
                            CN_RecvDataTime, rain.TimeRecieved.ToString(),
                            CN_DataState, rain.BState,
                            //CN_RainID, rains[i].RainID
                            CN_StationId, rain.StationID,
                            CN_DataTime, rain.TimeCollect.ToString()
                        );
                    if (!this.ExecuteSQLCommand(sql.ToString()))
                    {
                        // 保存失败
                        return;
                    }
                    sql.Clear(); //清除以前的所有命令
                }

                ResetAll();
            }

            m_mutexDataTable.ReleaseMutex();
        }

        public List<CEntityRain> getListRainForUpdate(string stationid, DateTime start, DateTime end)
        {
            List<CEntityRain> results = new List<CEntityRain>();
            string sql = "select * from " + CT_TableName + " where Datatime between '" + start + "' and '" + end + "' and  stationid = " + stationid + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            int flag = dataTableTemp.Rows.Count;
            for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
            {
                CEntityRain rain = new CEntityRain();
                rain.StationID = dataTableTemp.Rows[rowid][CN_StationId].ToString();
                rain.TimeCollect = DateTime.Parse(dataTableTemp.Rows[rowid][CN_DataTime].ToString());
                rain.TotalRain = decimal.Parse(dataTableTemp.Rows[rowid][CN_TotalRain].ToString());
                results.Add(rain);
            }
            return results;
        }

        public List<string> getUpdateStations(DateTime start, DateTime end)
        {
            List<string> results = new List<string>();
            string sql = "select distinct stationid  from " + CT_TableName + " where Datatime between '" + start + "' and '" + end + "' and  state = 2 " + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
            {
                string result = dataTableTemp.Rows[rowid][0].ToString();
                results.Add(result);
            }
            return results;
        }

        public List<DateTime> getExistsTime(string stationid, DateTime startTime, DateTime endTime)
        {
            List<DateTime> results = new List<DateTime>();
            string sql = "select datatime from " + CT_TableName + " where stationid= " + stationid + " and datatime>= '" + startTime + "' and datatime<=  '" + endTime + "' order by datatime;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTemp = new DataTable();
            adapter.Fill(dataTableTemp);
            for (int rowid = 0; rowid < dataTableTemp.Rows.Count; ++rowid)
            {
                DateTime dt = DateTime.Parse(dataTableTemp.Rows[rowid][0].ToString());
                results.Add(dt);
            }
            return results;
        }

        public void AddNewRows_DataModify(List<Hydrology.Entity.CEntityRain> rains)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            foreach (CEntityRain rain in rains)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_StationId] = rain.StationID;
                row[CN_DataTime] = rain.TimeCollect.ToString(CDBParams.GetInstance().DBDateTimeFormat);
                row[CN_PeriodRain] = rain.PeriodRain;
                row[CN_DifferenceRain] = rain.DifferneceRain;
                row[CN_TotalRain] = rain.TotalRain;
                row[CN_DayRain] = rain.DayRain;

                row[CN_TransType] = CEnumHelper.ChannelTypeToDBStr(rain.ChannelType);
                row[CN_MsgType] = CEnumHelper.MessageTypeToDBStr(rain.MessageType);

                row[CN_RecvDataTime] = rain.TimeRecieved.ToString(CDBParams.GetInstance().DBDateTimeFormat);

                row[CN_DataState] = rain.BState;
                m_tableDataAdded.Rows.Add(row);

                // 判断是否需要创建新分区
                // CSQLPartitionMgr.Instance.MaintainRain(rain.TimeCollect);
            }

            // 如果超过最大值，写入数据库
            NewTask(() => { AddDataToDB(); });

            // AddDataToDB(); 
            m_mutexDataTable.ReleaseMutex();
        }
        public bool UpdateRows_DataModify(List<CEntityRain> rains)
        {
            // 除主键外和站点外，其余信息随意修改
            StringBuilder sql = new StringBuilder();
            //  int currentBatchCount = 0;
            for (int i = 0; i < rains.Count; i++)
            {
                //   ++currentBatchCount;
                //第一个修改，下一条最近的数据
                sql.AppendFormat("update {0} set {1}={2},{3}={4},{5}={6},{7}={8},{9}={10},{11}={12},{13}='{14}',{15} ='{16}' where {17}={18} and {19}='{20}';",
                    CT_TableName,
                    //  CN_DataTime, DateTimeToDBStr(rains[i].TimeCollect),
                    CN_PeriodRain, rains[i].PeriodRain.HasValue ? rains[i].PeriodRain.Value.ToString() : "null",
                    CN_TotalRain, rains[i].TotalRain.HasValue ? rains[i].TotalRain.Value.ToString() : "null",
                    CN_DifferenceRain, rains[i].DifferneceRain.HasValue ? rains[i].DifferneceRain.Value.ToString() : "null",
                    CN_DayRain, rains[i].DayRain.HasValue ? rains[i].DayRain.Value.ToString() : "null",
                    CN_TransType, CEnumHelper.ChannelTypeToDBStr(rains[i].ChannelType),
                    CN_MsgType, CEnumHelper.MessageTypeToDBStr(rains[i].MessageType),
                    CN_RecvDataTime, rains[i].TimeRecieved.ToString(),
                    CN_DataState, rains[i].BState,
                    //CN_RainID, rains[i].RainID
                    CN_StationId, rains[i].StationID,
                    CN_DataTime, rains[i].TimeCollect.ToString()
                );

            }
            if (!this.ExecuteSQLCommand(sql.ToString()))
            {
                return false;
            }
            ResetAll();

            for (int i = 0; i < rains.Count; i++)
            {
                if (rains[i].TimeCollect.Hour == 8 && rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改当前8点整点的日雨量
                    CEntityRain result1 = GetLastDayRain_1(rains[i]);
                    if (result1 != null)
                    {
                        rains[i].DayRain = rains[i].TotalRain - result1.TotalRain;
                        UpdateThisDayRain(rains[i]);
                    }
                }
                if (rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改本条整点的时段雨量
                    CEntityRain result2 = GetLastSharpRain_1(rains[i]);
                    if (result2 != null)
                    {
                        rains[i].PeriodRain = rains[i].TotalRain - result2.TotalRain;
                        UpdateThisSharpRain(rains[i]);
                    }
                }

                //修改本条数据的差值雨量
                CEntityRain result3 = GetLastRain_1(rains[i]);
                if (result3 != null)
                {
                    rains[i].DifferneceRain = rains[i].TotalRain - result3.TotalRain;
                    UpdateThisRain(rains[i]);
                }
            }
            return true;
        }
        public bool UpdateOtherRows_DataModify(List<CEntityRain> rains)
        {
            for (int i = 0; i < rains.Count; i++)
            {
                if (rains[i].TimeCollect.Hour == 8 && rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改下一条8点的日雨量
                    CEntityRain result1 = GetNextDayRain(rains[i]);
                    if (result1 != null)
                    {
                        result1.DayRain = result1.TotalRain - rains[i].TotalRain;
                        UpdateNextDayRain(result1);
                    }
                }
                if (rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改最近一条整点的时段雨量
                    CEntityRain result2 = GetNextSharpRain(rains[i]);
                    if (result2 != null)
                    {
                        result2.PeriodRain = result2.TotalRain - rains[i].TotalRain;
                        UpdateNextSharpRain(result2);
                    }
                }

                //修改最近一条数据的差值雨量
                CEntityRain result3 = GetNextRain(rains[i]);
                if (result3 != null)
                {
                    result3.DifferneceRain = result3.TotalRain - rains[i].TotalRain;
                    UpdateNextRain(result3);
                }

            }
            return true;
        }

        public bool UpdateOtherRows_DataModify_1(List<CEntityRain> rains)
        {
            for (int i = 0; i < rains.Count; i++)
            {
                if (rains[i].TimeCollect.Hour == 8 && rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改下一条8点的日雨量
                    CEntityRain result1 = GetNextDayRain(rains[i]);
                    CEntityRain result2 = GetLastRain_1(rains[i]);
                    if (result1 != null)
                    {
                        result1.DayRain = result1.TotalRain - result2.TotalRain;
                        UpdateNextDayRain(result1);
                    }
                }
                if (rains[i].TimeCollect.Minute + rains[i].TimeCollect.Second == 0)
                {
                    //修改最近一条整点的时段雨量
                    CEntityRain result2 = GetNextSharpRain(rains[i]);
                    CEntityRain result4 = GetLastRain_1(rains[i]);
                    if (result2 != null)
                    {
                        result2.PeriodRain = result2.TotalRain - result4.TotalRain;
                        UpdateNextSharpRain(result2);
                    }
                }

                //修改最近一条数据的差值雨量
                CEntityRain result3 = GetNextRain(rains[i]);
                CEntityRain result5 = GetLastRain_1(rains[i]);
                if (result3 != null)
                {
                    result3.DifferneceRain = result3.TotalRain - result5.TotalRain;
                    UpdateNextRain(result3);
                }

            }
            return true;
        }

        public CEntityRain GetNextRain(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + rain.StationID + " and datatime> '" + rain.TimeCollect + "' order by datatime asc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }

            return result;
        }
        public CEntityRain GetNextSharpRain(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();
            // string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where periodrain is not null and stationid= " + rain.StationID + " and datatime> '" + rain.TimeCollect + "' order by datatime asc;";
            string sql = "select stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect.AddHours(1) + "' order by datatime asc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                //string sql1 = "select stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect.AddHours(1) + "' order by datatime asc;";
                //SqlDataAdapter adapter1 = new SqlDataAdapter(sql1, CDBManager.GetInstacne().GetConnection());
                //DataTable dataTableTmp1 = new DataTable();
                //adapter.Fill(dataTableTmp);
                //if (dataTableTmp1.Rows.Count == 1)
                //{
                //    result.StationID = dataTableTmp1.Rows[0][CN_StationId].ToString();
                //    result.TotalRain = decimal.Parse((dataTableTmp1.Rows[0])[CN_TotalRain].ToString());
                //    result.TimeCollect = DateTime.Parse(dataTableTmp1.Rows[0][CN_DataTime].ToString());
                //}
                //else
                //{
                result = null;
                //}
            }
            return result;
        }
        public CEntityRain GetNextDayRain(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();
            // string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where dayrain is not null and stationid= " + rain.StationID + " and datatime> '" + rain.TimeCollect + "' order by datatime asc;";
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect.AddDays(1) + "' order by datatime asc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }
            return result;
        }

        public CEntityRain GetLastDayRain_1(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();
            // string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + "  where dayrain is not null and stationid= " + rain.StationID + " and datatime<'" + rain.TimeCollect + "' order by datatime desc";
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + "  where  stationid= " + rain.StationID + " and datatime<='" + rain.TimeCollect.Subtract(new TimeSpan(24, 0, 0)) + "' order by datatime desc";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }
            return result;
        }
        public CEntityRain GetLastSharpRain_1(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();

            // string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where periodrain is not null and stationid= " + rain.StationID + " and datatime<= '" + rain.TimeCollect + "' order by datatime desc;";
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where  stationid= " + rain.StationID + " and datatime<= '" + rain.TimeCollect.Subtract(new TimeSpan(1, 0, 0)) + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }
            return result;


            //CEntityRain result = new CEntityRain();
            //DateTime tmp = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
            //string sql = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime= '" + tmp + "' order by datatime desc;";
            //SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            //DataTable dataTableTmp = new DataTable();
            //adapter.Fill(dataTableTmp);
            //if (dataTableTmp.Rows.Count == 1)
            //{
            //    result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
            //    result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            //}
            //else
            //{
            //    string sql2 = "select top(1) totalrain,datatime from " + CT_TableName + " where stationid= " + stationId + " and datatime< '" + tmp + "' order by datatime desc;";
            //    SqlDataAdapter adapter2 = new SqlDataAdapter(sql2, CDBManager.GetInstacne().GetConnection());
            //    DataTable dataTableTmp2 = new DataTable();
            //    adapter2.Fill(dataTableTmp2);
            //    if (dataTableTmp2.Rows.Count == 1)
            //    {
            //        result.TotalRain = decimal.Parse((dataTableTmp2.Rows[0])[CN_TotalRain].ToString());
            //        result.TimeCollect = DateTime.Parse(dataTableTmp2.Rows[0][CN_DataTime].ToString());
            //    }
            //}
            //return result;
        }
        public CEntityRain GetLastRain_1(CEntityRain rain)
        {
            CEntityRain result = new CEntityRain();
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + rain.StationID + " and datatime< '" + rain.TimeCollect + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }

            return result;
        }

        public bool UpdateThisRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set differencerain= " + rain.DifferneceRain + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect + " '; ";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }
        public bool UpdateThisSharpRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set periodrain= " + rain.PeriodRain + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect + " ' ;";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }
        public bool UpdateThisDayRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set dayrain= " + rain.DayRain + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect + " ' ;";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }

        public bool UpdateNextRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set differencerain= " + rain.DifferneceRain + " where stationid= " + rain.StationID + " and datatime= ' " + rain.TimeCollect + " ' ;";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }
        public bool UpdateNextSharpRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set periodrain= " + rain.PeriodRain + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect + " ' ;";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }
        public bool UpdateNextDayRain(CEntityRain rain)
        {
            string sql = "update  " + CT_TableName + " set dayrain= " + rain.DayRain + " where stationid= " + rain.StationID + " and datatime= '" + rain.TimeCollect + " '; ";
            // 更新数据库
            if (!this.ExecuteSQLCommand(sql))
            {
                return false;
            }
            ResetAll();
            return true;
        }


        public CEntityRain getRainsForInit(string stationid, DateTime dt)
        {
            CEntityRain result = new CEntityRain();
            string sql = "select top(1) stationid,totalrain,datatime from " + CT_TableName + " where stationid= " + stationid + " and datatime<= '" + dt + "' order by datatime desc;";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                result.StationID = dataTableTmp.Rows[0][CN_StationId].ToString();
                result.TotalRain = decimal.Parse((dataTableTmp.Rows[0])[CN_TotalRain].ToString());
                result.TimeCollect = DateTime.Parse(dataTableTmp.Rows[0][CN_DataTime].ToString());
            }
            if (dataTableTmp.Rows.Count == 0)
            {
                result = null;
            }
            return result;
        }
    }
}
