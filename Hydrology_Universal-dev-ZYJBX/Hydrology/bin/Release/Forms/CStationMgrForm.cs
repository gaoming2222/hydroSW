using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using System.IO;
using System.Diagnostics;
using Hydrology.Entity;

namespace Hydrology.Forms
{
    public partial class CStationMgrForm : Form
    {
        /// <summary>
        /// 水情站的dgv
        /// </summary>
        private CDataGridViewStation m_dgvStatioin;

        public CStationMgrForm()
        {
            InitializeComponent();
            InitUI();
            InitDataSource();      
            CreateMsgBinding();
        }
        // 初始化数据源
        public void InitDataSource()
        {
            m_dgvStatioin.InitDataSource();
        }

        private void InitUI()
        {
            tableLayoutPanel.SuspendLayout();
            m_dgvStatioin = new CDataGridViewStation();
            m_dgvStatioin.AllowUserToAddRows = false;
            m_dgvStatioin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
            //m_dgvRain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dgvStatioin.Dock = DockStyle.Fill;
            //m_dgvUser.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dgvStatioin.AllowUserToResizeRows = false;
            m_dgvStatioin.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dgvStatioin.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            m_dgvStatioin.RowHeadersWidth = 60;
            m_dgvStatioin.ColumnHeadersHeight = 25;

            m_dgvStatioin.SetEditMode(true); //目前只支持编写了编辑模式


            tableLayoutPanel.Controls.Add(m_dgvStatioin, 0, 1);
            tableLayoutPanel.ResumeLayout(false);
        }

        private void EHFormLoad(object sender, EventArgs e)
        {
            // 加载数据
            m_dgvStatioin.LoadData();
        }

        private void EHStationCountChanged(object sender, Entity.CEventSingleArgs<int> e)
        {
            labelUserCount.Text = String.Format("共 {0} 个水情测站配置", e.Value);
        }

        private void EHSave(object sender, EventArgs e)
        {
            m_dgvStatioin.DoSave();
        }

        private void EHFormClosing(object sender, FormClosingEventArgs e)
        {
            // 窗体关闭事件，检测是否需要保存数据
            if (!m_dgvStatioin.Close())
            {
                // 不让退出
                DialogResult result = MessageBox.Show("是否强行退出？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (DialogResult.Yes == result)
                {
                    // 强行退出
                    //e.Cancel = true;
                }
                else
                {
                    e.Cancel = true;
                }
                //e.Cancel = true;
            }
        }

        private void EHRevert(object sender, EventArgs e)
        {
            m_dgvStatioin.Revert();
        }

        private void EHExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void EHAddStation(object sender, EventArgs e)
        {
            //  m_dgvStatioin.AddNewDataConfig();

            //注册form2_MyEvent方法的MyEvent事件

            CStationMgrForm2 form = new CStationMgrForm2();
            form.AddStationEvent += new MyDelegate(form_MyEvent);
            if (form != null)
            {
                form.ShowDialog();
            }
        }

        //处理
        void form_MyEvent(CEntityStation station)
        {
            m_dgvStatioin.m_listAddedStation.Add(station);
            m_dgvStatioin.m_proxyStation.AddRange(m_dgvStatioin.m_listAddedStation);
            //m_dgvStatioin.Revert();
            // 重新加载
            CDBDataMgr.Instance.UpdateAllStation();
            m_dgvStatioin.Revert();
            m_dgvStatioin.UpdateDataToUI();
            //this.listBox1.Items.RemoveAt(index);
            //this.listBox1.Items.Insert(index, text);
        }

        private void EHDeleteStation(object sender, EventArgs e)
        {
            m_dgvStatioin.DoDelete();
        }

        private void EHImport(object sender, EventArgs e)
        {
            // 从文件中导入，格式如(1,212.23,2230.344)，如果不对，不导入
            OpenFileDialog dlg = new OpenFileDialog();
            try
            {
                dlg.Title = "选择水情测站文件";
                dlg.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                DialogResult result = dlg.ShowDialog();
                if (DialogResult.OK == result)
                {

                    // 打开文件，并进行处理
                    CMessageBox msgBox = new CMessageBox() { MessageInfo = "正在处理数据" };
                    msgBox.ShowDialog(this);
                    StreamReader reader = new StreamReader(dlg.FileName, Encoding.Default);
                    string linebuffer;
                    int linenumber = 0;
                    string strErrorInfo = "";
                    while ((linebuffer = reader.ReadLine()) != null)
                    {
                        // 处理一行数据
                        linenumber += 1;
                        if (!DealWithLineData(linebuffer, linenumber, ref strErrorInfo))
                        {
                            // 数据非法
                            msgBox.CloseDialog();
                            MessageBox.Show(strErrorInfo); // 显示错误信息
                            return;
                        }
                    }
                    reader.Close();
                    msgBox.CloseDialog();
                    MessageBox.Show("数据导入成功");
                }//end of ok
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Focus(); //防止窗体最小化
            }

        }

        /// <summary>
        /// 处理单行数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool DealWithLineData(string str, int lineNumber, ref string strErroInfo)
        {
            Boolean flag = false;
            if (str.StartsWith("(") && str.EndsWith(")"))
            {
                // gm 添加
                string tmp = str.Substring(1, str.Length - 2);
                string[] arrayStr = tmp.Split(',');
                if (arrayStr.Length == 21)
                {
                    if (arrayStr[0].Length == 4 && arrayStr[1].Length == 1)
                        try
                        {
                            // 刚好三个的话，正好
                            int one1 = Convert.ToInt32(arrayStr[0]);
                            int one2 = Convert.ToInt32(arrayStr[1]);

                            string stationId = arrayStr[0].Trim();
                            if ( CDBDataMgr.Instance.GetStationById(stationId)!=null)
                            {
                                strErroInfo = string.Format("行：{0} 站点不能重复\"{2}\"", lineNumber, stationId);
                                return false;
                            }
                            int subcenterid = int.Parse(arrayStr[1]);
                            string stationname = arrayStr[2].Trim();
                            int stationtype = int.Parse(arrayStr[3]);

                            Decimal WBase = Decimal.Parse(arrayStr[4]);
                            Decimal WMax = Decimal.Parse(arrayStr[5]);
                            Decimal WMin = Decimal.Parse(arrayStr[6]);
                            Decimal WChange = Decimal.Parse(arrayStr[7]);
                            Decimal RAccuracy = Decimal.Parse(arrayStr[8]);
                            Decimal RChange = Decimal.Parse(arrayStr[9]);
                            
                            string Gsm = arrayStr[10].Trim();
                            string Gprs = arrayStr[11].Trim();
                            string BDSatellite = arrayStr[12].Trim();
                            string BDmember = arrayStr[13].Trim();
                            Decimal VoltageMin = Decimal.Parse(arrayStr[14]);
                            string maintran = arrayStr[15].Trim();
                            string subtran = arrayStr[16].Trim();
                            string dataprotocol = arrayStr[17].Trim();
                            string watersensor = arrayStr[18].Trim();
                            string rainsensor = arrayStr[19].Trim();
                            string reportinterval = arrayStr[20].Trim();
                            if (arrayStr[0].Length == 4 && one1 <= 9999)
                            {
                                flag = true;
                            }
                        }
                        catch
                        {

                        }
                }

            }
            else
            {
                // 格式不对
                strErroInfo = (string.Format("行：{0} 开始结束符号\"(\"\")\"格式错误", lineNumber));

            }
            return flag;

            //if (str.StartsWith("(") && str.EndsWith(")"))
            //{

            //    return false;
            //}
            //else
            //{
            //    // 格式不对
            //    strErroInfo = (string.Format("行：{0} 开始结束符号\"(\"\")\"格式错误", lineNumber));
            //    return false;
            //}
        }


        /// <summary>
        /// 建立消息绑定
        /// </summary>
        private void CreateMsgBinding()
        {
            m_dgvStatioin.E_StationCountChanged += new EventHandler<Entity.CEventSingleArgs<int>>(EHStationCountChanged);
            this.Load += new EventHandler(EHFormLoad);
            this.FormClosing += new FormClosingEventHandler(EHFormClosing);

            tsButAdd.Click += new EventHandler(EHAddStation);
            tsButDelete.Click += new EventHandler(EHDeleteStation);
            tsButExit.Click += new EventHandler(EHExit);
            tsButRevert.Click += new EventHandler(EHRevert);
            tsButSave.Click += new EventHandler(EHSave);
            tsButImport.Click += new EventHandler(EHImport);
        }

        private void textBox_Search_TextChanged(object sender, EventArgs e)
        {
  
            // 如果输入框中的文本发生变化，则更新列表框中显示的站点
            string filter = textBox_Search.Text;
            // 隐藏某些行，再说
            //for (int i = 0; i < listBox_StationName.Items.Count; ++i)
            //{
            //    if (listBox_StationName.Items[i].ToString().Contains(filter))
            //    {
            //        //listBox_StationName.Items[i].
            //    }
            //}
            // 全部重新加载
          //  listBox_StationName.Items.Clear();
          //  m_iPreSelectedStationIndex = -1;
            List<CEntityStation> m_listStation = CDBDataMgr.Instance.GetAllStationData();
            List<CEntityStation> m_listStation1=new List<CEntityStation>();
            foreach (CEntityStation station in m_listStation)
            {
                string tmp = GetDisplayStationName(station);
                if (tmp.Contains(filter))
                {
                    m_listStation1.Add(CDBDataMgr.Instance.GetStationById(station.StationID));
                   // listBox_StationName.Items.Add(tmp);
                    m_dgvStatioin.SetStation(m_listStation1);
                }
            }
       // }
        }

        private string GetDisplayStationName(CEntityStation station)
        {
            return string.Format("({0,-4}|{1})", station.StationID, station.StationName);
        }

        private void tsButImport_Click(object sender, EventArgs e)
        {
            // 从文件中导入，格式如(1,212.23,2230.344)，如果不对，不导入
            OpenFileDialog dlg = new OpenFileDialog();
            try
            {
                dlg.Title = "选择水情测站文件";
                dlg.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
                DialogResult result = dlg.ShowDialog();
                if (DialogResult.OK == result)
                {
                    // 打开文件，并进行处理
                    CMessageBox msgBox = new CMessageBox() { MessageInfo = "正在处理数据" };
                    msgBox.ShowDialog(this);
                    StreamReader reader = new StreamReader(dlg.FileName, Encoding.Default);
                    string linebuffer;
                    int linenumber = 0;
                    string strErrorInfo = "";
                    while ((linebuffer = reader.ReadLine()) != null)
                    {
                        // 处理一行数据
                        linenumber += 1;
                        if (!DealWithLineData(linebuffer, linenumber, ref strErrorInfo))
                        {
                            // 数据非法
                            msgBox.CloseDialog();
                            MessageBox.Show(strErrorInfo); // 显示错误信息
                            return;
                        }
                    }
                    reader.Close();
                    msgBox.CloseDialog();
                    MessageBox.Show("数据导入成功");
                }//end of ok
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                this.Focus(); //防止窗体最小化
            }
        }

    }
}


//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;

//namespace Hydrology.Forms
//{
//    public partial class CStationMgrForm : Form
//    {
//        public CStationMgrForm()
//        {
//            InitializeComponent();
//        }
//    }
//}