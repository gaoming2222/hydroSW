using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using Hydrology.Forms;
using Protocol.Manager;
using System.IO;
using Hydrology.Utils;
using Hydrology.Bridage;
using Hydrology.DBManager;

namespace Hydrology
{
    public partial class MainForm : Form
    {
        #region 事件定义
        /// <summary>
        /// 更改当前用户模式消息
        /// </summary>
        public event EventHandler<CEventSingleArgs<bool>> UserModeChanged;

        #endregion 事件定义

        #region 成员变量
        private CRTDForm m_formRTD;  //最新实时数据
        private CRTDSoilForm m_formSoilRTD; //  最新墒情实时数据
        private CStationStateForm m_formStationState;   //站点最新状态
        private CExTabControl m_tabControlUp;           //右上Tab控件
        private CExTabControl m_tabControlBottom;       //右下Tab控件
        private CListFormTabPage m_listFormSystemInfo;  //系统信息
        private CListFormTabPage m_listFormWarningInfo; //告警信息
        private CListFormTabPage m_listFormComState;    //串口状态
        private bool m_bIsInAdministrator;              //当前用户模式

        private List<Thread> m_listSimulators;
        private System.Windows.Forms.Timer m_timer = new System.Windows.Forms.Timer()
            {
                Enabled = false,
                Interval = 30 * 60 * 1000// 30分钟
            };
        private System.Windows.Forms.Timer m_sysTimer;  //用于刷新系统时间
        #endregion

        public MainForm()
        {
            //取得当前时间
            DateTime timeBegin = DateTime.Now;
            //进入载入界面
            CWelcomePage page = new CWelcomePage();
            //载入界面居中
            page.StartPosition = FormStartPosition.CenterScreen;
            page.ShowDialog();

            InitializeComponent();

            try
            {
                // 如果数据库配置失败, 初始化数据管理，此处可考虑显示数据库配置界面等等
                if (!CDBDataMgr.Instance.Init() || (!CDBSoilDataMgr.Instance.Init()))
                {
                    var msgboxResult = MessageBox.Show("数据库配置错误,请重新配置", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (DialogResult.OK == msgboxResult)
                    {
                        ;
                    }
                    // 关闭欢迎界面
                    //page.Invoke((Action)delegate { page.Close(); });
                    // 此处抛出异常，用于终止程序，此时程序无法工作，谨慎处理
                    //throw new Exception("数据库配置错误"); // 抛出异常
                }

                //  系统启动顺序：先加载数据，然后再加载页面

                //  读取配置文件，初始化北斗卫星，短信，GPRS参数
                //XmlDocManager.Instance.ReadFromXml();

                InitCustomerControls();
                //窗体最大化
                this.WindowState = FormWindowState.Maximized;

                m_statusStrip.ForeColor = Color.White;
                m_statusLable.Text = "";
                // 设置分割区域宽度
                splitContainer1.SplitterWidth = 6;
                splitContainer2.SplitterWidth = 8;

                // 初始化定时器
                //m_timer = new System.Windows.Forms.Timer()
                //{
                //    Enabled = false,
                //    Interval = 30 * 60 * 1000// 30分钟
                //};
                m_timer.Tick += new EventHandler(EH_Timer);

                this.CopyRightLabel.Text = "版权所有:湖北一方科技发展有限公司 V1.0";
                this.lblSysTimer.Alignment = ToolStripItemAlignment.Right;
                //  初始化系统时间定时器
                m_sysTimer = new System.Windows.Forms.Timer()
                {
                    Enabled = true,
                    Interval = 1000
                };
                m_sysTimer.Tick += (s, e) =>
                {
                    this.lblSysTimer.Text = "时间:  " + DateTime.Now.ToString();
                };
                m_sysTimer.Start();

                InitMainFormMenu();
                CreateMsgBinding();

                ChangeUserMode(false);

                CPortDataMgr.Instance.InitGsms();
                CPortDataMgr.Instance.InitBeidouNormal();
                CPortDataMgr.Instance.StartGprs();
                CPortDataMgr.Instance.InitBeidou500();

                // 计算启动时间，写入日志文件
                TimeSpan span = DateTime.Now - timeBegin;
                CSystemInfoMgr.Instance.AddInfo(string.Format("系统启动正常, 用时 {0} s", span.TotalSeconds.ToString("0.00")), DateTime.Now);

                //  显示实时数据表
                CDBDataMgr.Instance.SentRTDMsg();
                //  显示实时墒情表
                CDBSoilDataMgr.Instance.SendSoilDataMsg();

                // 生成测试数据
                // GenerateTestData();
            }
            catch (Exception exp)
            {
                CSystemInfoMgr.Instance.AddInfo(exp.ToString());
                //throw exp; //再次抛出异常
            }
            finally
            {
                //if (this.IsHandleCreated)
                //{
                // 关闭欢迎界面
                page.Invoke((Action)delegate { page.Close(); });
                //}
            }
        }

       
        public void ShowRTDOfTabPage(string subcenerName)
        {
            m_tabControlUp.SelectedIndex = m_formRTD.TabPageIndex;
            m_formRTD.ShowTabOfSubCenter(subcenerName);
        }

     
        /// <summary>
        /// 定时器时间，用于控制用户登陆时长，避免忘记退出等消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        #region 事件处理

        /// <summary>
        /// 关闭页面事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EHClosing(object sender, FormClosingEventArgs e)
        {
            // 只有管理员才能关闭页面
            if (!m_bIsInAdministrator)
            {
                e.Cancel = true;
                MessageBox.Show("权限不够");
                return;
            }
            // 再次提示
            DialogResult result = MessageBox.Show("确定退出系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.No == result)
            {
                e.Cancel = true;
                return;
            }

            CWelcomePage page = new CWelcomePage();
            page.SetClosedPicture();
            page.StartPosition = FormStartPosition.CenterScreen;
            page.Show();
            try
            {
                // 停止模拟线程
                if (m_listSimulators != null)
                {
                    foreach (Thread t in m_listSimulators)
                    {
                        try
                        {
                            t.Abort();
                        }
                        catch (System.Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }

                    }
                }
                // 停止端口以及串口服务
                CPortDataMgr.Instance.CloseAll();

                // 停止数据库服务
                CDBDataMgr.Instance.StopDBService();
                CDBSoilDataMgr.Instance.StopDBService();

                // 写入停止信息
                CSystemInfoMgr.Instance.AddInfo("退出系统");

                // 停止系统信息记录服务
                CSystemInfoMgr.Instance.Close();
                //Thread.Sleep(2000);

            }
            catch (Exception epx)
            {

            }
            finally
            {
                page.Close();
            }
        }

        /// <summary>
        /// 用户登陆成功的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void EHUserModeChanged(object sender, CEventSingleArgs<bool> e)
        {
            this.ChangeUserMode(e.Value);
            this.MI_UserLogout.Enabled = true;
            // 开启定时器，用于检测过于一段时间，自己退出登陆
            m_timer.Start();
            if (UserModeChanged != null)
            {
                UserModeChanged(this, new CEventSingleArgs<bool>(e.Value));
            }
        }

        private void EHMI_ToolBar_Click(object sender, EventArgs e)
        {
            MI_ToolBar.Checked = !MI_ToolBar.Checked;
            this.m_toolStrip.Visible = MI_ToolBar.Checked;
        }
        private void EHMI_StatusBar_Click(object sender, EventArgs e)
        {
            MI_StatusBar.Checked = !MI_StatusBar.Checked;
            this.m_statusStrip.Visible = MI_StatusBar.Checked;
        }
        private void EHMI_CommPortState_Click(object sender, EventArgs e)
        {
            if (null == m_listFormComState)
                return;
            MI_ComPortState.Checked = !MI_ComPortState.Checked;
            if (MI_ComPortState.Checked)
            {
                // 显示
                m_tabControlBottom.AddPage(m_listFormComState);
                m_tabControlBottom.SelectedIndex = m_tabControlBottom.TabCount - 1;
                m_listFormComState.Show();
            }
            else
            {
                // 隐藏
                m_tabControlBottom.RemovePage(m_listFormComState);
                m_listFormComState.Hide();
            }
        }
        private void EHMI_Soil_Click(object sender, EventArgs e)
        {
            MI_Soil.Checked = !MI_Soil.Checked;
            if (MI_Soil.Checked)
            {
                // 显示
                m_tabControlUp.AddPage(m_formSoilRTD);
                m_tabControlUp.SelectedIndex = m_tabControlUp.TabCount - 1;
                m_formStationState.Show();
            }
            else
            {
                // 隐藏
                m_tabControlUp.RemovePage(m_formSoilRTD);
                m_formStationState.Hide();
            }
        }
        private void EHMI_StationRTState_Click(object sender, EventArgs e)
        {
            MI_StationStatus.Checked = !MI_StationStatus.Checked;
            if (MI_StationStatus.Checked)
            {
                // 显示
                m_tabControlUp.AddPage(m_formStationState);
                m_tabControlUp.SelectedIndex = m_tabControlUp.TabCount - 1;
                m_formStationState.Show();
            }
            else
            {
                // 隐藏
                m_tabControlUp.RemovePage(m_formStationState);
                m_formStationState.Hide();
            }
        }
        private void EHMI_WarningInfo_Click(object sender, EventArgs e)
        {
            MI_WarningInfo.Checked = !MI_WarningInfo.Checked;
            if (MI_WarningInfo.Checked)
            {
                // 显示
                m_tabControlBottom.AddPage(m_listFormWarningInfo);
                m_tabControlBottom.SelectedIndex = m_tabControlBottom.TabCount - 1;
                m_listFormWarningInfo.Show();
            }
            else
            {
                // 隐藏
                m_tabControlBottom.RemovePage(m_listFormWarningInfo);
                m_listFormWarningInfo.Hide();
            }
        }
        private void EH_CommPort_Page_Closed(object sender, EventArgs e)
        {
            this.MI_ComPortState.Checked = false;
        }

        private void EH_Soil_Page_Closed(object sender, EventArgs e)
        {
            this.MI_Soil.Checked = false;
        }
        private void EH_StationState_Page_Closed(object sender, EventArgs e)
        {
            this.MI_StationStatus.Checked = false;
        }
        private void EH_WarningInfo_Page_Closed(object sender, EventArgs e)
        {
            this.MI_WarningInfo.Checked = false;
        }

        /// <summary>
        /// 收到告警信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EH_Recv_New_Warning_Info(object sender, CEventSingleArgs<CTextInfo> e)
        {
            m_listFormWarningInfo.AddText(string.Format("{0} {1}", e.Value.Time.ToString("yyyy-MM-dd HH:mm:ss"), e.Value.Info), ETextMsgState.ENormal);
        }

        /// <summary>
        /// 收到系统信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></parParallel
        private void EH_Recv_New_System_Info(object sender, CEventSingleArgs<CTextInfo> e)
        {
            m_listFormSystemInfo.AddText(string.Format("{0} {1}", e.Value.Time.ToString("yyyy-MM-dd HH:mm:ss"), e.Value.Info), e.Value.EState);
        }
        private void EH_Timer(object sender, EventArgs e)
        {
            m_timer.Stop();
            Logout();
            CSystemInfoMgr.Instance.AddInfo("用户超时，自动退出登录");
        }
        #endregion

        #region  帮助方法
        // 初始化自定义控件
        private void InitCustomerControls()
        {
            this.SuspendLayout();
            //m_dataGridLatestData = new CDataGridTabPage() { Title = "实时数据", BTabRectClosable = false };
            //m_dataGridLatestData.DataGrid = new CDataGridViewRTD();
            m_formRTD = new CRTDForm() { Title = "实时水情数据", BTabRectClosable = false, MdiParent = this };
            m_formRTD.Dock = DockStyle.Fill;

            m_formSoilRTD = new CRTDSoilForm() { Title = "实时墒情数据", BTabRectClosable = false, MdiParent = this };
            m_formSoilRTD.Dock = DockStyle.Fill;

            m_listFormSystemInfo = new CListFormTabPage() { Title = "运行日志", BTabRectClosable = false, BCloseButton = false, MdiParent = this, Text = "实时信息" };
            m_listFormSystemInfo.Dock = DockStyle.Fill;
            m_listFormSystemInfo.ListView.AutoUpdateLists = true;

            m_listFormComState = new CListFormTabPage() { Title = "串口状态", BTabRectClosable = false, MdiParent = this, Text = "通讯口状态" };
            m_listFormComState.Dock = DockStyle.Fill;
            m_listFormComState.ListView = new CListViewSerialPortState();
            m_listFormComState.ListView.AutoUpdateLists = true;
            //m_listFormComState.SetMsgViewMode(true);

            m_listFormWarningInfo = new CListFormTabPage() { Title = "告警信息", BTabRectClosable = false, MdiParent = this, Text = "告警信息2" };
            m_listFormWarningInfo.Dock = DockStyle.Fill;
            m_listFormWarningInfo.ListView.AutoUpdateLists = true;

            m_formStationState = new CStationStateForm() { Title = "站点状态", BTabRectClosable = true, MdiParent = this };
            m_formStationState.Dock = DockStyle.Fill;


            m_formRTD.MouseEnter += MainForm_MouseEnter;
            m_formSoilRTD.MouseEnter += MainForm_MouseEnter;
            m_listFormSystemInfo.MouseEnter += MainForm_MouseEnter;
            m_listFormComState.MouseEnter += MainForm_MouseEnter;
            m_listFormWarningInfo.MouseEnter += MainForm_MouseEnter;
            m_formStationState.MouseEnter += MainForm_MouseEnter;


            #region TABCONTROL

            m_tabControlUp = new CExTabControl();
            m_tabControlUp.SuspendLayout();
            m_tabControlUp.AddPage(m_formRTD);
            m_tabControlUp.AddPage(m_formSoilRTD);
            m_tabControlUp.AddPage(m_formStationState);
            //m_tabControlUp.AddPage(m_formSoilRTD);
            splitContainer2.Panel1.Controls.Add(m_tabControlUp);
            m_tabControlUp.ResumeLayout(false);
            //this.m_tabControlUp.MouseDown += (s, e) => { this.m_timer.Stop(); this.m_timer.Start(); MessageBox.Show("up enter"); };

            m_tabControlBottom = new CExTabControl();
            m_tabControlBottom.SuspendLayout();
            m_tabControlBottom.Alignment = TabAlignment.Bottom; //选项卡位于底部
            m_tabControlBottom.AddPage(m_listFormSystemInfo);
            m_tabControlBottom.AddPage(m_listFormComState);
            m_tabControlBottom.AddPage(m_listFormWarningInfo); //告警信息
            splitContainer2.Panel2.Controls.Add(m_tabControlBottom);
            m_tabControlBottom.ResumeLayout(false);

            //m_tabControlBottom.MouseDown += (s, e) => { this.m_timer.Stop(); this.m_timer.Start(); MessageBox.Show("bottom enter"); };
            #endregion m_tabControl

            // 初始化表头
            //m_dataGridLatestData.DataGrid.Header = new string[] { "站名", "站点编号", "站点类型", "采集时间", "延迟", "水量", "雨量", "电压", "端口", "报文类型" };
            //m_dataGridView2.Header = new string[] { "站名", "站点编号", "站点类型", "采集时间", "延迟", "水量", "雨量", "电压", "端口", "报文类型" };
            m_listFormSystemInfo.Show();
            m_listFormComState.Show();

            m_formRTD.Show();
            m_formStationState.Show();
            //if (IsLoadSoilRTD())
            //{
            //    this.MI_Soil.Enabled = false;
            //    this.MI_Soil.Checked = true;
            //    this.MI_Soil.Enabled = true;
            m_formSoilRTD.Show();
            //}
            m_listFormWarningInfo.Show();

            // 加载 CTreeView
            CMainFormAndCTreeViewBridage.LoadTreeView(this, this.m_tabControlUp);

            this.ResumeLayout(false);
        }

        private void InitMainFormMenu()
        {
            // 设置FormHelper的对象指针
            FormHelper.MainFormRef = this;

            /**********    系统    **********/
            this.MI_ChannelProtocolCfg.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_DataProtocolCfg.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_SerialPortConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_Beidou.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_Beidou500.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_DatabaseConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_VoiceConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MI_SystemExit.Click += new EventHandler(FormHelper.SysExit);
            this.MI_LogIn.Click += new EventHandler(FormHelper.ShowForm);          // 用户登录  
            this.MI_UserLogout.Click += new EventHandler(FormHelper.UserLogOut);      // 用户退出  
            this.MI_UserMgr.Click += new EventHandler(FormHelper.ShowForm);    // 用户管理

            /**********    视图    **********/
            this.MI_ToolBar.Click += new EventHandler(this.EHMI_ToolBar_Click);
            this.MI_StatusBar.Click += new EventHandler(this.EHMI_StatusBar_Click);
            this.MI_StationStatus.Click += new EventHandler(this.EHMI_StationRTState_Click);
            this.MI_Soil.Click += new EventHandler(this.EHMI_Soil_Click);
            this.MI_ComPortState.Click += new EventHandler(this.EHMI_CommPortState_Click);
            this.MI_WarningInfo.Click += new EventHandler(this.EHMI_WarningInfo_Click);

            /**********   管理栏   **********/
            // 分中心管理
            this.MI_SubStationMgr.Click += new EventHandler(FormHelper.ShowForm);
            // 测站管理
            this.MI_StationMgr.Click += new EventHandler(FormHelper.ShowForm);
            // 墒情站管理
            this.MI_SoilStationMgr.Click += new EventHandler(FormHelper.ShowForm);

            /**********   工具栏   **********/
            // 历史数据查询
            this.DataQuery.Click += new EventHandler(FormHelper.ShowForm);
            // 读取与设置
            this.ReadAndSetting.Click += new EventHandler(FormHelper.ShowForm);
            // 批量传输U盘数据
     //       this.MI_TransmitUpan.Click += new EventHandler(FormHelper.ShowForm);
            // 批量传输Flash数据
            // 批量传输数据
    //        this.MI_TransmitFlash.Click += new EventHandler(FormHelper.ShowForm);
            this.BatchTransmit.Click += new EventHandler(FormHelper.ShowForm);
            // 历史数据校正
            this.HistoryDataAdjust.Click += new EventHandler(FormHelper.ShowForm);
            // 告警信息查询
            this.MI_WarningInfoQuery.Click += new EventHandler(FormHelper.ShowForm);
            // 告警信息校正
            // this.MI_WarningInfoAdjust.Click += new EventHandler(FormHelper.ShowForm);
            // 水位流量（库容）管理
            this.MI_StageFlowMapMgr.Click += new EventHandler(FormHelper.ShowForm);
            //// 畅通率报表
            //this.MI_CommunicationRate.Click += new EventHandler(FormHelper.ShowForm);
            // 系统对时
            this.MI_SysTimerAdjust.Click += new EventHandler(FormHelper.ShowForm);

            /**********   帮助栏   **********/

            this.MI_GPRS.Click += new EventHandler(FormHelper.ShowForm);

            //  快捷菜单栏
            this.MITool_ChannelProtocol.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_DataProtocol.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_SerialPortConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_GPRSConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_BeiDouConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_BeiDouConfig500.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_DBConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_VoiceConfig.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_SubCenterMgr.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_StationMgr.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_SoilMgr.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_DataQuery.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_DataAdjust.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_WarnningInfoQuery.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_CommunicationRate.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_StageFlowMapMgr.Click += new EventHandler(FormHelper.ShowForm);

            this.MITool_ReadAndSetting.Click += new EventHandler(FormHelper.ShowForm);
         //   this.MITool_UDisk.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_Flash.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_SysTime.Click += new EventHandler(FormHelper.ShowForm);
            this.MITool_Exit.Click += new EventHandler(FormHelper.SysExit);

            //  更新菜单栏状态
            UpdateMenuState();
        }

        private void CreateMsgBinding()
        {
            this.FormClosing += new FormClosingEventHandler(this.EHClosing);
            //用于管理员权限被释放时，关闭用户的管理员功能
            this.UserModeChanged += FormHelper.EHUserModeChanged;

            // 绑定消息
            m_formSoilRTD.TabClosed += new EventHandler(this.EH_Soil_Page_Closed);
            m_listFormComState.TabClosed += new EventHandler(this.EH_CommPort_Page_Closed);
            m_formStationState.TabClosed += new EventHandler(this.EH_StationState_Page_Closed);
            m_listFormWarningInfo.TabClosed += new EventHandler(this.EH_WarningInfo_Page_Closed);

            // 收到系统信息与报警信息
            CSystemInfoMgr.Instance.RecvedNewSystemInfo += new EventHandler<CEventSingleArgs<CTextInfo>>(this.EH_Recv_New_System_Info);
            CWarningInfoMgr.Instance.RecvedNewWarningInfo += new EventHandler<CEventSingleArgs<CTextInfo>>(this.EH_Recv_New_Warning_Info);

            // 绑定串口改变消息
            CDBDataMgr.Instance.SerialPortUpdated += new EventHandler((this.m_listFormComState.ListView as CListViewSerialPortState).EHSerialPortChanged);
            CDBDataMgr.Instance.SerialPortUpdated += CPortDataMgr.Instance.EHSerialPortUpdated;
            //CPortDataMgr += new EventHandler((this.m_listFormComState.ListView as CListViewSerialPortState).EHSerialPortStateUpdated);

            CProtocolEventManager.SerialPortStateChanged4UI += (this.m_listFormComState.ListView as CListViewSerialPortState).EHSerialPortStateUpdated;

            CDBDataMgr.Instance.StationUpdated += new EventHandler(TreeMenuReload);
            CDBDataMgr.Instance.SubCenterUpdated += new EventHandler(TreeMenuReload);
            CDBDataMgr.Instance.SerialPortUpdated += new EventHandler(TreeMenuReload);
            //CPortDataMgr.RecvStationDataEvent += new EventHandler<CEventRecvStationDataArgs>(CDBDataMgr.Instance.EHRecvStationData);
        }

        private void TreeMenuReload(object sender, EventArgs e)
        {
            //  更新树形控件
            CTree.Instance.LoadTree();

            //  更新墒情站显示逻辑
            UpdateMenuState();
        }

        private bool IsLoadSoilRTD()
        {
            bool isLoad = false;
            //  更新墒情站显示逻辑
            List<CEntityStation> listStation = CDBDataMgr.Instance.GetAllStation();
            foreach (var item in listStation)
            {
                if (item.StationType == EStationType.ESoil ||
                    item.StationType == EStationType.ESoilHydrology ||
                    item.StationType == EStationType.ESoilRain ||
                    item.StationType == EStationType.ESoilWater)
                {
                    isLoad = true;
                    break;
                }
            }
            return isLoad;
        }

        private void UpdateMenuState()
        {
            try
            {
                bool hasSoilStation = false;
                if (CCurrentLoginUser.Instance.IsLogin)
                {
                    //  更新墒情站显示逻辑
                    List<CEntityStation> listStation = CDBDataMgr.Instance.GetAllStation();
                    foreach (var item in listStation)
                    {
                        if (item.StationType == EStationType.ESoil ||
                            item.StationType == EStationType.ESoilHydrology ||
                            item.StationType == EStationType.ESoilRain ||
                            item.StationType == EStationType.ESoilWater)
                        {
                            hasSoilStation = true;
                            break;
                        }
                    }
                }
                //this.MI_SoilStationMgr.Enabled = hasSoilStation;
                //this.MITool_SoilMgr.Enabled = hasSoilStation;
                if (!hasSoilStation)
                {
                    this.MI_Soil.Checked = false;
                    this.MI_Soil.Enabled = false;
                }
                else
                {
                    this.MI_Soil.Enabled = true;
                }
            }
            catch (Exception exp) { }
        }

        public static void ProtocolConfigChanged(object sender, EventArgs e)
        {
            CTree.Instance.LoadTree();
        }

        private void ChangeUserMode(bool bAdministrator)
        {
            m_bIsInAdministrator = bAdministrator;
            // 设置一些菜单项的启用
            this.MI_LogIn.Enabled = !bAdministrator;
            // 协议配置
            this.MI_ChannelProtocolCfg.Enabled = m_bIsInAdministrator;
            this.MI_DataProtocolCfg.Enabled = m_bIsInAdministrator;
            this.MI_VoiceConfig.Enabled = m_bIsInAdministrator;
            // 通讯口配置
            this.MI_CommunicationPort.Enabled = m_bIsInAdministrator;
            this.MI_SerialPortConfig.Enabled = m_bIsInAdministrator;
            this.MI_DatabaseConfig.Enabled = m_bIsInAdministrator;
            this.MI_GPRS.Enabled = m_bIsInAdministrator;
            this.ReadAndSetting.Enabled = m_bIsInAdministrator;
            this.BatchTransmit.Enabled = m_bIsInAdministrator;
         //   this.MI_TransmitUpan.Enabled = m_bIsInAdministrator;
         //   this.MI_TransmitFlash.Enabled = m_bIsInAdministrator;
            this.MI_SysTimerAdjust.Enabled = m_bIsInAdministrator;
            // 告警信息校正
            // this.MI_WarningInfoAdjust.Enabled = m_bIsInAdministrator;

            this.MI_UserMgr.Enabled = m_bIsInAdministrator;

            this.MI_SubStationMgr.Enabled = m_bIsInAdministrator;
            this.MI_StationMgr.Enabled = m_bIsInAdministrator;
            this.MI_SoilStationMgr.Enabled = m_bIsInAdministrator;


            // 水位流量（库容）管理
            this.MI_StageFlowMapMgr.Enabled = m_bIsInAdministrator;

            this.HistoryDataAdjust.Enabled = m_bIsInAdministrator;
            this.MI_SystemExit.Enabled = m_bIsInAdministrator;

            //// 畅通率管理
            //this.MI_CommunicationRate.Enabled = m_bIsInAdministrator;

            //  快捷菜单栏
            this.MITool_ChannelProtocol.Enabled = m_bIsInAdministrator;
            this.MITool_DataProtocol.Enabled = m_bIsInAdministrator;
            this.MITool_SerialPortConfig.Enabled = m_bIsInAdministrator;
            this.MITool_GPRSConfig.Enabled = m_bIsInAdministrator;
            this.MITool_BeiDouConfig.Enabled = m_bIsInAdministrator;
            this.MITool_BeiDouConfig500.Enabled = m_bIsInAdministrator;
            this.MITool_DBConfig.Enabled = m_bIsInAdministrator;
            this.MITool_VoiceConfig.Enabled = m_bIsInAdministrator;
            this.MITool_SubCenterMgr.Enabled = m_bIsInAdministrator;
            this.MITool_StationMgr.Enabled = m_bIsInAdministrator;
            this.MITool_SoilMgr.Enabled = m_bIsInAdministrator;
            //this.MITool_DataQuery.Enabled = m_bIsInAdministrator;
            this.MITool_DataAdjust.Enabled = m_bIsInAdministrator;
            //this.MITool_WarnningInfoQuery.Enabled = m_bIsInAdministrator;
            this.MITool_CommunicationRate.Enabled = m_bIsInAdministrator;
            this.MITool_StageFlowMapMgr.Enabled = m_bIsInAdministrator;
            this.MITool_ReadAndSetting.Enabled = m_bIsInAdministrator;
           // this.MITool_UDisk.Enabled = m_bIsInAdministrator;
            this.MITool_Flash.Enabled = m_bIsInAdministrator;
            this.MITool_SysTime.Enabled = m_bIsInAdministrator;
            this.MITool_Exit.Enabled = m_bIsInAdministrator;

            UpdateMenuState();
        }

        public void Logout()
        {
            // 退出当前登陆
            ChangeUserMode(false);
            this.MI_UserLogout.Enabled = false;
            // 停止定时器
            m_timer.Stop();

            if (UserModeChanged != null)
            {
                UserModeChanged(this, new CEventSingleArgs<bool>(false));
            }

            CCurrentLoginUser.Instance.LogOut();

            UpdateMenuState();
        }

        #endregion ///<帮助方法

        #region 测试数据
        private void GenerateTestData()
        {
            #region LIST_PAGE
            // 初始化系统信息
            //for (int i = 0; i < 40; ++i)
            //{
            //    if (5 == i % 10)
            //    {
            //        m_listFormSystemInfo.AddText(String.Format("   \t{0} 错误{1}", DateTime.Now, i), CListFormTabPage.EMsgState.EError);
            //    }
            //    else
            //    {
            //        m_listFormSystemInfo.AddText(String.Format("###\t{0} 正常{1}", DateTime.Now, i), CListFormTabPage.EMsgState.ENormal);
            //    }
            //}


            // 初始化串口状态信息
            for (int i = 0; i < 16; ++i)
            {
                if (5 == i % 10)
                {
                    //m_listFormComState.AddText(String.Format("北斗COM({0})", i + 1), CListFormTabPage.EMsgState.EError);
                    //(m_listFormComState.ListView as CListViewSerialPortState)
                }
                else
                {
                    //m_listFormComState.AddText(String.Format("北斗COM({0})", i + 1), CListFormTabPage.EMsgState.ENormal);
                }
            }
            #endregion ///<LIST_PAGE

            m_listSimulators = new List<Thread>();

            List<CEntityStation> listStation = CDBDataMgr.Instance.GetAllStation();
            int groupcount = 100; //一次性提交站点的个数
            for (int i = 0; i < listStation.Count; i += groupcount)
            {
                //Task task = new Task(() => { SimulatorStation(listStation[i]); });
                Thread t = new Thread(new ParameterizedThreadStart(SimulatorStationData));
                List<CEntityStation> listStationArgs = new List<CEntityStation>();
                listStationArgs.Add(listStation[i]);
                t.Start(listStationArgs);
                /*Thread t = new Thread(new ParameterizedThreadStart(SimulatorStationDatas));
                List<CEntityStation> listStationArgs = new List<CEntityStation>();
                for (int min = i; min < listStation.Count && min < i + groupcount; ++min)
                {
                    listStationArgs.Add(listStation[min]);
                }
                t.Start(listStationArgs);*/
                m_listSimulators.Add(t);
                break;
            }

        }

        public static void SimulatorStationData(object stations)
        {
            // 模拟站点发送数据,每隔一分钟发一次
            List<CEntityStation> listStation = stations as List<CEntityStation>;
            Debug.WriteLine(String.Format("->站点{0} 线程启动", listStation[0].StationID));
            Random random = new Random();
            DateTime baseTime = new DateTime(2014, 1, 1);
            Thread.Sleep(5 * 1000);
            //while (true)
            //{
            CEventRecvStationDataArgs args = new CEventRecvStationDataArgs();
            args.DataTime = baseTime;
            args.EChannelType = EChannelType.GPRS;
            args.EMessageType = EMessageType.ETimed;
            args.StrSerialPort = "串口4";
            Decimal baseRain = 1.0M;
            Decimal baseWater = 1000M;
            foreach (CEntityStation station in listStation)
            {
                args.EStationType = (station as CEntityStation).StationType;
                args.StrStationID = (station as CEntityStation).StationID;

                args.DataTime = baseTime.AddHours(1);
                args.RecvDataTime = args.DataTime.AddMinutes(random.Next(0, 15));
                if (args.EStationType == EStationType.EHydrology || args.EStationType == EStationType.ERainFall)
                {
                    // 雨量
                    args.TotalRain = baseRain + (Decimal)random.Next(0, 1);

                }
                if (args.EStationType == EStationType.EHydrology || args.EStationType == EStationType.ERiverWater)
                {
                    // 水位
                    args.WaterStage = baseWater + (Decimal)random.NextDouble();

                }
                args.Voltage = random.Next(10, 25) - 10;
                CDBDataMgr.Instance.EHRecvStationData(args);

            }
            baseTime = baseTime.AddHours(1); //时间增加一小时
            baseRain = baseRain + random.Next(0, 10);
            baseWater = baseWater + random.Next(0, 10);
            Debug.WriteLine("Time:" + baseTime.ToString());

            Thread.Sleep(60 * 1000); //60 s
            //} // end of while
        }

        public static void SimulatorStationDatas(object stations)
        {
            // 模拟站点发送数据,每隔一分钟发一次
            List<CEntityStation> listStation = stations as List<CEntityStation>;
            Debug.WriteLine(String.Format("->站点{0} 多数据线程启动 共{1}个站点", listStation[0].StationID, listStation.Count));
            Random random = new Random();
            DateTime baseTime = new DateTime(2010, 1, 1);
            Thread.Sleep(10 * 1000);
            Decimal baseRain = 1.0M; // 雨量0.1增长
            Decimal baseWater = 1000M;
            while (true)
            {
                foreach (CEntityStation station in listStation)
                {
                    CEventRecvStationDatasArgs args = new CEventRecvStationDatasArgs();
                    //args.DataTime = baseTime;
                    args.EChannelType = EChannelType.GPRS;
                    args.EMessageType = EMessageType.ETimed;
                    args.StrSerialPort = "串口4";
                    args.EStationType = (station as CEntityStation).StationType;
                    args.StrStationID = (station as CEntityStation).StationID;
                    // 12条记录，五分钟采集一次
                    for (int i = 0; i < 12; ++i)
                    {
                        CSingleStationData data = new CSingleStationData();
                        data.DataTime = baseTime.AddMinutes(i * 5); //五分钟一条数据
                        if (args.EStationType == EStationType.EHydrology || args.EStationType == EStationType.ERainFall)
                        {
                            // 雨量
                            data.TotalRain = baseRain + (Decimal)random.Next(0, 2);
                            baseRain = data.TotalRain.Value;

                        }
                        if (args.EStationType == EStationType.EHydrology || args.EStationType == EStationType.ERiverWater)
                        {
                            // 水位
                            data.WaterStage = baseWater + (Decimal)random.NextDouble();

                        }
                        data.Voltage = random.Next(10, 25);
                        args.RecvDataTime = data.DataTime.AddMinutes(random.Next(0, 15));
                        args.Datas.Add(data);
                    }
                    CDBDataMgr.Instance.EHRecvStationDatas(null, args);
                    Thread.Sleep(200);
                }//end of foreach
                baseTime = baseTime.AddHours(1); //时间增加一小时
                //baseRain = baseRain + random.Next(0, 10);
                baseWater = baseWater + random.Next(0, 10);
                Debug.WriteLine(string.Format("Time:{0} --->stationstart: {1}", baseTime.ToString(), listStation[0].StationID));

                Thread.Sleep(5 * 60 * 1000); //60 s
            } // end of while
        }

        // 更新实时数据的线程模拟
        public void SimulatorRTD()
        {
            int count = 0;
            while (true)
            {
                Thread.Sleep(10 * 1000);
                for (int i = 1; i < 401; ++i)
                {
                    CEntityRealTime entity = new CEntityRealTime();
                    entity.StrStationName = String.Format("站点{0}", i);
                    entity.StrStationID = (1000 + i).ToString();
                    entity.DDayRainFall = 1000 + i * 2 + count * 2;
                    entity.StrPort = "COM4";
                    entity.TimeDeviceGained = DateTime.Now;
                    entity.TimeReceived = DateTime.Now.AddMinutes(count % 7 + 4);
                    entity.EIChannelType = EChannelType.BeiDou;
                    entity.EIMessageType = EMessageType.ETimed;
                    entity.EIStationType = EStationType.ERainFall;
                    m_formRTD.UpdateRTD(entity);
                    // 睡眠10s钟
                    Thread.Sleep(10 * 1000);
                }
                ++count;
            }
        }
        #endregion ///<测试数据

    
        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            m_timer.Stop();
            m_timer.Start();
        }

        private void MainForm_MouseEnter(object sender, EventArgs e)
        {
            m_timer.Stop();
            m_timer.Start();
        }









    } // end of class

}//end of namespace MDIParent
