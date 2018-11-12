using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Hydrology.DBManager.DB.SQLServer
{
    /// <summary>
    /// 分区管理类，对雨量表，水量表以及电压表进行分表管理
    /// 每张表，都已经建好了索引，只需要维护分区函数的边界值
    /// 而且，只会对未来的数据进行判断，并不会对以前没有分区的数据进行分区和管理
    /// </summary>
    public class CSQLPartitionMgr
    {
        #region 静态常量

        public static readonly string CS_PF_Rain = "PF_DataTime_Rain";    //雨量表的分区函数名字
        public static readonly string CS_PS_Rain = "PS_DataTime_Rain";    //雨量表的分区模式名字

        public static readonly string CS_PF_Water = "PF_DataTime_Water";  //水量表的分区函数名字
        public static readonly string CS_PS_Water = "PS_DataTime_Water";  //水量表的分区模式名字

        public static readonly string CS_PF_Voltage = "PF_DataTime_Voltage";  //电压表的分区函数名字
        public static readonly string CS_PS_Voltage = "PS_DataTime_Voltage";  //电压表的分区模式名字

        private readonly string CONFIG_PATH = "Config/DBPartitionConfig.xml";

        #endregion ///<静态常量

        #region 成员变量

        private TimeSpan m_timeSpanRain;      //雨量表分表的时间间隔
        private TimeSpan m_timeSpanWater;     //水量表分表的时间间隔
        private TimeSpan m_timeSpanVoltage;   //电压表分表的时间间隔

        //上一个雨量记录点的时间，如果分区边界值没有的话，就选择雨量表中的日期最小值
        //如果边界值存在，就选择边界值中的最大值
        private Nullable<DateTime> m_timePrePointRain;
        private Nullable<DateTime> m_timePrePointWater;
        private Nullable<DateTime> m_timePrePointVoltage;

        // 互斥量
        private Mutex m_mutexMatainRain;
        private Mutex m_mutexMatainWater;
        private Mutex m_mutexMatainVoltage;

        #endregion ///<成员变量

        #region 单件模式
        public static CSQLPartitionMgr m_instance;
        public static CSQLPartitionMgr Instance
        {
            get
            {
                if (null == m_instance)
                {
                    m_instance = new CSQLPartitionMgr();
                }
                return m_instance;
            }
        }
        private CSQLPartitionMgr()
        {
            //  默认分表结构
            m_timeSpanRain = new TimeSpan(180, 0, 0, 0, 0);     //180天,6个月
            m_timeSpanWater = new TimeSpan(180, 0, 0, 0, 0);    //180天，6个月
            m_timeSpanVoltage = new TimeSpan(180, 0, 0, 0, 0);  //180天，6个月

            m_timePrePointRain = null;
            m_timePrePointWater = null;
            m_timePrePointVoltage = null;

            m_mutexMatainRain = new Mutex();
            m_mutexMatainVoltage = new Mutex();
            m_mutexMatainWater = new Mutex();

            try
            {
                if (!File.Exists(CONFIG_PATH))
                {
                    WriteToXML();
                }
                ReadFromXML();
            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
        }
        #endregion ///<单件模式

        #region 公共方法

        public void MaintainRain(DateTime newTime)
        {
            // 根据最新的时间，尝试添加新的分区表
            m_mutexMatainRain.WaitOne();
            if (!m_timePrePointRain.HasValue)
            {
                GetPrePointRain();
            }
            if (!m_timePrePointRain.HasValue)
            {
                // 数据表没有数据项
                m_mutexMatainRain.ReleaseMutex();
                return;
            }
            // 判断新的时间与上个记录点的时间是否已经超过了设定的时间差
            TimeSpan tmp = newTime - m_timePrePointRain.Value;
            if (tmp > m_timeSpanRain)
            {
                // 如果超过了时间标尺，新建一个分区
                string sql = GetNewPartitionSQL(CS_PF_Rain, CS_PS_Rain, newTime);
                // 获取更改雨量表的互斥量
                CDBMutex.Mutex_TB_Rain.WaitOne();
                ExecuteNonQuery(sql); //直接执行
                CDBMutex.Mutex_TB_Rain.ReleaseMutex();
                m_timePrePointRain = newTime; //更新时间点
            }
            m_mutexMatainRain.ReleaseMutex();
        }

        public void MaintainWater(DateTime newTime)
        {
            // 根据最新的时间，尝试添加新的分区表
            m_mutexMatainWater.WaitOne();
            if (!m_timePrePointWater.HasValue)
            {
                GetPrePointWater();
            }
            if (!m_timePrePointWater.HasValue)
            {
                // 数据表没有数据项
                m_mutexMatainWater.ReleaseMutex();
                return;
            }
            // 判断新的时间与上个记录点的时间是否已经超过了设定的时间差
            TimeSpan tmp = newTime - m_timePrePointWater.Value;
            if (tmp > m_timeSpanWater)
            {
                // 如果超过了时间标尺，新建一个分区
                string sql = GetNewPartitionSQL(CS_PF_Water, CS_PS_Water, newTime);
                CDBMutex.Mutex_TB_Water.WaitOne();
                ExecuteNonQuery(sql); //直接执行
                CDBMutex.Mutex_TB_Water.ReleaseMutex();
                m_timePrePointWater = newTime; //更新时间点
            }
            m_mutexMatainWater.ReleaseMutex();
        }

        public void MaintainVoltage(DateTime newTime)
        {
            // 根据最新的时间，尝试添加新的分区表
            m_mutexMatainVoltage.WaitOne();
            if (!m_timePrePointVoltage.HasValue)
            {
                GetPrePointVoltage();
            }
            if (!m_timePrePointVoltage.HasValue)
            {
                // 数据表没有数据项
                m_mutexMatainVoltage.ReleaseMutex();
                return;
            }
            // 判断新的时间与上个记录点的时间是否已经超过了设定的时间差
            TimeSpan tmp = newTime - m_timePrePointVoltage.Value;
            if (tmp > m_timeSpanVoltage)
            {
                // 如果超过了时间标尺，新建一个分区
                string sql = GetNewPartitionSQL(CS_PF_Voltage, CS_PS_Voltage, newTime);
                CDBMutex.Mutex_TB_Voltage.WaitOne();
                ExecuteNonQuery(sql); //直接执行
                CDBMutex.Mutex_TB_Voltage.ReleaseMutex();
                m_timePrePointVoltage = newTime; //更新时间点
            }
            m_mutexMatainVoltage.ReleaseMutex();
        }

        #endregion ///<公共方法

        #region 帮助方法

        private void GetPrePointRain()
        {
            // 先查阅分区参数
            string sql = GetQueryPartitonParamsSQL(CS_PF_Rain);
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count > 0)
            {
                m_timePrePointRain = DateTime.Parse(dataTableTmp.Rows[0][0].ToString());
            }
            else
            {
                // 从数据表中读取最小值
                DateTime tmp = DateTime.Now;
                if ((new CSQLRain()).GetMinDataTime(ref tmp))
                {
                    m_timePrePointRain = tmp;
                }
            }
        }

        private void GetPrePointWater()
        {
            // 先查阅分区参数
            string sql = GetQueryPartitonParamsSQL(CS_PF_Water);
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count > 0)
            {
                m_timePrePointWater = DateTime.Parse(dataTableTmp.Rows[0][0].ToString());
            }
            else
            {
                // 从数据表中读取最小值
                DateTime tmp = DateTime.Now;
                if ((new CSQLWater()).GetMinDataTime(ref tmp))
                {
                    m_timePrePointWater = tmp;
                }
            }
        }

        private void GetPrePointVoltage()
        {
            // 先查阅分区参数
            string sql = GetQueryPartitonParamsSQL(CS_PF_Voltage);
            SqlDataAdapter adapter = new SqlDataAdapter(sql, CDBManager.GetInstacne().GetConnection());
            DataTable dataTableTmp = new DataTable();
            adapter.Fill(dataTableTmp);
            if (dataTableTmp.Rows.Count > 0)
            {
                m_timePrePointVoltage = DateTime.Parse(dataTableTmp.Rows[0][0].ToString());
            }
            else
            {
                // 从数据表中读取最小值
                DateTime tmp = DateTime.Now;
                if ((new CSQLVoltage()).GetMinDataTime(ref tmp))
                {
                    m_timePrePointVoltage = tmp;
                }
            }
        }

        private string GetQueryPartitonParamsSQL(string partitionFunctionName)
        {
            // 根据分区函数，按照日期倒序查阅最近日期的分区参数，获得最近的分区边界值或者为空
            return string.Format("select top 1 a.value from sys.partition_range_values a, sys.partition_functions c where c.function_id=a.function_id and c.name='{0}' order by a.value desc;", partitionFunctionName);
        }

        private string GetNewPartitionSQL(string paritionFunctionName, string partitionScheme, DateTime newTime)
        {
            StringBuilder strBuilder = new StringBuilder();
            // 增加文件组
            strBuilder.AppendFormat("alter partition scheme {0} next used [primary];", partitionScheme);
            strBuilder.AppendFormat("alter partition function {0}() split range({1});", paritionFunctionName, DateTimeToDBStr(newTime));
            return strBuilder.ToString();
        }

        private void ExecuteNonQuery(String sqlText)
        {
            var sqlConn = CDBManager.GetInstacne().GetConnection();
            try
            {
                sqlConn.Open();                     // 建立数据库连接
                //  查询
                SqlCommand sqlCmd = new SqlCommand(sqlText, sqlConn);
                int lines = sqlCmd.ExecuteNonQuery();
                Debug.WriteLine("{0}行收到影响", lines);
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                sqlConn.Close();                    //  关闭数据库连接
            }
        }

        // 将时间转换成SQLSERVER中的时间字符串
        private string DateTimeToDBStr(DateTime time)
        {
            return " CAST(\'" + time.ToString(CDBParams.GetInstance().DBDateTimeFormat) + "\' as datetime) ";
        }

        private bool ReadFromXML()
        {
            try
            {
                // 从配置文件中分表时间信息
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(CONFIG_PATH);
                //取根结点
                var root = xmlDoc.DocumentElement;//取到根结点
                //  读取rain结点
                int rainDay = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/day").InnerText.ToString().Trim());
                int rainHour = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/hours").InnerText.ToString().Trim());
                int rainMinute = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/minutes").InnerText.ToString().Trim());
                int rainSecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/seconds").InnerText.ToString().Trim());
                int rainMillisecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/milliseconds").InnerText.ToString().Trim());

                //  读取water结点
                int waterDay = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Water/day").InnerText.ToString().Trim());
                int waterHour = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Water/hours").InnerText.ToString().Trim());
                int waterMinute = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Water/minutes").InnerText.ToString().Trim());
                int waterSecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Water/seconds").InnerText.ToString().Trim());
                int waterMillisecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Water/milliseconds").InnerText.ToString().Trim());
                //  读取电压结点
                int voltageDay = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Voltage/day").InnerText.ToString().Trim());
                int voltageHour = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Voltage/hours").InnerText.ToString().Trim());
                int voltageMinute = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Voltage/minutes").InnerText.ToString().Trim());
                int voltageSecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Voltage/seconds").InnerText.ToString().Trim());
                int voltageMillisecond = Int32.Parse(xmlDoc.SelectSingleNode("DBPartitionConfig/Voltage/milliseconds").InnerText.ToString().Trim());

                this.m_timeSpanRain = new TimeSpan(rainDay, rainHour, rainMinute, rainSecond, rainMillisecond);
                this.m_timeSpanWater = new TimeSpan(waterDay, waterHour, waterMinute, waterSecond, waterMillisecond);
                this.m_timeSpanVoltage = new TimeSpan(voltageDay, voltageHour, voltageMinute, voltageSecond, voltageMillisecond);

                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }
            return false;
        }

        private bool WriteToXML()
        {
            try
            {
                if (!Directory.Exists("Config"))
                {
                    // 创建文件夹
                    Directory.CreateDirectory("Config");
                }
                // 将用户名，密码以及数据源等信息写入XML文件
                XElement xElement =
                    new XElement("DBPartitionConfig",
                        new XElement("Rain",
                            new XElement("day", m_timeSpanRain.Days),
                            new XElement("hours", m_timeSpanRain.Hours),
                            new XElement("minutes", m_timeSpanRain.Minutes),
                            new XElement("seconds", m_timeSpanRain.Seconds),
                            new XElement("milliseconds", m_timeSpanRain.Milliseconds)
                            ),
                        new XElement("Water",
                            new XElement("day", m_timeSpanWater.Days),
                            new XElement("hours", m_timeSpanWater.Hours),
                            new XElement("minutes", m_timeSpanWater.Minutes),
                            new XElement("seconds", m_timeSpanWater.Seconds),
                            new XElement("milliseconds", m_timeSpanWater.Milliseconds)
                            ),
                        new XElement("Voltage",
                            new XElement("day", m_timeSpanVoltage.Days),
                            new XElement("hours", m_timeSpanVoltage.Hours),
                            new XElement("minutes", m_timeSpanVoltage.Minutes),
                            new XElement("seconds", m_timeSpanVoltage.Seconds),
                            new XElement("milliseconds", m_timeSpanVoltage.Milliseconds)
                            )
                    );
                //需要指定编码格式，否则在读取时会抛：根级别上的数据无效。 第 1 行 位置 1异常
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                XmlWriter xw = XmlWriter.Create(CONFIG_PATH, settings);
                xElement.Save(xw);
                //写入文件
                xw.Flush();
                xw.Close();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            
        }

        #endregion ///<帮助方法
    }

}
