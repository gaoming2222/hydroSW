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
using System.IO.Ports;
using Protocol.Manager;
using Hydrology.CControls;

namespace Hydrology.Forms
{
    public partial class CBeidou500MgrForm : Form
    {
        private CExDataGridView m_dgvUserInfo;
        private CExDataGridView m_dgvStatusInfo;
        public static string localCommunicateStr_1 = "8888#####";
        public static long ReportNum = 0;
        public static int flag;
        public CBeidou500MgrForm()
        {
            flag = 0;
            InitializeComponent();
            var dataProtocolCount = XmlDocManager.Instance.DataProtocolNames;
            var beidouComsCount = XmlDocManager.Instance.Beidou500ProtocolNames;
            if (beidouComsCount.Count <= 0 || dataProtocolCount.Count <= 0)
            {
                MessageBox.Show("未配置北斗卫星(指挥机)通讯方式和数据协议，请配置串口!");
                throw new Exception("未配置北斗卫星(指挥机)通讯方式和数据协议，请配置串口!");
            }


            var portNames = XmlDocManager.Instance.Beidou500ComPortsName();
            var sysPorts = new List<string>(SerialPort.GetPortNames());
            var portsInDBs = CDBDataMgr.Instance.GetAllSerialPortName();
            foreach (var item in portNames)
            {
                if (sysPorts.Contains(item) && portsInDBs.Contains(item))
                {
                    this.comboBox1.Items.Add(item);
                }
            }
           

            InitDgv();
            RegisterEvents();
            InitData();
            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = 0;
          //11.22
          //  this.btnQuery_Click(null, null);
        }


        /// <summary>
        /// 事件注册
        /// </summary>
        private void RegisterEvents()
        {
            CProtocolEventManager.Beidou500BJXXForUI += CProtocolEventManager_Beidou500BJXXForUI;
            CProtocolEventManager.Beidou500SJXXForUI += CProtocolEventManager_Beidou500SJXXForUI;
            CProtocolEventManager.Beidou500ZTXXForUI += CProtocolEventManager_Beidou500ZTXXForUI;
        }
        private void CProtocolEventManager_Beidou500BJXXForUI(object sender, Beidou500BJXXEventArgs e)
        {
            var bjxx = e.BJXXInfo;
            if (null == bjxx)
                return;
            //  更新"本地用户ID"
            string LocalAddr = bjxx.LocalAddr;
            m_dgvUserInfo.UpdateRowData(0, new string[] { "本地用户ID", LocalAddr }, CExDataGridView.EDataState.ENormal);

            //  更新"通播ID"
            string BroadCastAddr = bjxx.BroadCastAddr;
            m_dgvUserInfo.UpdateRowData(1, new string[] { "通播ID", BroadCastAddr }, CExDataGridView.EDataState.ENormal);

            //  更新"序列号"
            string CardNum = bjxx.CardNum;
            m_dgvUserInfo.UpdateRowData(2, new string[] { "序列号", CardNum }, CExDataGridView.EDataState.ENormal);

            //  更新"服务频率"
            string ServiceFrequency = string.Format("{0}秒", bjxx.ServiceFrequency);
            m_dgvUserInfo.UpdateRowData(3, new string[] { "服务频率", ServiceFrequency }, CExDataGridView.EDataState.ENormal);

            //  更新"保密标志"
            string ConfidentFlag = bjxx.ConfidentFlag.Trim().Equals("0") ? "非保密" : "加密通信等级";
            m_dgvUserInfo.UpdateRowData(4, new string[] { "保密标志", ConfidentFlag }, CExDataGridView.EDataState.ENormal);

            //  更新"通信等级"
            string CommLevel = bjxx.CommLevel;
            m_dgvUserInfo.UpdateRowData(5, new string[] { "通信等级", CommLevel }, CExDataGridView.EDataState.ENormal);

            //  更新显示
            m_dgvUserInfo.UpdateDataToUI();
        }
        private void CProtocolEventManager_Beidou500SJXXForUI(object sender, Beidou500SJXXEventArgs e)
        {
            var sjxx = e.SJXXInfo;
            if (null == sjxx)
                return;
            //  更新"卫星时间"
            string timeInfo = String.Format("{0}-{1}-{2} {3}:{4}:{5}", sjxx.Year, sjxx.Month, sjxx.Day, sjxx.Hour, sjxx.Minute, sjxx.Second);
            m_dgvUserInfo.UpdateRowData(6, new string[] { "卫星时间", timeInfo }, CExDataGridView.EDataState.ENormal);

            //  更新显示
            m_dgvUserInfo.UpdateDataToUI();
        }
        private void CProtocolEventManager_Beidou500ZTXXForUI(object sender, Beidou500ZTXXEventArgs e)
        {
            var ztxx = e.ZTXXInfo;
            if (null == ztxx)
                return;
            //  更新"IC卡状态"
            var CardStatus = ztxx.CardStatus.Trim().Equals("0") ? "正常" : "故障";
            m_dgvStatusInfo.UpdateRowData(0, new string[] { "IC卡状态", CardStatus }, CExDataGridView.EDataState.ENormal);

            //  更新"整机状态"
            var WholeMachineState = ztxx.WholeMachineState.Trim().Equals("0") ? "正常" : "故障";
            m_dgvStatusInfo.UpdateRowData(1, new string[] { "整机状态", WholeMachineState }, CExDataGridView.EDataState.ENormal);


            //  更新"入站状态"
            var InStationStatus = ztxx.InStationStatus.Trim().Equals("0") ? "正常" : "抑制";
            m_dgvStatusInfo.UpdateRowData(2, new string[] { "入站状态", InStationStatus }, CExDataGridView.EDataState.ENormal);

            //  更新"电池电量"
            m_dgvStatusInfo.UpdateRowData(3, new string[] { "电池电量", ztxx.Electricity }, CExDataGridView.EDataState.ENormal);
            //  更新"响应波束"
            m_dgvStatusInfo.UpdateRowData(4, new string[] { "响应波束", ztxx.ResponseBeam }, CExDataGridView.EDataState.ENormal);

            //  更新"信号强度"
            string singalStrength = string.Format("{0},{1},{2},{3},{4},{5}", ztxx.SignalStrength1, ztxx.SignalStrength2, ztxx.SignalStrength3, ztxx.SignalStrength4, ztxx.SignalStrength5, ztxx.SignalStrength6);
            m_dgvStatusInfo.UpdateRowData(5, new string[] { "信号强度", singalStrength }, CExDataGridView.EDataState.ENormal);

            //  更新"回执状态"
            var ReceiptStatus = ztxx.ReceiptStatus.Trim().Equals("0") ? "自动回执" : "不回执";
            m_dgvStatusInfo.UpdateRowData(6, new string[] { "回执状态", ReceiptStatus }, CExDataGridView.EDataState.ENormal);

            //  更新显示
            m_dgvStatusInfo.UpdateDataToUI();
        }

        public void InitDgv()
        {
            this.SuspendLayout();
            //  初始化用户信息
            m_dgvUserInfo = new CExDataGridView() { BPartionPageEnable = false };
            m_dgvUserInfo.Header = new string[] { "kjl", "asdfasdf" };
            m_dgvUserInfo.AllowUserToAddRows = false;
            m_dgvUserInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            m_dgvUserInfo.Dock = DockStyle.Fill;
            m_dgvUserInfo.AllowUserToResizeRows = false;
            m_dgvUserInfo.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvUserInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvUserInfo.RowHeadersWidth = 50;
            m_dgvUserInfo.ColumnHeadersHeight = 25;
            m_dgvUserInfo.ColumnHeadersVisible = false;//隐藏表头
            m_dgvUserInfo.RowHeadersVisible = false;
            m_dgvUserInfo.ScrollBars = ScrollBars.None;

            panelUserInfo.Controls.Add(m_dgvUserInfo);

            //  初始化状态信息
            m_dgvStatusInfo = new CExDataGridView() { BPartionPageEnable = false };
            m_dgvStatusInfo.Header = new string[] { "kjl", "afdasdfaf" };
            m_dgvStatusInfo.AllowUserToAddRows = false;
            m_dgvStatusInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            m_dgvStatusInfo.Dock = DockStyle.Fill;
            m_dgvStatusInfo.AllowUserToResizeRows = false;
            m_dgvStatusInfo.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvStatusInfo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvStatusInfo.RowHeadersWidth = 50;
            m_dgvStatusInfo.ColumnHeadersVisible = false;//隐藏表头
            m_dgvStatusInfo.RowHeadersVisible = false;
            m_dgvStatusInfo.ColumnHeadersHeight = 25;
            m_dgvStatusInfo.ScrollBars = ScrollBars.None;

            panelStatusInfo.Controls.Add(m_dgvStatusInfo);

            this.ResumeLayout(false);
        }

        private void InitData()
        {
            m_dgvUserInfo.AddRow(new string[] { "本地用户ID", "---" });
            m_dgvUserInfo.AddRow(new string[] { "通播ID", "---" });
            m_dgvUserInfo.AddRow(new string[] { "序列号", "---" });
            m_dgvUserInfo.AddRow(new string[] { "服务频率", "---" });
            m_dgvUserInfo.AddRow(new string[] { "保密标志", "---" });
            m_dgvUserInfo.AddRow(new string[] { "通信等级", "---" });
            m_dgvUserInfo.AddRow(new string[] { "卫星时间", "---" });
            m_dgvUserInfo.UpdateDataToUI();

            m_dgvStatusInfo.AddRow(new string[] { "IC卡状态", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "整机状态", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "入站状态", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "电池电量", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "响应波束", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "信号强度", "---" });
            m_dgvStatusInfo.AddRow(new string[] { "回执状态", "---" });
            m_dgvStatusInfo.UpdateDataToUI();
        }

        //private void btnQuery_Click(object sender, EventArgs e)
        //{
        //    this.lblWarning.Visible = false;
        //    if (this.comboBox1.SelectedIndex < 0)
        //    {
        //        MessageBox.Show("请选择串口！");
        //        return;
        //    }

        //    var beidou = CPortDataMgr.Instance.FindBeidou500(this.comboBox1.Text);
        //    if (null == beidou)
        //    {
        //        this.lblWarning.Text = string.Format("请检查指挥机的串口配置信息");
        //        this.lblWarning.Visible = true;
        //    }
        //    else
        //    {
        //        if (beidou.Port.IsOpen)
        //        {
        //            beidou.Query();
        //        }
        //        else
        //        {
        //            this.lblWarning.Text = string.Format("请打开串口{0}", this.comboBox1.Text);
        //            this.lblWarning.Visible = true;
        //        }
        //    }

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

        //本地通信
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_SendAddress.Text.Equals("") || textBox_Communicate.Text.Equals(""))
            {
                MessageBox.Show("发送地址或通讯报文不能为空！");
                return;
            }
            CBeiDouTTCA cTTCAStruct = new CBeiDouTTCA();
            if (ReportNum != 16777216)
            {
                ReportNum++;
                cTTCAStruct.PeripheralReportNum = ReportNum.ToString();
            }
            else
            {
                ReportNum = 1;
                cTTCAStruct.PeripheralReportNum = ReportNum.ToString();
            }
            cTTCAStruct.SenderID = textBox_SendAddress.Text.ToString();
            cTTCAStruct.ReceiverAddr = textBox_SendAddress.Text.ToString();
            cTTCAStruct.ConfidentialityRequirements = "1";
            cTTCAStruct.ReceiptFlag = "0";
            cTTCAStruct.MsgContent = textBox_Communicate.Text.ToString();
            cTTCAStruct.MsgLength = cTTCAStruct.MsgContent.Length.ToString();
            localCommunicateStr_1 = cTTCAStruct.SenderID;

            CPortDataMgr.Instance.SendBeidou500TTCA(this.comboBox1.Text, cTTCAStruct);
        

        }

        public static void MessageSendCompleted1(object sender, SendOrRecvMsgEventArgs e)
        {
            //  if (e.Msg.Contains("COUT") && e.Msg.Contains("212880"))
            try
            {
                if (e.Msg.Contains("COUT") && e.Msg.Contains(localCommunicateStr_1))
                {
                    String str = e.Msg.ToString();
                    String[] arraystr = str.Split(',');
                    //if (arraystr.Count() == 10)
                    //{
                        localCommunicateStr_1 = "8888#####";
                        MessageBox.Show("北斗卫星指挥机通信成功! \n返回信息：\n" + arraystr[8]);
                   // }
                }
                else if (e.Msg.Contains("SJXX")&&flag==1)
                {
                    String str = e.Msg.ToString();
                    String[] arraystr = str.Split(',');
                    //string time1 = arraystr[1];
                    SystemTime MySystemTime = new SystemTime();
                    //$SJXX,2016,11,4,16,1,3,%
                    MySystemTime.vYear = ushort.Parse(arraystr[1]);

                    MySystemTime.vMonth = ushort.Parse(arraystr[2]);

                    MySystemTime.vDay = ushort.Parse(arraystr[3]);

                    MySystemTime.vHour = ushort.Parse(arraystr[4]);

                    MySystemTime.vMinute = ushort.Parse(arraystr[5]);

                    MySystemTime.vSecond = ushort.Parse(arraystr[6]);

                    SystemTimeConfig.SetLocalTime(MySystemTime);
                    flag = 0;
                    MessageBox.Show("授时成功！");

                }
            }
#pragma warning disable CS0168 // 声明了变量“e1”，但从未使用过
            catch (Exception e1)
#pragma warning restore CS0168 // 声明了变量“e1”，但从未使用过
            {
            }
        }

        //授时申请
        private void button2_Click(object sender, EventArgs e)
        {
            flag = 1;
            CTAPPStruct cTAPPStruct = new CTAPPStruct();
            //TAPP
            //$TAPP,校验和<0x0D><0X0A>
            //2016.9.22
            //  CPortDataMgr.Instance.SendBeidouTAPP("COM3", cTAPPStruct);
            CPortDataMgr.Instance.SendBeidou500CXSJ(this.comboBox1.Text);
           
        }
       

        //通播通信
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox_SendAddress.Text.Equals("") || textBox_Communicate.Text.Equals(""))
            {
                MessageBox.Show("发送地址或通讯报文不能为空！");
                return;
            }
            CBeiDouTTCA cTTCAStruct = new CBeiDouTTCA();
            if (ReportNum != 16777216)
            {
                ReportNum++;
                cTTCAStruct.PeripheralReportNum = ReportNum.ToString();
            }
            else
            {
                ReportNum = 1;
                cTTCAStruct.PeripheralReportNum = ReportNum.ToString();
            }
            cTTCAStruct.SenderID = textBox_SendAddress.Text.ToString();
            cTTCAStruct.ReceiverAddr = textBox_SendAddress.Text.ToString();
            cTTCAStruct.ConfidentialityRequirements = "1";
            cTTCAStruct.ReceiptFlag = "0";
            cTTCAStruct.MsgContent = textBox_Communicate.Text.ToString();
            cTTCAStruct.MsgLength = cTTCAStruct.MsgContent.Length.ToString();
            localCommunicateStr_1 = cTTCAStruct.SenderID;
  
            //textBox_Receive.Text = 
            CPortDataMgr.Instance.SendBeidou500TTCA(this.comboBox1.Text, cTTCAStruct);
          
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            this.lblWarning.Visible = false;
            if (this.comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("请选择串口！");
                return;
            }

            var beidou = CPortDataMgr.Instance.FindBeidou500(this.comboBox1.Text);
            if (null == beidou)
            {
                this.lblWarning.Text = string.Format("请检查指挥机的串口配置信息");
                this.lblWarning.Visible = true;
            }
            else
            {
                if (beidou.Port.IsOpen)
                {
                    beidou.Query();
                }
                else
                {
                    this.lblWarning.Text = string.Format("请打开串口{0}", this.comboBox1.Text);
                    this.lblWarning.Visible = true;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_dgvUserInfo.ClearAllRows();
            m_dgvStatusInfo.ClearAllRows();
            InitData();
        }

      
  

     
     
    }
}
