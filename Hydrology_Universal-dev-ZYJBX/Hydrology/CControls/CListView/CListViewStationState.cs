using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrology.CControls
{
    class CListViewStationState : CExListView
    {
        private class SItem
        {
            public string strStationId { get; set; }
            public string strName { get; set; }
            public EStationState state { get; set; }
        }
        // 站点状态值，对应与不同的图片
        public enum EStationState { ENormal/*绿色*/, EWarning /*黄色*/ , EError/*红色*/};

        #region 成员变量
        private ToolStripMenuItem m_itemWarningFirst;
        private ToolStripMenuItem m_itemNormalFirst;
        private ToolStripMenuItem m_itemDefualt;
        private List<SItem> m_listAllStation;
        #endregion ///<成员变量
        public CListViewStationState()
            : base()
        {
            base.PartitionPageEnable = true; //开启分页模式
            //图片显示
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(20, 20);
            imgList.Images.Add(Hydrology.Properties.Resources.station_normal);
            imgList.Images.Add(Hydrology.Properties.Resources.station_waning);
            imgList.Images.Add(Hydrology.Properties.Resources.station_error);
            this.LargeImageList = imgList;
            this.View = View.LargeIcon;
            // 成员变量初始化
            m_listAllStation = new List<SItem>();
        }

        public void ClearAllStations()
        {
            //清除所有站点
            m_listAllStation.Clear();
            base.m_iCurrentPage = 1;
            base.m_iTotalPage = 1;
        }

        public void AddStation(string stationId, string name, EStationState state)
        {
            SItem item = new SItem();
            item.strName = name;
            item.state = state;
            item.strStationId = stationId;
            m_listAllStation.Add(item);
            RefreshVisibleItem(); //刷新显示的内容项
        }
        /// <summary>
        /// 添加站点并不更新界面
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public void AddStationWithoutNotify(string stationId, string name, EStationState state)
        {
            SItem item = new SItem();
            item.strName = name;
            item.state = state;
            item.strStationId = stationId;
            m_listAllStation.Add(item);
        }
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshUI()
        {
            RefreshVisibleItem();
        }

        /// <summary>
        /// 更新站点状态
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateStationState(string stationId, EStationState state)
        {
            try
            {
                if (this.IsHandleCreated)
                {
                    // 跨线程调用
                    this.Invoke(new Action(() =>
                    {
                        int index = FindStationIndexByID(stationId);
                        if (-1 != index)
                        {
                        //return false;
                        // 更新状态值
                        m_listAllStation[index].state = state;
                            int itemIndex = this.Items.IndexOfKey(stationId.ToString());
                            if (-1 != itemIndex)
                            {
                                this.Items[itemIndex].ImageIndex = (int)state;
                            }
                        }
                    }));
                }
                else
                {
                    int index = FindStationIndexByID(stationId);
                    if (-1 == index)
                    {
                        return false;
                    }
                    // 更新状态值
                    m_listAllStation[index].state = state;
                    int itemIndex = this.Items.IndexOfKey(stationId.ToString());
                    if (-1 != itemIndex)
                    {
                        this.Items[itemIndex].ImageIndex = (int)state;
                    }
                }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            }catch(Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                MessageBox.Show("点击位置被覆盖");
            }

            return true;
        }


        #region 重写

        protected override void InitContextMenu()
        {
            base.InitContextMenu();
            // 添加警告优先，正常优先，以及筛选器选项
            m_itemWarningFirst = new ToolStripMenuItem() { Text = "警告优先" };
            m_itemNormalFirst = new ToolStripMenuItem() { Text = "正常优先" };
            m_itemDefualt = new ToolStripMenuItem() { Text = "恢复默认" };

            ToolStripSeparator seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(seperator);
            m_contextMenu.Items.Add(m_itemWarningFirst);
            m_contextMenu.Items.Add(m_itemNormalFirst);
            m_contextMenu.Items.Add(m_itemDefualt);


            // 绑定消息
            m_itemWarningFirst.Click += EHWarningFirst;
            m_itemNormalFirst.Click += EHNormalFirst;
            m_itemDefualt.Click += new EventHandler(EHDefaultSort);
        }



        protected override void OnMenuNextPage(object sender, EventArgs e)
        {
            base.OnMenuNextPage(sender, e);
            RefreshVisibleItem();
        }
        protected override void OnMenuPreviousPage(object sender, EventArgs e)
        {
            base.OnMenuPreviousPage(sender, e);
            RefreshVisibleItem();
        }
        protected override void OnMenuFirstPage(object sender, EventArgs e)
        {
            base.OnMenuFirstPage(sender, e);
            RefreshVisibleItem();
        }

        protected override void OnMenuLastPage(object sender, EventArgs e)
        {
            base.OnMenuLastPage(sender, e);
            RefreshVisibleItem();
        }

        #endregion ///<OVER_WRITE

        #region EVENT_HANDLE
        private void EHNormalFirst(object sender, EventArgs e)
        {
            //正常优先
            m_itemNormalFirst.Checked = true;
            m_itemWarningFirst.Checked = false;
            m_itemDefualt.Checked = false;
            //// 排序
            m_listAllStation.Sort(CompareByState);
            RefreshVisibleItem();
        }

        private void EHWarningFirst(object sender, EventArgs e)
        {
            //警告优先
            m_itemNormalFirst.Checked = false;
            m_itemWarningFirst.Checked = true;
            m_itemDefualt.Checked = false;
            // 排序
            m_listAllStation.Sort(CompareByStateReverse);
            RefreshVisibleItem();
        }

        private void EHDefaultSort(object sender, EventArgs e)
        {
            //默认排序
            m_itemNormalFirst.Checked = false;
            m_itemWarningFirst.Checked = false;
            m_itemDefualt.Checked = true;
            // 排序
            m_listAllStation.Sort(CompareById);
            RefreshVisibleItem();
        }
        #endregion ///< EVENT_HANDLE

        #region 帮助方法
        // 刷新显示范围
        private void RefreshVisibleItem()
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    this.SuspendLayout();
                    this.Items.Clear();
                    int startIndex = 0;
                    base.TotalPage = (Int32)Math.Ceiling(m_listAllStation.Count / (double)m_iPageRowCount);
                    if (m_iCurrentPage > 1)
                    {
                        startIndex = (base.m_iCurrentPage - 1) * base.m_iPageRowCount - 1;
                    }
                    int endIndex = Math.Min(startIndex + base.m_iPageRowCount, m_listAllStation.Count);
                    for (int i = startIndex; i < endIndex; ++i)
                    {
                        this.Items.Add(m_listAllStation[i].strStationId, m_listAllStation[i].strStationId + "|" + m_listAllStation[i].strName, (int)m_listAllStation[i].state);
                    }
                    this.ResumeLayout(false);
                    EventAddItems(m_listAllStation.Count);
                }));
            }
            else {
                this.SuspendLayout();
                this.Items.Clear();
                int startIndex = 0;
                base.TotalPage = (Int32)Math.Ceiling(m_listAllStation.Count / (double)m_iPageRowCount);
                if (m_iCurrentPage > 1)
                {
                    startIndex = (base.m_iCurrentPage - 1) * base.m_iPageRowCount - 1;
                }
                int endIndex = Math.Min(startIndex + base.m_iPageRowCount, m_listAllStation.Count);
                for (int i = startIndex; i < endIndex; ++i)
                {
                    this.Items.Add(m_listAllStation[i].strStationId, m_listAllStation[i].strStationId + "|" + m_listAllStation[i].strName, (int)m_listAllStation[i].state);

                }
                this.ResumeLayout(false);
                EventAddItems(m_listAllStation.Count);
            }
        }

        private int FindStationIndexByID(string iStaionId)
        {
            int result = -1;
            for (int i = 0; i < m_listAllStation.Count; ++i)
            {
                if (iStaionId == m_listAllStation[i].strStationId)
                {
                    return i;
                }
            }
            return result;
        }
        #endregion

        #region 静态方法用于排序
        private static int CompareById(SItem obj1, SItem obj2)
        {
            // 根据ID大小来排序
            // 只支持整形
            try
            {
                int id1 = int.Parse(obj1.strStationId);
                int id2 = int.Parse(obj2.strStationId);
                return id1 - id2;
            }
            catch (System.Exception)
            {

            }
            return 0;
        }

        private static int CompareByState(SItem ojb1, SItem obj2)
        {
            return ojb1.state - obj2.state;
        }
        private static int CompareByStateReverse(SItem ojb1, SItem obj2)
        {
            return obj2.state - ojb1.state;
        }
        #endregion 静态方法用于排序
    }
}
