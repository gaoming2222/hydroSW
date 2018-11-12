using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Hydrology.Entity;
using System.Threading;

namespace Hydrology.CControls
{
    // 自定义ListView,控制滚动条的显示
    class CExListView : ListView
    {
        #region  事件定义
        /// <summary>
        /// 当前页面发生改变
        /// </summary>
        public event EventHandler<CEventSingleArgs<int>> CurrentPageChanged;

        /// <summary>
        /// 添加行的时间
        /// </summary>
        public event EventHandler<CEventSingleArgs<int>> ItemAdded;

        #endregion ///<事件定义

        #region 滚动条相关
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_CTL = 2;
        public const int SB_BOTH = 3;

        private bool m_bVerticalScrollShow; //是否显示垂直滚动条
        private bool m_bHorizentalScrollShow; //是否显示水平滚动条

        public bool BVertialScrollVisible
        {
            get { return m_bVerticalScrollShow; }
            set { m_bVerticalScrollShow = value; }
        }
        public bool BHorizentalScroolVisible
        {
            get { return m_bHorizentalScrollShow; }
            set { m_bHorizentalScrollShow = value; }
        }
        #endregion ///<滚动条相关

        #region 分页相关
        protected int m_iTotalPage;   // 总页码
        protected int m_iCurrentPage; // 当前页码
        protected int m_iPageRowCount = 1000;    //一页的数量,默认是1000
        protected bool m_bPartionPageEnable = false; //是否开启分页，默认不开启，要分页的话自己需要重写事件函数

        // 右键菜单
        protected ContextMenuStrip m_contextMenu;
        protected ToolStripMenuItem m_menuItemFirstPage;
        protected ToolStripMenuItem m_menuItemLastPage;
        protected ToolStripMenuItem m_menuItemPreviousPage;
        protected ToolStripMenuItem m_menuItemNextPage;

        /// <summary>
        /// 总页码数
        /// </summary>
        public int TotalPage
        {
            get { return m_iTotalPage; }
            set
            {
                m_iTotalPage = value;
                if (0 == m_iCurrentPage)
                {
                    m_iCurrentPage = 1;
                }
                if (m_iTotalPage > m_iCurrentPage)
                {
                    m_menuItemNextPage.Enabled = true;
                    m_menuItemLastPage.Enabled = true; //尾页可用
                }
            }
        }
        public bool PartitionPageEnable
        {
            get { return m_bPartionPageEnable; }
            set
            {
                m_bPartionPageEnable = value;
                if (m_bPartionPageEnable)
                {
                    // 初始化右键菜单
                    InitContextMenu();
                }
            }
        }
        public int PageRowCount
        {
            get { return m_iPageRowCount; }
            set { m_iPageRowCount = value; }
        }
        public int CurrentPageIndex
        {
            get { return m_iCurrentPage; }
        }
        #endregion ///<分页相关

        #region 属性
        private const int MAX_RECORD_COUNT = 500;
        private bool m_autoUpdateLists; //  是否自动删除多余MAX_RECORD_COUNT的行记录

        public bool AutoUpdateLists
        {
            get { return m_autoUpdateLists; }
            set { m_autoUpdateLists = value; }
        }
        #endregion 属性

        #region 成员变量
        /// <summary>
        /// item的互斥量
        /// </summary>
        private Mutex m_mutexItem;
        #endregion 成员变量

        //  构造函数
        public CExListView()
            : base()
        {
            // 默认显示垂直和水平滚动条
            m_bHorizentalScrollShow = true;
            m_bVerticalScrollShow = true;

            this.DoubleBuffered = true; // 设置双缓存
            m_autoUpdateLists = false;

            m_mutexItem = new Mutex();
        }
        /// <summary>
        /// 提供添加Item的接口
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ListViewItem item)
        {
            try
            {
                m_mutexItem.WaitOne();
                if (this.IsHandleCreated)
                {
                    this.Invoke((Action)delegate()
                    {
                        this.Items.Add(item);
                        // 滚动到最底部
                        this.EnsureVisible(this.Items.Count - 1);
                    });
                }
                else
                {
                    this.Items.Add(item);
                    
                }
                
                
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            finally
            {
                m_mutexItem.ReleaseMutex();
                RefreshItems();
            }
            
        }


        [DllImport("user32")]

        public static extern int ShowScrollBar(IntPtr hwnd, int wBar, int bShow);

        #region 重载
        // 设定滚动条
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //CListFormTabPage.ShowScrollBar((int)this.m_listView.Handle, SB_HORZ, 0); //去掉水平滚动条
            //base.WndProc(ref m);
            if (this.View == View.List)
            {
                ShowScrollBar(this.Handle, SB_VERT, m_bVerticalScrollShow ? 1 : 0);
                ShowScrollBar(this.Handle, SB_HORZ, m_bHorizentalScrollShow ? 1 : 0);
            }
            if (this.View == View.Details)
            {
                ShowScrollBar(this.Handle, SB_VERT, m_bVerticalScrollShow ? 1 : 0);
                ShowScrollBar(this.Handle, SB_HORZ, m_bHorizentalScrollShow ? 1 : 0);
            }
            base.WndProc(ref m);
        }

        // 响应右键
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Right)
            {
                //右键单击，弹出菜单
                if (m_bPartionPageEnable)
                {
                    m_contextMenu.Show(this, e.Location.X, e.Location.Y);
                }
            }
        }

        #endregion 重载

        // 初始化右键菜单
        protected virtual void InitContextMenu()
        {
            m_contextMenu = new ContextMenuStrip();

            /* ToolStripMenuItem*/
            m_menuItemFirstPage = new ToolStripMenuItem() { Text = "首页" };
            /*ToolStripMenuItem*/
            m_menuItemLastPage = new ToolStripMenuItem() { Text = "尾页" };
            /*ToolStripMenuItem*/
            m_menuItemPreviousPage = new ToolStripMenuItem() { Text = "上一页" };
            /*ToolStripMenuItem*/
            m_menuItemNextPage = new ToolStripMenuItem() { Text = "下一页" };
            m_menuItemPreviousPage.Enabled = false;
            m_menuItemNextPage.Enabled = false;
            m_menuItemFirstPage.Enabled = false;
            m_menuItemLastPage.Enabled = false;

            ToolStripSeparator seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(m_menuItemNextPage);
            m_contextMenu.Items.Add(m_menuItemPreviousPage);
            m_contextMenu.Items.Add(seperator);
            seperator = new ToolStripSeparator();
            m_contextMenu.Items.Add(m_menuItemFirstPage);
            m_contextMenu.Items.Add(m_menuItemLastPage);

            // 绑定消息
            m_menuItemNextPage.Click += OnMenuNextPage;
            m_menuItemPreviousPage.Click += OnMenuPreviousPage;
            m_menuItemFirstPage.Click += new EventHandler(OnMenuFirstPage);
            m_menuItemLastPage.Click += new EventHandler(OnMenuLastPage);
        }

        /// <summary>
        /// 尾页菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuLastPage(object sender, EventArgs e)
        {
            m_iCurrentPage = m_iTotalPage;
            m_menuItemLastPage.Enabled = false; //尾页不能用
            m_menuItemFirstPage.Enabled = true; //首页可用
            m_menuItemNextPage.Enabled = false; //下一页不可用
            m_menuItemPreviousPage.Enabled = true;//上一页可用
            if (CurrentPageChanged != null)
            {
                CurrentPageChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        /// <summary>
        /// 首页菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnMenuFirstPage(object sender, EventArgs e)
        {
            m_iCurrentPage = 1;
            m_menuItemLastPage.Enabled = true;//尾页可以用
            m_menuItemFirstPage.Enabled = false; //首页不可用
            m_menuItemNextPage.Enabled = true; //下一页可以用
            m_menuItemPreviousPage.Enabled = false; //上一页不能用
            if (CurrentPageChanged != null)
            {
                CurrentPageChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        // 下一页的事件响应消息
        protected virtual void OnMenuNextPage(object sender, EventArgs e)
        {
            m_iCurrentPage += 1;
            if (m_iCurrentPage >= m_iTotalPage)
            {
                m_menuItemNextPage.Enabled = false;
                m_menuItemLastPage.Enabled = false;//尾页不可用
            }
            if (m_iCurrentPage > 1)
            {
                m_menuItemPreviousPage.Enabled = true;
                m_menuItemFirstPage.Enabled = true; //首页可用
            }
            if (CurrentPageChanged != null)
            {
                CurrentPageChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        // 上一页的事件响应消息
        protected virtual void OnMenuPreviousPage(object sender, EventArgs e)
        {
            m_iCurrentPage -= 1;
            if (m_iCurrentPage <= 1)
            {
                m_menuItemPreviousPage.Enabled = false;
                m_menuItemFirstPage.Enabled = false; //首页不可用
            }
            if (m_iCurrentPage < m_iTotalPage)
            {
                m_menuItemNextPage.Enabled = true;
                m_menuItemLastPage.Enabled = true;  //尾页可用
            }
            if (CurrentPageChanged != null)
            {
                CurrentPageChanged(this, new CEventSingleArgs<int>(m_iCurrentPage));
            }
        }

        // 添加项事件，自定义事件
        protected virtual void EventAddItems(int itemcount)
        {
            if (ItemAdded != null)
            {
                ItemAdded.Invoke(this, new CEventSingleArgs<int>(itemcount));
            }
            RefreshItems();
        }

        private void RefreshItems()
        {
            if (!AutoUpdateLists)
                return;
            if (!m_mutexItem.WaitOne(0))
            {
                // 当前正忙，直接退出
                return;
            }
            int currentRecordCount = this.Items.Count;
            if (currentRecordCount > MAX_RECORD_COUNT)
            {
                for (int i = 0; i < MAX_RECORD_COUNT; i++)
                {
                    this.Items.RemoveAt(0);
                }
            }
            m_mutexItem.ReleaseMutex(); //释放互斥量
        }
    }
}
