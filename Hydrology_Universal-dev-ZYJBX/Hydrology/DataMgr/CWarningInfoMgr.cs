using System;
using Hydrology.Entity;
using System.Threading.Tasks;
using Hydrology.Forms;

namespace Hydrology.DataMgr
{
    /// <summary>
    /// 告警信息的管理类
    /// </summary>
    class CWarningInfoMgr
    {
        #region 事件定义
        /// <summary>
        /// 收到新的系统信息事件
        /// </summary>
        public event EventHandler<CEventSingleArgs<CTextInfo>> RecvedNewWarningInfo;

        #endregion 事件定义

        #region 成员变量

        #endregion 成员变量

        #region 单例模式
        private CWarningInfoMgr()
        {
        }
        private static CWarningInfoMgr m_instance;
        public static CWarningInfoMgr Instance
        {
            get
            {
                if (null == m_instance)
                {
                    m_instance = new CWarningInfoMgr();
                }
                return m_instance;
            }
        }
        #endregion 单例模式

        #region 公共方法
        public void AddInfo(string info, DateTime time)
        {
            // 异步写入数据库
            CDBDataMgr.Instance.GetWarningInfoProxy().AddNewRow(new Entity.CEntityWarningInfo() { DataTime = time, InfoDetail = info });
            Task.Factory.StartNew(() =>
            {
                if (null != RecvedNewWarningInfo)
                {
                    // 通知其它订阅者
                    RecvedNewWarningInfo.Invoke(this, new CEventSingleArgs<CTextInfo>(new CTextInfo() { Time = time, Info = info }));
                }
                //CVoicePlayer.Instance.Play();

            });
        }

        public void AddInfo(string info)
        {
            AddInfo(info, DateTime.Now);
        }

        /// <summary>
        /// 添加站点的告警信息，2014.5.17新增需求
        /// </summary>
        /// <param name="info"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <param name="StrStationID"></param>
        public void AddInfo(string info, DateTime time, EWarningInfoCodeType type, string strStationID)
        {
            // 异步写入数据库
            CDBDataMgr.Instance.GetWarningInfoProxy().AddNewRow(new Entity.CEntityWarningInfo()
            {
                DataTime = time,
                InfoDetail = info,
                StrStationId = strStationID,
                WarningInfoCodeType = type
            });
            Task.Factory.StartNew(() =>
            {
                if (null != RecvedNewWarningInfo)
                {
                    // 通知其它订阅者
                    RecvedNewWarningInfo.Invoke(this, new CEventSingleArgs<CTextInfo>(new CTextInfo() { Time = time, Info = info }));
                }
                //CVoicePlayer.Instance.Play();

            });
        }
        #endregion  公共方法

    }
}
