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
    public class CSQLStation : CSQLBase, IStationProxy
    {
        #region 静态常量
        private const string CT_EntityName = "CEntityStation"; //数据库表Station实体类   
        public static readonly string CT_TableName = "hydlstation"; //数据库测站表的名字
        public static readonly string CN_StationID = "StationID"; //测站的唯一ID
        public static readonly string CN_SubCenterID = "SubCenterID"; //测站所属分中心的唯一ID
        public static readonly string CN_StationName = "CName"; //站点名字
        public static readonly string CN_StationType = "CType";    //站点类型
        public static readonly string CN_WaterBase = "WBase";    //水位基值
        public static readonly string CN_WaterMax = "WMax";  //水位最大值
        public static readonly string CN_WaterMin = "WMin";     //水位最小值
        public static readonly string CN_WaterChange = "WChange";  //允许的水位变化量
        public static readonly string CN_RainAccuracy = "RAccuracy";  //雨量精度
        public static readonly string CN_RainChange = "RChange";       //允许的雨量变化值
        public static readonly string CN_GSM = "Gsm";       //GSM号码
        public static readonly string CN_GPRS = "Gprs";       //Gprs号码
        public static readonly string CN_BDSatellite = "BDSatellite";       //北斗卫星号码
        public static readonly string CN_BDMember = "BDmember";       //北斗卫星号码
        public static readonly string CN_VoltageMin = "VoltageMin";       //电压的最小值，用于报警
        public static readonly string CN_Maintran = "maintran";
        public static readonly string CN_Subtran = "subtran";
        public static readonly string CN_Dataprotocol = "dataprotocol";
        public static readonly string CN_Watersensor = "watersensor";
        public static readonly string CN_Rainsensor = "rainsensor";
        public static readonly string CN_Reportinterval = "reportinterval";
        #endregion

        private const int CN_FIELD_COUNT = 21;

        // 为之前简单测试使用
        private List<string> m_listDelRows;            // 删除站点记录的链表
        private List<CEntityStation> m_listUpdateRows; // 更新雨量录的链表


        #region 公有方法

        public CSQLStation()
            : base()
        {
            m_listDelRows = new List<string>();
            m_listUpdateRows = new List<CEntityStation>();
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_StationID);
            m_tableDataAdded.Columns.Add(CN_SubCenterID);
            m_tableDataAdded.Columns.Add(CN_StationName);
            m_tableDataAdded.Columns.Add(CN_StationType);
            m_tableDataAdded.Columns.Add(CN_WaterBase);
            m_tableDataAdded.Columns.Add(CN_WaterMax);
            m_tableDataAdded.Columns.Add(CN_WaterMin);
            m_tableDataAdded.Columns.Add(CN_WaterChange);
            m_tableDataAdded.Columns.Add(CN_RainAccuracy);
            m_tableDataAdded.Columns.Add(CN_RainChange);
            m_tableDataAdded.Columns.Add(CN_GSM);
            m_tableDataAdded.Columns.Add(CN_GPRS);
            m_tableDataAdded.Columns.Add(CN_BDSatellite);
            m_tableDataAdded.Columns.Add(CN_BDMember);
            m_tableDataAdded.Columns.Add(CN_VoltageMin);
            m_tableDataAdded.Columns.Add(CN_Maintran);
            m_tableDataAdded.Columns.Add(CN_Subtran);
            m_tableDataAdded.Columns.Add(CN_Dataprotocol);
            m_tableDataAdded.Columns.Add(CN_Watersensor);
            m_tableDataAdded.Columns.Add(CN_Rainsensor);
            m_tableDataAdded.Columns.Add(CN_Reportinterval);
            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_Station;
        }

        // 添加新列
        public void AddNewRow(CEntityStation entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_StationID] = entity.StationID;
            row[CN_SubCenterID] = entity.SubCenterID;
            row[CN_StationName] = entity.StationName;
            row[CN_StationType] = (Int32)entity.StationType;
            row[CN_WaterBase] = entity.DWaterBase;
            row[CN_WaterMax] = entity.DWaterMax;
            row[CN_WaterMin] = entity.DWaterMin;
            row[CN_WaterChange] = entity.DWaterChange;
            row[CN_RainAccuracy] = entity.DRainAccuracy;
            row[CN_RainChange] = entity.DRainChange;
            row[CN_GSM] = entity.GSM;
            row[CN_GPRS] = entity.GPRS;
            row[CN_BDSatellite] = entity.BDSatellite;
            row[CN_BDMember] = entity.BDMemberSatellite;
            row[CN_VoltageMin] = entity.DVoltageMin;
            row[CN_Maintran] = entity.Maintran;
            row[CN_Subtran] = entity.Subtran;
            row[CN_Dataprotocol] = entity.Datapotocol;
            row[CN_Watersensor] = entity.Watersensor;
            row[CN_Rainsensor] = entity.Rainsensor;
            row[CN_Reportinterval] = entity.Reportinterval;

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

        public bool AddRange(List<CEntityStation> items)
        {
            if (items.Count <= 0)
            {
                return true;
            }
            // 实时写入数据库
            m_mutexDataTable.WaitOne();
            foreach (CEntityStation entity in items)
            {
                DataRow row = m_tableDataAdded.NewRow();
                row[CN_StationID] = entity.StationID;
                row[CN_SubCenterID] = entity.SubCenterID;
                row[CN_StationName] = entity.StationName;
                row[CN_StationType] = (Int32)entity.StationType;
                row[CN_WaterBase] = entity.DWaterBase;
                row[CN_WaterMax] = entity.DWaterMax;
                row[CN_WaterMin] = entity.DWaterMin;
                row[CN_WaterChange] = entity.DWaterChange;
                row[CN_RainAccuracy] = entity.DRainAccuracy;
                row[CN_RainChange] = entity.DRainChange;
                row[CN_GSM] = entity.GSM;
                row[CN_GPRS] = entity.GPRS;
                row[CN_BDSatellite] = entity.BDSatellite;
                row[CN_BDMember] = entity.BDMemberSatellite;
                row[CN_VoltageMin] = entity.DVoltageMin;
                row[CN_Maintran] = entity.Maintran;
                row[CN_Subtran] = entity.Subtran;
                row[CN_Dataprotocol] = entity.Datapotocol;
                row[CN_Watersensor] = entity.Watersensor;
                row[CN_Rainsensor] = entity.Rainsensor;
                row[CN_Reportinterval] = entity.Reportinterval;
                m_tableDataAdded.Rows.Add(row);
            }
            m_mutexDataTable.ReleaseMutex();
            return AddDataToDB();
        }

        /// <summary>
        /// 更新站点信息
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool UpdateRange(List<CEntityStation> items)
        {
            if (items.Count <= 0)
            {
                return true;
            }
            string sqlcommand = "";
            foreach (CEntityStation station in items)
            {
                sqlcommand += GetUpdateSQL(station);
            }
            return base.ExecuteSQLCommand(sqlcommand);
        }


        public bool DeleteRange(List<string> items)
        {
            if (items.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            foreach (string stationId in items)
            {
                sql.AppendFormat("delete from {0} where {1}='{2}';",
                    CT_TableName,
                    CN_StationID, stationId);
            }
            return ExecuteSQLCommand(sql.ToString());
        }

        public List<CEntityStation> QueryAll()
        {
            string sql = " select * from " + CT_TableName;
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            // 构建结果集
            List<CEntityStation> results = new List<CEntityStation>();
            //dataTableTmp.Rows.Count
            for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
            {
                CEntityStation station = new CEntityStation();
                station.StationID = dataTableTmp.Rows[rowid][CN_StationID].ToString();
                station.SubCenterID = int.Parse(dataTableTmp.Rows[rowid][CN_SubCenterID].ToString());
                station.StationName = dataTableTmp.Rows[rowid][CN_StationName].ToString();
                station.StationType = CEnumHelper.DBStrToStationType(dataTableTmp.Rows[rowid][CN_StationType].ToString());
                if (!dataTableTmp.Rows[rowid][CN_WaterBase].ToString().Equals(""))
                {
                    station.DWaterBase = Decimal.Parse(dataTableTmp.Rows[rowid][CN_WaterBase].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_WaterMax].ToString().Equals(""))
                {
                    station.DWaterMax = Decimal.Parse(dataTableTmp.Rows[rowid][CN_WaterMax].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_WaterMin].ToString().Equals(""))
                {
                    station.DWaterMin = Decimal.Parse(dataTableTmp.Rows[rowid][CN_WaterMin].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_WaterChange].ToString().Equals(""))
                {
                    station.DWaterChange = Decimal.Parse(dataTableTmp.Rows[rowid][CN_WaterChange].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_RainAccuracy].ToString().Equals(""))
                {
                    station.DRainAccuracy = float.Parse(dataTableTmp.Rows[rowid][CN_RainAccuracy].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_RainChange].ToString().Equals(""))
                {
                    station.DRainChange = Decimal.Parse(dataTableTmp.Rows[rowid][CN_RainChange].ToString());
                }
                if (!dataTableTmp.Rows[rowid][CN_VoltageMin].ToString().Equals(""))
                {
                    station.DVoltageMin = float.Parse(dataTableTmp.Rows[rowid][CN_VoltageMin].ToString());
                }

                station.GSM = dataTableTmp.Rows[rowid][CN_GSM].ToString();
                station.GPRS = dataTableTmp.Rows[rowid][CN_GPRS].ToString();
                station.BDMemberSatellite = dataTableTmp.Rows[rowid][CN_BDMember].ToString();
                station.BDSatellite = dataTableTmp.Rows[rowid][CN_BDSatellite].ToString();
                station.Maintran = dataTableTmp.Rows[rowid][CN_Maintran].ToString();
                station.Subtran = dataTableTmp.Rows[rowid][CN_Subtran].ToString();
                station.Datapotocol = dataTableTmp.Rows[rowid][CN_Dataprotocol].ToString();
                station.Watersensor = dataTableTmp.Rows[rowid][CN_Watersensor].ToString();
                station.Rainsensor = dataTableTmp.Rows[rowid][CN_Rainsensor].ToString();
                station.Reportinterval = dataTableTmp.Rows[rowid][CN_Reportinterval].ToString();
                results.Add(station);
            }
            return results;
        }

        public string QueryGPRSById(string id)
        {
            string sql = " select Gprs from " + CT_TableName + " where StationID = " + id + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count > 0)
            {
                if (dataTableTmp.Rows[0][0].ToString() != "")
                {
                    return dataTableTmp.Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }
            }

            else
            {
                return "";
            }
        }

        //1009
        public CEntityStation QueryByGprs(string gprsID)
        {
            // List<CEntityStation> results = new List<CEntityStation>();
            CEntityStation result = new CEntityStation();
            string sql = "select * from " + CT_TableName + " where " + CN_GPRS + " = '" + gprsID + "';";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {

                result.StationID = dataTableTmp.Rows[0][CN_StationID].ToString();
                result.GPRS = dataTableTmp.Rows[0][CN_GPRS].ToString();
                result.StationName = dataTableTmp.Rows[0][CN_StationName].ToString();
                result.SubCenterID = int.Parse(dataTableTmp.Rows[0][CN_SubCenterID].ToString());
                // results.Add(result);

            }
            return result;
        }

        #endregion ///<共有方法

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
                SqlConnection conn = CDBManager.GetInstacne().GetConnection();
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.DestinationTableName = CT_TableName;
                    //bulkCopy.ColumnMappings.Add(CN_RainID, CN_RainID);
                    bulkCopy.ColumnMappings.Add(CN_StationID, CN_StationID);
                    bulkCopy.ColumnMappings.Add(CN_SubCenterID, CN_SubCenterID);
                    bulkCopy.ColumnMappings.Add(CN_StationName, CN_StationName);
                    bulkCopy.ColumnMappings.Add(CN_StationType, CN_StationType);
                    bulkCopy.ColumnMappings.Add(CN_WaterBase, CN_WaterBase);
                    bulkCopy.ColumnMappings.Add(CN_WaterMax, CN_WaterMax);
                    bulkCopy.ColumnMappings.Add(CN_WaterMin, CN_WaterMin);
                    bulkCopy.ColumnMappings.Add(CN_WaterChange, CN_WaterChange);
                    bulkCopy.ColumnMappings.Add(CN_RainAccuracy, CN_RainAccuracy);
                    bulkCopy.ColumnMappings.Add(CN_RainChange, CN_RainChange);
                    bulkCopy.ColumnMappings.Add(CN_GSM, CN_GSM);
                    bulkCopy.ColumnMappings.Add(CN_GPRS, CN_GPRS);
                    bulkCopy.ColumnMappings.Add(CN_BDSatellite, CN_BDSatellite);
                    bulkCopy.ColumnMappings.Add(CN_BDMember, CN_BDMember);
                    bulkCopy.ColumnMappings.Add(CN_VoltageMin, CN_VoltageMin);
                    bulkCopy.ColumnMappings.Add(CN_Maintran, CN_Maintran);
                    bulkCopy.ColumnMappings.Add(CN_Subtran, CN_Subtran);
                    bulkCopy.ColumnMappings.Add(CN_Dataprotocol, CN_Dataprotocol);
                    bulkCopy.ColumnMappings.Add(CN_Watersensor, CN_Watersensor);
                    bulkCopy.ColumnMappings.Add(CN_Rainsensor, CN_Rainsensor);
                    bulkCopy.ColumnMappings.Add(CN_Reportinterval, CN_Reportinterval);
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
            Debug.WriteLine("###{0} :add {1} lines to db", DateTime.Now, tmp.Rows.Count);
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }
        /// <summary>
        /// 添加站点，不用
        /// </summary>
        //private void Add(CEntityStation entity)
        //{
        //    String sqlStr = GetInsertSQL(entity);
        //    ExecuteNonQuery(sqlStr);
        //}

        //private String GetInsertSQL(CEntityStation entity)
        //{
        //    String strStationID = entity.StationID;
        //    String strSubCenterID = entity.SubCenterID.HasValue ? entity.SubCenterID.Value.ToString() : "null";
        //    String strCName = String.Format("'{0}'", entity.StationName);
        //    String strCType = CEnumHelper.StationTypeToDBStr(entity.StationType);

        //    String strWaterBase = entity.DWaterBase.HasValue ? entity.DWaterBase.ToString() : "null";
        //    String strWaterMax = entity.DWaterMax.HasValue ? entity.DWaterMax.ToString() : "null";
        //    String strWaterMin = entity.DWaterMin.HasValue ? entity.DWaterMin.ToString() : "null";
        //    String strWaterChange = entity.DWaterChange.HasValue ? entity.DWaterChange.ToString() : "null";

        //    String strRainAccuracy = entity.DRainAccuracy.ToString();
        //    String strRainChange =entity.DRainChange.ToString();
        //    String strCommParam = entity.CommParam == null ? "null" : String.Format("'{0}'", entity.CommParam);
        //    String strGSM = entity.GSM == null ? "null" : String.Format("'{0}'", entity.GSM);

        //    String strGprs = entity.GPRS == null ? "null" : String.Format("'{0}'", entity.GPRS);
        //    String strPstv = entity.PSTV == null ? "null" : String.Format("'{0}'", entity.PSTV);
        //    String strBD = entity.BDSatellite == null ? "null" : String.Format("'{0}'", entity.BDSatellite);
        //    String strSerialPort = entity.SerialPort == null ? "null" : String.Format("'{0}'", entity.SerialPort);
        //    return String.Format(
        //        "INSERT INTO {0} ([{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}],[{9}],[{10}],[{11}],[{12}],[{13}],[{14}],[{15}],[{16}]) VALUES ({17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32})",

        //        CT_TableName,

        //        CN_StationID, CN_SubCenterID, CN_StationName, CN_StationType,
        //        CN_WaterBase, CN_WaterMax, CN_WaterMin, CN_WaterChange,
        //        CN_RainAccuracy, CN_RainChange, CN_CommParam, CN_GSM,
        //        CN_GPRS, CN_PSTV, CN_BDSatellite, CN_SerialPort,

        //        strStationID, strSubCenterID, strCName, strCType,
        //        strWaterBase, strWaterMax, strWaterMin, strWaterChange,
        //        strRainAccuracy, strRainChange, strCommParam, strGSM,
        //        strGprs, strPstv, strBD, strSerialPort
        //    );
        //}

        ///// <summary>
        ///// 删除分站点
        ///// </summary>
        //public void Delete(CEntityStation entity)
        //{
        //    String sqlStr = GetDeleteSQL(entity);
        //    ExecuteNonQuery(sqlStr);
        //}

        //private String GetDeleteSQL(CEntityStation entity)
        //{
        //    return String.Format("DELETE FROM {0} WHERE [{1}]={2}",
        //        CT_TableName,

        //        CN_StationID, entity.StationID
        //    );
        //}

        /// <summary>
        /// 修改站点信息
        /// </summary>
        //public void Update(CEntityStation entity)
        //{
        //    String sqlStr = GetUpdateSQL(entity);
        //    ExecuteNonQuery(sqlStr);
        //}

        private String GetUpdateSQL(CEntityStation entity)
        {
            String strStationID = entity.StationID;
            String strSubCenterID = entity.SubCenterID.HasValue ? entity.SubCenterID.Value.ToString() : "null";
            String strCName = String.Format("'{0}'", entity.StationName);
            String strCType = CEnumHelper.StationTypeToDBStr(entity.StationType);

            String strWaterBase = entity.DWaterBase.HasValue ? entity.DWaterBase.ToString() : "null";
            String strWaterMax = entity.DWaterMax.HasValue ? entity.DWaterMax.ToString() : "null";
            String strWaterMin = entity.DWaterMin.HasValue ? entity.DWaterMin.ToString() : "null";
            String strWaterChange = entity.DWaterChange.HasValue ? entity.DWaterChange.ToString() : "null";
            String strVoltageMin = entity.DVoltageMin.HasValue ? entity.DVoltageMin.ToString() : "null";

            String strRainAccuracy = entity.DRainAccuracy.ToString();
            String strRainChange = entity.DRainChange.HasValue ? entity.DRainChange.ToString() : "null";

            String strGSM = entity.GSM == null ? "null" : String.Format("'{0}'", entity.GSM);
            String strGprs = entity.GPRS == null ? "null" : String.Format("'{0}'", entity.GPRS);

            String strBD = entity.BDSatellite == null ? "null" : String.Format("'{0}'", entity.BDSatellite);
            String strBDMember = entity.BDMemberSatellite == null ? "null" : String.Format("'{0}'", entity.BDMemberSatellite);

            String strMaintran = entity.Maintran == null ? "null" : String.Format("'{0}'", entity.Maintran);
            String strSubtran = entity.Subtran == null ? "null" : String.Format("'{0}'", entity.Subtran);
            String strDataprotocol = entity.Datapotocol == null ? "null" : String.Format("'{0}'", entity.Datapotocol);
            String strWatersensor = entity.Watersensor == null ? "null" : String.Format("'{0}'", CEnumHelper.WaterSensorTypeToDBStr(entity.Watersensor));
            String strRainsensor = entity.Rainsensor == null ? "null" : String.Format("'{0}'", CEnumHelper.RainSensorTypeToDBStr(entity.Rainsensor));
            String strReportinterval = entity.Reportinterval == null ? "null" : String.Format("'{0}'", entity.Reportinterval);
            //  String strSerialPort = entity.SerialPort == null ? "null" : String.Format("'{0}'", entity.SerialPort);

            //  string strBatchType = CEnumHelper.StationBatchTypeToDBStr(entity.BatchTranType);

            return String.Format(
                "UPDATE {0} SET [{1}]={2},[{3}]={4},[{5}]={6},[{7}]={8},[{9}]={10},[{11}]={12},[{13}]={14},[{15}]={16},[{17}]={18},[{19}]={20},[{21}]={22},[{23}]={24},[{25}]={26},[{27}]={28},[{29}]={30},[{31}]={32},[{33}]={34} ,[{35}]={36},[{37}]={38},[{39}]={40} WHERE [{41}]='{42}';",
                CT_TableName,

                CN_SubCenterID, strSubCenterID,
                CN_StationName, strCName,
                CN_StationType, strCType,
                CN_WaterBase, strWaterBase,
                CN_WaterMax, strWaterMax,
                CN_WaterMin, strWaterMin,
                CN_WaterChange, strWaterChange,
                CN_RainAccuracy, strRainAccuracy,
                CN_RainChange, strRainChange,
                CN_GSM, strGSM,
                CN_GPRS, strGprs,
                CN_BDSatellite, strBD,
                CN_BDMember, strBDMember,
                CN_VoltageMin, strVoltageMin,
                CN_Maintran, strMaintran,
                CN_Subtran, strSubtran,
                CN_Dataprotocol, strDataprotocol,
                CN_Watersensor, strWatersensor,
                CN_Rainsensor, strRainsensor,
                CN_Reportinterval, strReportinterval,
                CN_StationID, strStationID
            );
        }


        /// <summary>
        /// 获取所有站点信息
        /// </summary>
        //         public IList<CEntityStation> QueryAll()
        //         {
        //             var result = new List<CEntityStation>();
        //             var sqlConn = CDBManager.GetInstacne().getConnection();
        //             try
        //             {
        //                 m_mutexWriteToDB.WaitOne();         // 取对数据库的唯一访问权
        //                 m_mutexDataTable.WaitOne();         // 获取内存表的访问权
        //                 sqlConn.Open();                     // 建立数据库连接
        // 
        //                 /**********异步查询数据库**********/
        //                 String sqlStr = GetQueryAllSQL();
        // 
        //                 SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConn);
        // 
        //                 SqlDataReader reader = sqlCmd.ExecuteReader();
        // 
        //                 Debug.Assert(reader.FieldCount == CN_FIELD_COUNT, CT_TableName + "表与类" + CT_EntityName + "中定义字段不符合");
        // 
        //                 //  处理查询结果
        //                 while (reader.Read())
        //                 {
        //                     try
        //                     {
        //                         var item = new CEntityStation();
        // 
        //                         item.StationID = (String)reader[CN_StationID];
        // 
        //                         if (reader[CN_SubCenterID] is DBNull)
        //                             item.SubCenterID = null;
        //                         else
        //                             item.SubCenterID = (Int32)reader[CN_SubCenterID];
        // 
        //                         item.StationName = (String)reader[CN_StationName];
        // 
        //                         item.StationType = CEnumHelper.DBStrToStationType((String)reader[CN_StationType]);
        // 
        //                         if (reader[CN_WaterBase] is DBNull)
        //                             item.DWaterBase = null;
        //                         else
        //                             item.DWaterBase = (Decimal)reader[CN_WaterBase];
        // 
        //                         if (reader[CN_WaterMax] is DBNull)
        //                             item.DWaterMax = null;
        //                         else
        //                             item.DWaterMax = (Decimal)reader[CN_WaterMax];
        // 
        //                         if (reader[CN_WaterMin] is DBNull)
        //                             item.DWaterMin = null;
        //                         else
        //                             item.DWaterMin = (Decimal)reader[CN_WaterMin];
        // 
        //                         if (reader[CN_WaterChange] is DBNull)
        //                             item.DWaterChange = null;
        //                         else
        //                             item.DWaterChange = (Decimal)reader[CN_WaterChange];
        // 
        // 
        //                             item.DRainAccuracy = (float)reader[CN_RainAccuracy];
        // 
        //                         if (reader[CN_RainChange] is DBNull)
        //                             item.DRainChange = null;
        //                         else
        //                             item.DRainChange = (Decimal)reader[CN_RainChange];
        // 
        //                         if (reader[CN_CommParam] is DBNull)
        //                             item.CommParam = null;
        //                         else
        //                             item.CommParam = (String)reader[CN_CommParam];
        // 
        //                         if (reader[CN_GSM] is DBNull)
        //                             item.GSM = null;
        //                         else
        //                             item.GSM = (String)reader[CN_GSM];
        // 
        //                         if (reader[CN_GPRS] is DBNull)
        //                             item.GPRS = null;
        //                         else
        //                             item.GPRS = (String)reader[CN_GPRS];
        // 
        //                         if (reader[CN_PSTV] is DBNull)
        //                             item.PSTV = null;
        //                         else
        //                             item.PSTV = (String)reader[CN_PSTV];
        // 
        //                         if (reader[CN_BDSatellite] is DBNull)
        //                             item.BDSatellite = null;
        //                         else
        //                             item.BDSatellite = (String)reader[CN_BDSatellite];
        // 
        //                         if (reader[CN_SerialPort] is DBNull)
        //                             item.SerialPort = null;
        //                         else
        //                             item.SerialPort = (String)reader[CN_SerialPort];
        // 
        //                         result.Add(item);
        //                     }
        //                     catch
        //                     {
        // 
        //                     }
        //                 }
        //             }
        //             catch (Exception exp)
        //             {
        //                 throw exp;
        //             }
        //             finally
        //             {
        //                 sqlConn.Close();                    //  关闭数据库连接
        //                 m_mutexDataTable.ReleaseMutex();    //  释放内存表的访问权
        //                 m_mutexWriteToDB.ReleaseMutex();    //  释放数据库的访问权
        //             }
        //             return result;
        //         }

        //private String GetQueryAllSQL()
        //{
        //    return String.Format(
        //        "SELECT [{0}],[{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}],[{9}],[{10}],[{11}], [{12}],[{13}],[{14}],[{15}] FROM {16}",
        //        CN_StationID, CN_SubCenterID, CN_StationName, CN_StationType,
        //        CN_WaterBase, CN_WaterMax, CN_WaterMin, CN_WaterChange,
        //        CN_RainAccuracy, CN_RainChange, CN_CommParam, CN_GSM,
        //        CN_GPRS, CN_PSTV, CN_BDSatellite, CN_SerialPort,

        //        CT_TableName
        //    );
        //}

        public CEntityStation QueryById(string stationid)
        {
            CEntityStation result = new CEntityStation();
            string sql = "select * from " + CT_TableName + " where " + CN_StationID + " = '" + stationid + "';";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count == 1)
            {
                try
                {
                    result.StationID = dataTableTmp.Rows[0][CN_StationID].ToString();
                }
                catch
                {
                    return null;
                }
                try
                {
                    result.GPRS = dataTableTmp.Rows[0][CN_GPRS].ToString();
                }
                catch
                {
                    return null;
                }

                try
                {
                    result.StationName = dataTableTmp.Rows[0][CN_StationName].ToString();
                }
                catch
                {

                }
                try
                {
                    result.SubCenterID = int.Parse(dataTableTmp.Rows[0][CN_SubCenterID].ToString());
                }
                catch
                {

                }
                try
                {
                    result.DRainAccuracy = float.Parse(dataTableTmp.Rows[0][CN_RainAccuracy].ToString());
                }
                catch
                {
                    return null;
                }

                try
                {
                    result.DRainChange = decimal.Parse(dataTableTmp.Rows[0][CN_RainChange].ToString());
                }
                catch
                {
                    return null;
                }

                try
                {
                    result.DWaterBase = decimal.Parse(dataTableTmp.Rows[0][CN_WaterBase].ToString());
                    result.DWaterChange = decimal.Parse(dataTableTmp.Rows[0][CN_WaterChange].ToString());
                }
                catch
                {

                }

            }
            return result;
        }



        #endregion ///< HELP_METHOD

        public List<string> getAllGprs()
        {
            List<string> results = new List<string>();
            string sql = "select distinct  Gprs from " + CT_TableName + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
            {
                string result = dataTableTmp.Rows[rowid][0].ToString();
                results.Add(result);
            }
            return results;
        }

        //115 gm
        public List<CEntityStation> getAllGprs_1()
        {
            List<CEntityStation> results = new List<CEntityStation>();
            string sql = "select distinct  Gprs,stationid from " + CT_TableName + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
            {
                CEntityStation result = new CEntityStation();
                result.GPRS =  dataTableTmp.Rows[rowid][0].ToString();
                result.StationID = dataTableTmp.Rows[rowid][1].ToString();
                results.Add(result);
            }
            return results;
        }

    }
}
