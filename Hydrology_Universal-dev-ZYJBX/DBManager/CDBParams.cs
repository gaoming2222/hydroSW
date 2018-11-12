using System.Xml;
using System;
using System.Xml.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Hydrology.DBManager
{
    /// <summary>
    /// 数据库参数配置类
    /// </summary>
    public class CDBParams
    {
        #region  PROPERTY
        public int AddToDbDelay { get { return m_iAddToDBDelay; } }
        public int DBPageRowCount { get { return m_iPageRowCount; } }
        public int AddBufferMax { get { return m_iAddBufferMax; } }
        public int UIPageRowCount { get { return m_iUIPageRowCount; } }
        public int UpdateBufferMax { get { return m_iUpdateBufferMax; } }
        public string DBDateTimeFormat { get { return m_strDBDataTimeFormat; } }
        #endregion

        private int m_iAddToDBDelay;    // 添加到数据库中的数据延迟时间，默认为一分钟,单位为ms
        private int m_iPageRowCount;    // 一次查询数据库查询行数，默认为1000
        private int m_iUIPageRowCount;  // 界面一次返回的行数，默认为100，默认m_iUIPageRowCount是PagerRowCount的因数
        private int m_iAddBufferMax;    // 添加到数据库的记录的最大缓存数，默认为1000,超过或等于1000条记录，写入数据库
        private int m_iUpdateBufferMax; // 更新与删除数据库记录的最大缓存数，默认是1000,超过或等于1000条记录，写入数据库
        private string m_strDBDataTimeFormat; // 数据库的时间格式

        private readonly string CONFIG_PATH = "Config/DBParamConfig.xml";

        private CDBParams()
        {
            // 读取配置文件
            m_iAddToDBDelay = 60 * 1000;
            m_iPageRowCount = 1000;
            m_iAddBufferMax = 1000;
            m_iUIPageRowCount = 1000;
            m_iUpdateBufferMax = 1000;
            m_strDBDataTimeFormat = "yyyy-MM-dd HH:mm:ss";

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
        private static CDBParams m_instance = null;
        public static CDBParams GetInstance()
        {
            if (null == m_instance)
            {
                m_instance = new CDBParams();
            }
            return m_instance;
        }

        #region 帮助方法
        public bool ReadFromXML()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(CONFIG_PATH);
                //取根结点
                var root = xmlDoc.DocumentElement;//取到根结点
                //取指定的单个结点
                //XmlNode singleChild = xmlDoc.SelectSingleNode("DBPartitionConfig/Rain/day");
                m_iAddToDBDelay = Int32.Parse(xmlDoc.SelectSingleNode("DBParamConfig/AddToDBDelay").InnerText.ToString().Trim());
                m_iPageRowCount = Int32.Parse(xmlDoc.SelectSingleNode("DBParamConfig/PageRowCount").InnerText.ToString().Trim());
                m_iAddBufferMax = Int32.Parse(xmlDoc.SelectSingleNode("DBParamConfig/AddBufferMax").InnerText.ToString().Trim());
                m_iUIPageRowCount = Int32.Parse(xmlDoc.SelectSingleNode("DBParamConfig/UIPageRowCount").InnerText.ToString().Trim());
                m_iUpdateBufferMax = Int32.Parse(xmlDoc.SelectSingleNode("DBParamConfig/UpdateBufferMax").InnerText.ToString().Trim());

                return true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
            }
            return false;
        }
        public bool WriteToXML()
        {
            try
            {
                if (!Directory.Exists("Config"))
                {
                    // 创建文件夹
                    Directory.CreateDirectory("Config");
                }
                XElement xElement =
                    new XElement("DBParamConfig",
                        new XElement("AddToDBDelay", m_iAddToDBDelay),
                        new XElement("PageRowCount", m_iPageRowCount),
                        new XElement("AddBufferMax", m_iAddBufferMax),
                        new XElement("UIPageRowCount", m_iUIPageRowCount),
                        new XElement("UpdateBufferMax", m_iUpdateBufferMax)
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
        #endregion
    }
}
