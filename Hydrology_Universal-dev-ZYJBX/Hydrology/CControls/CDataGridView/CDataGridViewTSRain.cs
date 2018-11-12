using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DBManager;
using Hydrology.DBManager.DB.SQLServer;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using System.Data;
using Hydrology.Utils;
using Hydrology.Forms;
using Hydrology.DataMgr;
namespace Hydrology.CControls
{
    class CDataGridViewTSRain : CExDataGridView
    {
        #region 静态常量
        public static readonly string CS_StationID = "站号";
        public static readonly string CS_StationName = "站名";
        public static readonly string CS_TotalRain = "累计雨量";
        public static readonly string CS_TimeCollected = "采集时间";
        public static readonly string CS_TimeReceived = "接收时间";
        public static readonly string CS_MsgType = "报文类型";
        public static readonly string CS_ChannelType = "通讯方式";
        public static readonly string CS_TimeFormat = "yyy-MM-dd HH:mm:ss";
        #endregion  ///<STATIC_STRING

        // 查询相关信息
        private ITSRainProxy m_proxyTSRain; //雨量表的操作接口
        private string m_strStaionId;    //查询的车站ID
        private DateTime m_dateTimeStart;   //查询的起点日期
        private DateTime m_dateTimeEnd;   //查询的起点日期

        public CDataGridViewTSRain()
            : base()
        {
            m_proxyTSRain = new CDBSQLTSRain();
            this.Header = new string[]
           {
                CS_StationID,CS_StationName,CS_TimeCollected, CS_TotalRain,CS_TimeReceived, CS_ChannelType, CS_MsgType
           };

            // 设置一页的数量
            this.PageRowCount = CDBParams.GetInstance().UIPageRowCount;
        }
        public void InitDataSource(ITSRainProxy proxy)
        {
            m_proxyTSRain = proxy;
        }
        public void SetRain(List<CEntityTSRain> listTSRain)
        {
            base.m_dataTable.Rows.Clear();
            // 判断状态值
            List<string[]> newRows = new List<string[]>();
            List<EDataState> states = new List<EDataState>();
            for (int i = 0; i < listTSRain.Count; ++i)
            {
                EDataState state;
                double rate = 0.5;
                state = GetState(rate);
                string strStationName = "";
                string strStationId = "";
                CEntityStation station = CDBDataMgr.Instance.GetStationById(listTSRain[i].StationID);
                if (null != station)
                {
                    strStationName = station.StationName;
                    strStationId = station.StationID;
                }
                string[] newRow = new string[]
                {
                        strStationId,
                        strStationName,
                        listTSRain[i].TimeCollect.ToString(CS_TimeFormat), /*采集时间*/
                        listTSRain[i].TotalRain.ToString(), /*累计雨量*/
                        listTSRain[i].TimeRecieved.ToString(CS_TimeFormat), /*接收时间*/
                        CEnumHelper.ChannelTypeToUIStr(listTSRain[i].ChannelType), /*通讯方式*/
                        CEnumHelper.MessageTypeToUIStr(listTSRain[i].MessageType) /*报文类型*/

                };
                newRows.Add(newRow);
                states.Add(state);
            }
            base.ClearAllRows();
            base.AddRowRange(newRows, states);
            base.UpdateDataToUI();
        }

        private EDataState GetState(double rate)
        {
            return EDataState.ENormal;
        }
        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            ClearAllState();
            m_strStaionId = iStationId;
            m_dateTimeStart = timeStart;
            m_dateTimeEnd = timeEnd;
            m_proxyTSRain.SetFilter(iStationId, timeStart, timeEnd, TimeSelect);
            if (false)
            {
                // 查询失败
                //MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                //return false;
            }
            else
            {
                // 并查询数据，显示第一页
                this.OnMenuFirstPage(this, null);
               // base.TotalPageCount = m_proxyTSRain.GetPageCount();
                //base.TotalRowCount = m_proxyRain.GetRowCount();
                SetRain(m_proxyTSRain.GetPageData(1));
                return true;
            }
        }
    }
}
