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
    public class CSQLSerialPort : CSQLBase, ISerialPortProxy
    {
        #region STATIC_MEMBER
        private const string CT_EntityName = "CEntitySerialPort";   //  串口配置表对应的实体类名称
        private static readonly string CT_TableName = "SerialPort"; //数据库串口配置表的名字
        private static readonly string CN_PortNumber = "PortNumber";    //串口号，1~16，主键
        private static readonly string CN_TransType = "TransType";    //传输类型，1，短波...
        private static readonly string CN_Baudrate = "Baudrate";     //波特率
        private static readonly string CN_Databit = "Databit";   //数据位
        private static readonly string CN_Stopbit = "Stopbit";   //停止位
        private static readonly string CN_Parity = "Parity";   //校验方式
        private static readonly string CN_Stream = "Stream";   //流控制方式
        private static readonly string CN_Break = "Break";   //中断开关
        private static readonly string CN_Open = "Open";   //开关状态
        #endregion
        private const int CN_FIELD_COUNT = 9;

        #region PRIVATE_DATAMEMBER

        private List<int> m_listDelRows;            // 删除串口记录的链表
        private List<CEntitySerialPort> m_listUpdateRows; // 更新用户信息的链表

        #endregion ///<PRIVATE_DATAMEMBER

        #region 公共方法

        public CSQLSerialPort()
            : base()
        {
            m_listDelRows = new List<int>();
            m_listUpdateRows = new List<CEntitySerialPort>();
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_PortNumber);
            m_tableDataAdded.Columns.Add(CN_TransType);
            m_tableDataAdded.Columns.Add(CN_Baudrate);
            m_tableDataAdded.Columns.Add(CN_Databit);
            m_tableDataAdded.Columns.Add(CN_Stopbit);
            m_tableDataAdded.Columns.Add(CN_Parity);
            m_tableDataAdded.Columns.Add(CN_Stream);
            m_tableDataAdded.Columns.Add(CN_Break);
            m_tableDataAdded.Columns.Add(CN_Open);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_SerialPort;
        }

        // 添加新列
        public void AddNewRow(CEntitySerialPort entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_PortNumber] = entity.PortNumber;
            row[CN_TransType] = CEnumHelper.SerialTransTypeToDBStr(entity.TransType);
            row[CN_Baudrate] = entity.Baudrate;
            row[CN_Databit] = entity.DataBit;
            row[CN_Stopbit] = entity.StopBit;
            row[CN_Parity] = CEnumHelper.PortParityTypeToDBChar(entity.ParityType);
            row[CN_Stream] = CEnumHelper.SerialPortStreamTypeToDBStr(entity.Stream);
            row[CN_Break] = entity.Break;
            row[CN_Open] = entity.SwitchSatus;
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

        #endregion ///<PUBLIC_METHOD

        #region 帮助方法

        /// <summary>
        /// 添加串口集合
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool AddRange(List<CEntitySerialPort> listPort)
        {
            foreach (var item in listPort)
            {
                AddNewRow(item);
            }
            return AddDataToDB();
        }

        /// <summary>
        /// 更新串口信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool UpdateRange(List<CEntitySerialPort> listPort)
        {
            StringBuilder sql = new StringBuilder();
            foreach (var item in listPort)
            {
                string transType = CEnumHelper.SerialTransTypeToDBStr(item.TransType);
                char parity = CEnumHelper.PortParityTypeToDBChar(item.ParityType);
                string breakValue = item.Break.HasValue ? item.Break.Value.ToString() : "null";
                string open = item.SwitchSatus.HasValue ? item.SwitchSatus.Value.ToString() : "null";
                sql.AppendFormat("UPDATE {0} SET [{1}]='{2}',[{3}]='{4}',[{5}]='{6}',[{7}]='{8}',[{9}]='{10}',[{11}]='{12}',[{13}]='{14}',[{15}]='{16}' WHERE [{17}]={18}\r",
                    CT_TableName,

                    CN_TransType, transType,
                    CN_Baudrate, item.Baudrate,
                    CN_Databit, item.DataBit,
                    CN_Stopbit, item.StopBit,
                    CN_Parity, parity,
                    CN_Stream, CEnumHelper.SerialPortStreamTypeToDBStr(item.Stream),
                    CN_Break, breakValue,
                    CN_Open, open,

                    CN_PortNumber, item.PortNumber
                    );
            }
            return base.ExecuteSQLCommand(sql.ToString());
        }

        /// <summary>
        /// 删除串口信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool DeleteRange(List<int> listPortID)
        {
            StringBuilder sql = new StringBuilder();
            foreach (var item in listPortID)
            {
                sql.AppendFormat("DELETE FROM {0} WHERE [{1}]={2}",
                CT_TableName,

                CN_PortNumber, item);
            }
            return base.ExecuteSQLCommand(sql.ToString());
        }




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
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(CDBManager.GetInstacne().GetConnectionString()))
                {
                    bulkCopy.DestinationTableName = CT_TableName;
                    bulkCopy.BatchSize = 1;
					bulkCopy.BulkCopyTimeout = 1800;
                    //bulkCopy.ColumnMappings.Add(CN_RainID, CN_RainID);
                    bulkCopy.ColumnMappings.Add(CN_PortNumber, CN_PortNumber);
                    bulkCopy.ColumnMappings.Add(CN_TransType, CN_TransType);
                    bulkCopy.ColumnMappings.Add(CN_Baudrate, CN_Baudrate);
                    bulkCopy.ColumnMappings.Add(CN_Databit, CN_Databit);
                    bulkCopy.ColumnMappings.Add(CN_Stopbit, CN_Stopbit);
                    bulkCopy.ColumnMappings.Add(CN_Parity, CN_Parity);
                    bulkCopy.ColumnMappings.Add(CN_Stream, CN_Stream);
                    bulkCopy.ColumnMappings.Add(CN_Break, CN_Break);
                    bulkCopy.ColumnMappings.Add(CN_Open, CN_Open);
                    try
                    {
                        bulkCopy.WriteToServer(tmp);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                m_mutexWriteToDB.ReleaseMutex();
                return false;
            }
            Debug.WriteLine("###{0} :add {1} lines to serial port db", DateTime.Now, tmp.Rows.Count);
            m_mutexWriteToDB.ReleaseMutex();
            return true;
        }
        /// <summary>
        /// 添加分中心
        /// </summary>
        public void Add(CEntitySerialPort entity)
        {
            String sqlStr = GetInsertSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetInsertSQL(CEntitySerialPort entity)
        {
            String strPortNum = entity.PortNumber.ToString();
            String strTransType = CEnumHelper.SerialTransTypeToDBStr(entity.TransType);
            String strBaudrate = entity.Baudrate.ToString();
            String strDataBit = entity.DataBit.ToString();
            String strStopBit = entity.StopBit.ToString();
            String strParityType = CEnumHelper.PortParityTypeToDBChar(entity.ParityType).ToString();
            String strStream = CEnumHelper.SerialPortStreamTypeToDBStr(entity.Stream);
            String strBreak = "NULL";
            if (entity.Break.HasValue)
            {
                strBreak = entity.Break.Value.ToString();
                //if (entity.Break.Value)
                //    strBreak = "1";
                //else
                //    strBreak = "0";
            }

            String strOpen = "NULL";
            if (entity.SwitchSatus.HasValue)
            {
                strBreak = entity.SwitchSatus.Value.ToString();
                //if (entity.SwitchSatus.Value)
                //    strOpen = "1";
                //else
                //    strOpen = "0";
            }

            return String.Format(
                        "INSERT INTO {0} ([{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}],[{9}]) VALUES ({10},{11},{12},{13},{14},{15},{16},{17},{18})",
                        CT_TableName,

                        CN_PortNumber, CN_TransType, CN_Baudrate,
                        CN_Databit, CN_Stopbit, CN_Parity,
                        CN_Stream, CN_Break, CN_Open,

                        strPortNum, strTransType, strBaudrate,
                        strDataBit, strStopBit, strParityType,
                        strStream, strBreak, strOpen
            );
        }

        /// <summary>
        /// 删除分中心
        /// </summary>
        public void Delete(CEntitySerialPort entity)
        {
            String sqlStr = GetDeleteSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetDeleteSQL(CEntitySerialPort entity)
        {
            return String.Format("DELETE FROM {0} WHERE [{1}]={2}",
                CT_TableName,

                CN_PortNumber, entity.PortNumber
            );
        }

        /// <summary>
        /// 修改分中心信息
        /// </summary>
        public void Update(CEntitySerialPort entity)
        {
            String sqlStr = GetUpdateSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetUpdateSQL(CEntitySerialPort entity)
        {
            String strPortNum = entity.PortNumber.ToString();
            String strTransType = CEnumHelper.SerialTransTypeToDBStr(entity.TransType);
            String strBaudrate = entity.Baudrate.ToString();
            String strDataBit = entity.DataBit.ToString();
            String strStopBit = entity.StopBit.ToString();
            String strParityType = CEnumHelper.PortParityTypeToDBChar(entity.ParityType).ToString();
            String strStream = CEnumHelper.SerialPortStreamTypeToDBStr(entity.Stream);
            String strBreak = "NULL";
            if (entity.Break.HasValue)
            {
                strBreak = entity.Break.Value.ToString();
                //if (entity.Break.Value)
                //    strBreak = "1";
                //else
                //    strBreak = "0";
            }

            String strOpen = "NULL";
            if (entity.SwitchSatus.HasValue)
            {
                strOpen = entity.SwitchSatus.Value.ToString();
                //if (entity.SwitchSatus.Value)
                //    strOpen = "1";
                //else
                //    strOpen = "0";
            }

            return String.Format(
                    "UPDATE {0} SET [{1}]={2},[{3}]={4},[{5}]={6},[{7}]={8},[{9}]={10},[{11}]={12},[{13}]={14},[{15}]={16} WHERE [{17}]={18}",
                    CT_TableName,

                    CN_TransType, strTransType,
                    CN_Baudrate, strBaudrate,
                    CN_Databit, strDataBit,
                    CN_Stopbit, strStopBit,
                    CN_Parity, strParityType,
                    CN_Stream, strStream,
                    CN_Break, strBreak,
                    CN_Open, strOpen,

                    CN_PortNumber, strPortNum
                );
        }

        /// <summary>
        /// 获取所有分中心信息
        /// </summary>
        public List<CEntitySerialPort> QueryAll()
        {
            var result = new List<CEntitySerialPort>();
            var sqlConn = CDBManager.GetInstacne().GetConnection();
            try
            {
                m_mutexWriteToDB.WaitOne();         // 取对数据库的唯一访问权
                m_mutexDataTable.WaitOne();         // 获取内存表的访问权
                sqlConn.Open();                     // 建立数据库连接

                String sqlStr = GetQuerySQL();

                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConn);

                SqlDataReader reader = sqlCmd.ExecuteReader();

                Debug.Assert(reader.FieldCount == CN_FIELD_COUNT, CT_TableName + "表与类" + CT_EntityName + "中定义字段不符合");

                //  处理查询结果
                while (reader.Read())
                {
                    try
                    {
                        var port = new CEntitySerialPort();

                        port.PortNumber = (Int32)reader[CN_PortNumber];
                        port.TransType = CEnumHelper.DBStrToSerialTransType((String)reader[CN_TransType]);
                        port.Baudrate = (Int32)reader[CN_Baudrate];
                        port.DataBit = (Int32)reader[CN_Databit];
                        port.StopBit = (Int32)reader[CN_Stopbit];
                        port.ParityType = CEnumHelper.DBCharToPortParityType(((String)reader[CN_Parity])[0]);
                        port.Stream = CEnumHelper.DBStrToSerialPortStreamType(reader[CN_Stream].ToString());
                        port.Break = (Boolean)reader[CN_Break];
                        port.SwitchSatus = (Boolean)reader[CN_Open];
                        result.Add(port);
                    }
                    catch(Exception exp)
                    {
                        Debug.WriteLine(exp.Message);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                sqlConn.Close();                    //  关闭数据库连接
                m_mutexDataTable.ReleaseMutex();    //  释放内存表的访问权
                m_mutexWriteToDB.ReleaseMutex();    //  释放数据库的访问权
            }
            return result;
        }
        private String GetQuerySQL()
        {
            return String.Format(
                "SELECT DISTINCT [{0}],[{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}] FROM {9}",
                CN_PortNumber, CN_TransType, CN_Baudrate, CN_Databit,
                CN_Stopbit, CN_Parity, CN_Stream, CN_Break, CN_Open,

                CT_TableName
            );
        }
        #endregion ///< HELP_METHOD
    }
}
