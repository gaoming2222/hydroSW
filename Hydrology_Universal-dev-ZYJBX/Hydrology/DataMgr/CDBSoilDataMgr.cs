using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using Hydrology.DBManager.Interface;
using Hydrology.DBManager.DB.SQLServer;
using Hydrology.Utils;
using Hydrology.DBManager;
using Hydrology.CControls;

namespace Hydrology.DataMgr
{
    /// <summary>
    /// 墒情相关数据管理,归纳多CBDInstance中管理
    /// </summary>
    class CDBSoilDataMgr
    {
        #region 事件定义
        /// <summary>
        /// 收到实时墒情数据的事件定义
        /// </summary>
        public event EventHandler<CEventSingleArgs<CEntitySoilData>> RecvedRTDSoilData;

        /// <summary>
        /// 收到清空实时数据墒情的数据的事件定义
        /// </summary>
        public event EventHandler RTDSoilDataClear;

        /// <summary>
        /// 站点基本信息更新消息
        /// </summary>
        public event EventHandler SoilStationUpdated;
        #endregion 事件定义

        #region 单件模式
        private static CDBSoilDataMgr m_sInstance;   //实例指针
        private CDBSoilDataMgr()
        {
            m_taskMgr = new CMultiTaskWrapper();
        }
        public static CDBSoilDataMgr Instance
        {
            get { return GetInstance(); }
        }
        public static CDBSoilDataMgr GetInstance()
        {
            if (m_sInstance == null)
            {
                m_sInstance = new CDBSoilDataMgr();
            }
            return m_sInstance;
        }
        #endregion ///<单件模式

        #region  成员变量
        private ISoilDataProxy m_proxySoilData;
        private ISoilStationProxy m_proxySoilStationProxy;

        /// <summary>
        /// 站点ID,和墒情信息的映射
        /// </summary>
        public Dictionary<string, CEntitySoilStation> m_mapStaionSoilInfo;

        /// <summary>
        /// 站点最新的墒情数据
        /// </summary>
        private Dictionary<string, CEntitySoilData> m_mapStataionLastData;

        /// <summary>
        /// 多任务管理器
        /// </summary>
        private CMultiTaskWrapper m_taskMgr;

        private ISubCenterProxy m_proxySubCenter;
        private List<CEntitySubCenter> m_listSubCenter; //所有分中心内存副本
        #endregion 成员变量

        #region 公共方法
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            m_proxySoilData = new CSQLSoilData();
            m_proxySoilStationProxy = new CSQLSoilStation();
            m_proxySubCenter = new CSQLSubCenter();
            m_listSubCenter = m_proxySubCenter.QueryAll();
            m_mapStataionLastData = new Dictionary<string, CEntitySoilData>();
            // 站点信息变更消息
            CDBDataMgr.Instance.StationUpdated += new EventHandler(this.EHStationChanged);
            CDBSoilDataMgr.Instance.SoilStationUpdated += new EventHandler(TreeMenuReload);
            // 初始化站点，建立集合映射
            InitSoilStation();

            // 读取实时文件
            ReadSoilXML();

            InitSoilStationLastData();

            return true;
        }
        public void UpdateAllSoilStation()
        {
            if (SoilStationUpdated != null)
            {
                SoilStationUpdated.Invoke(this, new EventArgs());
            }
        }
        private void TreeMenuReload(object sender, EventArgs e)
        {
            //  更新树形控件
            CTree.Instance.LoadTree();

            //  更新墒情站显示逻辑
            // UpdateMenuState();
        }
        public ISoilDataProxy GetSoilDataProxy()
        {
            return m_proxySoilData;
        }

        public ISoilStationProxy GetSoilStationProxy()
        {
            return m_proxySoilStationProxy;
        }

        /// <summary>
        /// 发消息，通知界面
        /// </summary>
        public void SendSoilDataMsg()
        {
            if (null != RecvedRTDSoilData && null != m_mapStataionLastData)
            {
                foreach (KeyValuePair<string, CEntitySoilData> pair in m_mapStataionLastData)
                {
                    RecvedRTDSoilData.Invoke(this, new CEventSingleArgs<CEntitySoilData>(pair.Value));
                }
            }
        }

        public void RecvData(object sender, CEventSingleArgs<CEntitySoilData> e)
        {
            if (RecvedRTDSoilData != null)
            {
                RecvedRTDSoilData.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 根据站点ID,获取站点的墒情数据，如果没有墒情数据，返回NULL
        /// </summary>
        /// <returns></returns>
        public CEntitySoilStation GetSoilStationInfoByStationId(string strStationId)
        {
            if (m_mapStaionSoilInfo.ContainsKey(strStationId))
            {
                return m_mapStaionSoilInfo[strStationId];
            }
            return null;
        }

        public CEntitySubCenter GetSubCenterBySoilId(int subcenterID)
        {
            foreach (CEntitySubCenter subcenter in m_listSubCenter)
            {
                if (subcenterID == subcenter.SubCenterID)
                {
                    return subcenter;
                }
            }
            return null; //没找到匹配，返回空
        }

        /// <summary>
        /// 返回所有的墒情站点信息
        /// </summary>
        /// <returns></returns>
        public List<CEntitySoilStation> GetAllSoilStation()
        {
            List<CEntitySoilStation> result = new List<CEntitySoilStation>();
            foreach (KeyValuePair<string, CEntitySoilStation> pair in m_mapStaionSoilInfo)
            {
                result.Add(pair.Value);
            }
            return result;
        }

        //public List<CEntitySoilStation> GetAllSoilStationData()
        //{
        //    List<CEntitySoilStation> result = new List<CEntitySoilStation>();
        //    result = m_proxySoilStationProxy.QueryAllSoilStation();
        //    return result;
        //}

        public List<CEntitySoilStation> GetAllSoilStationData()
        {
            List<CEntitySoilStation> result = new List<CEntitySoilStation>();
            result = m_proxySoilStationProxy.QueryAllSoilStation();
            m_mapStaionSoilInfo.Clear();
            for (int i = 0; i < result.Count; ++i)
            {
                m_mapStaionSoilInfo.Add(result[i].StationID, result[i]);
            }
            return result;
        }


        public CEntityStation GetStationInfoBySoilDataInfo(CEntitySoilData soil)
        {
            if (null == soil || String.IsNullOrEmpty(soil.StrDeviceNumber))
                return null;
            return GetStationInfoByDeviceNum(soil.StrDeviceNumber);
        }

        private CEntityStation GetStationInfoByDeviceNum(string deviceNum)
        {
            CEntityStation result = null;

            if (String.IsNullOrEmpty(deviceNum))
            {
                return result;
            }

            var soilStationLists = GetAllSoilStation();
            CEntitySoilStation soilStation = null;
            //foreach (var item in soilStationLists)
            //{
            //    if (deviceNum == item.StrDeviceNumber)
            //    {
            //        soilStation = item;
            //        break;
            //    }
            //}

            if (null != soilStation && !String.IsNullOrEmpty(soilStation.StationID))
            {
                result = CDBDataMgr.Instance.GetStationById(soilStation.StationID);
            }

            return result;
        }

        /// <summary>
        /// 收到墒情数据，从通信协议中收到,多线程处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void EHRecvSoildData(object sender, CSingleSoilDataArgs args)
        {
            m_taskMgr.NewTask(() => { DealSoilData(args); });
        }

        /// <summary>
        /// 重新加载数据库
        /// </summary>
        public void ReloadDatabase()
        {
            m_proxySoilData = new CSQLSoilData();
            m_proxySoilStationProxy = new CSQLSoilStation();

            // 初始化站点，建立集合映射
            InitSoilStation();

            // 先清空墒情站的以前数据
            if (RTDSoilDataClear != null)
            {
                RTDSoilDataClear.Invoke(this, new EventArgs());
            }

            // 通知界面,查询结果
            SendSoilDataMsg();
        }

        /// <summary>
        /// 重新加载墒情站点数据
        /// </summary>
        public void ReloadSoilStation()
        {
            InitSoilStation();
            //if (StationUpdated != null)
            //{
            //    StationUpdated.Invoke(this, new EventArgs());
            //}
        }

        /// <summary>
        /// 停止数据库服务,阻塞被调用线程
        /// </summary>
        public void StopDBService()
        {
            // 等待当前的任务完成
            m_taskMgr.Close();

            m_proxySoilData.Close();

            // 写入文件
            List<CEntitySoilData> listSoilData = new List<CEntitySoilData>();
            foreach (KeyValuePair<string, CEntitySoilData> pair in m_mapStataionLastData)
            {
                listSoilData.Add(pair.Value);
            }
            CXmlRTDSoilSerializer.Instance.Serialize(listSoilData);
        }
        #endregion 公共方法

        #region 帮助方法
        /// <summary>
        /// 初始化墒情站点，查询所有的墒情站点，并建立映射关系
        /// </summary>
        private void InitSoilStation()
        {
            m_mapStaionSoilInfo = new Dictionary<string, CEntitySoilStation>();
            List<CEntitySoilStation> listStation = m_proxySoilStationProxy.QueryAllSoilStation();
            for (int i = 0; i < listStation.Count; ++i)
            {
                m_mapStaionSoilInfo.Add(listStation[i].StationID, listStation[i]);
            }
        }

        /// <summary>
        /// 初始化墒情站最新实时数据
        /// </summary>
        private void InitSoilStationLastData()
        {
            List<CEntitySoilStation> _listSoilStation = GetAllSoilStation();
            foreach (CEntitySoilStation station in _listSoilStation)
            {
                if (m_mapStataionLastData.ContainsKey(station.StationID))
                {
                    // 如果已经从文件中读取出来的话， 就不必要查询数据库了
                    continue;
                }
                CEntitySoilData data = null;
                // 不就是浪费一点空间？小事儿呗
                m_proxySoilData.GetLastStationData(station.StationID, out data);
                if (null != data)
                {
                    m_mapStataionLastData.Add(station.StationID, data);
                }

            }
        }

        /// <summary>
        /// 读取实时数据表的文件
        /// </summary>
        private void ReadSoilXML()
        {
            // 读取XML,初始化实时数据表
            List<CEntitySoilData> listRTD = CXmlRTDSoilSerializer.Instance.Deserialize();
            if (null == listRTD)
            {
                return;
            }
            for (int i = 0; i < listRTD.Count; ++i)
            {
                // 如果墒情站有数据的话
                if (m_mapStaionSoilInfo.ContainsKey(listRTD[i].StationID))
                {
                    // 存入内存中
                    m_mapStataionLastData.Add(listRTD[i].StationID, listRTD[i]);
                }
                else
                {
                    // 位置站点，读取实时数据文件不匹配
                    CSystemInfoMgr.Instance.AddInfo(string.Format("实时墒情数据中站点\"{0}\"在数据库中匹配失败", listRTD[i].StationID));
                }
            }
            // 删除实时数据表
            CXmlRTDSoilSerializer.Instance.DeleteFile();
        }

        /// <summary>
        /// 计算函数率，根据电压值，如果只要一个为空，就返回null
        /// </summary>
        /// <param name="fVoltage"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        private Nullable<float> CalMoisture(Nullable<float> fVoltage, Nullable<decimal> a, Nullable<decimal> b, Nullable<decimal> c, Nullable<decimal> d)
        {
            //Nullable<float> result = null;
            //if (fVoltage.HasValue && a.HasValue && b.HasValue && c.HasValue && d.HasValue)
            //{
            //    // 当且仅当所有的数据都有值的时候，才计算含水率
            //    result = (float)((double)a * Math.Pow((double)fVoltage, 3) +
            //        (double)b * Math.Pow((double)fVoltage, 2) +
            //        (double)c * fVoltage +
            //        (double)d);
            //}
            if (!fVoltage.HasValue)
            {
                return null;
            }
            if (a.HasValue || b.HasValue || c.HasValue || d.HasValue)
            {
                double sum = 0;

                if (a.HasValue)
                {
                    sum += (double)a * Math.Pow((double)fVoltage.Value, 3);
                }
                if (b.HasValue)
                {
                    sum += (double)b * Math.Pow((double)fVoltage.Value, 2);
                }
                if (c.HasValue)
                {
                    sum += (double)c * fVoltage.Value;
                }
                if (d.HasValue)
                {
                    sum += (double)d;
                }
                return (float)sum;
            }
            return null;
        }

        private Nullable<float> CalMoisture(Nullable<float> fVoltage, Nullable<decimal> a, Nullable<decimal> b, Nullable<decimal> c, Nullable<decimal> d, Nullable<decimal> m, Nullable<decimal> n)
        {
            if (!fVoltage.HasValue)
            {
                return null;
            }
            double sum = 0;
            //if (a.HasValue || b.HasValue || c.HasValue || d.HasValue||m.HasValue || n.HasValue)
            //{
            //    double sum = 0;

            //    if (a.HasValue)
            //    {
            //        sum += (double)a * Math.Pow((double)fVoltage.Value, 3);
            //    }
            //    if (b.HasValue)
            //    {
            //        sum += (double)b * Math.Pow((double)fVoltage.Value, 2);
            //    }
            //    if (c.HasValue)
            //    {
            //        sum += (double)c * fVoltage.Value;
            //    }
            //    if (d.HasValue)
            //    {
            //        sum += (double)d;
            //    }

            //    if (m.HasValue && m != 0)
            //    {
            //        sum /= (double)m;
            //    }

            //    if (n.HasValue && n != 0)
            //    {
            //        sum += (double)n;
            //    }
            //    return (float)sum;
            //}
            if (m.HasValue || n.HasValue)
            {
                if (m.HasValue && m != 0)
                {
                    sum = (double)fVoltage / (float)m;
                }
                if (n.HasValue && n != 0)
                {
                    sum += (double)n;
                }
                sum = Math.Round(sum, 2);
                return (float)sum;
            }

            return null;

        }
        /// <summary>
        /// 处理数据的任务入口
        /// </summary>
        /// <param name="args"></param>
        private void DealSoilData(CSingleSoilDataArgs args)
        {
            CEntitySoilData data = new CEntitySoilData();
            // if (null == CDBDataMgr.Instance.GetStationById(args.StrStationId))
            if (null == GetSoilStationInfoByStationId(args.StrStationId))
            {
                // 也就是收到未知站点的数据
                CSystemInfoMgr.Instance.AddInfo(string.Format("收到未知站点{0}墒情数据", args.StrStationId));
                return;
            }
            // 判断当前数据库配置中是否有站点的墒情配置
            CEntitySoilStation _soilStation = GetSoilStationInfoByStationId(args.StrStationId);
            if (null == _soilStation)
            {
                CSystemInfoMgr.Instance.AddInfo(string.Format("站点{0}未配置墒情参数，数据丢弃", args.StrStationId));
                return;
            }
            data.StationID = args.StrStationId;
            //  data.StrDeviceNumber = _soilStation.StrDeviceNumber;
            data.DataTime = args.DataTime;
            data.DVoltage = decimal.Parse(args.Voltage.ToString());
            data.ChannelType = args.EChannelType;
            data.MessageType = args.EMessageType;

            data.reciveTime = DateTime.Now;
            data.state = 1;

            // 计算10cm出的含水率
            data.Voltage10 = args.D10Value;
            data.Moisture10 = CalMoisture(args.D10Value, _soilStation.A10, _soilStation.B10, _soilStation.C10, _soilStation.D10, _soilStation.M10, _soilStation.N10);
            //data.Moisture10 = CalMoisture(args.D10Value, _soilStation.A10, _soilStation.B10, _soilStation.C10, _soilStation.D10);

            // 计算20cm处的含水率
            data.Voltage20 = args.D20Value;
            data.Moisture20 = CalMoisture(args.D20Value, _soilStation.A20, _soilStation.B20, _soilStation.C20, _soilStation.D20, _soilStation.M20, _soilStation.N20);

            //// 计算30cm处的含水率
            //data.Voltage30 = args.D30Value;
            //data.Moisture30 = CalMoisture(args.D30Value, _soilStation.A30, _soilStation.B30, _soilStation.C30, _soilStation.D30);

            // 计算40cm处的含水率
            data.Voltage40 = args.D40Value;
            data.Moisture40 = CalMoisture(args.D40Value, _soilStation.A40, _soilStation.B40, _soilStation.C40, _soilStation.D40, _soilStation.M40, _soilStation.N40);

            //// 计算60cm处的含水率
            //data.Voltage60 = args.D60Value;
            //data.Moisture60 = CalMoisture(args.D60Value, _soilStation.A60, _soilStation.B60, _soilStation.C60, _soilStation.D60);

            // 写入数据库
            try
            {
                m_proxySoilData.AddNewRow(data);
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }

            if (RecvedRTDSoilData != null)
            {
                // 发消息通知界面
                RecvedRTDSoilData.Invoke(this, new CEventSingleArgs<CEntitySoilData>(data));
            }
            // 更新内存最新数据，便于写入文件
            if (m_mapStataionLastData.ContainsKey(args.StrStationId))
            {
                m_mapStataionLastData[args.StrStationId] = data;
            }
            else
            {
                // 新建一个，进行更新
                m_mapStataionLastData.Add(args.StrStationId, data);
            }
        }


        private void EHStationChanged(object sender, EventArgs e)
        {
            // 站点个数发生改变的消息
            // 清空所有站点
            //(m_lvStationStateAllPage.ListView as CListViewStationState).ClearAllStations();
            //// 删除原先的分中心站点
            //foreach (KeyValuePair<int, CListViewTabPage> item in m_mapSubCenterPage)
            //{
            //    (item.Value.ListView as CListViewStationState).ClearAllStations();
            //}

            //InitStationState(); //重新加载站点内容
        }
        #endregion 帮助方法
    }
}
