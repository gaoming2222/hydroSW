using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager
{
    public class CDBLog
    {
#region  事件定义
        public event EventHandler<CEventSingleArgs<CTextInfo>> NewDBSystemInfo;
#endregion 事件定义
        #region  单例模式

        private CDBLog()
        {
        }

        public static CDBLog GetInstacne()
        {
            if (null == m_instance)
            {
                m_instance = new CDBLog();
            }
            return m_instance;
        }
        public static CDBLog m_instance; //单例
        public static CDBLog Instance
        {
            get { return GetInstacne(); }
        }
        #endregion ///<单例模式

        public void AddInfo(string info)
        {
            CTextInfo textInfo = new CTextInfo();
            textInfo.Info = info;
            textInfo.Time = DateTime.Now;
            if (NewDBSystemInfo != null)
            {
                NewDBSystemInfo(this, new CEventSingleArgs<CTextInfo>(textInfo));
            }
        }
    }
}
