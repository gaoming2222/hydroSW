using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Hydrology.Utils
{
    /// <summary>
    /// 将datatable导出的excel的类
    /// </summary>
    public class CExcelExport
    {
        /// <summary>
        /// 将数据导出到指定的Excel文件中
        /// </summary>
        /// <param name="listView">System.Windows.Forms.ListView,指定要导出的数据源</param>
        /// <param name="destFileName">指定目标文件路径</param>
        /// <param name="tableName">要导出到的表名称</param>
        /// <param name="overWrite">指定是否覆盖已存在的表</param>
        /// <param name="startRow">不包括endRow的行数</param>
        /// <returns>导出的记录的行数</returns>
        private static bool ExportToExcel(System.Data.DataTable dt, string destFileName, string tableName,int startRow, int endRow)
        {
            try
            {
                //定义数据连接
                OleDbConnection connection = new OleDbConnection();
                connection.ConnectionString = GetConnectionString(destFileName);
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                //打开数据库连接
                connection.Open();
                //创建数据库表
                //command.CommandText = GetCreateTableSql("[" + tableName + "]", szFields.Split(','));
                command.CommandText = SqlCreate(dt, tableName);
                command.ExecuteNonQuery();

                //循环处理数据------------------------------------------
                StringBuilder tmpColumnName = new StringBuilder();
                StringBuilder tmpColumnsParams = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    tmpColumnName.Append(string.Format("[{0}]",dt.Columns[i].ColumnName));
                    tmpColumnsParams.Append("?");
                    if (i != (dt.Columns.Count - 1))
                    {
                        tmpColumnName.Append(",");
                        tmpColumnsParams.Append(",");
                    }
                }
                command.CommandText = string.Format("INSERT INTO [{0}]({1}) VALUES({2})", tableName, tmpColumnName, tmpColumnsParams);
                // 设定数据类型
                for (int i = 0; i < dt.Columns.Count; ++i)
                {
                    Type type = dt.Columns[i].DataType;
                    var par = command.Parameters.Add(dt.Columns[i].ColumnName
                        , StringToOleDbType(type)
                        , GetDataSize(type));
                    if (type.Name.Equals("Decimal") || type.Name.Equals("double"))
                    {
                        // 最大值
                        par.Precision = byte.MaxValue;
                        par.Scale = 6; //保留小数点后6位
                    }
                }
                // 开始逐步读取数据表中的每一行数据，并写入文件
                for (int i = startRow; i < endRow; i++)
                {
                    #region BEFORE
                    //szValues = "";
                    //for (int j = 0; j < dt.Columns.Count; j++)
                    //{
                    //    szValues += "'" + dt.Rows[i][j] + "',";
                    //}
                    //szValues = szValues.TrimEnd(',');
                    ////组合成SQL语句并执行
                    //string szSql = "INSERT INTO [" + tableName + "](" + szFields + ") VALUES(" + szValues + ")";
                    //command.CommandText = szSql;
                    //recordCount += command.ExecuteNonQuery();
                    #endregion BEFORE
                    for (int j = 0; j < dt.Columns.Count; ++j)
                    {
                        command.Parameters[j].Value = dt.Rows[i][j];
                    }
                    if (0 == i)
                    {
                        // 预编译
                        command.Prepare();
                    }
                    command.ExecuteNonQuery();
                }
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 考虑到分表的因素，单线程运行，只能等待，阻塞被调用线程
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="path"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool ExportToExcelWrapper(DataTable dt, string path, string tableName)
        {
            try
            {
                // 判断文件是否存在
                if (File.Exists(path))
                {
                    // 如果存在，则删除文件
                    File.Delete(path);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            bool result = true;
            if (dt.Rows.Count <= 65536)
            {
                // 可以写入一个表
                result = ExportToExcel(dt, path, tableName, 0, dt.Rows.Count);
            }
            else
            {
                int iStart = 0;
                int iEnd = 65535; //由于表头占了一行，所有刚好65535行
                int tableCount = 1;
                // 一旦写入某个表格失败，返回
                while (iStart < dt.Rows.Count && (iStart < iEnd) && result)
                {
                    // 写入多个表格
                    result = result && ExportToExcel(dt, path, string.Format("{0}_{1}", tableName, tableCount), iStart, iEnd);
                    iStart = iEnd;
                    tableCount = tableCount + 1;
                    iEnd = iEnd + 65535;
                    if (iEnd > dt.Rows.Count)
                    {
                        iEnd = dt.Rows.Count;
                    }
                } // end of while
            }
            return result;
        }

        //得到连接字符串
        private static String GetConnectionString(string fullPath)
        {
            string szConnection;
            szConnection = "Provider=Microsoft.JET.OLEDB.4.0;Extended Properties=Excel 8.0;data source=" + fullPath;
            return szConnection;
        }

        //得到创建表的SQL语句
        private static string GetCreateTableSql(string tableName, string[] fields)
        {
            string szSql = "CREATE TABLE " + tableName + "(";
            for (int i = 0; i < fields.Length; i++)
            {
                szSql += fields[i] + " VARCHAR(200),";
            }
            szSql = szSql.TrimEnd(',') + ")";
            return szSql;
        }

        private static OleDbType StringToOleDbType(Type i)
        {
            OleDbType s;
            switch (i.Name)
            {
                case "String":
                    s = OleDbType.Char;
                    break;
                case "Int32":
                    s = OleDbType.Integer;
                    break;
                case "Int64":
                    s = OleDbType.Integer;
                    break;
                case "Int16":
                    s = OleDbType.Integer;
                    break;
                case "Double":
                    s = OleDbType.Double;
                    break;
                case "Decimal":
                    s = OleDbType.Decimal;
                    break;
                default:
                    s = OleDbType.Char;
                    break;
            }
            return s;
        }

        private static string GetDataType(Type i)
        {
            string s;
            switch (i.Name)
            {
                case "String":
                    s = "Char";
                    break;
                case "Int32":
                    s = "Int";
                    break;
                case "Int64":
                    s = "Int";
                    break;
                case "Int16":
                    s = "Int";
                    break;
                case "Double":
                    s = "Double";
                    break;
                case "Decimal":
                    s = "Double";
                    break;
                default:
                    s = "Char";
                    break;
            }
            return s;
        }

        private static int GetDataSize(Type i)
        {
            int s = 100;
            switch (i.Name)
            {
                case "String":
                    s = 200; //最大200个字符
                    break;
                case "Int32":
                    s = 4;
                    break;
                case "Int64":
                    s = 8;
                    break;
                case "Int16":
                    s = 2;
                    break;
                case "Double":
                    s = 8;
                    break;
                case "Decimal":
                    s = 16;
                    break;
                case "Long":
                    s = 8;
                    break;
                default:
                    break;
            }
            return s;
        }

        private static string SqlCreate(DataTable dt, string SheetName)
        {
            string sql;
            sql = "CREATE TABLE " + SheetName + " (";
            foreach (DataColumn dc in dt.Columns)
            {
                sql += "[" + dc.ColumnName + "] " + GetDataType(dc.DataType) + " ,";
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")";
            return sql;
        }

    }
}
