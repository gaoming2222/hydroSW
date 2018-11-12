using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Hydrology.Utils
{
    public class CExcel
    {
        private const int MAX_SHEET_ROWS_COUNT = 65535;
        private const int DEFAULT_SHEET_ROWS_COUNT = 50000; // must less than 65535
        private const string Prefix_SheetName = "Sheet";
        /// <summary>
        /// 将数据导出到指定的Excel文件中
        /// </summary>
        /// <param name="listView">System.Windows.Forms.ListView,指定要导出的数据源</param>
        /// <param name="destFileName">指定目标文件路径</param>
        /// <param name="tableName">要导出到的表名称</param>
        /// <param name="overWrite">指定是否覆盖已存在的表</param>
        /// <returns>导出的记录的行数</returns>
        public static bool DataTableToExcel(DataTable dt, string destFileName)
        {
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }
            //得到字段名
            string szFields = "";
            string szValues = "";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                szFields += "[" + dt.Columns[i] + "],";
            }
            szFields = szFields.TrimEnd(',');
            //定义数据连接
            OleDbConnection connection = new OleDbConnection();
            connection.ConnectionString = GetConnectionString(destFileName);
            OleDbCommand command = new OleDbCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            string defaultSheetName = "Sheet";
            //创建数据库表
            try
            {
                //打开数据库连接
                connection.Open();

                //循环处理数据------------------------------------------
                int rowCounts = dt.Rows.Count;

                int sheetsCount = rowCounts / DEFAULT_SHEET_ROWS_COUNT + 1;
                var sheets = new List<string>();
                for (int i = 0; i < sheetsCount; i++)
                {
                    sheets.Add(defaultSheetName + (i + 1).ToString());
                }
                //  创建表
                foreach (var item in sheets)
                {
                    System.Diagnostics.Debug.WriteLine("Create Table [" + item + "]");
                    command.CommandText = GetCreateTableSql("[" + item + "]", szFields.Split(','));
                    command.ExecuteNonQuery();
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int sheetIndex = i / DEFAULT_SHEET_ROWS_COUNT;

                    string sheetName = sheets[sheetIndex];

                    szValues = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        szValues += "'" + dt.Rows[i][j] + "',";
                    }
                    szValues = szValues.TrimEnd(',');
                    //组合成SQL语句并执行
                    string szSql = "INSERT INTO [" + sheetName + "](" + szFields + ") VALUES(" + szValues + ")";
                    command.CommandText = szSql;
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return false;
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
    }
}
