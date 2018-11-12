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
    public class CSQLSubCenter : CSQLBase, ISubCenterProxy
    {
        #region STATIC_MEMBER
        private const string CT_EntityName = "CEntitySubCenter";   //  数据库表SubCenter实体类
        private static readonly string CT_TableName = "SubCenter"; //数据库分中心表的名字
        private static readonly string CN_SubCenterID = "SubCenterID"; //分中心记录的唯一ID
        private static readonly string CN_SubCenterName = "SubCenterName"; //分中心的名字
        private static readonly string CN_Comment = "Comment";   //备注信息
        private const int CN_FIELD_COUNT = 3;
        #endregion

        #region PRIVATE_DATAMEMBER

        private List<int> m_listDelRows;            // 删除分中心记录的链表
        private List<CEntitySubCenter> m_listUpdateRows; // 更新分中心的链表

        #endregion ///<PRIVATE_DATAMEMBER

        #region 公共方法

        public CSQLSubCenter()
            : base()
        {
            m_listDelRows = new List<int>();
            m_listUpdateRows = new List<CEntitySubCenter>();
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_SubCenterID);
            m_tableDataAdded.Columns.Add(CN_SubCenterName);
            m_tableDataAdded.Columns.Add(CN_Comment);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_SubCenter;
        }

        // 添加新列
        public void AddNewRow(CEntitySubCenter entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_SubCenterName] = entity.SubCenterName;
            row[CN_Comment] = entity.Comment;
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

        #region HELP_METHOD
        // 将当前所有数据写入数据库

        /// <summary>
        /// 批量添加分中心
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool AddRange(List<CEntitySubCenter> items)
        {
            foreach (var item in items)
                AddNewRow(item);
            return AddDataToDB();
        }

        //gm2017_02
        //public bool AddSubRain(List<CEntitySubCenter> items)
        //{
        //    try
        //    {
        //        foreach (var item in items)
        //        {
        //            string name = item.SubCenterName;
        //        }
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        /// <summary>
        /// 更新分中心信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool UpdateRange(List<CEntitySubCenter> items)
        {
            if (items.Count <= 0)
            {
                return true;
            }
            // 除主键外和站点外，其余信息随意修改
            StringBuilder sql = new StringBuilder();
            foreach (var item in items)
            {
                String strID = item.SubCenterID.ToString();
                String strName = item.SubCenterName == null ? "null" : item.SubCenterName;
                String strComment = item.Comment == null ? "null" : item.Comment;

                sql.AppendFormat("UPDATE {0} SET [{1}]='{2}',[{3}]='{4}' WHERE [{5}]={6};",
                    CT_TableName,
                    CN_SubCenterName, strName,
                    CN_Comment, strComment,
                    CN_SubCenterID, strID);
            }
            // 更新数据库
            return base.ExecuteSQLCommand(sql.ToString());
        }

        /// <summary>
        /// 删除分中心
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool DeleteRange(List<int> items)
        {
            if (items.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            foreach (var item in items)
            {
                sql.AppendFormat("DELETE FROM {0} WHERE [{1}]={2};",
                CT_TableName,
                CN_SubCenterID, item);
            }
            return base.ExecuteSQLCommand(sql.ToString());
        }


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
                    bulkCopy.ColumnMappings.Add(CN_SubCenterName, CN_SubCenterName);
                    bulkCopy.ColumnMappings.Add(CN_Comment, CN_Comment);
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
        /// 添加分中心
        /// </summary>
        public void Add(CEntitySubCenter entity)
        {
            String sqlStr = GetInsertSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetInsertSQL(CEntitySubCenter entity)
        {
            String strID = entity.SubCenterID.ToString();
            String strName = entity.SubCenterName == null ? "null" : String.Format("'{0}'", entity.SubCenterName);
            String strComment = entity.Comment == null ? "null" : String.Format("'{0}'", entity.Comment);

            return String.Format(
                "INSERT INTO {0} ([{1}], [{2}]) VALUES ({3},{4})",
                CT_TableName,

                CN_SubCenterName, CN_Comment,

                strName, strComment
            );
        }
        /// <summary>
        /// 删除分中心
        /// </summary>
        public void Delete(CEntitySubCenter entity)
        {
            String sqlStr = GetDeleteSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetDeleteSQL(CEntitySubCenter entity)
        {
            return String.Format("DELETE FROM {0} WHERE [{1}]={2}",
                CT_TableName,
                CN_SubCenterID, entity.SubCenterID
            );
        }
        /// <summary>
        /// 修改分中心信息
        /// </summary>
        public void Update(CEntitySubCenter entity)
        {
            String sqlStr = GetUpdateSQL(entity);
            ExecuteNonQuery(sqlStr);
        }
        private String GetUpdateSQL(CEntitySubCenter entity)
        {
            String strID = entity.SubCenterID.ToString();
            String strName = entity.SubCenterName == null ? "null" : String.Format("'{0}'", entity.SubCenterName);
            String strComment = entity.Comment == null ? "null" : String.Format("'{0}'", entity.Comment);

            return String.Format("UPDATE {0} SET [{1}]={2},[{3}]={4} WHERE [{5}]={6}",
                CT_TableName,
                CN_SubCenterName, strName,
                CN_Comment, strComment,
                CN_SubCenterID, strID
            );
        }
        /// <summary>
        /// 获取所有分中心信息
        /// </summary>
        public List<CEntitySubCenter> QueryAll()
        {
            var result = new List<CEntitySubCenter>();
            var sqlConn = CDBManager.GetInstacne().GetConnection();
            try
            {
                m_mutexWriteToDB.WaitOne();         // 取对数据库的唯一访问权
                m_mutexDataTable.WaitOne();         // 获取内存表的访问权
                sqlConn.Open();                     // 建立数据库连接

                /**********异步查询数据库**********/
                String sqlStr = GetQueryAllSQL();

                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConn);


                SqlDataReader reader = sqlCmd.ExecuteReader();

                Debug.Assert(reader.FieldCount == CN_FIELD_COUNT, CT_TableName + "表与类" + CT_EntityName + "中定义字段不符合");

                //  处理查询结果
                while (reader.Read())
                {
                    try
                    {
                        var item = new CEntitySubCenter();

                        item.SubCenterID = (Int32)reader[CN_SubCenterID];

                        if (reader[CN_SubCenterName] is DBNull)
                            item.SubCenterName = null;
                        else
                            item.SubCenterName = (String)reader[CN_SubCenterName];

                        if (reader[CN_Comment] is DBNull)
                            item.Comment = null;
                        else
                            item.Comment = (String)reader[CN_Comment];

                        result.Add(item);
                    }
                    catch
                    {

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
        private String GetQueryAllSQL()
        {
            return String.Format("SELECT DISTINCT [{0}],[{1}],[{2}] FROM {3}",
                CN_SubCenterID, CN_SubCenterName, CN_Comment,

                CT_TableName
            );
        }
        #endregion ///< HELP_METHOD
    }
}
