using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Hydrology.Entity;
using Hydrology.DataMgr;

namespace Hydrology.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class CXmlRTDSoilSerializer
    {
        #region 单件模式
        private static CXmlRTDSoilSerializer m_sInstance;   //实例指针
        private CXmlRTDSoilSerializer()
        {

        }
        public static CXmlRTDSoilSerializer Instance
        {
            get { return GetInstance(); }
        }
        public static CXmlRTDSoilSerializer GetInstance()
        {
            if (m_sInstance == null)
            {
                m_sInstance = new CXmlRTDSoilSerializer();
            }
            return m_sInstance;
        }
        #endregion ///<单件模式

        private string m_path = "Config/RealTimeSoilData.xml";

        /// <summary>
        /// 序列化实时数据xml
        /// </summary>
        public void Serialize(List<CEntitySoilData> lists)
        {
            try
            {
                return; //不写文件
                // 判断Config文件夹是否存在
#pragma warning disable CS0162 // 检测到无法访问的代码
                if (!Directory.Exists("Config"))
#pragma warning restore CS0162 // 检测到无法访问的代码
                {
                    // 创建文件夹
                    Directory.CreateDirectory("Config");
                }
                var infos = new CEntityRealTimeSoilCollection()
                {
                    Items = lists
                };
                using (Stream fileStream = new FileStream(m_path, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityRealTimeSoilCollection));
                    serializer.Serialize(fileStream, infos);
                }
                CSystemInfoMgr.Instance.AddInfo(string.Format("写入实时墒情数据表完成, 文件名：\"{0}\"",m_path));
            }
            catch (Exception exp) 
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("写入实时墒情表异常, 文件名：\"{0}\"\r\n{1}", m_path, exp.ToString()));
            }
        }
        /// <summary>
        /// 反序列化实时数据xml
        /// </summary>
        public List<CEntitySoilData> Deserialize()
        {
            try
            {
                return new List<CEntitySoilData>(); // 不读文件
#pragma warning disable CS0162 // 检测到无法访问的代码
                CEntityRealTimeSoilCollection result = null;
#pragma warning restore CS0162 // 检测到无法访问的代码
                using (Stream fileStream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CEntityRealTimeSoilCollection));
                    result = (CEntityRealTimeSoilCollection)serializer.Deserialize(fileStream);
                }
                if (result != null)
                {
                    CSystemInfoMgr.Instance.AddInfo(string.Format("读取实时墒情表完成, 文件名：\"{0}\"", m_path));
                    return result.Items;
                }
            }
            catch (Exception exp) 
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("读取实时墒情表异常, 文件名：\"{0}\"\r\n{1}", m_path, exp.ToString()));
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
                CSystemInfoMgr.Instance.AddInfo(string.Format("删除实时墒情表文件\"{0}\"", m_path), false);
            }
            catch (System.Exception ex)
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("删除实时墒情表文件\"{0}\"异常\r\n{1}", m_path, ex.ToString()), false);
            }
        }
    }
}
