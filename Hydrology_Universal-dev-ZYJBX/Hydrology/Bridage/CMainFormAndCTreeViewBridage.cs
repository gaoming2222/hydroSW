using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using Hydrology.Forms;
using Protocol.Manager;
using System.Xml;

namespace Hydrology.Bridage
{
    public class CMainFormAndCTreeViewBridage
    {
        private static CExTabControl m_tabControlUp;           //右上Tab控件
        private static MainForm m_refMainForm;                  //主窗体

        private delegate void PopupFormDelegate(object obj);
        private static CDictionary<CTreeType, PopupFormDelegate> PopupFormMap = new CDictionary<CTreeType, PopupFormDelegate>()
        {
          { CTreeType.CeZhan3, new PopupFormDelegate(CPopupFormHelper.PopupCStationMgrForm)},
          { CTreeType.CommunicationPort2,new PopupFormDelegate(CPopupFormHelper.PopupCSerialPortConfigForm) },
          {CTreeType.DataProtocolCfg2,new PopupFormDelegate(CPopupFormHelper.PopupCDataConfigForm)},
          {CTreeType.ChannelProtocolCfg2,new PopupFormDelegate(CPopupFormHelper.PopupCChannelConfigForm)},
          {CTreeType.UserSetting,new PopupFormDelegate(CPopupFormHelper.PopupCUserMgrForm) },
          //{CTreeType.UserLogin,new PopupFormDelegate(CPopupFormHelper.PopupCUserMgrForm) }
        };
        private static void PopupForm(CTreeType treeType, object obj)
        {
            PopupFormMap[treeType](obj);
        }
        internal class CPopupFormHelper
        {
            public static void PopupCStationMgrForm(object obj)
            {
                if (CCurrentLoginUser.Instance.IsAdmin)
                {
                    var node = obj as CTreeNode;
                    if (node == null)
                        return;
                    if (node.SelectedImageIndex == 15)
                    {
                        var window = new CSoilStationMgrForm();
                        PopUpWindow(window);
                    }
                    else
                    {
                        var window = new CStationMgrForm();
                        PopUpWindow(window);
                    }

                    //              window.SetDefaultStation(node.ID);

                }
            }
            public static void PopupCSerialPortConfigForm(object obj)
            {
                if (CCurrentLoginUser.Instance.IsAdmin)
                {
                    var window = new CSerialPortConfigForm();
                    PopUpWindow(window);
                }
            }
            public static void PopupCDataConfigForm(object obj)
            {
                if (CCurrentLoginUser.Instance.IsAdmin)
                {
                    var window = new CProtocolConfigForm(false);
                    var node = obj as CTreeNode;
                    if (node != null)
                    {
                        window.SetDefaultText(node.Text);
                    }
                    window.ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                    PopUpWindow(window);
                }
            }
            public static void PopupCChannelConfigForm(object obj)
            {
                if (CCurrentLoginUser.Instance.IsAdmin)
                {
                    var window = new CProtocolConfigForm(true);
                    var node = obj as CTreeNode;
                    if (node != null)
                    {
                        window.SetDefaultText(node.Text);
                    }
                    window.ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                    PopUpWindow(window);
                }
            }
            public static void PopupCUserMgrForm(object obj)
            {
                //  如果是管理员
                if (CCurrentLoginUser.Instance.IsAdmin)
                {
                    //  弹出“用户管理”界面
                    var window = new CUserMgrForm();
                    PopUpWindow(window);
                }
            }

            private static void PopUpWindow(Form form)
            {
                if (form == null)
                    return;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }

        private static void Instance_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var treeview = sender as CTree;
            if (treeview == null)
                return;
            var node = treeview.SelectedNode as CTreeNode;
            if (node == null)
                return;
            switch (node.Type)
            {
                case CTreeType.CeZhan3:
                case CTreeType.CommunicationPort2:
                case CTreeType.DataProtocolCfg2:
                case CTreeType.ChannelProtocolCfg2:
                    PopupForm(node.Type, node);
                    break;
                case CTreeType.CeZhan2:
                    if (node.Nodes.Count == 0)
                    {
                        m_refMainForm.ShowRTDOfTabPage(node.Text);
                    }
                    else if (node.IsExpanded)
                    {
                        m_refMainForm.ShowRTDOfTabPage(node.Text);
                    }
                    break;

                case CTreeType.CommunicationPort:
                    if (node.IsExpanded)
                    {
                        AddPortSettingsTab();
                    }
                    break;
                case CTreeType.DataProtocolCfg:
                    if (node.IsExpanded)
                    {
                        AddProtocolTab();
                    }
                    break;
                case CTreeType.ChannelProtocolCfg:
                    if (node.IsExpanded)
                    {
                        AddChannelTab();
                    }
                    break;
                case CTreeType.UserSetting: AddUserTab(); break;
                default: break;
            }
        }

        public static void LoadTreeView(MainForm mainform, CExTabControl tabControl)
        {
            try
            {
                CTree.Instance.LoadTree();
            }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
            catch (System.Exception exp) { }
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过
            CTree.Instance.NodeMouseDoubleClick += CMainFormAndCTreeViewBridage.Instance_NodeMouseDoubleClick;
            m_tabControlUp = tabControl;
            m_refMainForm = mainform;
            mainform.splitContainer1.Panel1.Controls.Add(CTree.Instance);
        }

        private static CDataGridTabPage m_SerialPortTabPage;
        private static void AddPortSettingsTab()
        {
            if (m_SerialPortTabPage != null)
            {
                m_SerialPortTabPage.DataGrid.MouseDoubleClick -= new MouseEventHandler(SerialPortTabPage_DataGrid_MouseDoubleClick);
                m_tabControlUp.RemovePage(m_SerialPortTabPage);
                m_SerialPortTabPage = null;
            }
            m_SerialPortTabPage = new CDataGridTabPage()
            {
                Title = "串口配置",
                BTabRectClosable = true,
                //DataGrid = new CDataGridViewSerialPort()
                DataGrid = new CExDataGridView()
                {
                    Header = new string[] { "串口号", "通讯方式", "数据协议", "波特率", "数据位", "停止位", "校验方式","当前状态" }
                }
            };

            //(m_SerialPortTabPage.DataGrid as CDataGridViewSerialPort).InitDataSource(CDBDataMgr.GetInstance().GetSerialPortProxy());
            //(m_SerialPortTabPage.DataGrid as CDataGridViewSerialPort).LoadData();
            m_SerialPortTabPage.DataGrid.MouseDoubleClick += new MouseEventHandler(SerialPortTabPage_DataGrid_MouseDoubleClick);
            m_SerialPortTabPage.DisabledLeftLabel();
            m_SerialPortTabPage.TabClosed += (s, e) =>
            {
                m_SerialPortTabPage = null;
            };
            m_tabControlUp.AddPage(m_SerialPortTabPage);
            m_tabControlUp.SelectedIndex = m_tabControlUp.TabPages.Count - 1;
            var listPorts = CDBDataMgr.Instance.GetAllSerialPort();
            //  添加串口数据到表格中
            foreach (var item in listPorts)
            {
                int com = item.PortNumber;
                var channelDll = XmlDocManager.Instance.GetChannelDllByComOrPort(com, true);
                var dataDll = XmlDocManager.Instance.GetDataDllByComOrPort(com, true);
                if (channelDll == null || dataDll == null)
                    continue;
                m_SerialPortTabPage.DataGrid.AddRow(new string[] 
                { 
                    "COM" + item.PortNumber,
                    channelDll.Name,
                    dataDll.Name,
                    item.Baudrate.ToString(),
                    item.DataBit.ToString(),
                    item.StopBit.ToString(),
                    CEnumHelper.PortParityTypeToUIStr(item.ParityType).ToString(),
                    (item.SwitchSatus.Value?"开启":"关闭"),
                }, CExDataGridView.EDataState.ENormal);
            }
            //  更新UI
            m_SerialPortTabPage.DataGrid.UpdateDataToUI();
        }
        private static void SerialPortTabPage_DataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PopupForm(CTreeType.CommunicationPort2, null);
        }

        private static CDataGridTabPage m_ProtocolTabPage;
        private static void AddProtocolTab()
        {
            if (m_ProtocolTabPage != null)
            {
                m_ProtocolTabPage.DataGrid.CellDoubleClick -= new DataGridViewCellEventHandler(ProtocolTabPage_DataGrid_CellDoubleClick);
                m_tabControlUp.RemovePage(m_ProtocolTabPage);
                m_ProtocolTabPage = null;
            }
            m_ProtocolTabPage = new CDataGridTabPage()
            {
                Title = "数据协议",
                BTabRectClosable = true,
                DataGrid = new CExDataGridView()
                {
                    Header = new string[] { "数据协议" }
                }
            };
            m_ProtocolTabPage.DataGrid.CellDoubleClick += new DataGridViewCellEventHandler(ProtocolTabPage_DataGrid_CellDoubleClick);
            m_ProtocolTabPage.DisabledLeftLabel();
            var m_dllCollections = Protocol.Manager.XmlDocManager.Deserialize();
            m_ProtocolTabPage.TabClosed += (s, e) =>
            {
                m_ProtocolTabPage = null;
            };
            m_tabControlUp.AddPage(m_ProtocolTabPage);
            m_tabControlUp.SelectedIndex = m_tabControlUp.TabPages.Count - 1;
            foreach (var item in m_dllCollections.Infos)
            {
                if (item.Type == "data" && item.Enabled)
                {
                    m_ProtocolTabPage.DataGrid.AddRow(new string[] { item.Name }, CExDataGridView.EDataState.ENormal);
                }
            }
          
            //  更新UI
            m_ProtocolTabPage.DataGrid.UpdateDataToUI();
        }
        static void ProtocolTabPage_DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CCurrentLoginUser.Instance.IsLogin)
            {
                var name = m_ProtocolTabPage.DataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                var window = new CProtocolConfigForm(false)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                window.SetDefaultText(name);
                window.ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                window.ShowDialog();
            }
        }

        private static CDataGridTabPage m_ChannelTabPage;
        private static void AddChannelTab()
        {
            if (m_ChannelTabPage != null)
            {
                m_ChannelTabPage.DataGrid.CellDoubleClick -= new DataGridViewCellEventHandler(ChannelTabPage_DataGrid_CellDoubleClick);
                m_tabControlUp.RemovePage(m_ChannelTabPage);
                m_ChannelTabPage = null;
            }
            m_ChannelTabPage = new CDataGridTabPage()
            {
                Title = "通讯方式",
                BTabRectClosable = true,
                DataGrid = new CExDataGridView()
                {
                    Header = new string[] { "通讯方式" }
                }
            };
            m_ChannelTabPage.DataGrid.CellDoubleClick += new DataGridViewCellEventHandler(ChannelTabPage_DataGrid_CellDoubleClick);
            m_ChannelTabPage.DisabledLeftLabel();
            var m_dllCollections = Protocol.Manager.XmlDocManager.Deserialize();
            m_ChannelTabPage.TabClosed += (s, e) =>
            {
                m_ChannelTabPage = null;
            };
            m_tabControlUp.AddPage(m_ChannelTabPage);
            m_tabControlUp.SelectedIndex = m_tabControlUp.TabPages.Count - 1;
            foreach (var item in m_dllCollections.Infos)
            {
                if (item.Type == "channel" && item.Enabled)
                {
                    m_ChannelTabPage.DataGrid.AddRow(new string[] { item.Name }, CExDataGridView.EDataState.ENormal);
                }
            }
            //  更新UI
            m_ChannelTabPage.DataGrid.UpdateDataToUI();
        }
        static void ChannelTabPage_DataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CCurrentLoginUser.Instance.IsAdmin)
            {
                var name = m_ChannelTabPage.DataGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                var window = new CProtocolConfigForm(true)
                {
                    StartPosition = FormStartPosition.CenterScreen
                };
                window.SetDefaultText(name);
                window.ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                window.Show();
            }
        }

        private static CDataGridTabPage m_UserTabPage;
        private static void AddUserTab()
        {
            if (m_UserTabPage != null)
            {
                m_UserTabPage.DataGrid.MouseDoubleClick -= new MouseEventHandler(UserTabPage_DataGrid_MouseDoubleClick);
                m_tabControlUp.RemovePage(m_UserTabPage);
                m_UserTabPage = null;
            }
            m_UserTabPage = new CDataGridTabPage()
            {
                Title = "登录用户",
                BTabRectClosable = true,
                DataGrid = new CExDataGridView()
                {
                    Header = new string[] { "用户名", "口令", "权限" }
                }
            };
            m_UserTabPage.DataGrid.MouseDoubleClick += new MouseEventHandler(UserTabPage_DataGrid_MouseDoubleClick);
            m_UserTabPage.DisabledLeftLabel();
            m_UserTabPage.TabClosed += (s, e) =>
            {
                m_UserTabPage = null;
            };
            m_tabControlUp.AddPage(m_UserTabPage);
            m_tabControlUp.SelectedIndex = m_tabControlUp.TabPages.Count - 1;
            bool islogin = CCurrentLoginUser.Instance.IsLogin;
            if (islogin)
            {
                string name = CCurrentLoginUser.Instance.Name;
                string admin = CCurrentLoginUser.Instance.IsAdmin ? "管理员" : "普通用户";
                m_UserTabPage.DataGrid.AddRow(new string[] 
                {
                    name,
                    "******",
                   admin
                }, CExDataGridView.EDataState.ENormal);
            }
            //  更新UI
            m_UserTabPage.DataGrid.UpdateDataToUI();
        }
        static void UserTabPage_DataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PopupForm(CTreeType.UserSetting, null);
        }


    }

}
