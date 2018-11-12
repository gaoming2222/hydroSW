using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Data;
using Hydrology.DBManager;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Diagnostics;
using Hydrology.DBManager.Interface;

namespace Hydrology.DBManager.DB.SQLServer
{
    /// <summary>
    /// 墒情站的附加实体类数据信息
    /// </summary>
    public class CSQLSoilStation : CSQLBase, ISoilStationProxy
    {
        #region 静态常量
        public static readonly string CT_TableName = "soilstation";      //数据库中墒情站表的名字
        public static readonly string CN_StationId = "stationid";   // 站点唯一ID
        public static readonly string CN_SubCenterID = "subcenterid"; //测站所属分中心的唯一ID
        public static readonly string CN_StationName = "cname";
        public static readonly string CN_StationType = "ctype";    //站点类型

        //   public static readonly string CN_DeviceNumber = "DeviceNumber";         // 设备终端机号

        public static readonly string CN_A10 = "A10";
        public static readonly string CN_B10 = "B10";
        public static readonly string CN_C10 = "C10";
        public static readonly string CN_D10 = "D10";
        public static readonly string CN_M10 = "M10";
        public static readonly string CN_N10 = "N10";

        public static readonly string CN_A20 = "A20";
        public static readonly string CN_B20 = "B20";
        public static readonly string CN_C20 = "C20";
        public static readonly string CN_D20 = "D20";
        public static readonly string CN_M20 = "M20";
        public static readonly string CN_N20 = "N20";


        public static readonly string CN_A30 = "A30";
        public static readonly string CN_B30 = "B30";
        public static readonly string CN_C30 = "C30";
        public static readonly string CN_D30 = "D30";
        public static readonly string CN_M30 = "M30";
        public static readonly string CN_N30 = "N30";

        public static readonly string CN_A40 = "A40";
        public static readonly string CN_B40 = "B40";
        public static readonly string CN_C40 = "C40";
        public static readonly string CN_D40 = "D40";
        public static readonly string CN_M40 = "M40";
        public static readonly string CN_N40 = "N40";

        public static readonly string CN_A60 = "A60";
        public static readonly string CN_B60 = "B60";
        public static readonly string CN_C60 = "C60";
        public static readonly string CN_D60 = "D60";
        public static readonly string CN_M60 = "M60";
        public static readonly string CN_N60 = "N60";

        public static readonly string CN_Voltagemin = "voltagemin";

        public static readonly string CN_GSM = "gsm";       //GSM号码
        public static readonly string CN_GPRS = "gprs";       //Gprs号码
        public static readonly string CN_BDSatellite = "bdsatellite";       //北斗卫星号码
        public static readonly string CN_BDMember = "bdmember";       //北斗卫星号码
        public static readonly string CN_Maintran = "maintran";
        public static readonly string CN_Subtran = "subtran";
        public static readonly string CN_Dataprotocol = "dataprotocol";
        public static readonly string CN_Reportinterval = "reportinterval";
        #endregion

        #region 公共方法
        public CSQLSoilStation()
            : base()
        {
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_StationId);
            m_tableDataAdded.Columns.Add(CN_SubCenterID);
            m_tableDataAdded.Columns.Add(CN_StationName);
            m_tableDataAdded.Columns.Add(CN_StationType);

            //    m_tableDataAdded.Columns.Add(CN_DeviceNumber);


            m_tableDataAdded.Columns.Add(CN_A10);
            m_tableDataAdded.Columns.Add(CN_B10);
            m_tableDataAdded.Columns.Add(CN_C10);
            m_tableDataAdded.Columns.Add(CN_D10);
            m_tableDataAdded.Columns.Add(CN_M10);
            m_tableDataAdded.Columns.Add(CN_N10);

            m_tableDataAdded.Columns.Add(CN_A20);
            m_tableDataAdded.Columns.Add(CN_B20);
            m_tableDataAdded.Columns.Add(CN_C20);
            m_tableDataAdded.Columns.Add(CN_D20);
            m_tableDataAdded.Columns.Add(CN_M20);
            m_tableDataAdded.Columns.Add(CN_N20);

            m_tableDataAdded.Columns.Add(CN_A30);
            m_tableDataAdded.Columns.Add(CN_B30);
            m_tableDataAdded.Columns.Add(CN_C30);
            m_tableDataAdded.Columns.Add(CN_D30);
            m_tableDataAdded.Columns.Add(CN_M30);
            m_tableDataAdded.Columns.Add(CN_N30);

            m_tableDataAdded.Columns.Add(CN_A40);
            m_tableDataAdded.Columns.Add(CN_B40);
            m_tableDataAdded.Columns.Add(CN_C40);
            m_tableDataAdded.Columns.Add(CN_D40);
            m_tableDataAdded.Columns.Add(CN_M40);
            m_tableDataAdded.Columns.Add(CN_N40);

            m_tableDataAdded.Columns.Add(CN_A60);
            m_tableDataAdded.Columns.Add(CN_B60);
            m_tableDataAdded.Columns.Add(CN_C60);
            m_tableDataAdded.Columns.Add(CN_D60);
            m_tableDataAdded.Columns.Add(CN_M60);
            m_tableDataAdded.Columns.Add(CN_N60);

            m_tableDataAdded.Columns.Add(CN_Voltagemin);

            m_tableDataAdded.Columns.Add(CN_GSM);
            m_tableDataAdded.Columns.Add(CN_GPRS);
            m_tableDataAdded.Columns.Add(CN_BDSatellite);
            m_tableDataAdded.Columns.Add(CN_BDMember);
          
            m_tableDataAdded.Columns.Add(CN_Maintran);
            m_tableDataAdded.Columns.Add(CN_Subtran);
            m_tableDataAdded.Columns.Add(CN_Dataprotocol);
            m_tableDataAdded.Columns.Add(CN_Reportinterval);
        
            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_SoilStationInfo;
        }

        /// <summary>
        /// 异步添加一个数据记录
        /// </summary>
        /// <param name="entity"></param>
        public bool AddNewRow(CEntitySoilStation entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            bool result = true;
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();

            row[CN_StationId] = entity.StationID;
            row[CN_SubCenterID] = entity.SubCenterID;
            row[CN_StationName] = entity.StationName;
            row[CN_StationType] = (Int32)entity.StationType;

            //   row[CN_DeviceNumber] = entity.StrDeviceNumber;


            row[CN_A10] = entity.A10;
            row[CN_B10] = entity.B10;
            row[CN_C10] = entity.C10;
            row[CN_D10] = entity.D10;
            row[CN_M10] = entity.M10;
            row[CN_N10] = entity.N10;

            row[CN_A20] = entity.A20;
            row[CN_B20] = entity.B20;
            row[CN_C20] = entity.C20;
            row[CN_D20] = entity.D20;
            row[CN_M20] = entity.M20;
            row[CN_N20] = entity.N20;

            row[CN_A30] = entity.A30;
            row[CN_B30] = entity.B30;
            row[CN_C30] = entity.C30;
            row[CN_D30] = entity.D30;
            row[CN_M30] = entity.M30;
            row[CN_N30] = entity.N30;

            row[CN_A40] = entity.A40;
            row[CN_B40] = entity.B40;
            row[CN_C40] = entity.C40;
            row[CN_D40] = entity.D40;
            row[CN_M40] = entity.M40;
            row[CN_N40] = entity.N40;

            row[CN_A60] = entity.A60;
            row[CN_B60] = entity.B60;
            row[CN_C60] = entity.C60;
            row[CN_D60] = entity.D60;
            row[CN_M60] = entity.M60;
            row[CN_N60] = entity.N60;

            row[CN_Voltagemin] = entity.VoltageMin;

            row[CN_GSM] = entity.GSM;
            row[CN_GPRS] = entity.GPRS;
            row[CN_BDSatellite] = entity.BDSatellite;
            row[CN_BDMember] = entity.BDMemberSatellite;

            row[CN_Maintran] = entity.Maintran;
            row[CN_Subtran] = entity.Subtran;
            row[CN_Dataprotocol] = entity.Datapotocol;
            row[CN_Reportinterval] = entity.Reportinterval;

            m_tableDataAdded.Rows.Add(row);
            if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
            {
                // 如果超过最大值，写入数据库
                m_mutexDataTable.ReleaseMutex();
                result = result && AddDataToDB();
                m_mutexDataTable.WaitOne();
            }
            m_mutexDataTable.ReleaseMutex();
            result = result && AddDataToDB();
            return result;
        }

        /// <summary>
        /// 添加系列的墒情站
        /// </summary>
        /// <param name="listStation"></param>
        /// <returns></returns>
        public bool AddSoilStationRange(List<CEntitySoilStation> listStation)
        {
            if (listStation.Count <= 0)
            {
                return true;
            }
            bool result = true;
            m_mutexDataTable.WaitOne();
            foreach (CEntitySoilStation entity in listStation)
            {
                DataRow row = m_tableDataAdded.NewRow();

                row[CN_StationId] = entity.StationID;
                row[CN_SubCenterID] = entity.SubCenterID;
                // row[CN_SubCenterID] = 1;
                // row[CN_StationName] = 1;
                row[CN_StationName] = entity.StationName;
                // row[CN_StationType] = 1;
                row[CN_StationType] = (Int32)entity.StationType;

                //      row[CN_DeviceNumber] = entity.StrDeviceNumber;
                //row[CN_A10] = 0;
                //row[CN_B10] = 0;
                //row[CN_C10] = 0;
                //row[CN_D10] = 0;
                //row[CN_M10] = 0;
                //row[CN_N10] = 0;

                //row[CN_A20] = 0;
                //row[CN_B20] = 0;
                //row[CN_C20] = 0;
                //row[CN_D20] = 0;
                //row[CN_M20] = 0;
                //row[CN_N20] = 0;

                //row[CN_A30] = 0;
                //row[CN_B30] = 0;
                //row[CN_C30] = 0;
                //row[CN_D30] = 0;
                //row[CN_M30] = 0;
                //row[CN_N30] = 0;

                //row[CN_A40] = 0;
                //row[CN_B40] = 0;
                //row[CN_C40] = 0;
                //row[CN_D40] = 0;
                //row[CN_M40] = 0;
                //row[CN_N40] = 0;

                //row[CN_A60] = 0;
                //row[CN_B60] = 0;
                //row[CN_C60] = 0;
                //row[CN_D60] = 0;
                //row[CN_M60] = 0;
                //row[CN_N60] = 0;


                row[CN_A10] = entity.A10;
                row[CN_B10] = entity.B10;
                row[CN_C10] = entity.C10;
                row[CN_D10] = entity.D10;
                row[CN_M10] = entity.M10;
                row[CN_N10] = entity.N10;

                row[CN_A20] = entity.A20;
                row[CN_B20] = entity.B20;
                row[CN_C20] = entity.C20;
                row[CN_D20] = entity.D20;
                row[CN_M20] = entity.M20;
                row[CN_N20] = entity.N20;

                row[CN_A30] = entity.A30;
                row[CN_B30] = entity.B30;
                row[CN_C30] = entity.C30;
                row[CN_D30] = entity.D30;
                row[CN_M30] = entity.M30;
                row[CN_N30] = entity.N30;

                row[CN_A40] = entity.A40;
                row[CN_B40] = entity.B40;
                row[CN_C40] = entity.C40;
                row[CN_D40] = entity.D40;
                row[CN_M40] = entity.M40;
                row[CN_N40] = entity.N40;

                row[CN_A60] = entity.A60;
                row[CN_B60] = entity.B60;
                row[CN_C60] = entity.C60;
                row[CN_D60] = entity.D60;
                row[CN_M60] = entity.M60;
                row[CN_N60] = entity.N60;

                row[CN_Voltagemin] = entity.VoltageMin;

                row[CN_GSM] = entity.GSM;
                row[CN_GPRS] = entity.GPRS;
                row[CN_BDSatellite] = entity.BDSatellite;
                row[CN_BDMember] = entity.BDMemberSatellite;

                row[CN_Maintran] = entity.Maintran;
                row[CN_Subtran] = entity.Subtran;
                row[CN_Dataprotocol] = entity.Datapotocol;
                row[CN_Reportinterval] = entity.Reportinterval;
           //     row[CN_Voltagemin] = 0;
                m_tableDataAdded.Rows.Add(row);
                //if (m_tableDataAdded.Rows.Count >= CDBParams.GetInstance().AddBufferMax)
                //{
                //    // 如果超过最大值，写入数据库
                //    m_mutexDataTable.ReleaseMutex();
                //    result = result && AddDataToDB();
                //    m_mutexDataTable.WaitOne();
                //}
            }
            m_mutexDataTable.ReleaseMutex();
            result = result && AddDataToDB();
            return result;
        }

        /// <summary>
        /// 删除站点的
        /// </summary>
        /// <param name="listStationId"></param>
        /// <returns></returns>
        public bool DeleteSoilStationRange(List<string> listStationId)
        {
            if (listStationId.Count <= 0)
            {
                return true;
            }
            // 删除某条雨量记录
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < listStationId.Count; i++)
            {
                ++currentBatchCount;
                sql.AppendFormat("delete from {0} where {1}={2};",
                    CT_TableName,
                    CN_StationId, listStationId[i]
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
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新一系列站点的配置数据
        /// </summary>
        /// <param name="listStation"></param>
        /// <returns></returns>
        public bool UpdateSoilStation(List<CEntitySoilStation> listStation)
        {
            if (listStation.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            int currentBatchCount = 0;
            for (int i = 0; i < listStation.Count; i++)
            {
                ++currentBatchCount;
                string formatstr = "update {0} set " + GenerateSQL(34, 1) + " , {69}='{70}' ,{71}='{72}' ,{73}='{74}' , {75}='{76}' ,{77}='{78}',  "
                    + " {79}='{80}' , {81}='{82}' , {83}='{84}' "
                    + " where {85} = {86};";
                sql.AppendFormat(formatstr,
                    CT_TableName,
                    CN_SubCenterID, listStation[i].SubCenterID,
                    CN_StationName, String.Format("'{0}'", listStation[i].StationName),
                    CN_StationType, CEnumHelper.StationTypeToDBStr(listStation[i].StationType),
                    //   CN_DeviceNumber, listStation[i].StrDeviceNumber,
                    CN_Voltagemin, listStation[i].VoltageMin,
                    CN_A10, GetNullableSQLString(listStation[i].A10),
                    CN_B10, GetNullableSQLString(listStation[i].B10),
                    CN_C10, GetNullableSQLString(listStation[i].C10),
                    CN_D10, GetNullableSQLString(listStation[i].D10),
                    CN_M10, GetNullableSQLString(listStation[i].M10),
                    CN_N10, GetNullableSQLString(listStation[i].N10),
                    CN_A20, GetNullableSQLString(listStation[i].A20),
                    CN_B20, GetNullableSQLString(listStation[i].B20),
                    CN_C20, GetNullableSQLString(listStation[i].C20),
                    CN_D20, GetNullableSQLString(listStation[i].D20),
                    CN_M20, GetNullableSQLString(listStation[i].M20),
                    CN_N20, GetNullableSQLString(listStation[i].N20),
                    CN_A30, GetNullableSQLString(listStation[i].A30),
                    CN_B30, GetNullableSQLString(listStation[i].B30),
                    CN_C30, GetNullableSQLString(listStation[i].C30),
                    CN_D30, GetNullableSQLString(listStation[i].D30),
                    CN_M30, GetNullableSQLString(listStation[i].M30),
                    CN_N30, GetNullableSQLString(listStation[i].N30),
                    CN_A40, GetNullableSQLString(listStation[i].A40),
                    CN_B40, GetNullableSQLString(listStation[i].B40),
                    CN_C40, GetNullableSQLString(listStation[i].C40),
                    CN_D40, GetNullableSQLString(listStation[i].D40),
                    CN_M40, GetNullableSQLString(listStation[i].M40),
                    CN_N40, GetNullableSQLString(listStation[i].N40),
                    CN_A60, GetNullableSQLString(listStation[i].A60),
                    CN_B60, GetNullableSQLString(listStation[i].B60),
                    CN_C60, GetNullableSQLString(listStation[i].C60),
                    CN_D60, GetNullableSQLString(listStation[i].D60),
                    CN_M60, GetNullableSQLString(listStation[i].M60),
                    CN_N60, GetNullableSQLString(listStation[i].N60),
                    CN_GSM, listStation[i].GSM,
                    CN_GPRS,listStation[i].GPRS,
                    CN_BDSatellite,listStation[i].BDSatellite,
                    CN_BDMember,listStation[i].BDMemberSatellite,
                    CN_Maintran,listStation[i].Maintran,
                    CN_Subtran,listStation[i].Subtran,
                    CN_Dataprotocol,listStation[i].Datapotocol,
                    CN_Reportinterval,listStation[i].Reportinterval,
                    CN_StationId, listStation[i].StationID
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
            return true;
        }

        /// <summary>
        /// 查询所有的墒情站点信息
        /// </summary>
        /// <returns></returns>
        public List<CEntitySoilStation> QueryAllSoilStation()
        {
            string sql = " select * from " + CT_TableName;
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            // 构建结果集
            List<CEntitySoilStation> results = new List<CEntitySoilStation>();
            for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
            {
                CEntitySoilStation soilStation = new CEntitySoilStation();
                soilStation.StationID = dataTableTmp.Rows[rowid][CN_StationId].ToString();
                soilStation.SubCenterID = int.Parse(dataTableTmp.Rows[rowid][CN_SubCenterID].ToString());
                soilStation.StationName = dataTableTmp.Rows[rowid][CN_StationName].ToString();
                soilStation.StationType = CEnumHelper.DBStrToStationType(dataTableTmp.Rows[rowid][CN_StationType].ToString());


                //  soilStation.StrDeviceNumber = dataTableTmp.Rows[rowid][CN_DeviceNumber].ToString();


                soilStation.A10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_A10]);
                soilStation.B10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_B10]);
                soilStation.C10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_C10]);
                soilStation.D10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_D10]);
                soilStation.M10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_M10]);
                soilStation.N10 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_N10]);

                soilStation.A20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_A20]);
                soilStation.B20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_B20]);
                soilStation.C20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_C20]);
                soilStation.D20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_D20]);
                soilStation.M20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_M20]);
                soilStation.N20 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_N20]);

                soilStation.A30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_A30]);
                soilStation.B30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_B30]);
                soilStation.C30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_C30]);
                soilStation.D30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_D30]);
                soilStation.M30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_M30]);
                soilStation.N30 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_N30]);

                soilStation.A40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_A40]);
                soilStation.B40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_B40]);
                soilStation.C40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_C40]);
                soilStation.D40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_D40]);
                soilStation.M40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_M40]);
                soilStation.N40 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_N40]);

                soilStation.A60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_A60]);
                soilStation.B60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_B60]);
                soilStation.C60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_C60]);
                soilStation.D60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_D60]);
                soilStation.M60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_M60]);
                soilStation.N60 = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_N60]);

                soilStation.VoltageMin = GetCellDecimalValue(dataTableTmp.Rows[rowid][CN_Voltagemin]);

                soilStation.GSM = dataTableTmp.Rows[rowid][CN_GSM].ToString();
                soilStation.GPRS = dataTableTmp.Rows[rowid][CN_GPRS].ToString();
                soilStation.BDSatellite = dataTableTmp.Rows[rowid][CN_BDSatellite].ToString();
                soilStation.BDMemberSatellite = dataTableTmp.Rows[rowid][CN_BDMember].ToString();

                soilStation.Maintran = dataTableTmp.Rows[rowid][CN_Maintran].ToString();
                soilStation.Subtran = dataTableTmp.Rows[rowid][CN_Subtran].ToString();
                soilStation.Datapotocol = dataTableTmp.Rows[rowid][CN_Dataprotocol].ToString();
                soilStation.Reportinterval = dataTableTmp.Rows[rowid][CN_Reportinterval].ToString();
                results.Add(soilStation);
            }
            return results;
        }

        ////1009
        //public List<CEntityStation> QueryByGprs(string gprsID)
        //{
        //    CEntityStation result = new CEntityStation();
        //    string sql = "select * from " + CT_TableName + " where " + CN_GPRS + " = '" + gprsID + "';";
        //    SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
        //    DataTable dataTableTmp = new DataTable();
        //    adapter.Fill(dataTableTmp);
        //    for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
        //    {
        //        result.StationID = dataTableTmp.Rows[rowid][CN_StationId].ToString();
        //        result.GPRS = dataTableTmp.Rows[rowid][CN_StationId].ToString();
        //    }
        //    return result;
        //}

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
                //将临时表中的内容写入数据库
                SqlConnection conn = CDBManager.GetInstacne().GetConnection();
                conn.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = CT_TableName;
                    bulkCopy.BatchSize = 1;
                    bulkCopy.BulkCopyTimeout = 1800;
                    bulkCopy.ColumnMappings.Add(CN_StationId, CN_StationId);
                    bulkCopy.ColumnMappings.Add(CN_SubCenterID, CN_SubCenterID);
                    bulkCopy.ColumnMappings.Add(CN_StationName, CN_StationName);
                    bulkCopy.ColumnMappings.Add(CN_StationType, CN_StationType);

                    //     bulkCopy.ColumnMappings.Add(CN_DeviceNumber, CN_DeviceNumber);


                    bulkCopy.ColumnMappings.Add(CN_A10, CN_A10);
                    bulkCopy.ColumnMappings.Add(CN_B10, CN_B10);
                    bulkCopy.ColumnMappings.Add(CN_C10, CN_C10);
                    bulkCopy.ColumnMappings.Add(CN_D10, CN_D10);
                    bulkCopy.ColumnMappings.Add(CN_M10, CN_M10);
                    bulkCopy.ColumnMappings.Add(CN_N10, CN_N10);

                    bulkCopy.ColumnMappings.Add(CN_A20, CN_A20);
                    bulkCopy.ColumnMappings.Add(CN_B20, CN_B20);
                    bulkCopy.ColumnMappings.Add(CN_C20, CN_C20);
                    bulkCopy.ColumnMappings.Add(CN_D20, CN_D20);
                    bulkCopy.ColumnMappings.Add(CN_M20, CN_M20);
                    bulkCopy.ColumnMappings.Add(CN_N20, CN_N20);

                    bulkCopy.ColumnMappings.Add(CN_A30, CN_A30);
                    bulkCopy.ColumnMappings.Add(CN_B30, CN_B30);
                    bulkCopy.ColumnMappings.Add(CN_C30, CN_C30);
                    bulkCopy.ColumnMappings.Add(CN_D30, CN_D30);
                    bulkCopy.ColumnMappings.Add(CN_M30, CN_M30);
                    bulkCopy.ColumnMappings.Add(CN_N30, CN_N30);

                    bulkCopy.ColumnMappings.Add(CN_A40, CN_A40);
                    bulkCopy.ColumnMappings.Add(CN_B40, CN_B40);
                    bulkCopy.ColumnMappings.Add(CN_C40, CN_C40);
                    bulkCopy.ColumnMappings.Add(CN_D40, CN_D40);
                    bulkCopy.ColumnMappings.Add(CN_M40, CN_M40);
                    bulkCopy.ColumnMappings.Add(CN_N40, CN_N40);

                    bulkCopy.ColumnMappings.Add(CN_A60, CN_A60);
                    bulkCopy.ColumnMappings.Add(CN_B60, CN_B60);
                    bulkCopy.ColumnMappings.Add(CN_C60, CN_C60);
                    bulkCopy.ColumnMappings.Add(CN_D60, CN_D60);
                    bulkCopy.ColumnMappings.Add(CN_M60, CN_M60);
                    bulkCopy.ColumnMappings.Add(CN_N60, CN_N60);

                    bulkCopy.ColumnMappings.Add(CN_Voltagemin, CN_Voltagemin);

                    bulkCopy.ColumnMappings.Add(CN_GSM, CN_GSM);
                    bulkCopy.ColumnMappings.Add(CN_GPRS, CN_GPRS);
                    bulkCopy.ColumnMappings.Add(CN_BDSatellite, CN_BDSatellite);
                    bulkCopy.ColumnMappings.Add(CN_BDMember, CN_BDMember);
                    bulkCopy.ColumnMappings.Add(CN_Maintran, CN_Maintran);
                    bulkCopy.ColumnMappings.Add(CN_Subtran, CN_Subtran);
                    bulkCopy.ColumnMappings.Add(CN_Dataprotocol, CN_Dataprotocol);
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
            Debug.WriteLine("###{0} :add {1} lines to soil station db", DateTime.Now, tmp.Rows.Count);
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }
        #endregion ///< 帮助方法

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

        public List<CEntitySoilStation> getAllGprs_1()
        {
            List<CEntitySoilStation> results = new List<CEntitySoilStation>();
            string sql = "select distinct  Gprs,stationid  from " + CT_TableName + ";";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            for (int rowid = 0; rowid < dataTableTmp.Rows.Count; ++rowid)
            {
                CEntitySoilStation result = new CEntitySoilStation();
                string gprs = dataTableTmp.Rows[rowid][0].ToString();
                string stationid = dataTableTmp.Rows[rowid][1].ToString();
                result.GPRS = gprs;
                result.StationID = stationid;
                results.Add(result);
            }
            return results;
        }


    }
}

