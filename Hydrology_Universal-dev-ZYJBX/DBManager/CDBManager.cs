using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace Hydrology.DBManager
{
    public class CDBManager
    {
        #region 成员变量
        private string m_strDataSource;
        private string m_strDBName;
        private string m_strUserName;
        private string m_strPassword;
        private int m_iConnectTimeout; //超时

        private string m_strConnection;
        private bool m_bInConnStrMode;
        /// <summary>
        /// 设置连接字符串的时候，记得要设定模式，否则设定无效
        /// </summary>
        public string ConnectionString
        {
            set { m_strConnection = value; }
        }
        /// <summary>
        /// 是否是自定义字符串模式，默认否
        /// </summary>
        public bool BInSelfDefineConnectionStrMode
        {
            get { return m_bInConnStrMode; }
            set { m_bInConnStrMode = value; }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource
        {
            get { return m_strDataSource; }
            set { m_strDataSource = value; }
        }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string DataBaseName
        {
            get { return m_strDBName; }
            set { m_strDBName = value; }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        //private string m_strConnection = "Data Source=127.0.0.1;Initial Catalog=HydrologyDB;User ID=admin;Password=123456"; //连接字符串
        #endregion ///<成员变量

        #region  单例模式

        private CDBManager()
        {
            m_bInConnStrMode = false;
            //m_strDataSource = "WANGZF-KYS";
            //m_strDBName = "HydrologyDB";
            //m_strUserName = "sa";
            //m_strPassword = "cjswkys";

            //m_strDataSource = @"WIN-P2POA24F489\SQLEXPRESS";
            //m_strDBName = "HydrologyDB";
            //m_strUserName = "sa";
            //m_strPassword = "123456";
            //m_iConnectTimeout = 30;//默认30秒超时

            m_strDataSource = @"";
            m_strDBName = "";
            m_strUserName = "";
            m_strPassword = "";
            m_iConnectTimeout = 30;//默认30秒超时
            
            try
            {
                // 没有的话 ，就用默认的配置
                ReadFromXML();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            
        }

        public static CDBManager GetInstacne()
        {
            if (null == m_instance)
            {
                m_instance = new CDBManager();
            }
            return m_instance;
        }
        public static CDBManager m_instance; //单例
        public static CDBManager Instance
        {
            get { return GetInstacne(); }
        }
        #endregion ///<单例模式

        // 获取连接, 需要手动关闭连接
        public SqlConnection GetConnection()
        {
            // SQL SERVER 2008
            // Data Source=127.0.0.1;Initial Catalog=HydrologyPureDB;Persist Security Info=True;User ID=admin;Password=123456;Connect Timeout=30
            string connstr = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Connect Timeout={4}",
                m_strDataSource, m_strDBName, 
                m_strUserName, m_strPassword,
                m_iConnectTimeout);
            return new SqlConnection(connstr);
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            string connstr = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Connect Timeout={4}",
                m_strDataSource, m_strDBName,
                m_strUserName, m_strPassword,
                m_iConnectTimeout);
            return connstr.ToString();
        }

        public string GetConnectionString_BS()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Config/DBConfig-BS.xml");
            XmlNode xn = doc.SelectSingleNode("DataBaseConfig");
            XmlNodeList xnl = xn.ChildNodes;
            string DataSource = xnl.Item(0).InnerText;
            string DatabaseName = xnl.Item(1).InnerText;
            string UserName = xnl.Item(2).InnerText;
            string Password = xnl.Item(3).InnerText;
            int Timeout = int.Parse(xnl.Item(4).InnerText);
            string connstr = string.Format("Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Connect Timeout={4}",
                DataSource, DatabaseName,
                UserName, Password,
                Timeout);
            return connstr.ToString();
        }

        // 测试连接
        public bool TryToConnection()
        {
            SqlConnection conn = GetConnection();
            try
            {
                conn.Open();
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return false;
            }
            finally
            {
                conn.Close();
                WriteToXML(); //总是写入到文件中
            }
        }

        #region 帮助方法
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
                    new XElement("DataBaseConfig",
                        new XElement("DatabaseSource", m_strDataSource),
                        new XElement("DatabaseName", m_strDBName),
                        new XElement("UserName", m_strUserName),
                        new XElement("Password", m_strPassword),
                        new XElement("Timeout", m_iConnectTimeout)
                    );
                //需要指定编码格式，否则在读取时会抛：根级别上的数据无效。 第 1 行 位置 1异常
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                XmlWriter xw = XmlWriter.Create("Config/DBConfig.xml", settings);
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
        private void ReadFromXML()
        {
            // 从配置文件中读取用户名和密码等信息
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Config/DBConfig.xml");
            //取根结点
            var root = xmlDoc.DocumentElement;//取到根结点
            //取指定的单个结点
            XmlNode singleChild = xmlDoc.SelectSingleNode("DataBaseConfig/DatabaseSource");
            m_strDataSource = singleChild.InnerText.ToString();

            singleChild = xmlDoc.SelectSingleNode("DataBaseConfig/DatabaseName");
            m_strDBName = singleChild.InnerText.ToString();

            singleChild = xmlDoc.SelectSingleNode("DataBaseConfig/UserName");
            m_strUserName = singleChild.InnerText.ToString();

            singleChild = xmlDoc.SelectSingleNode("DataBaseConfig/Password");
            m_strPassword = singleChild.InnerText.ToString();

            singleChild = xmlDoc.SelectSingleNode("DataBaseConfig/Timeout");
            m_iConnectTimeout = int.Parse(singleChild.InnerText.ToString());
        }
        #endregion

    }
}
