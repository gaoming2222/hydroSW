using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Hydrology.Entity;
using Hydrology.DataMgr;

namespace Hydrology.Utils
{
    /// <summary>
    /// 实时数据序列化类
    /// </summary>
    public class CXmlRealTimeDataSerializer
    {
        #region 单件模式
        private static CXmlRealTimeDataSerializer m_sInstance;   //实例指针
        private CXmlRealTimeDataSerializer()
        {

        }
        public static CXmlRealTimeDataSerializer Instance
        {
            get { return GetInstance(); }
        }
        public static CXmlRealTimeDataSerializer GetInstance()
        {
            if (m_sInstance == null)
            {
                m_sInstance = new CXmlRealTimeDataSerializer();
            }
            return m_sInstance;
        }
        #endregion ///<单件模式

        private string m_path = "Config/RealTime.xml";

        /// <summary>
        /// 序列化实时数据xml
        /// </summary>
        public void Serialize(List<CEntityRealTime> lists)
        {
            try
            {
                //return;
                // 判断Config文件夹是否存在
                if (!Directory.Exists("Config"))
                {
                    // 创建文件夹
                    Directory.CreateDirectory("Config");
                }
                var infos = new CEntityRealTimeCollection()
                {
                    Items = lists
                };
                using (Stream fileStream = new FileStream(m_path, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityRealTimeCollection));
                    serializer.Serialize(fileStream, infos);
                }
                CSystemInfoMgr.Instance.AddInfo(string.Format("写入实时数据表完成, 文件名：\"{0}\"",m_path));
            }
            catch (Exception exp) 
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("写入实时数据表异常, 文件名：\"{0}\"\r\n{1}", m_path,exp.ToString()));
            }
        }
        /// <summary>
        /// 反序列化实时数据xml
        /// </summary>
        public List<CEntityRealTime> Deserialize()
        {
            try
            {
                //return new List<CEntityRealTime>();
                CEntityRealTimeCollection result = null;
                using (Stream fileStream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityRealTimeCollection));
                    result = (CEntityRealTimeCollection)serializer.Deserialize(fileStream);
                }
                if (result != null)
                {
                    CSystemInfoMgr.Instance.AddInfo(string.Format("读取实时数据表完成, 文件名：\"{0}\"", m_path));
                    return result.Items;
                }
            }
            catch (Exception exp) 
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("读取实时数据表异常, 文件名：\"{0}\"\r\n{1}", m_path,exp.ToString()));
            }
            return null;
        }

        /// <summary>
        /// 删除实时数据文件，如果遇到异常情况退出，删除，重新加载
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                File.Delete(m_path);
                CSystemInfoMgr.Instance.AddInfo(string.Format("删除实时数据表文件\"{0}\"",m_path), false);
            }
            catch (System.Exception ex)
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("删除实时数据表文件\"{0}\"异常\r\n{1}", m_path, ex.ToString()), false);
            }
        }
    }
}
