using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;
using Hydrology.Forms;
using Protocol.Manager;
using System.Drawing;

namespace Hydrology.CControls
{
    /// <summary>
    /// 串口配置，至少要有一个信道协议，
    /// </summary>
    class CDataGridViewSerialPort : CExDataGridView
    {
        public event EventHandler<CEventSingleArgs<int>> SerialPortCountChanged;

        #region 静态常量

        private static readonly string CS_PortNumber = "串口号";
        private static readonly string CS_Delete = "删除";
        private static readonly string CS_TransType = "通讯方式1";
        private static readonly string CS_ProtocolChannel = "通讯方式";
        private static readonly string CS_ProtocolData = "数据协议";
        private static readonly string CS_Baudrate = "波特率";
        private static readonly string CS_DataBit = "数据位";
        private static readonly string CS_StopBit = "停止位";
        private static readonly string CS_ParityType = "校验方式";
        private static readonly string CS_Stream = "流控制方式";
        private static readonly string CS_Break = "中断开关";
        private static readonly string CS_Open = "当前状态";

        private static readonly string CS_PrePortNumber = "之前的串口号";

        private static readonly string CS_PID = "pid"; // 用来表示是否是添加的串口号， -1表示新增的串口号

        private static readonly string CS_Break_Enabled = "启用"; // 对应于false
        private static readonly string CS_Break_Disabled = "不启用"; // 对应于true

        private static readonly string CS_SwitchStatus_Open = "开启";
        private static readonly string CS_SwitchStatus_Close = "关闭";

        private static readonly string CS_None = "无"; //信道协议或者数据协议为无

        #endregion ///<静态常量

        private int m_iPortCount = 0;
        public int PortCount
        {
            get { return m_iPortCount; }
        }

        private ISerialPortProxy m_porxy;
        private List<CEntitySerialPort> m_listUpdatedPorts; //  更新串口集合
        private List<int> m_listDeletedPorts;               //  删除串口集合
        private List<CEntitySerialPort> m_listAddedPort;    //  添加串口集合
        private List<int> m_listActualPortNumber;           //  机器的串口号

        private List<string> m_listProtocolCom;             // 串口信道协议列表
        private List<string> m_listProtocolData;            // 数据协议列表
        private List<CPortProtocolConfig> m_listPortConfig; // 所有的串口和协议配置

        public CDataGridViewSerialPort()
            : base()
        {
            this.m_listUpdatedPorts = new List<CEntitySerialPort>();
            this.m_listDeletedPorts = new List<int>();
            this.m_listAddedPort = new List<CEntitySerialPort>();

            m_listActualPortNumber = new List<int>();

            // 初始化串口列表
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; ++i)
            {
                try
                {
                    int tmp = int.Parse(ports[i].Substring(3));
                    m_listActualPortNumber.Add(tmp);
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            // 读取配置文件，获取当前的信道协议与数据协议配置
            m_listProtocolData = XmlDocManager.Instance.DataProtocolNames;
            m_listProtocolCom = XmlDocManager.Instance.ComProtocolChannelNames;
            // 当前的端口配置协议
            m_listPortConfig = XmlDocManager.Instance.GetComOrPortConfig(true);
        }

        public void InitDataSource(ISerialPortProxy proxy)
        {
            this.m_porxy = proxy;
        }

        public bool LoadData()
        {
            // 判断是否有数据协议或者信道协议
            if (m_listProtocolCom.Count <= 0 || m_listProtocolData.Count <= 0)
            {
                MessageBox.Show("请先配置通讯方式和数据协议");
                // 关闭窗体
                return false;
            }
            if (m_porxy != null)
            {
                this.SetPorts(CDBDataMgr.Instance.GetAllSerialPort());
                return true;
            }
            return false;
        }

        public void DoDelete()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            //GetUpdatedData();
            foreach (var item in base.m_listMaskedDeletedRows)
            {
                string tag = base.Rows[item].Cells[CS_PID].Value.ToString();
                // 并且从编辑项中减去这一行
                m_listEditedRows.Remove(item);
                if ("-1" != tag)
                {
                    m_listDeletedPorts.Add(Int32.Parse(base.Rows[item].Cells[CS_PortNumber].Value.ToString()));
                }
            }
            for (int i = 0; i < m_listMaskedDeletedRows.Count; ++i)
            {
                base.DeleteRowData(m_listMaskedDeletedRows[i]);
            }
            m_listMaskedDeletedRows.Clear(); //清空
            base.UpdateDataToUI();
            UpdatePortsCount(m_dataTable.Rows.Count);
        }

        public override bool DoSave()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            if (!AssertSaveData())
            {
                // 有非法数据 
                return false;
            }
            GenaratePortXMLConfig();
            GetUpdatedData();
            GetDeletedData();
            if ( (m_listAddedPort.Count > 0 || m_listUpdatedPorts.Count > 0 || m_listDeletedPorts.Count > 0) && IsDataChanged())
            {
                // 应该作为一个事物一起处理
                //return base.DoSave();
                bool result = true;
                // 先删除，然后添加
                if (m_listDeletedPorts.Count > 0)
                {
                    // 删除
                    result = result && m_porxy.DeleteRange(m_listDeletedPorts);
                }
                if (m_listAddedPort.Count > 0)
                {
                    // 增加
                    result = result && m_porxy.AddRange(m_listAddedPort);
                }
                if (m_listUpdatedPorts.Count > 0)
                {
                    // 更新
                    result = result && m_porxy.UpdateRange(m_listUpdatedPorts);
                }
                
                result = result &&  XmlDocManager.Instance.ResetComOrPortConfig(m_listPortConfig,true);
                if (result)
                {
                    CMessageBox box = new CMessageBox();
                    box.MessageInfo = "正在更新串口";
                    //box.StartPosition = new Point(0, 0);
                    box.StartPosition = FormStartPosition.Manual;

                  
                    int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度

                    int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
                    box.Top = (int)(yHeight*0.3);
                    box.Left = (int)(xWidth* 0.4); 
               //     box.Location = new Point(SystemInformation.WorkingArea.Width - this.Width, SystemInformation.WorkingArea.Height - this.Height);
                    box.ShowDialog(this);
                    // 通知其他界面更新消息
                    CDBDataMgr.Instance.UpdateAllSerialPort();
                    box.CloseDialog();
                    this.Hide();
                    MessageBox.Show("保存成功");
                    this.Show();
                }
                else
                {
                    this.Hide();
                    //MessageBox.Show("保存失败, 串口配置或者数据库连接错误");
                    MessageBox.Show("保存失败, 连接通信设备失败");
                    this.Show();
                }
                // 重新加载
                ClearAllState();
                m_listPortConfig = XmlDocManager.Instance.GetComOrPortConfig();
                LoadData();
                UpdateDataToUI();
            }
            else
            {
                this.Revert();
                MessageBox.Show("没有任何修改，无需保存");
            }

            return true;
        }

        public bool DoClearGSM()
        {
            bool flag = false;
            CPortDataMgr cdm = CPortDataMgr.Instance;


            // 判断串口号不能为空,以及不能重复
            for (int i = 0; i < this.Rows.Count; ++i)
            {
                if (base.m_dataTable.Rows[i][CS_ProtocolChannel].Equals("GSM"))
                {
                    int portId = 0;
                    if (base.m_dataTable.Rows[i][CS_PortNumber].Equals(""))
                    {
                        MessageBox.Show("串口号为空，不能进行清空操作");
                        return false;
                    }
                    else
                    {

                        portId = int.Parse(base.m_dataTable.Rows[i][CS_PortNumber].ToString());
                    }
                    if (!base.m_dataTable.Rows[i][CS_Open].Equals("开启"))
                    {
                        MessageBox.Show("串口未开启，不能进行清空短信操作");
                        return false;
                    }

                    CPortDataMgr.ClearGSM(portId);

                }
            }
            return flag;
        }

        public void SetEditMode(bool bEnable)
        {
            if (bEnable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                    CS_Delete, 
                    CS_PortNumber,
                    CS_TransType,
                    CS_ProtocolChannel,
                    CS_ProtocolData,
                    CS_Baudrate,
                    CS_DataBit,
                    CS_StopBit,
                    CS_ParityType ,
                    CS_Stream ,
                    CS_Break,
                    CS_Open ,
                    CS_PID,/*用来标识是否是添加的串口号*/
                    CS_PrePortNumber /*之前的串口号，用来更改串口号，如果更改串口号，生成两条语句*/
                };
               // this.HideColomns = new int[] { 2, 8, 9, 10, /*11,*/12, 13 };
                this.HideColomns = new int[] { 2,  9, 10, /*11,*/12, 13 };
                var delCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, delCol);

                var portNumberCol = new DataGridViewComboBoxColumn();
                portNumberCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                // 16个串口
                /*portNumberCol.Items.AddRange(new object[]
                {
                    "1","2","3","4","5","6","7","8","9","10",
                    "11","12","13","14","15","16"
                });*/
                foreach (int portnumber in m_listActualPortNumber)
                {
                    portNumberCol.Items.Add(portnumber.ToString());
                }


                base.SetColumnEditStyle(1, portNumberCol);

                var transCol = new DataGridViewComboBoxColumn();
                transCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                transCol.Items.AddRange(new object[] 
                {
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.Nothing),
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.EUSM),
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.ETelephone),
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.EBDSatelite),
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.EMarisat),
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.ESMS)
                });
                base.SetColumnEditStyle(2, transCol);

                // 信道协议
                DataGridViewComboBoxColumn colProtocolChannel = new DataGridViewComboBoxColumn();
                colProtocolChannel.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                colProtocolChannel.Items.Add(CS_None);
                // 添加信道协议
                for (int i = 0; i < m_listProtocolCom.Count; ++i)
                {
                    colProtocolChannel.Items.Add(m_listProtocolCom[i]);
                }
                base.SetColumnEditStyle(3, colProtocolChannel);

                // 数据协议
                DataGridViewComboBoxColumn colProtocolData = new DataGridViewComboBoxColumn();
                colProtocolData.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                colProtocolData.Items.Add(CS_None);
                // 添加数据协议
                for (int i = 0; i < m_listProtocolData.Count; ++i)
                {
                    colProtocolData.Items.Add(m_listProtocolData[i]);
                }
                base.SetColumnEditStyle(4, colProtocolData);

                //  edit by xiewj
                //var transCol = new DataGridViewComboBoxColumn();
                //transCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                //var items = Protocol.Manager.XmlDocManager.Instance.ChannelProtocolNames;
                //foreach (var item in items)
                //{
                //    transCol.Items.Add(item);
                //}
                //base.SetColumnEditStyle(2, transCol);

                var baudrateCol = new DataGridViewComboBoxColumn();
                baudrateCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                baudrateCol.Items.AddRange(new object[] 
                {
                   /* "300","600",*/"1200","2400","4800","9600","19200","38400","57600","115200","230400","460800","921600"
                });
                base.SetColumnEditStyle(5, baudrateCol);

                var dataBitCol = new DataGridViewComboBoxColumn();
                dataBitCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                dataBitCol.Items.AddRange(new object[] 
                {
                    "8","7","6"
                });
                base.SetColumnEditStyle(6, dataBitCol);

                var stopBitCol = new DataGridViewComboBoxColumn();
                stopBitCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                stopBitCol.Items.AddRange(new object[] 
                {
                    "1","2"
                });
                base.SetColumnEditStyle(7, stopBitCol);

                //CS_ParityType ,
                //    CS_Stream ,
                //    CS_Break,
                //    CS_Open ,
                //    CS_PID

                var parityCol = new DataGridViewComboBoxColumn();
                parityCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                parityCol.Items.AddRange(new object[] 
                {
                    CEnumHelper.PortParityTypeToUIStr(EPortParityType.ENone),
                    CEnumHelper.PortParityTypeToUIStr(EPortParityType.EEven),
                    CEnumHelper.PortParityTypeToUIStr(EPortParityType.EOdd)
                });
                base.SetColumnEditStyle(8, parityCol);

                var streamCol = new DataGridViewComboBoxColumn();
                streamCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                streamCol.Items.AddRange(new object[] 
                {
                    CEnumHelper.SerialPortStreamTypeToUIStr(ESerialPortStreamType.ENone),
                    CEnumHelper.SerialPortStreamTypeToUIStr(ESerialPortStreamType.ERts),
                    CEnumHelper.SerialPortStreamTypeToUIStr(ESerialPortStreamType.EDtr),
                });
                base.SetColumnEditStyle(9, streamCol);

                var breakCol = new DataGridViewComboBoxColumn();
                breakCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                breakCol.Items.AddRange(new object[] { CS_Break_Disabled,
                    CS_Break_Enabled, });
                base.SetColumnEditStyle(10, breakCol);

                var openCol = new DataGridViewComboBoxColumn();
                openCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                openCol.Items.AddRange(new object[] { CS_SwitchStatus_Open,
                    CS_SwitchStatus_Close, });
                base.SetColumnEditStyle(11, openCol);

                var pidCol = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(12, pidCol);
            }
            else
            {
                this.Header = new string[] 
                { 
                    CS_Delete, 
                    CS_PortNumber,
                    CS_TransType,
                    CS_ProtocolChannel,
                    CS_ProtocolData,
                    CS_Baudrate,
                    CS_DataBit,
                    CS_StopBit,
                    CS_ParityType ,
                    CS_Stream ,
                    CS_Break,
                    CS_Open ,
                    CS_PID,/*用来标识是否是添加的串口号*/
                    CS_PrePortNumber /*之前的串口号，用来更改串口号，如果更改串口号，生成两条语句*/
                };
            }
        }

        public void AddNewPort()
        {
            // 判断当前串口数为多少，如果已经有16个，则退出
            if (base.m_dataTable.Rows.Count == m_listActualPortNumber.Count)
            {
                MessageBox.Show("当前配置已经达到了串口的最大值,无法继续添加串口配置");
                return;
            }
            base.AddRow(new string[] 
                {
                    "False",
                    "",
                    CEnumHelper.SerialTransTypeToUIStr(ESerialPortTransType.Nothing),
                    CS_None, /*信道协议*/
                    CS_None, /*数据协议*/
                    "9600",
                    "8",
                    "1",
                    CEnumHelper.PortParityTypeToUIStr(EPortParityType.ENone),
                    CEnumHelper.SerialPortStreamTypeToUIStr(ESerialPortStreamType.ENone),
                    CS_Break_Disabled,
                    CS_SwitchStatus_Open, /*默认启用*/
                    "-1",
                    "-1" /*之前的串口号不存在*/
                }, EDataState.ENormal,false);
            base.m_listEditedRows.Add(m_dataTable.Rows.Count - 1);
            UpdatePortsCount(base.m_dataTable.Rows.Count);
            base.ClearSelection();
            base.UpdateDataToUI();
        }

        public bool Close()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            if (!AssertSaveData())
            {
                // 不让退出
                DialogResult result = MessageBox.Show("是否强行退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    return true;
                }
                return false;
            }
            // 关闭方法
            GetUpdatedData();
            GetDeletedData();
            if ( (m_listAddedPort.Count > 0 || m_listUpdatedPorts.Count > 0 || m_listDeletedPorts.Count > 0 ) && IsDataChanged())
            {
                DialogResult result = MessageBox.Show("当前所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    return false;
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    DoSave();
                }
                else if (DialogResult.No == result)
                {
                    return true;
                }
            }
            return true;
        }

        public void Revert()
        {
            // 清空所有状态
            this.ClearAllState();
            m_listPortConfig = XmlDocManager.Instance.GetComOrPortConfig(true);
            this.LoadData();
            UpdateDataToUI();
        }

        private void SetPorts(IList<CEntitySerialPort> ports)
        {
            base.m_dataTable.Rows.Clear();
            base.m_dataTableBak.Rows.Clear();
            int iActualPortCount = 0;
            string strMessageInfo = "";
            bool _bDoDelete = false; //是否需要修改数据库，默认为false
            List<int> _listDoDelete = new List<int>();
            foreach (var item in ports)
            {
                if (!m_listActualPortNumber.Contains(item.PortNumber))
                {
                    _bDoDelete = true;
                    _listDoDelete.Add(item.PortNumber); //删除列表
                    strMessageInfo += string.Format("数据库中配置的COM{0}不存在，自动删除\r\n", item.PortNumber);
                    continue;
                }

                // 获取数据协议与信道协议的配置
                CPortProtocolConfig config = GetPortConfig(item.PortNumber);
                if (config == null)
                {
                    config = new CPortProtocolConfig();
                    config.ProtocolChannelName = CS_None;
                    config.ProtocolDataName = CS_None;
                }
                else
                {
                    if (config.ProtocolDataName == null || config.ProtocolDataName.Equals(""))
                    {
                        config.ProtocolDataName = CS_None;
                    }
                    if (config.ProtocolChannelName == null || config.ProtocolChannelName.Equals(""))
                    {
                        config.ProtocolChannelName = CS_None;
                    }
                }

                base.AddRow(new string[] 
                {
                    "False",
                    item.PortNumber.ToString(),
                    CEnumHelper.SerialTransTypeToUIStr(item.TransType),
                    config.ProtocolChannelName,
                    config.ProtocolDataName,
                    item.Baudrate.ToString(),
                    item.DataBit.ToString(),
                    item.StopBit.ToString(),
                    CEnumHelper.PortParityTypeToUIStr(item.ParityType),
                    CEnumHelper.SerialPortStreamTypeToUIStr(item.Stream),
                    item.Break.Value ? CS_Break_Enabled : CS_Break_Disabled,
                    item.SwitchSatus.Value ? CS_SwitchStatus_Open : CS_SwitchStatus_Close,
                    "1",
                    item.PortNumber.ToString()
                }, EDataState.ENormal);
                iActualPortCount += 1;
            }
            UpdatePortsCount(iActualPortCount);
            if (!strMessageInfo.Equals(""))
            {
                MessageBox.Show(strMessageInfo);
                if (_bDoDelete)
                {
                    m_porxy.DeleteRange(_listDoDelete);
                    // 通知其他界面更新消息
                    CDBDataMgr.Instance.UpdateAllSerialPort();
                }
            }

        }

        private void GetUpdatedData()
        {
            // 标记为删除的就不需要添加的修改或者添加的分中心中了
            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 将去重后的项赋给编辑项
            base.m_listEditedRows = listEditRows;
            // 获取修改了的数据
            foreach (var item in base.m_listEditedRows)
            {
                var port = new CEntitySerialPort();
                port.PortNumber = Int32.Parse(base.Rows[item].Cells[CS_PortNumber].Value.ToString());
                port.TransType = CEnumHelper.UIStrToTransType(base.Rows[item].Cells[CS_TransType].Value.ToString());
                port.Baudrate = Int32.Parse(base.Rows[item].Cells[CS_Baudrate].Value.ToString());
                port.DataBit = Int32.Parse(base.Rows[item].Cells[CS_DataBit].Value.ToString());
                port.StopBit = Int32.Parse(base.Rows[item].Cells[CS_StopBit].Value.ToString());
                port.ParityType = CEnumHelper.UIStrToParityType(base.Rows[item].Cells[CS_ParityType].Value.ToString());
                port.Stream = CEnumHelper.UIStrToSerialPortStreamType(base.Rows[item].Cells[CS_Stream].Value.ToString());
                port.Break = base.Rows[item].Cells[CS_Break].Value.ToString() == CS_Break_Enabled ? true : false;
                port.SwitchSatus = base.Rows[item].Cells[CS_Open].Value.ToString() == CS_SwitchStatus_Open ? true : false;

                int prePortNumber = Int32.Parse(base.Rows[item].Cells[CS_PrePortNumber].Value.ToString());

                string tag = base.Rows[item].Cells[CS_PID].Value.ToString();
                if (tag == "-1")
                {
                    // 新增的记录
                    m_listAddedPort.Add(port);
                }
                else
                {
                    // 修改的记录，判断如果串口号不一致，则变成删除以前的串口，添加一个新串口
                    if (port.PortNumber == prePortNumber)
                    {
                        m_listUpdatedPorts.Add(port);
                    }
                    else
                    {
                        // 新增一个记录，删除一个记录
                        m_listAddedPort.Add(port);
                        m_listDeletedPorts.Add(prePortNumber);
                    }
                }
            }
            m_listEditedRows.Clear();
        }

        private void GetDeletedData()
        {
            foreach (var item in base.m_listMaskedDeletedRows)
            {
                string tag = base.Rows[item].Cells[CS_PID].Value.ToString();
                // 新增的串口配置不需要删除
                if (tag != "-1")
                {
                    m_listDeletedPorts.Add(Int32.Parse(base.Rows[item].Cells[CS_PortNumber].Value.ToString()));
                }
            }
            m_listMaskedDeletedRows.Clear();
        }

        private void UpdatePortsCount(int count)
        {
            m_iPortCount = count;
            if (SerialPortCountChanged != null)
            {
                SerialPortCountChanged(this, new CEventSingleArgs<int>(count));
            }
        }

        private bool AssertSaveData()
        {
            // 判断当前数据是否合法
            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 将去重后的项赋给编辑项
            base.m_listEditedRows = listEditRows;

            // 判断串口号不能为空,以及不能重复
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                // 没有删除，并且不是新增的

                if (base.m_dataTable.Rows[listEditRows[i]][CS_PortNumber].Equals("")
                    )
                {
                    MessageBox.Show("串口号不能为空");
                    return false;
                }
                int port = int.Parse(base.m_dataTable.Rows[listEditRows[i]][CS_PortNumber].ToString());
                //if (portnumber.Contains(port))
                //{
                //    MessageBox.Show("串口号不能重复");
                //    return false;
                //}
                //portnumber.Add(port);
            }
            List<int> listCurrentComs = new List<int>();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                if (!m_listMaskedDeletedRows.Contains(i))
                {
                    //不包含，则认为有效
                    listCurrentComs.Add(int.Parse(m_dataTable.Rows[i][CS_PortNumber].ToString()));
                }
            }
            // 然后再看是否重复添加
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                if (!m_listMaskedDeletedRows.Contains(i))
                {
                    int comnumber = int.Parse(m_dataTable.Rows[i][CS_PortNumber].ToString());
                    if (listCurrentComs.IndexOf(comnumber) != listCurrentComs.LastIndexOf(comnumber))
                    {
                        // 串口号不能重复
                        MessageBox.Show("串口号不能重复");
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 查询端口号，如果没有找到匹配，返回NULL
        /// </summary>
        /// <param name="portNumber"></param>
        /// <returns></returns>
        private CPortProtocolConfig GetPortConfig(int portNumber)
        {
            for (int i = 0; i < m_listPortConfig.Count; ++i)
            {
                if (m_listPortConfig[i].PortNumber == portNumber)
                {
                    return m_listPortConfig[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 生成端口的信道协议与数据协议配置
        /// </summary>
        private void GenaratePortXMLConfig()
        {
            // 标记为删除的就不需要添加到结果集中了
            m_listPortConfig = new List<CPortProtocolConfig>();
            // 获取修改了的所有数据
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                if (!m_listMaskedDeletedRows.Contains(i))
                {
                    // 不包含才需要判断
                    CPortProtocolConfig config = new CPortProtocolConfig();
                    config.PortNumber = int.Parse(m_dataTable.Rows[i][CS_PortNumber].ToString());
                    config.ProtocolChannelName = m_dataTable.Rows[i][CS_ProtocolChannel].ToString();
                    if (config.ProtocolChannelName == CS_None)
                    {
                        config.ProtocolChannelName = ""; //为空
                    }

                    config.ProtocolDataName = m_dataTable.Rows[i][CS_ProtocolData].ToString();
                    if (config.ProtocolDataName == CS_None)
                    {
                        config.ProtocolDataName = ""; //为空
                    }
                    m_listPortConfig.Add(config);
                }// end of if 
            }
        }

        #region 重写

        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    // 删除列
                    if (base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals("True"))
                    {
                        // 删除项
                        base.MarkRowDeletedOrNot(e.RowIndex, true);
                    }
                    else
                    {
                        base.MarkRowDeletedOrNot(e.RowIndex, false);
                    }
                }
                else
                {
                    // 非删除列
                    base.EHValueChanged(sender, e);
                }
                base.UpdateDataToUI();
            }
        }

        // 重写双击事件
        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellMouseDoubleClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellMouseDoubleClick(e);
            }
        }

        // 单击事件
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellClick(e);
            }
        }

        // 清空所有数据，恢复到刚加载的状态
        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listUpdatedPorts.Clear();
            m_listDeletedPorts.Clear();
            m_listAddedPort.Clear();
        }

        #endregion  ///<重写

    }
}
