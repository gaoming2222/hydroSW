using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using Hydrology.Entity;
using Protocol.Channel.Interface;
using Protocol.Data.Interface;
using System.IO;
//using Protocol.Data.HJJBX;
//using Protocol.Data.Lib;
//using Protocol.Data.SXDZ;
using Protocol.Data.ZYJBX;
using System.Text;
//using Protocol.Data.XYJBX;


namespace Protocol.Channel.HDGprs
{
    public class HDGpesParser : IHDGprs
    {
        internal class MyMessage
        {
            public string ID;
            public string MSG;
        }
        #region 成员变量
        private ushort currentPort;
        static bool s_isFirstSend = true;
        private Semaphore m_semaphoreData;    //用来唤醒消费者处理缓存数据
        private Mutex m_mutexListDatas;     // 内存data缓存的互斥量
        private Thread m_threadDealData;    // 处理数据线程
        private List<HDModemDataStruct> m_listDatas;   //存放data的内存缓存

        private System.Timers.Timer m_timer = new System.Timers.Timer()
        {
            Enabled = true,
            Interval = 5000
        };
        private int GetReceiveTimeOut()
        {
            return (int)(m_timer.Interval);
        }

        public static CDictionary<String, String> HdProtocolMap = new CDictionary<string, string>();
        #endregion
        
        #region 构造方法
        public HDGpesParser()
        {
            m_semaphoreData = new Semaphore(0, Int32.MaxValue);
            m_listDatas = new List<HDModemDataStruct>();
            m_mutexListDatas = new Mutex();

            m_threadDealData = new Thread(new ThreadStart(this.DealData));
            m_threadDealData.Start();

            DTUList = new List<HDModemInfoStruct>();

            m_timer.Elapsed += new ElapsedEventHandler(m_timer_Elapsed);
        }
        #endregion
        void m_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int second = GetReceiveTimeOut();
            InvokeMessage(String.Format("系统接收数据时间超过{0}毫秒", second), "系统超时");
            if (this.ErrorReceived != null)
                this.ErrorReceived.Invoke(null, new ReceiveErrorEventArgs()
                {
                    Msg = String.Format("系统接收数据时间超过{0}秒", second)
                });
            if (null != this.GPRSTimeOut)
            {
                this.GPRSTimeOut(null, new ReceivedTimeOutEventArgs() { Second = second });
            }
            Debug.WriteLine("系统超时,停止计时器");
            m_timer.Stop();
        }
        #region 属性
        private List<CEntityStation> m_stationLists;
        public IUp Up { get; set; }
        public IDown Down { get; set; }
        public IUBatch UBatch { get; set; }
        public IFlashBatch FlashBatch { get; set; }
        public ISoil Soil { get; set; }

        public List<HDModemInfoStruct> DTUList { get; set; }

        public bool IsCommonWorkNormal { get; set; }
        private System.Timers.Timer tmrData;
        private System.Timers.Timer tmrDTU;
        private EChannelType m_channelType;
        private EListeningProtType m_portType;
        #endregion

        #region 日志记录
        public void InvokeMessage(string msg, string description)
        {
            if (this.MessageSendCompleted != null)
                this.MessageSendCompleted(null, new SendOrRecvMsgEventArgs()
                {
                    ChannelType = this.m_channelType,
                    Msg = msg,
                    Description = description
                });
        }
        #endregion



        #region 事件
        public event EventHandler<BatchEventArgs> BatchDataReceived;
        public event EventHandler<BatchSDEventArgs> BatchSDDataReceived;
        public event EventHandler<DownEventArgs> DownDataReceived;
        public event EventHandler<ReceiveErrorEventArgs> ErrorReceived;
        public event EventHandler<SendOrRecvMsgEventArgs> MessageSendCompleted;
        public event EventHandler<ReceivedTimeOutEventArgs> GPRSTimeOut;
        public event EventHandler<CEventSingleArgs<CSerialPortState>> SerialPortStateChanged;
        public event EventHandler<CEventSingleArgs<CEntitySoilData>> SoilDataReceived;
        public event EventHandler<UpEventArgs> UpDataReceived;
        public event EventHandler<UpEventArgs_new> UpDataReceived_new;
        public event EventHandler<ModemDataEventArgs> ModemDataReceived;
        public event EventHandler HDModemInfoDataReceived;
        #endregion


        #region 用户列表维护
        private bool inDtuTicks = false;
        private void tmrDTU_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inDtuTicks) return;
            inDtuTicks = true;
            try
            {
                Dictionary<string, HDModemInfoStruct> dtuList;
                if (this.getDTUList(out dtuList) == 0)
                {
                    this.DTUList.Clear();
                    foreach (var item in dtuList)
                    {
                        this.DTUList.Add(item.Value);
                    }

                    if (this.HDModemInfoDataReceived != null)
                        this.HDModemInfoDataReceived(this, null);
                }

            }
            catch (Exception eee)
            {
            }
            finally
            {
                inDtuTicks = false;
            }

        }

        #endregion
        public void Init()
        {
            InitMap();
            this.m_channelType = EChannelType.GPRS;
            this.m_portType = EListeningProtType.Port;
            if (tmrData == null)
                tmrData = new System.Timers.Timer(250);
            tmrData.Elapsed += new ElapsedEventHandler(tmrData_Elapsed);

            if (tmrDTU == null)
                tmrDTU = new System.Timers.Timer(2000);
            tmrDTU.Elapsed += new ElapsedEventHandler(tmrDTU_Elapsed);

            if (DTUList == null)
                DTUList = new List<HDModemInfoStruct>();
        }
        public void Close()
        {
            //this.DSStopService(null);
        }

        public void InitInterface(IUp up, IDown down, IUBatch udisk, IFlashBatch flash, ISoil soil)
        {
            this.Up = up;
            this.Down = down;
            this.UBatch = udisk;
            this.FlashBatch = flash;
            this.Soil = soil;
        }
       
        public void InitStations(List<CEntityStation> stations)
        {
            this.m_stationLists = stations;
        }

        public void InitMap()
        {
            String[] rows = File.ReadAllLines("Config/map.txt");
            foreach (String row in rows)
            {
                String[] pieces = row.Split(',');
                if (pieces.Length == 2)
                    if (!HdProtocolMap.ContainsKey(pieces[0]))
                    {
                        HdProtocolMap.Add(pieces[0], pieces[1]);
                    }
                    else
                    {
                        HdProtocolMap[pieces[0]] = pieces[1];
                    }
            }
        }
        private CEntityStation FindStationBySID(string sid)
        {
            if (this.m_stationLists == null)
                throw new Exception("GPRS模块未初始化站点！");

            CEntityStation result = null;
            foreach (var station in this.m_stationLists)
            {
                if (station.StationID.Equals(sid))
                {
                    result = station;
                    break;
                }
            }
            return result;
        }

        public int DSStartService(ushort port, int protocol, int mode, string mess, IntPtr ptr)
        {
            StringBuilder mess1 = new StringBuilder();
            currentPort = port;
            bool flag = false;
            int started = DTUdll.Instance.StartService(port, protocol, mode, mess1, ptr);
            if (started == 0)
            {
                tmrData.Start();
                tmrDTU.Start();
                flag = true;
            }
            if (SerialPortStateChanged != null)
                SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                {
                    PortType = this.m_portType,
                    PortNumber = port,
                    BNormal = flag
                }));
            //InvokeMessage(String.Format("开启端口{0}   {1}!", port, started ? "成功" : "失败"), "初始化");
            return started;
        }

        public int AddPort(ushort port)
        {
            currentPort = port;
            int result = -1;
            result = DTUdll.Instance.addPort(port);
            return result;
        }

        public int DSStopService(int nServerPort)
        {
            bool stoped = false;
            int ended = 0;
            ended = DTUdll.Instance.StopService(nServerPort);
            if (ended == 1)
            {
                stoped = true;
            }
            tmrData.Stop();
            tmrDTU.Stop();
            int port = DTUdll.Instance.ListenPort;
            if (SerialPortStateChanged != null)
                SerialPortStateChanged(this, new CEventSingleArgs<CSerialPortState>(new CSerialPortState()
                {
                    PortType = this.m_portType,
                    PortNumber = port,
                    BNormal = stoped
                }));
            InvokeMessage(String.Format("关闭端口{0}   {1}!", port, stoped ? "成功" : "失败"), "      ");
            return ended;
        }


        public ushort GetCurrentPort()
        {
            return currentPort;
        }
        public int sendHex(string userid, byte[] data, uint len, string mess)
        {
            int flag = 0;
            try
            {
                flag = DTUdll.Instance.SendHex(userid, data, len, null);
                return flag;

            }
            catch (Exception e)
            {
                return flag;
            }

        }

        public uint getDTUAmount()
        {
            return DTUdll.Instance.getDTUAmount();
        }
        public int getDTUInfo(string userid, out HDModemInfoStruct infoPtr)
        {
            infoPtr = new HDModemInfoStruct();
            return DTUdll.Instance.getDTUInfo(userid, out infoPtr);
        }
        public int getDTUByPosition(int index, out HDModemInfoStruct infoPtr)
        {
            infoPtr = new HDModemInfoStruct();
            return DTUdll.Instance.getDTUByPosition(index, out infoPtr);
        }
        public int getDTUList(out Dictionary<string, HDModemInfoStruct> dtuList)
        {
            return DTUdll.Instance.GetDTUList(out dtuList);
        }
        //帮助方法 20170602
        private int GetNextData(out HDModemDataStruct dat)
        {
            try
            {
                return DTUdll.Instance.GetNextData(out dat);
            }
            catch (Exception e)
            {
                dat = new HDModemDataStruct();
                return -1;
            }
        }
        private bool inDataTicks = false;
        private void tmrData_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inDataTicks || inDtuTicks) return;
            inDataTicks = true;
            try
            {
                //读取数据
                HDModemDataStruct dat = new HDModemDataStruct();
                while (this.GetNextData(out dat) == 0)

                {

                    //byte[] bts = new byte[] { 84, 82, 85, 13, 10 };
                    String str = System.Text.Encoding.Default.GetString(dat.m_data_buf);
                    String strid = System.Text.Encoding.Default.GetString(dat.m_modemId);
                    String strTime = System.Text.Encoding.Default.GetString(dat.m_recv_time);
                    //if (str.Contains("TRU"))
                    //    {
                    //        Debug.WriteLine("接收数据TRU完成,停止计时器");
                    //        //m_timer.Stop();
                    //        InvokeMessage("TRU " + strid , "接收");
                    //        if (this.ErrorReceived != null)
                    //            this.ErrorReceived.Invoke(null, new ReceiveErrorEventArgs()
                    //            {
                    //                //   Msg = "TRU " + dat.m_modemId
                    //                Msg = "TRU " + str
                    //            });
                    //    }
                    //if (str.Contains("1G21") || str.Contains("1G22") || str.Contains("1G25"))
                    //{
                    //    InvokeMessage("TRU " + strid, "发送");
                    //    this.sendHex(strid.Trim(), bts, (uint)bts.Length, null);
                    //}
                    m_mutexListDatas.WaitOne();
                    //Debug.WriteLine("协议接收数据: " + System.Text.Encoding.Default.GetString(dat.m_data_buf));
                    if ((strid.Substring(0, 1) != "/0")&&(strid.Substring(0, 1) != "\0"))
                    {
                        m_listDatas.Add(dat);
                    }
                    m_semaphoreData.Release(1);
                    m_mutexListDatas.ReleaseMutex();
                }
            }
            catch (Exception ee)
            {
                Debug.WriteLine("读取数据", ee.Message);
            }
            finally
            {
                inDataTicks = false;
            }
        }

        private void DealData()
        {
            while (true)
            {
                m_semaphoreData.WaitOne(); //阻塞当前线程，知道被其它线程唤醒
                // 获取对data内存缓存的访问权
                m_mutexListDatas.WaitOne();
                List<HDModemDataStruct> dataListTmp = m_listDatas;
                m_listDatas = new List<HDModemDataStruct>(); //开辟一快新的缓存区
                m_mutexListDatas.ReleaseMutex();
                for (int i = 0; i < dataListTmp.Count; ++i)
                {
                    try
                    {
                        HDModemDataStruct dat = dataListTmp[i];
                        string data = System.Text.Encoding.Default.GetString(dat.m_data_buf);
                        string temp = data.Trim();

                        string result = null;
                        if (temp.Contains("TRU"))
                        {
                            Debug.WriteLine("接收数据TRU完成,停止计时器");
                            //m_timer.Stop();
                            InvokeMessage("TRU " + System.Text.Encoding.Default.GetString(dat.m_modemId), "接收");
                            if (this.ErrorReceived != null)
                                this.ErrorReceived.Invoke(null, new ReceiveErrorEventArgs()
                                {
                                    //   Msg = "TRU " + dat.m_modemId
                                    Msg = "TRU " + System.Text.Encoding.Default.GetString(dat.m_modemId)
                                });
                        }
                        if (temp.Contains("ATE0"))
                        {
                            Debug.WriteLine("接收数据ATE0完成,停止计时器");
                            //m_timer.Stop();
                            // InvokeMessage("ATE0", "接收");
                            if (this.ErrorReceived != null)
                                this.ErrorReceived.Invoke(null, new ReceiveErrorEventArgs()
                                {
                                    Msg = "ATE0"
                                });
                        }
                        if (temp.Contains("$"))
                        {
                            result = temp.Substring(temp.IndexOf("$"), temp.IndexOf("\0"));
                            string sid = result.Substring(1, 4);
                            string type = result.Substring(5, 2);

                            //if (sid == "\0\0\0\0")
                            //{
                            //    continue;
                            //}

                            ////批量传输解析
                            //if (type == "1K")
                            //{
                            //    var station = FindStationBySID(sid);
                            //    if (station == null)
                            //        throw new Exception("批量传输，站点匹配错误");
                            //    CBatchStruct batch = new CBatchStruct();
                            //    if (Down.Parse_Flash(result, out batch))
                            //    {
                            //        InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                            //        if (this.BatchDataReceived != null)
                            //            this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                            //    }
                            //    if (Down.Parse_Batch(result, out batch))
                            //    {
                            //        InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                            //        if (this.BatchDataReceived != null)
                            //            this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                            //    }
                            //}

                            String dataProtocol = null;
                            if (HdProtocolMap.ContainsKey("HD-GPRS"))
                            {
                                dataProtocol = HdProtocolMap["HD-GPRS"];
                            }

                            switch (dataProtocol)
                            {
                                //case "LN":
                                //    break;
                                //case "SXDZ":
                                //    {
                                //        CReportStruct report = new CReportStruct();
                                //        CDownConf downReport = new CDownConf();
                                //        Data.SXDZ.UpParser Up1 = new Data.SXDZ.UpParser();
                                //        Data.SXDZ.DownParser down1 = new Data.SXDZ.DownParser();

                                //        //批量传输解析
                                //        if (type == "1K")
                                //        {
                                //            var station = FindStationBySID(sid);
                                //            if (station == null)
                                //                throw new Exception("批量传输，站点匹配错误");
                                //            CBatchStruct batch = new CBatchStruct();
                                //            if (down1.Parse_Flash(result, out batch))
                                //            {
                                //                InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                                //                if (this.BatchDataReceived != null)
                                //                    this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                //            }
                                //            //if (down1.Parse_Batch(result, out batch))
                                //            //{
                                //            //    InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                                //            //    if (this.BatchDataReceived != null)
                                //            //        this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                //            //}
                                //        }

                                //        Up1.Parse(result, out report);
                                //        down1.Parse(result, out downReport);
                                //        if (report != null)
                                //        {
                                //            report.ChannelType = EChannelType.GPRS;
                                //            report.ListenPort = this.GetListenPort().ToString();
                                //            InvokeMessage(String.Format("{0,-10}   ", "") + result, "接收");
                                //            //TODO 重新定义事件
                                //            if (this.UpDataReceived != null)
                                //            {
                                //                this.UpDataReceived.Invoke(null, new UpEventArgs() { Value = report, RawData = result });
                                //            }
                                //        }
                                //        if (downReport != null)
                                //        {
                                //            InvokeMessage(String.Format("{0,-10}   ", "下行指令读取参数") + result, "接收");
                                //            if (this.DownDataReceived != null)
                                //                this.DownDataReceived.Invoke(null, new DownEventArgs() { Value = downReport, RawData = result });
                                //        }
                                //        break;
                                //    }
                                //case "HJJBX":
                                //    //需要处理新老协议

                                //    //TODO 判断新老协议
                                //    bool isNew = false;

                                //    if (isNew)
                                //    {
                                //        List<CUpReport> reports = new List<CUpReport>();
                                //        Up = new Protocol.Data.HJJBX.UpParser();
                                //        Up.Parse_New(result, out reports);
                                //        foreach(var report2 in reports)
                                //        {
                                //            report2.ChannelType = EChannelType.GPRS;
                                //            report2.ListenPort = this.GetListenPort().ToString();
                                //        }
                                //        InvokeMessage(String.Format("{0,-10}   ","") + result, "接收");
                                //        //TODO 重新定义事件
                                //        if (this.UpDataReceived_new != null)
                                //        {
                                //            this.UpDataReceived_new.Invoke(null, new UpEventArgs_new() { Value = reports, RawData = result });
                                //        }

                                //    }
                                //    else
                                //    {
                                //        List<CUpReport> reports = new List<CUpReport>();
                                //        Up = new Protocol.Data.HJJBX.UpParser();
                                //        Up.Parse_Old(result, out reports);
                                //        foreach (var report1 in reports)
                                //        {
                                //            report1.ChannelType = EChannelType.GPRS;
                                //            report1.ListenPort = this.GetListenPort().ToString();
                                //        }
                                //        InvokeMessage(String.Format("{0,-10}   ", "") + result, "接收");
                                //        //测试代码
                                //        Dictionary<string, HDModemInfoStruct> data11 = new Dictionary<string, HDModemInfoStruct>();
                                //        this.getDTUList(out data11);

                                //        //TODO 重新定义事件
                                //        if (this.UpDataReceived_new != null)
                                //        {
                                //            this.UpDataReceived_new.Invoke(null, new UpEventArgs_new() { Value = reports, RawData = result });
                                //        }
                                //    }
                                //    break;
                                case "ZYJBX":
                                    {
                                        CReportStruct report = new CReportStruct();
                                        CDownConf downReport = new CDownConf();
                                        Data.ZYJBX.UpParser Up1 = new Data.ZYJBX.UpParser();
                                        Data.ZYJBX.DownParser down1 = new Data.ZYJBX.DownParser();

                                        //批量传输解析
                                        if (type == "1K")
                                        {
                                            var station = FindStationBySID(sid);
                                            if (station == null)
                                                throw new Exception("批量传输，站点匹配错误");
                                            CBatchStruct batch = new CBatchStruct();
                                            InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                                            if (down1.Parse_Flash(result,EChannelType.GPRS, out batch))
                                            {
                                                if (this.BatchDataReceived != null)
                                                    this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                            }else if (down1.Parse_Batch(result, out batch))
                                            {
                                                if (this.BatchDataReceived != null)
                                                    this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                            }
                                        }

                                        if (result.Contains("1G21")|| result.Contains("1G22")|| result.Contains("1G25"))
                                        {
                                            var lists = result.Split('$');
                                            foreach (var msg in lists)
                                            {
                                                if (msg.Length < 5)
                                                {
                                                    continue;
                                                }
                                                string plusMsg = "$" + msg;
                                                Up1.Parse(plusMsg, out report);
                                                if (report != null)
                                                {
                                                    string gprs = System.Text.Encoding.Default.GetString(dat.m_modemId);
                                                    gprs = gprs.Contains("\0")? gprs.Replace("\0", "") : gprs;
                                                    report.ChannelType = EChannelType.GPRS;
                                                    report.ListenPort = this.GetListenPort().ToString();
                                                    report.flagId = gprs;
                                                    string rtype = report.ReportType == EMessageType.EAdditional ? "加报" : "定时报";
                                                    InvokeMessage("gprs号码:  " + gprs + "   " + String.Format("{0,-10}   ", rtype) + plusMsg, "接收");
                                                    //TODO 重新定义事件
                                                    if (this.UpDataReceived != null)
                                                    {
                                                        this.UpDataReceived.Invoke(null, new UpEventArgs() { Value = report, RawData = plusMsg });
                                                    }
                                                    InvokeMessage("TRU " + gprs, "发送");
                                                    byte[] bts = new byte[] { 84, 82, 85, 13, 10 };
                                                    this.sendHex(gprs.Trim(), bts, (uint)bts.Length, null);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            down1.Parse(result, out downReport);
                                            if (downReport != null)
                                            {
                                                InvokeMessage(String.Format("{0,-10}   ", "下行指令读取参数") + result, "接收");
                                                if (this.DownDataReceived != null)
                                                    this.DownDataReceived.Invoke(null, new DownEventArgs() { Value = downReport, RawData = result });
                                            }
                                        }
                                        
                                        break;
                                    }
                                //case "XYJBX":
                                //    {
                                //        CReportStruct report = new CReportStruct();
                                //        CDownConf downReport = new CDownConf();
                                //        Data.XYJBX.UpParser Up1 = new Data.XYJBX.UpParser();
                                //        Data.XYJBX.DownParser down1 = new Data.XYJBX.DownParser();

                                //        //批量传输解析
                                //        if (type == "1K")
                                //        {
                                //            var station = FindStationBySID(sid);
                                //            if (station == null)
                                //                throw new Exception("批量传输，站点匹配错误");
                                //            CBatchStruct batch = new CBatchStruct();
                                //            //if (down1.Parse_Flash(result, out batch))
                                //            //{
                                //            //    InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                                //            //    if (this.BatchDataReceived != null)
                                //            //        this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                //            //}
                                //            //if (down1.Parse_Batch(result, out batch))
                                //            //{
                                //            //    InvokeMessage(String.Format("{0,-10}   ", "批量传输") + temp, "接收");

                                //            //    if (this.BatchDataReceived != null)
                                //            //        this.BatchDataReceived.Invoke(null, new BatchEventArgs() { Value = batch, RawData = temp });
                                //            //}
                                //        }

                                //        Up1.Parse(result, out report);
                                //        down1.Parse(result, out downReport);
                                //        if (report != null)
                                //        {
                                //            report.ChannelType = EChannelType.GPRS;
                                //            report.ListenPort = this.GetListenPort().ToString();
                                //            InvokeMessage(String.Format("{0,-10}   ", "") + result, "接收");
                                //            //TODO 重新定义事件
                                //            if (this.UpDataReceived != null)
                                //            {
                                //                this.UpDataReceived.Invoke(null, new UpEventArgs() { Value = report, RawData = result });
                                //            }
                                //        }
                                //        if (downReport != null)
                                //        {
                                //            InvokeMessage(String.Format("{0,-10}   ", "下行指令读取参数") + result, "接收");
                                //            if (this.DownDataReceived != null)
                                //                this.DownDataReceived.Invoke(null, new DownEventArgs() { Value = downReport, RawData = result });
                                //        }
                                //        break;
                                //    }
                                default:
                                    break;
                            }
                        }
                        //List<CUpReport> reports = new List<CUpReport>();
                        //Up = new UpParser();
                        //Up.Parse_New(result, out reports);


                        //获取DTU列表的函数,DTU在线列表测试
                        //uint amount = this.getDTUAmount();
                        //HDModemInfoStruct data1= new HDModemInfoStruct();
                        //this.getDTUByPosition(0, out data1);
                        //Dictionary<string, HDModemInfoStruct> datas = new Dictionary<string, HDModemInfoStruct>();
                        //this.getDTUList(out datas);
                        if (temp.Contains("BEG"))
                        {
                            Data.ZYJBX.DownParser down1 = new Data.ZYJBX.DownParser();
                            CSDStruct sd = new CSDStruct();
                            string gprs = System.Text.Encoding.Default.GetString(dat.m_modemId);
                            gprs = gprs.Replace("\0", "");
                            //string gprs = dat.m_modemId.ToString();
                            string id = Manager.XmlStationDataSerializer.Instance.GetStationByGprsID(gprs);
                            if (down1.Parse_SD(temp, id, out sd))
                            {
                                InvokeMessage(String.Format("{0,-10}   ", "批量SD传输") + temp, "接收");

                                if (this.BatchSDDataReceived != null)
                                    this.BatchSDDataReceived.Invoke(null, new BatchSDEventArgs() { Value = sd, RawData = temp });
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("" + e.Message);
                    }
                }
            }
        }
        #region 接口函数
        public ushort GetListenPort()
        {
            return DTUdll.Instance.ListenPort;
        }

        public bool FindByID(string userID, out byte[] dtuID)
        {
            dtuID = null;
            List<HDModemInfoStruct> DTUList_1 = DTUList;
            //foreach (var item in DTUList_1)
            for (int i = 0; i < DTUList_1.Count; i++)
            {
                HDModemInfoStruct item = DTUList_1[i];
                if (System.Text.Encoding.Default.GetString(item.m_modemId).Substring(0, 11) == userID)
                {
                    dtuID = item.m_modemId;
                    return true;
                }
            }
            return false;
        }

        public void SendDataTwice(string id, string msg)
        {
            m_timer.Interval = 600;
            SendData(id, msg);
            if (s_isFirstSend)
            {
                MyMessage myMsg = new MyMessage() { ID = id, MSG = msg };
                s_isFirstSend = false;
                Thread t = new Thread(new ParameterizedThreadStart(ResendRead))
                {
                    Name = "重新发送读取线程",
                    IsBackground = true
                };
                t.Start(myMsg);
            }
        }

        public void SendDataTwiceForBatchTrans(string id, string msg)
        {
            m_timer.Interval = 60000;
            SendData(id, msg);
            if (s_isFirstSend)
            {
                MyMessage myMsg = new MyMessage() { ID = id, MSG = msg };
                s_isFirstSend = false;
                Thread t = new Thread(new ParameterizedThreadStart(ResendRead))
                {
                    Name = "重新发送读取线程",
                    IsBackground = true
                };
                t.Start(myMsg);
            }
        }

        #endregion

        #region 帮助函数
        public bool SendData(string id, string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return false;
            }
            //      Debug.WriteLine("GPRS发送数据:" + msg);
            InvokeMessage(msg, "发送");
            //      Debug.WriteLine("先停止计时器，然后在启动计时器");
            //  先停止计时器，然后在启动计时器
            m_timer.Stop();
            m_timer.Start();
            byte[] bmesg = System.Text.Encoding.Default.GetBytes(msg);
            if (DTUdll.Instance.SendHex(id, bmesg, (uint)bmesg.Length, null) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ResendRead(object obj)
        {
            Debug.WriteLine(System.Threading.Thread.CurrentThread.Name + "休息1秒!");
            System.Threading.Thread.Sleep(1000);
            try
            {
                MyMessage myMsg = obj as MyMessage;
                if (null != myMsg)
                {
                    SendData(myMsg.ID, myMsg.MSG);
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
            finally { s_isFirstSend = true; }
        }


        #endregion
    }
}
