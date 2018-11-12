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
    public class CSQLUser : CSQLBase, IUserProxy
    {
        #region STATIC_MEMBER
        private static readonly string CT_TableName = "[User]"; //数据库用户表的名字
        private static readonly string CN_UserID = "UID";    //用户的唯一ID
        private static readonly string CN_UserName = "Name";    //分中心记录的唯一ID
        private static readonly string CN_Password = "Password";     //密码，采用MD5进行加密
        private static readonly string CN_Administrator = "Administrator";   //是否是管理员
        #endregion

        #region PRIVATE_DATAMEMBER

        private List<int> m_listDelRows;            // 删除用户记录的链表
        private List<CEntityUser> m_listUpdateRows; // 更新用户信息的链表

        #endregion ///<PRIVATE_DATAMEMBER

        #region PUBLIC_METHOD

        public CSQLUser()
            : base()
        {
            m_listDelRows = new List<int>();
            m_listUpdateRows = new List<CEntityUser>();
            // 为数据表添加列
            m_tableDataAdded.Columns.Add(CN_UserName);
            m_tableDataAdded.Columns.Add(CN_Password);
            m_tableDataAdded.Columns.Add(CN_Administrator);

            // 初始化互斥量
            m_mutexWriteToDB = CDBMutex.Mutex_TB_User;
        }

        // 添加新列
        public void AddNewRow(CEntityUser entity)
        {
            // 记录超过1000条，或者时间超过1分钟，就将当前的数据写入数据库
            m_mutexDataTable.WaitOne(); //等待互斥量
            DataRow row = m_tableDataAdded.NewRow();
            row[CN_UserName] = entity.UserName;
            row[CN_Password] = entity.Password;
            row[CN_Administrator] = entity.Administrator;
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

        /// <summary>
        /// 获取所有分中心信息
        /// </summary>
        public List<CEntityUser> QueryAllUser()
        {
            var result = new List<CEntityUser>();
            var sqlConn = CDBManager.GetInstacne().GetConnection();
            try
            {
                m_mutexWriteToDB.WaitOne();         // 取对数据库的唯一访问权
                m_mutexDataTable.WaitOne();         // 获取内存表的访问权
                sqlConn.Open();                     // 建立数据库连接

                String sqlStr = "select * from " + CT_TableName;
                SqlCommand sqlCmd = new SqlCommand(sqlStr, sqlConn);
                SqlDataReader reader = sqlCmd.ExecuteReader();

                //  处理查询结果
                while (reader.Read())
                {
                    try
                    {
                        var user = new CEntityUser();
                        user.UserID = (Int32)reader[CN_UserID];
                        user.UserName = (String)reader[CN_UserName];
                        user.Password = (String)reader[CN_Password];
                        user.Administrator = (Boolean)reader[CN_Administrator];
                        result.Add(user);
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

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool AddUserRange(List<CEntityUser> listUser)
        {
            for (int i = 0; i < listUser.Count; ++i)
            {
                AddNewRow(listUser[i]);
            }
            return AddDataToDB();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        public bool UpdateUserRange(List<CEntityUser> listUser)
        {
            // 除主键外和站点外，其余信息随意修改
            if (listUser.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < listUser.Count; i++)
            {
                sql.AppendFormat("update {0} set {1}='{2}',{3}='{4}',{5}='{6}' where {7}={8};",
                    CT_TableName,
                    CN_UserName, listUser[i].UserName,
                    CN_Password, listUser[i].Password,
                    CN_Administrator,listUser[i].Administrator,
                    CN_UserID, listUser[i].UserID
                );
            }
            // 更新数据库
            return base.ExecuteSQLCommand(sql.ToString());
        }

        public bool DeleteUserRange(List<int> listUserID)
        {
            if (listUserID.Count <= 0)
            {
                return true;
            }
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < listUserID.Count; i++)
            {
                sql.AppendFormat("delete from {0} where {1} = {2};",
                    CT_TableName,
                    CN_UserID, listUserID[i]
                );
            }
            // 更新数据库
            return base.ExecuteSQLCommand(sql.ToString());
        }

        public bool UserLogin(string username, string password, ref bool bAdministrator)
        {
            // 超级管理员
            if (username.Equals("admin") && password.Equals("admin"))
            {
                bAdministrator = true;
                return true;
            }
            string sql = string.Format("select {0} from {1} where {2}='{3}' and {4}='{5}';",
                CN_Administrator,
                CT_TableName,
                CN_UserName, username,
                CN_Password, password);
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
                DataTable dataTableTmp = new DataTable();
                adapter.Fill(dataTableTmp);
                if (dataTableTmp.Rows.Count > 0)
                {
                    // 存在该用户，至于多个同名用户的话，应该作出限制
                    bAdministrator = Boolean.Parse(dataTableTmp.Rows[0][CN_Administrator].ToString());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return false;
        }

        #endregion ///<PUBLIC_METHOD

        #region HELP_METHOD
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
                    bulkCopy.ColumnMappings.Add(CN_UserName, CN_UserName);
                    bulkCopy.ColumnMappings.Add(CN_Password, CN_Password);
                    bulkCopy.ColumnMappings.Add(CN_Administrator, CN_Administrator);
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
        #endregion ///< HELP_METHOD


    }
}
