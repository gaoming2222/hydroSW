using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.Entity;
using Protocol.Channel.Interface;
using Hydrology.DataMgr;
using System.Diagnostics;
using Protocol.Manager;
using System.IO.Ports;

namespace Hydrology.Forms
{
    public partial class CBeidouNewMgrForm : Form
    {
      //  event EventHandler<SendOrRecvMsgEventArgs> MessageSendCompleted1;
        private List<CComboBoxDataSouce> CmbBaudRateDataSource = new List<CComboBoxDataSouce>() 
        {
            new CComboBoxDataSouce(){ DisplayMember ="2.4Kbps", ValueMember =1 },
            new CComboBoxDataSouce(){ DisplayMember ="4.8Kbps", ValueMember =2 },
            new CComboBoxDataSouce(){ DisplayMember ="9.6Kbps", ValueMember =3 },
            new CComboBoxDataSouce(){ DisplayMember ="19.2Kbps", ValueMember =4 }
        };
        private List<CComboBoxDataSouce> CmbRespBeamDataSource = new List<CComboBoxDataSouce>() 
        {
            new CComboBoxDataSouce(){ DisplayMember ="1号波束", ValueMember =1 },
            new CComboBoxDataSouce(){ DisplayMember ="2号波束", ValueMember =2 },
            new CComboBoxDataSouce(){ DisplayMember ="3号波束", ValueMember =3 },
            new CComboBoxDataSouce(){ DisplayMember ="4号波束", ValueMember =4 },
            new CComboBoxDataSouce(){ DisplayMember ="5号波束", ValueMember =5 },
            new CComboBoxDataSouce(){ DisplayMember ="6号波束", ValueMember =6 },
            new CComboBoxDataSouce(){ DisplayMember ="7号波束", ValueMember =7 },
            new CComboBoxDataSouce(){ DisplayMember ="8号波束", ValueMember =8 },
            new CComboBoxDataSouce(){ DisplayMember ="9号波束", ValueMember =9 },
            new CComboBoxDataSouce(){ DisplayMember ="10号波束", ValueMember =10 }
        };
        private List<CComboBoxDataSouce> CmbReceiptTypeDataSource = new List<CComboBoxDataSouce>() 
        {
            new CComboBoxDataSouce(){ DisplayMember ="打开系统回执，关闭通信回执", ValueMember =0 },
            new CComboBoxDataSouce(){ DisplayMember ="打开通信回执，关闭系统回执", ValueMember =1 }
        };

        public static string localCommunicateStr = "99999#####";
        public CBeidouNewMgrForm()
        {
            InitializeComponent();
            label_Baurate.Visible = false;
            comboBox_Baurate.Visible = false;
            //textBox_Receive.TE
          
            #region Init ListView Control

           // this.tabPage1.Controls.Add(listView1);
            listView1.BackColor = System.Drawing.SystemColors.Info;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            //this.listView1.BHorizentalScroolVisible = true;
            //this.listView1.BVertialScrollVisible = true;
            this.listView1.Columns.Add("信息类型", 200);
            this.listView1.Columns.Add("信息内容", 200);
            this.listView1.View = View.Details;
            //this.listView1.Items.Add("1","1");
            this.listView1.Items.Add("通道1接收信号功率电平", "");
            this.listView1.Items.Add("通道2接收信号功率电平", "");
            this.listView1.Items.Add("通道1锁定卫星波束", "");
            this.listView1.Items.Add("通道2锁定卫星波束", "");
            this.listView1.Items.Add("响应波束", "");
            this.listView1.Items.Add("信号抑制", "");

            this.listView1.Items.Add("供电状态", "");
            this.listView1.Items.Add("终端ID", "");
            this.listView1.Items.Add("通播地址", "");
            this.listView1.Items.Add("服务频度", "");
            this.listView1.Items.Add("串口波特率", "");
            this.listView1.Items.Add("保密模块", "");
            this.listView1.Items.Add("气压测高模块", "");

            #endregion

            #region Init ComboBox Control

            var dataProtocolCount = XmlDocManager.Instance.DataProtocolNames;
            var beidouComsCount = XmlDocManager.Instance.BeidouNormalComPorts;
            if (beidouComsCount.Count <= 0 || dataProtocolCount.Count <= 0)
            {
                MessageBox.Show("未配置北斗卫星普通终端通讯方式和数据协议，请配置串口!");
                throw new Exception("未配置北斗卫星普通终端通讯方式和数据协议，请配置串口!");
            }

            var portNames = XmlDocManager.Instance.BeidouNormalComPortsName();
            var sysPorts = new List<string>(SerialPort.GetPortNames());
            var portsInDBs = CDBDataMgr.Instance.GetAllSerialPortName();
            foreach (var item in portNames)
            {
                if (sysPorts.Contains(item) && portsInDBs.Contains(item))
                {
                    this.comboBox1.Items.Add(item);
                }
            }
            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = 0;

            this.comboBox_Baurate.Items.Add("2400bps");
            this.comboBox_Baurate.Items.Add("4800bps");
            this.comboBox_Baurate.Items.Add("9600bps");
            this.comboBox_Baurate.Items.Add("19200bps");
            this.comboBox_Baurate.SelectedIndex = 2;

            //  初始化波特率

            this.cmbBaudRate.DisplayMember = "DisplayMember";
            this.cmbBaudRate.ValueMember = "ValueMember";
            this.cmbBaudRate.DataSource = this.CmbBaudRateDataSource;
            this.cmbBaudRate.SelectedIndex = 2;

            //  初始化响应波束
            this.cmbRespBeam.DisplayMember = "DisplayMember";
            this.cmbRespBeam.ValueMember = "ValueMember";
            this.cmbRespBeam.DataSource = this.CmbRespBeamDataSource;

            //  初始化回执类型
            this.cmbReceiptType.DisplayMember = "DisplayMember";
            this.cmbReceiptType.ValueMember = "ValueMember";
            this.cmbReceiptType.DataSource = this.CmbReceiptTypeDataSource;
            this.cmbReceiptType.SelectedIndex = 0;

            #endregion

            #region Register Events For UI
             
            CProtocolEventManager.TSTA4UI += new EventHandler<Entity.TSTAEventArgs>(Beidou_TSTACompleted);
            CProtocolEventManager.COUT4UI += new EventHandler<Entity.COUTEventArgs>(Beidou_COUTCompleted);
           // MessageSendCompleted1 += CProtocolEventManager.MessageSendCompleted;
            //CProtocolEventManager.MessageSendCompleted += new EventHandler<Entity.SendOrRecvMsgEventArgs>(Beidou_COUTCompleted);
            CProtocolEventManager.BeidouErrorForUI += new EventHandler<Entity.ReceiveErrorEventArgs>(CProtocolEventManager_BeidouErrorForUI);
            this.AddStationEvent2 += new MyDelegate2(form_MyEvent2);
            #endregion

            
            if (this.comboBox1.SelectedIndex >= 0)
            {
                var beidou = CPortDataMgr.Instance.FindBeidouNormal(this.comboBox1.Text);
                if (beidou != null)
                {
                    if (beidou.Port.IsOpen)
                    {
                        StateChange(true);
                      //11.22
                      //  this.btnNormalState_Click(null, null);
                        return;
                    }
                }

            } StateChange(false);

            lblAdjustBeam.Text = string.Empty;
            
        }

        #region Update data to UI

        public void initListView()
        {
            Dictionary<string, string> dics = new Dictionary<string, string>();
            dics.Add("通道1接收信号功率电平", "");
            dics.Add("通道2接收信号功率电平", "");
            dics.Add("通道1锁定卫星波束", "");
            dics.Add("通道2锁定卫星波束", "");
            dics.Add("响应波束", "");
            dics.Add("信号抑制", "");

            dics.Add("供电状态","");
            dics.Add("终端ID", "");
            dics.Add("通播地址","");
            dics.Add("服务频度", "");
            dics.Add("串口波特率", "");
            dics.Add("保密模块", "");
            dics.Add("气压测高模块", "");
            if (this.IsHandleCreated)
            {
                try
                {
                    this.listView1.Invoke((Action)delegate
                    {
                        if (this.listView1.Items.Count > 0)
                            this.listView1.Items.Clear();
                        foreach (var item in dics)
                        {
                            try
                            {
                                var item1 = new ListViewItem();
                                item1.SubItems.Clear();
                                item1.SubItems[0].Text = item.Key;
                                item1.SubItems.Add(item.Value);
                                this.listView1.Items.Add(item1);
                            }
                            catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        }
                    });
                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            }
        
        }
        private void Beidou_TSTACompleted(object sender, Entity.TSTAEventArgs e)
        {

            var info = e.TSTAInfo;
            string rawMsg = e.RawMsg;
            Dictionary<string, string> dics = new Dictionary<string, string>();

            var strChannel1RecvPowerLevel = string.Empty;
            switch (info.Channel1RecvPowerLevel)
            {
                case 0: strChannel1RecvPowerLevel = "0 dBW [0]"; break;
                case 1: strChannel1RecvPowerLevel = "<-157 dBW [1]"; break;
                case 2: strChannel1RecvPowerLevel = "-155~-156 dBW [2]"; break;
                case 3: strChannel1RecvPowerLevel = "-153~-154 dBW [3]"; break;
                case 4: strChannel1RecvPowerLevel = "-151~-152 dBW [4]"; break;
                case 5: strChannel1RecvPowerLevel = ">-151 dBW [5]"; break;
            }
            dics.Add("通道1接收信号功率电平", strChannel1RecvPowerLevel);

            var strChannel2RecvPowerLevel = string.Empty;
            switch (info.Channel2RecvPowerLevel)
            {
                case 0: strChannel2RecvPowerLevel = "0 dBW [0]"; break;
                case 1: strChannel2RecvPowerLevel = "<-157 dBW [1]"; break;
                case 2: strChannel2RecvPowerLevel = "-155~-156 dBW [2]"; break;
                case 3: strChannel2RecvPowerLevel = "-153~-154 dBW [3]"; break;
                case 4: strChannel2RecvPowerLevel = "-151~-152 dBW [4]"; break;
                case 5: strChannel2RecvPowerLevel = ">-151 dBW [5]"; break;
            }
            dics.Add("通道2接收信号功率电平", strChannel2RecvPowerLevel);

            var strChannel1LockingBeam = string.Empty;
            switch (info.Channel1LockingBeam)
            {
                case 0: strChannel1LockingBeam = "未锁定"; break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6: strChannel1LockingBeam = info.Channel1LockingBeam + "号波束"; break;
            }
            dics.Add("通道1锁定卫星波束", strChannel1LockingBeam);

            var strChannel2LockingBeam = string.Empty;
            switch (info.Channel1LockingBeam)
            {
                case 0: strChannel1LockingBeam = "未锁定"; break;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6: strChannel2LockingBeam = info.Channel2LockingBeam + "号波束"; break;
            }
            dics.Add("通道2锁定卫星波束", strChannel2LockingBeam);


            dics.Add("响应波束", info.ResponseOfBeam + "号波束");
            dics.Add("信号抑制", info.SignalSuppression ? "有信号抑制" : "无信号抑制");

            dics.Add("供电状态", info.PowerState ? "可以满足" : "不能满足");
            dics.Add("终端ID", info.TerminalID);
            dics.Add("通播地址", info.BroadcastAddr);
            dics.Add("服务频度", info.ServiceFrequency);

            var strSerialBaudRate = string.Empty;
            switch (info.SerialBaudRate)
            {
                case 1: strSerialBaudRate = "2.4Kbps"; break;
                case 2: strSerialBaudRate = "4.8Kbps"; break;
                case 3: strSerialBaudRate = "9.6Kbps"; break;
                case 4: strSerialBaudRate = "19.6Kbps"; break;
            }
            dics.Add("串口波特率", strSerialBaudRate);
            dics.Add("保密模块", info.SecurityModuleState == 0 ? "没有安装" : "已经安装");
            dics.Add("气压测高模块", info.BarometricAltimetryModuleState == 0 ? "没有安装" : "已经安装");

            if (this.IsHandleCreated)
            {
                try
                {
                    this.listView1.Invoke((Action)delegate
                    {
                        if (this.listView1.Items.Count > 0)
                            this.listView1.Items.Clear();
                        foreach (var item in dics)
                        {
                            try
                            {
                                var item1 = new ListViewItem();
                                item1.SubItems.Clear();
                                item1.SubItems[0].Text = item.Key;
                                item1.SubItems.Add(item.Value);
                                this.listView1.Items.Add(item1);
                            }
                            catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        }
                    });
                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            }
        }
        private void Beidou_COUTCompleted(object sender, Entity.COUTEventArgs e)
        {
        //    this.textBox_Receive.Text = "123";
        }
        private void CProtocolEventManager_BeidouErrorForUI(object sender, Entity.ReceiveErrorEventArgs e)
        {
            try
            {
                this.lblAdjustBeam.Invoke((Action)delegate
                {
                    this.lblAdjustBeam.Text = e.Msg;
                });

            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
        }
        #endregion

        #region Button Event Handler

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }
            if (this.comboBox_Baurate.SelectedIndex < 0)
            {
                MessageBox.Show("请选择波特率!");
                return;
            }
            StateChange(true);
            string portName = this.comboBox1.Text;
            int baudrate = 9600;
            switch (this.comboBox_Baurate.Text)
            {
                case "2400bps": baudrate = 2400; break;
                case "4800bps": baudrate = 4800; break;
                case "9600bps": baudrate = 9600; break;
                case "19200bps": baudrate = 19200; break;
            }

            CPortDataMgr.Instance.StartBeidouNormal(portName, baudrate);
            this.btnNormalState_Click(null, null);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
                this.listView1.Items.Clear();
        }
        //private void btnNormalState_Click(object sender, EventArgs e)
        //{
        //    CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "查询终端状态"));
        //    lblAdjustBeam.Text = string.Empty;
        //    //CPortDataMgr.Instance.SendBeidouQSTA();
        //    if (this.comboBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("请选择串口！");
        //        return;
        //    }
        //    CPortDataMgr.Instance.SendBeidouQSTA(this.comboBox1.Text);
        //}
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }
            CPortDataMgr.Instance.StopBeidouNormal(this.comboBox1.Text);
            StateChange(false);
            if (this.listView1.Items.Count > 0)
                this.listView1.Items.Clear();
        }
        /// <summary>
        /// 设置普通终端的参数
        /// </summary>
        //private void btnSetParam_Click(object sender, EventArgs e)
        //{
        //    if (this.comboBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("请选择串口！");
        //        return;
        //    }
        //    string portName = this.comboBox1.Text;

        //    int baudrate = (int)this.cmbBaudRate.SelectedValue;
        //    int respbeam = (int)this.cmbRespBeam.SelectedValue;
        //    bool receipttype = ((int)this.cmbReceiptType.SelectedValue) == 0 ? false : true;

        //    var beidou = CPortDataMgr.Instance.FindBeidouNormal(this.comboBox1.Text);
        //    if (beidou != null && beidou.Port.IsOpen)
        //    {
        //        CSTSTStruct param = new CSTSTStruct()
        //        {
        //            ServiceFrequency = "60",
        //            SerialBaudRate = baudrate,
        //            ResponseOfBeam = respbeam,
        //            AcknowledgmentType = receipttype
        //        };
        //        CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "设置终端状态"));

        //        //CPortDataMgr.Instance.SendBeidouSTST(param);

        //        //CPortDataMgr.Instance.SendBeidouSTST(portName, param);
        //        beidou.SendSTST(param);
        //    }
        //}

        #endregion

        private void StateChange(bool isStartBeidou)
        {
            this.btnStart.Enabled = !isStartBeidou;
            this.btnStop.Enabled = isStartBeidou;
            this.btnSetParam.Enabled = isStartBeidou;
            this.btnNormalState.Enabled = isStartBeidou;
        }
        internal class CComboBoxDataSouce
        {
            public string DisplayMember { get; set; }
            public int ValueMember { get; set; }
        }

        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (this.comboBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("请选择串口！");
        //        return;
        //    }
        //    var beidou = CPortDataMgr.Instance.FindBeidouNormal(this.comboBox1.Text);
        //    if (beidou != null && beidou.Port.IsOpen)
        //    {
        //        StateChange(beidou.Port.IsOpen);
        //       // this.btnNormalState_Click(null, null);
        //    }
        //    initListView();
        //}


        /// <summary>
        /// 生成CRC校验码
        /// </summary>
        /// <param name="rawStr">CRC校验码之前的字符串</param>
        /// <returns>CRC校验码</returns>
        private static String GenerateCRC(string rawStr)
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(rawStr);
            byte crc = byteArray[0];
            for (int i = 1; i < byteArray.Length; i++)
            {
                crc = (byte)(crc ^ byteArray[i]);
            }
            return ((char)crc).ToString();
        }
        public static String GetTTCAStr(CTTCAStruct param)
        {
            StringBuilder rawStr = new StringBuilder();
            rawStr.Append("$");
            rawStr.Append(CCACAStruct.CMD_PREFIX);
            rawStr.Append(",");
            rawStr.Append(param.SenderID);       // 发信方ID
            rawStr.Append(",");
            rawStr.Append(param.RecvAddr);       // 收信方地址
            rawStr.Append(",");
            rawStr.Append(param.Requirements);   // 保密要求
            rawStr.Append(",");
            rawStr.Append(param.ReceiptSign);  // 回执标志
            rawStr.Append(",");
            rawStr.Append(param.MsgLength); // 电文长度
            rawStr.Append(",");
            rawStr.Append(param.MsgContent); // 电文内容
            rawStr.Append(",");
            rawStr.Append(GenerateCRC(rawStr.ToString()));// 校验和
            rawStr.Append("\r\n");
            return rawStr.ToString();
        }
        private void SendTTCA(CTTCAStruct param)
        {
            string text = GetTTCAStr(param);
         //   Debug.WriteLine("发送TTCA：" + text);
           // InvokeMessage("自发自收  " + text, "发送");
           // SendText(text);
        }
        
        //private void SendText(string msg)
        //{
        //    if (this.SerialPortStateChanged != null)
        //        this.SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
        //        {
        //            BNormal = false,
        //            PortNumber = Int32.Parse(Port.PortName.Replace("COM", "")),
        //            PortType = this.m_portType
        //        }));
        //    try
        //    {
        //        this.Port.Write(msg);
        //    }
        //    catch (Exception exp)
        //    {

        //    }
        //}


        ////定义该委托的事件
        //public event Hydrology.DataMgr.CProtocolEventManager.MyDelegate2 AddStationEvent2;

        //定义一个需要string类型参数的委托
        public delegate void MyDelegate2(String str);

        //定义该委托的事件
#pragma warning disable CS0067 // 从不使用事件“CBeidouNewMgrForm.AddStationEvent2”
        public event MyDelegate2 AddStationEvent2;
#pragma warning restore CS0067 // 从不使用事件“CBeidouNewMgrForm.AddStationEvent2”

        public static string backstr;

        //本地通信
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_SendAddress.Text.Equals("") || textBox_Communicate.Text.Equals(""))
            {
                MessageBox.Show("发送地址或通讯报文不能为空！");
                return;
            }
            CTTCAStruct cTTCAStruct= new CTTCAStruct();
            cTTCAStruct.SenderID = textBox_SendAddress.Text.ToString();
            cTTCAStruct.RecvAddr = textBox_SendAddress.Text.ToString();
            cTTCAStruct.Requirements = "1";
            cTTCAStruct.ReceiptSign = false;
            cTTCAStruct.MsgContent = textBox_Communicate.Text.ToString();
            cTTCAStruct.MsgLength = cTTCAStruct.MsgContent.Length;
            localCommunicateStr = cTTCAStruct.SenderID;
            CPortDataMgr.Instance.SendBeidouTTCA(this.comboBox1.Text, cTTCAStruct);
          
           
        }

        public static void MessageSendCompleted1(object sender, SendOrRecvMsgEventArgs e)
        {
          //  if (e.Msg.Contains("COUT") && e.Msg.Contains("212880"))
            try
            {
                if (e.Msg.Contains("COUT") && e.Msg.Contains(localCommunicateStr))
                {
                    String str = e.Msg.ToString();
                    String[] arraystr = str.Split(',');
                    if (arraystr.Count() == 10 )
                    {
                        localCommunicateStr = "99999#####";
                        MessageBox.Show("北斗卫星普通型本地通信成功! \n返回信息：\n"+arraystr[8]);
                    }
                }
                else if (e.Msg.Contains("TINF"))
                {
                    String str = e.Msg.ToString();
                    String[] arraystr = str.Split(',');
                    string time1 = arraystr[1];
                    SystemTime MySystemTime = new SystemTime();
                    //2016-11-04 11:32:06.67
                    MySystemTime.vYear = ushort.Parse(time1.Substring(0,4));

                    MySystemTime.vMonth = ushort.Parse(time1.Substring(5, 2));

                    MySystemTime.vDay = ushort.Parse(time1.Substring(8, 2));

                    MySystemTime.vHour = ushort.Parse(time1.Substring(11, 2));

                    MySystemTime.vMinute = ushort.Parse(time1.Substring(14,2));

                    MySystemTime.vSecond = ushort.Parse(time1.Substring(17,2));

                    SystemTimeConfig.SetLocalTime(MySystemTime);
                    MessageBox.Show("授时成功！");

                }
            }
#pragma warning disable CS0168 // 声明了变量“e1”，但从未使用过
            catch(Exception e1)
#pragma warning restore CS0168 // 声明了变量“e1”，但从未使用过
            {
            }
        }
        public static void returnStr(String str)
        {
           // this.textBox_Receive.Text = str;
            backstr = str;
            //    b.Text=str;
        }
        //处理
        void form_MyEvent2(String str)
        {
            //textBox_Receive.Text = str;
        }

        //授时申请
        private void button2_Click(object sender, EventArgs e)
        {
            CTAPPStruct cTAPPStruct = new CTAPPStruct();
            //TAPP
            //$TAPP,校验和<0x0D><0X0A>
            //2016.9.22
          //  CPortDataMgr.Instance.SendBeidouTAPP("COM3", cTAPPStruct);
            CPortDataMgr.Instance.SendBeidouTAPP(this.comboBox1.Text);
        }

        private void btnNormalState_Click(object sender, EventArgs e)
        {
            CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "查询终端状态"));
            lblAdjustBeam.Text = string.Empty;
            //CPortDataMgr.Instance.SendBeidouQSTA();
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }
            CPortDataMgr.Instance.SendBeidouQSTA(this.comboBox1.Text);
        }

        private void btnSetParam_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }
            string portName = this.comboBox1.Text;

            int baudrate = (int)this.cmbBaudRate.SelectedValue;
            int respbeam = (int)this.cmbRespBeam.SelectedValue;
            bool receipttype = ((int)this.cmbReceiptType.SelectedValue) == 0 ? false : true;

            var beidou = CPortDataMgr.Instance.FindBeidouNormal(this.comboBox1.Text);
            if (beidou != null && beidou.Port.IsOpen)
            {
                CSTSTStruct param = new CSTSTStruct()
                {
                    ServiceFrequency = "60",
                    SerialBaudRate = baudrate,
                    ResponseOfBeam = respbeam,
                    AcknowledgmentType = receipttype
                };
                CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "设置终端状态"));

                //CPortDataMgr.Instance.SendBeidouSTST(param);

                //CPortDataMgr.Instance.SendBeidouSTST(portName, param);
                beidou.SendSTST(param);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }
            var beidou = CPortDataMgr.Instance.FindBeidouNormal(this.comboBox1.Text);
            if (beidou != null && beidou.Port.IsOpen)
            {
                StateChange(beidou.Port.IsOpen);
                // this.btnNormalState_Click(null, null);
            }
            initListView();
        }

        //private void btnSetParam_Click_1(object sender, EventArgs e)
        //{

        //}

        //private void btnNormalState_Click(object sender, EventArgs e)
        //{
        //    CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "查询终端状态"));
        //    lblAdjustBeam.Text = string.Empty;
        //    //CPortDataMgr.Instance.SendBeidouQSTA();
        //    if (this.comboBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("请选择串口！");
        //        return;
        //    }
        //    CPortDataMgr.Instance.SendBeidouQSTA(this.comboBox1.Text);
        //}

   

        //private void btnNormalState_Click(object sender, EventArgs e)
        //{
            //CSystemInfoMgr.Instance.AddInfo(string.Format("[{0}]---------------{1}----------------", EChannelType.BeiDou, "查询终端状态"));
            //lblAdjustBeam.Text = string.Empty;
            ////CPortDataMgr.Instance.SendBeidouQSTA();
            //if (this.comboBox1.SelectedIndex < 0)
            //{
            //    MessageBox.Show("请选择串口！");
            //    return;
            //}
            //CPortDataMgr.Instance.SendBeidouQSTA(this.comboBox1.Text);
        //}

    


       
    
   
    }
}
