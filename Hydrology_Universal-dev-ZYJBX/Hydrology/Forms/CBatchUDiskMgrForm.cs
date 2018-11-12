using System;
using System.Diagnostics;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;

namespace Hydrology.Forms
{
    public partial class CBatchUDiskMgrForm : Form
    {
        private EChannelType m_channelType = EChannelType.GPRS;

        public CBatchUDiskMgrForm()
        {
            InitializeComponent();

            Init();

            // 初始化焦点切换
            FormHelper.InitControlFocusLoop(this);
        }

        private void Init()
        {
            this.Height = this.Height - 40;
            this.Text = "批量传输 - U盘数据";
            dtp_StartTime.Format = DateTimePickerFormat.Custom;
            dtp_StartTime.CustomFormat = "yyyy-MM-dd HH";
            dtp_StartTime.Value = DateTime.Now;

            ReloadComboBox();
            ReloadListView();

            CProtocolEventManager.BatchForUI += this.BatchForUI_EventHandler;
            CProtocolEventManager.ErrorForUI += this.ErrorForUI_EventHandler;
        }
        private void ReloadComboBox()
        {
            this.groupBox2.Controls.Remove(this.cmbStation);
            this.cmbStation = new CStationComboBox(EStationBatchType.EUPan);
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(172, 17);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(167, 20);
            this.cmbStation.TabIndex = 4;
            this.groupBox2.Controls.Add(this.cmbStation);
        }
        private void ReloadListView()
        {
            this.groupBox3.Controls.Remove(this.listView1);

            this.listView1 = new CExListView() { BHorizentalScroolVisible = false };
            this.listView1.BackColor = System.Drawing.SystemColors.Info;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;

            this.listView1.Columns.Add("表头", -2, HorizontalAlignment.Left);
            this.groupBox3.Controls.Add(this.listView1);
        }

        private void ErrorForUI_EventHandler(object sender, ReceiveErrorEventArgs e)
        {
            string msg = e.Msg;
            if (this.IsHandleCreated)
            {
                try
                {
                    this.listView1.Invoke((Action)delegate
                    {
                        try
                        {
                            this.listView1.Items.Add(new ListViewItem()
                            {
                                Text = String.Format("({0}) 接收数据 : {1}", this.m_channelType.ToString(), msg)
                            });

                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    });

                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            }
        }
        private void BatchForUI_EventHandler(object sender, BatchEventArgs e)
        {
            var result = e.Value;
            string rawData = e.RawData;
            if (result != null)
            {
                if (this.IsHandleCreated)
                {
                    //  导入数据库
                    BatchDataImport.Import(result);

                    //  更新界面
                    this.listView1.Invoke((Action)delegate
                    {
                        try
                        {
                            this.listView1.Items.Add(new ListViewItem()
                            {
                                Text = "接收数据 : " + rawData
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        try
                        {
                            this.listView1.Items.Add(new ListViewItem()
                            {
                                Text = "站点ID : " + result.StationID
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        try
                        {
                            this.listView1.Items.Add(new ListViewItem()
                            {
                                Text = "站点类型: " + result.StationType == "01" ? "水位" : "雨量"
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        foreach (var item in result.Datas)
                        {
                            try
                            {
                                this.listView1.Items.Add(new ListViewItem()
                                {
                                    Text = item.Time.ToString() + "    " + item.Data
                                });
                            }
                            catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        }
                    });
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (this.listView1.Items.Count > 0)
                this.listView1.Items.Clear();
        }
        private void btnStartTrans_Click(object sender, EventArgs e)
        {
            var station = (this.cmbStation as CStationComboBox).GetStation();
            if (station == null)
                return;

            string sid = station.StationID;

            var stype = station.StationType;
            station.StationType = radioRain.Checked ? EStationType.ERainFall : EStationType.EHydrology;
            ETrans trans = this.radioHour.Checked ? ETrans.ByHour : ETrans.ByDay;

            // 写入系统日志
            string logMsg = String.Format("--------U盘批量传输    目标站点（{0:D4}）--------- ", int.Parse(sid));
            CSystemInfoMgr.Instance.AddInfo(logMsg);
            this.listView1.Items.Add(logMsg);

            string qry = CPortDataMgr.Instance.SendUDiskMsg(station, trans, this.dtp_StartTime.Value, this.m_channelType);
            this.listView1.Items.Add(new ListViewItem()
            {
                Text = String.Format("[{0}]  发送数据:  {1}", this.m_channelType.ToString(), qry)
            });
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioHour.Checked)
            {
                // 小时选择模式
                dtp_StartTime.CustomFormat = "yyyy-MM-dd HH";
            }
            else
            {
                // 整天选择模式
                dtp_StartTime.CustomFormat = "yyyy-MM-dd";
            }
        }
    }

    class BatchDataImport
    {
        public static void Import(CBatchStruct batch, bool isUpdate = false)
        {
            // 站点类型
            //     01为水位 
            //     02为雨量
            //  若数据库中没有时间相同的记录，则更新
            //  若数据库中包含时间相同的记录，且isUpdate = true 判断是否更新
        }
    }
}
