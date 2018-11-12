using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.DBManager;
using Protocol.Manager;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Threading;
using Hydrology.DBManager.Interface;

namespace Hydrology.Forms
{
    public delegate void DelUserHandler(string str);
    public partial class Form1 : Form
    {
        private IStationProxy m_proxyStation; //数据库接口对象
        //public Form2 f2;
        public Form1()
        {
            m_proxyStation = CDBDataMgr.Instance.GetStationProxy();
            InitializeComponent();

            //CEntityStation entity = new CEntityStation();
            //entity.StationID = "6018";
            //entity.GPRS = "60006018";
            //CPortDataMgr.Instance.SendHex(entity);
        }

        //public static void sendMyTru(String stationid)
        //{
        //    CEntityStation entity = new CEntityStation();
        //    //  entity.StationID = "6018";
        //    entity = CDBDataMgr.Instance.GetStationById(stationid);
        //    //  entity.StationID = stationid;
        //    //  entity.GPRS = "60006018";
        //    Form1 f1 = new Form1();
        //    //  entity.GPRS = f1.getGPRSback(stationid);
        //    if (entity != null)
        //    {
        //        CPortDataMgr.Instance.SendHex(entity);
        //    }
        //}
        //public static void sendMyTru(String stationid)
        //{
        //    CEntityStation entity = new CEntityStation();

        //    //  entity.StationID = "6018";
        //    entity = CDBDataMgr.Instance.GetStationById(stationid);
        //    //  entity.StationID = stationid;
        //    //  entity.GPRS = "60006018";
        //    Form1 f1 = new Form1();
        //    //  entity.GPRS = f1.getGPRSback(stationid);
        //    if (entity != null)
        //    {
        //        CPortDataMgr.Instance.SendHex(entity);
        //    }
        //    else
        //    {
        //        CEntitySoilStation soilEntity = new CEntitySoilStation();
        //        soilEntity = CDBSoilDataMgr.Instance.GetSoilStationInfoByStationId(stationid);
        //        if (soilEntity != null)
        //        {
        //            CPortDataMgr.Instance.SendSoilHex(soilEntity);
        //        }
        //    }
        //}



        public string getGPRSback(String str)
        {
            return m_proxyStation.QueryGPRSById(str);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //    CPortDataMgr.Instance.StartGprs();
            CEntityStation entity = new CEntityStation();
            entity.StationID = "6018";
            entity.GPRS = "60006018";
            CPortDataMgr.Instance.SendHex(entity);
        }

        //  public static void SendTru(){
        //这个就是我们的函数，我们把要对控件进行的操作放在这里
        public void SendTru(string str)
        {
            DelUserHandler handler = new DelUserHandler(SendTru);
            //this.Invoke(handler, new object[] { SendTru }); 
            CEntityStation entity = new CEntityStation();
            entity.StationID = "6018";
            entity.GPRS = "60006018";
            CPortDataMgr.Instance.SendHex(entity);
            //Button btn1 = new Button();
            //btn1.PerformClick();
            // button1_Click(this, null);
            // button1.InvokeMember("click");
            //CEntityStation entity = new CEntityStation();
            //entity.StationID = "6018";
            //entity.GPRS = "60006018";
            //CPortDataMgr.Instance.SendHex(entity);
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        ////    Form1.SendTru();
        //    ////Thread thread2 = new Thread(threadPro);//创建新线程 
        //    ////thread2.Start();

        //    //Thread thread2 = new Thread(threadPro);//创建新线程 
        //    //thread2.Start();
        //    //Form1.sendMyTru();
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            this.Text = threadId.ToString();
            // f2 = new Form2();
            //while (!Thread.CurrentThread.IsAlive)
            //{
            //    Thread.Sleep(3000);
            //}

            //for (char i = 'a'; i < 'k'; i++)
            //{
            //  //  Console.WriteLine("主线程：{0}", i);
            //    Thread.Sleep(100);
            //}
            //thread2.Join(); //主线程等待辅助线程结束  

            //CEntityStation entity = new CEntityStation();
            //entity.StationID = "6018";
            //entity.GPRS = "60006018";
            //CPortDataMgr.Instance.SendHex(entity);
        }

        //public void threadPro()
        //{
        //   //// button1_Click(button1, new EventArgs());
        //   // this.Show();
        //    //f2.Show();
        //    MethodInvoker MethInvo = new MethodInvoker(ShowForm2);
        //    BeginInvoke(MethInvo);  
        //}

        //public void ShowForm2()
        //{
        //    Form2 f2 = new Form2();
        //    f2.Show();
        //}  
    }
}
