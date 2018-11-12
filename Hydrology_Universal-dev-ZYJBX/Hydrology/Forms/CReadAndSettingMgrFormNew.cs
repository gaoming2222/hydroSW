using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.DataMgr;
using Hydrology.Entity;
using Hydrology.Utils;

namespace Hydrology.Forms
{
    public partial class CReadAndSettingMgrFormNew : Form
    {
        private EReadOrSetStatus m_RSStatus = EReadOrSetStatus.None;
        private EChannelType m_channelType = EChannelType.GPRS;
        private List<string> m_vNormalStateLists = new List<string>();
        private List<string> m_vWorkStatusLists = new List<string>();
        private List<int> m_vTimePeriodLists = new List<int>();
        private List<string> m_vTimeChoiceLists = new List<string>();
        private List<string> m_vMainChannelLists = new List<string>();
        private List<string> m_vViceChannelLists = new List<string>();
        private List<string> m_vSelectCollectionParagraphsLists = new List<string>();
        private List<string> m_vSensorLists = new List<string>();
        private List<string> m_vWaterLists = new List<string>();
        private string m_CmdStr = string.Empty;
        private string m_StatusIDStr = string.Empty;
        private CEntityStation m_CurrentStation;
        public CReadAndSettingMgrFormNew()
        {
            InitializeComponent();
            this.label19.Hide();
            InitComboBoxDataSource();
            Init();
            FormHelper.InitControlFocusLoop(this);
            FormHelper.InitUserModeEvent(this);
            CProtocolEventManager.DownForUI += this.DownForUI_EventHandler;
            CProtocolEventManager.ErrorForUI += this.ErrorForUI_EventHandler;
            CProtocolEventManager.GPRS_TimeOut4UI += (s, e) =>
            {
                if (null != e)
                {
                    //   SetWarningInfo(string.Format("GPRS: {0}{1}站点数据，接收数据时间超过{2}毫秒!", m_CmdStr, m_StatusIDStr, e.Second), Color.Red);
                    //   m_RSStatus = EReadOrSetStatus.None;
                }
            };
            CProtocolEventManager.GSM_TimeOut4UI += (s, e) =>
            {
                if (null != e)
                {
                    //    SetWarningInfo(string.Format("GSM: {0}{1}站点数据，接收数据时间超过{2}毫秒!", m_CmdStr, m_StatusIDStr, e.Second), Color.Red);
                    //    AddLog(string.Format("GSM: {0}{1}站点数据，接收数据时间超过{2}毫秒!", m_CmdStr, m_StatusIDStr, e.Second));
                    //    m_RSStatus = EReadOrSetStatus.None;
                }
            };
            InitListView();
        }
        //  初始化ComboBox数据源列表
        private void InitComboBoxDataSource()
        {
            //  初始化站点列表信息
            this.groupBox1.Controls.Remove(this.cmbStation);
            cmbStation = new CStationComboBox();
            this.cmbStation.FormattingEnabled = true;
            this.cmbStation.Location = new System.Drawing.Point(107, 25);
            this.cmbStation.Name = "cmbStation";
            this.cmbStation.Size = new System.Drawing.Size(130, 20);
            this.cmbStation.TabIndex = 26;
            (this.cmbStation as CStationComboBox).StationSelected += new EventHandler<CEventSingleArgs<CEntityStation>>(CReadAndSettingMgrForm_StationSelected);
            this.groupBox1.Controls.Add(this.cmbStation);

            //  初始化通讯方式列表
            var msgLists = new List<String>()
            {
                EChannelType.GPRS.ToString(),
                EChannelType.GSM.ToString(),
                //EChannelType.BeiDou.ToString(),
                //EChannelType.PSTN.ToString()
            };
            this.cmbMsgType.DataSource = msgLists;

            //  初始化常规状态,并绑定控件
            // TODO
            foreach (var item in ProtocolMaps.NormalState4UI)
            {
                m_vNormalStateLists.Add(item.Value);
            }
            this.vNormalState.DataSource = m_vNormalStateLists;
            //  初始化工作状态,并绑定控件
            foreach (var item in ProtocolMaps.WorkStatus4UI)
            {
                m_vWorkStatusLists.Add(item.Value);
            }
            this.vWorkStatus.DataSource = m_vWorkStatusLists;
            //  初始化定时段次
            foreach (var item in ProtocolMaps.TimePeriodMap)
            {
                m_vTimePeriodLists.Add(Int32.Parse(item.Value));
            }
            this.vTimePeriod.DataSource = m_vTimePeriodLists;
            //  初始化对时选择
            foreach (var item in ProtocolMaps.TimeChoice4UI)
            {
                m_vTimeChoiceLists.Add(item.Value);
            }
            this.vTimeChoice.DataSource = m_vTimeChoiceLists;
            foreach (var item in ProtocolMaps.SensorType4UIMap)
            {
                m_vSensorLists.Add(item.Value);
            }
            this.vSensorType.DataSource = m_vSensorLists;
            //  初始化水位选择
            foreach (var item in ProtocolMaps.WaterType4UIMap)
            {
                m_vWaterLists.Add(item.Value);
            }
            this.cmbWater.DataSource = m_vWaterLists;
            //  初始化主用信道
            m_vMainChannelLists = new List<String>()
            {
                EChannelType.GPRS.ToString(),
                EChannelType.GSM.ToString(),
                ProtocolMaps.ChannelType4UIMap.FindValue(EChannelType.BeiDou),
                EChannelType.PSTN.ToString(),
                EChannelType.VHF.ToString()
            };
            this.vMainChannel.DataSource = m_vMainChannelLists;
            //  初始化备用信道
            m_vViceChannelLists = new List<String>()
            {
                EChannelType.GPRS.ToString(),
                EChannelType.GSM.ToString(),
                ProtocolMaps.ChannelType4UIMap.FindValue(EChannelType.BeiDou),
                EChannelType.PSTN.ToString(),
                EChannelType.VHF.ToString(),
                ProtocolMaps.ChannelType4UIMap.FindValue(EChannelType.None)
            };
            this.vViceChannel.DataSource = m_vViceChannelLists;
            //  初始化采集端次选
            foreach (var item in ProtocolMaps.SelectCollectionParagraphs4UIMap)
            {
                m_vSelectCollectionParagraphsLists.Add(item.Value);
            }
            this.vSelectCollectionParagraphs.DataSource = m_vSelectCollectionParagraphsLists;
        }
        private void Init()
        {
            /************工作配置*****************/
            //  初始化时钟
            this.vClock.Value = DateTime.Now;
            //  初始化电压
            this.vVoltage.Value = 0;
            //  初始化站号
            this.vStationCmdID.Text = string.Empty;
            //  初始化常规状态
            this.vNormalState.SelectedIndex = 0;
            //  初始化版本号
            this.vVersionNum.Text = string.Empty;
            //  初始化工作状态
            this.vWorkStatus.SelectedIndex = 1;
            //  初始化定时段次
            this.vTimePeriod.SelectedIndex = 6;
            //  初始化对时选择
            this.vTimeChoice.SelectedIndex = 0;
            /************通訊配置*****************/
            //  初始化主备信道
            this.vMainChannel.SelectedIndex = 0;
            //  初始化备用信道
            this.vViceChannel.SelectedIndex = 1;
            //  初始化目的地手机
            this.vDestPhoneNum.Text = string.Empty;
            //  初始化SIM卡号
            this.vTeleNum.Text = string.Empty;
            //  初始化终端机号
            this.vTerminalNum.Text = string.Empty;
            //  初始化响应波束
            this.vRespBeam.Text = string.Empty;
            //  初始化振铃次数
            this.vRingsNum.Value = 1;

            /************工作配置*****************/
            //  初始化测站类型
            this.vStationType.Text = string.Empty;
            //  初始化水位
            this.vWater.Value = 0;
            //  初始化雨量
            this.vRain.Value = 0;
            //  初始化水位加报值
            this.vWaterPlusReportedValue.Value = 0;
            //  初始化雨量加报值
            this.vRainPlusReportedValue.Value = 0;
            //  初始化K
            this.vK.Text = string.Empty;
            //  初始化C
            this.vC.Text = string.Empty;
            //  初始化平均时间           
            this.vAvegTime.Text = string.Empty;
            //  初始化采集端次选
            this.vSelectCollectionParagraphs.SelectedIndex = 0;

            this.lblWarning.Text = string.Empty;
        }
        private void InitListView()
        {
            // 初始化listView
            listView1.Columns.Add("表头", -2, HorizontalAlignment.Left);
            listView1.HeaderStyle = ColumnHeaderStyle.None;

            // 设置ListView的行高
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 12);//设置 ImageList 的宽和高
            listView1.SmallImageList = imgList;
            listView1.View = View.Details;
            listView1.Dock = DockStyle.Fill;
        }

        private void ErrorForUI_EventHandler(object sender, ReceiveErrorEventArgs e)
        {
            string msg = e.Msg;
            if (this.IsHandleCreated)
            {
                try
                {
                    if (msg != "ATE0" && (!msg.Contains("超过")))
                    {
                        AddLog("接收数据 : " + msg);
                    }
                    switch (m_RSStatus)
                    {
                        case EReadOrSetStatus.None:
                            break;
                        case EReadOrSetStatus.Read:
                            //if (msg.Contains("ATE0"))
                            //{
                            //    SetWarningInfo(string.Format("{0}{1}站点数据失败!", m_CmdStr, m_StatusIDStr), Color.Red);
                            //}
                            //SetWarningInfo(String.Format("{0}{1}站点数据为ATE0!", m_CmdStr, m_StatusIDStr), Color.Red);
                            break;
                        case EReadOrSetStatus.Set:
                            //if (msg.Contains("ATE0"))
                            //{
                            //    //SetWarningInfo(string.Format("{0}{1}站点数据为ATE0!", m_CmdStr, m_StatusIDStr), Color.Red);
                            //}
                            //else 
                            if (msg.Contains("TRU"))
                            {
                                SetWarningInfo(string.Format("{0}{1}站点数据成功!", m_CmdStr, m_StatusIDStr), Color.Green);
                                m_RSStatus = EReadOrSetStatus.None;
                            }
                            break;
                        default: break;
                    }
                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            }
        }
        private void DownForUI_EventHandler(object sender, DownEventArgs e)
        {
            try
            {
                CDownConf info = e.Value;
                string rawData = e.RawData;
                if (info == null)
                    return;
                if (this.IsHandleCreated)
                {
                    #region 更新UI
                    if (info.Clock.HasValue)
                    {
                        try
                        {
                            this.vClock.Invoke((Action)delegate
                            {
                                this.vClock.Value = info.Clock.Value;
                                this.label19.Show();
                                //HighlightControl(this.vClock);
                                //this.vClock.BackColor = Color.Red;                             
                                //this.vClock.CalendarForeColor = Color.Red;
                                //this.vClock.CalendarMonthBackground = Color.Red;
                                //this.vClock.CalendarTitleBackColor = Color.Red;
                                //this.vClock.CalendarTrailingForeColor= Color.Red;
                                this.vClock.Invalidate();

                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.NormalState.HasValue)
                    {
                        try
                        {
                            this.vNormalState.Invoke((Action)delegate
                            {
                                this.vNormalState.Text = ProtocolMaps.NormalState4UI.FindValue(info.NormalState.Value);
                                HighlightControl(this.vNormalState);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.Voltage.HasValue)
                    {
                        try
                        {
                            this.vVoltage.Invoke((Action)delegate
                            {
                                this.vVoltage.Value = info.Voltage.Value;
                                HighlightControl(this.vVoltage);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.StationCmdID))
                    {
                        try
                        {
                            this.vStationCmdID.Invoke((Action)delegate
                            {
                                this.vStationCmdID.Text = info.StationCmdID;
                                HighlightControl(this.vStationCmdID);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.TimeChoice.HasValue)
                    {
                        try
                        {
                            this.vTimeChoice.Invoke((Action)delegate
                            {
                                this.vTimeChoice.Text = ProtocolMaps.TimeChoice4UI.FindValue(info.TimeChoice.Value);
                                HighlightControl(this.vTimeChoice);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.TimePeriod.HasValue)
                    {
                        try
                        {
                            this.vTimePeriod.Invoke((Action)delegate
                            {
                                this.vTimePeriod.Text = Int32.Parse(ProtocolMaps.TimePeriodMap.FindValue(info.TimePeriod.Value)).ToString();
                                HighlightControl(this.vTimePeriod);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.WorkStatus.HasValue)
                    {
                        try
                        {
                            this.vWorkStatus.Invoke((Action)delegate
                            {
                                this.vWorkStatus.Text = ProtocolMaps.WorkStatus4UI.FindValue(info.WorkStatus.Value);
                                HighlightControl(this.vWorkStatus);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.VersionNum))
                    {
                        try
                        {
                            this.vVersionNum.Invoke((Action)delegate
                            {
                                this.vVersionNum.Text = info.VersionNum;
                                HighlightControl(this.vVersionNum);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.MainChannel.HasValue && info.ViceChannel.HasValue)
                    {
                        try
                        {
                            this.vMainChannel.Invoke((Action)delegate
                            {
                                this.vMainChannel.Text = ProtocolMaps.ChannelType4UIMap.FindValue(info.MainChannel.Value);
                                HighlightControl(this.vMainChannel);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        try
                        {
                            this.vViceChannel.Invoke((Action)delegate
                            {
                                this.vViceChannel.Text = ProtocolMaps.ChannelType4UIMap.FindValue(info.ViceChannel.Value);
                                HighlightControl(this.vViceChannel);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.TeleNum))
                    {
                        try
                        {
                            this.vTeleNum.Invoke((Action)delegate
                            {
                                this.vTeleNum.Text = info.TeleNum;
                                HighlightControl(this.vTeleNum);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.RingsNum.HasValue)
                    {
                        try
                        {
                            this.vRingsNum.Invoke((Action)delegate
                            {
                                this.vRingsNum.Value = info.RingsNum.Value;
                                HighlightControl(this.vRingsNum);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.DestPhoneNum))
                    {
                        try
                        {
                            this.vDestPhoneNum.Invoke((Action)delegate
                            {
                                this.vDestPhoneNum.Text = info.DestPhoneNum;
                                HighlightControl(this.vDestPhoneNum);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.TerminalNum))
                    {
                        try
                        {
                            this.vTerminalNum.Invoke((Action)delegate
                            {
                                this.vTerminalNum.Text = info.TerminalNum;
                                HighlightControl(this.vTerminalNum);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.RespBeam))
                    {
                        try
                        {
                            this.vRespBeam.Invoke((Action)delegate
                            {
                                this.vRespBeam.Text = info.RespBeam;
                                HighlightControl(this.vRespBeam);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.AvegTime.HasValue)
                    {
                        try
                        {
                            this.vAvegTime.Invoke((Action)delegate
                            {
                                this.vAvegTime.Text = info.AvegTime.Value.ToString();
                                HighlightControl(this.vAvegTime);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.RainPlusReportedValue.HasValue)
                    {
                        try
                        {
                            this.vRainPlusReportedValue.Invoke((Action)delegate
                            {
                                if (m_CurrentStation == null)
                                {
                                    AddLog("未选择站点,或者该站点雨量精度不合法");
                                }
                                else
                                {
                                    decimal rainAccuracy = (decimal)m_CurrentStation.DRainAccuracy;
                                    this.vRainPlusReportedValue.Value = info.RainPlusReportedValue.Value * rainAccuracy;
                                    HighlightControl(this.vRainPlusReportedValue);
                                }
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (!String.IsNullOrEmpty(info.KC))
                    {
                        if (info.KC.Length != 20)
                        {
                            MessageBox.Show("KC值为" + info.KC);

                        }
                        else
                        {
                            try
                            {
                                this.vK.Invoke((Action)delegate
                                {
                                    this.vK.Text = info.KC.Substring(0, 10);
                                    HighlightControl(this.vK);
                                });
                            }
                            catch (Exception exp) { Debug.WriteLine(exp.Message); }
                            try
                            {
                                this.vC.Invoke((Action)delegate
                                {
                                    this.vC.Text = info.KC.Substring(10, 10);
                                    HighlightControl(this.vC);
                                });
                            }
                            catch (Exception exp) { Debug.WriteLine(exp.Message); }
                        }
                    }
                    if (info.Rain.HasValue)
                    {
                        try
                        {
                            this.vRain.Invoke((Action)delegate
                            {
                                if (m_CurrentStation == null || m_CurrentStation.DRainAccuracy.ToString() == "无")
                                {
                                    AddLog("未选择站点,或者该站点雨量精度不合法");
                                }
                                else
                                {
                                    decimal rainAccuracy = (decimal)m_CurrentStation.DRainAccuracy;
                                    this.vRain.Value = info.Rain.Value * rainAccuracy;
                                    HighlightControl(this.vRain);
                                }
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.storeWater.HasValue)
                    {
                        try
                        {
                            this.vWater.Invoke((Action)delegate
                            {
                                this.vWater.Value = info.storeWater.Value * (decimal)0.01;
                                HighlightControl(this.vWater);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.realWater.HasValue)
                    {
                        try
                        {
                            this.vWater.Invoke((Action)delegate
                            {
                                this.vWater.Value = info.realWater.Value * (decimal)0.01;
                                HighlightControl(this.vWater);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.WaterPlusReportedValue.HasValue)
                    {
                        try
                        {
                            this.vWaterPlusReportedValue.Invoke((Action)delegate
                            {
                                this.vWaterPlusReportedValue.Value = info.WaterPlusReportedValue.Value;
                                HighlightControl(this.vWaterPlusReportedValue);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.SelectCollectionParagraphs.HasValue)
                    {
                        try
                        {
                            this.vSelectCollectionParagraphs.Invoke((Action)delegate
                            {
                                this.vSelectCollectionParagraphs.Text = ProtocolMaps.SelectCollectionParagraphs4UIMap.FindValue(info.SelectCollectionParagraphs.Value);
                                HighlightControl(this.vSelectCollectionParagraphs);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.StationType.HasValue)
                    {
                        try
                        {
                            this.vStationType.Invoke((Action)delegate
                            {
                                this.vStationType.Text = ProtocolMaps.StationType4ProtoChineseMap.FindValue(info.StationType.Value);
                                HighlightControl(this.vStationType);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    if (info.SensorType.HasValue)
                    {
                        try
                        {
                            this.vSensorType.Invoke((Action)delegate
                            {
                                this.vSensorType.Text = ProtocolMaps.SensorType4UIMap.FindValue(info.SensorType.Value);
                                HighlightControl(this.vSensorType);
                            });
                        }
                        catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    }
                    //if (!String.IsNullOrEmpty(info.UserName))
                    //{
                    //    try
                    //    {
                    //        this.vUserName.Invoke((Action)delegate
                    //        {
                    //            this.vUserName.Text = info.UserName;
                    //            HighlightControl(this.vUserName);
                    //        });
                    //    }
                    //    catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    //}
                    //if (!String.IsNullOrEmpty(info.StationName))
                    //{
                    //    try
                    //    {
                    //        this.vStationName.Invoke((Action)delegate
                    //        {
                    //            this.vStationName.Text = info.StationName;
                    //            HighlightControl(this.vStationName);
                    //        });
                    //    }
                    //    catch (Exception exp) { Debug.WriteLine(exp.Message); }
                    //}
                    #endregion

                    AddLog("接收数据：" + rawData);
                    switch (m_RSStatus)
                    {
                        case EReadOrSetStatus.None:
                            break;
                        case EReadOrSetStatus.Read:
                            SetWarningInfo(string.Format("{0}{1}站点数据成功!", m_CmdStr, m_StatusIDStr), Color.Green);
                            m_RSStatus = EReadOrSetStatus.None;
                            break;
                        case EReadOrSetStatus.Set:
                            break;
                        default: break;
                    }
                }
            }
            catch (Exception exp) { Debug.WriteLine(exp.Message); }
        }

        #region ui logic

        private bool CheckLength(EDownParam param, string text)
        {
            int length = int.Parse(ProtocolMaps.DownParamLengthMap.FindValue(param));
            if (text.Length > length)
            {
                string type = ProtocolMaps.DownParam4ChineseMap.FindValue(param);
                MessageBox.Show(String.Format("{0}不能超过{1}个字符", type, length));
                return false;
            }
            return true;
        }
        /**********  Button   Event Handler **********/
        private void btn_Read_Click(object sender, EventArgs e)
        {
            try
            {
                Init();
                ResetAllControlsBackColor();
                /// 拼接查询字符串
                /// 00010G 03 05 08#13
                ///  站点id
                var station = (this.cmbStation as CStationComboBox).GetStation();
                m_CurrentStation = station;
                if (station == null)
                {
                    MessageBox.Show("请选择站点！");
                    return;
                }
                string sid = station.StationID;
                //  配置参数
                #region 配置参数
                var cmds = new List<EDownParam>();
                if (tabControl1.SelectedIndex == 0)
                {
                    if (this.chkClock.Checked)
                    {
                        cmds.Add(EDownParam.Clock);
                    }
                    if (this.chkVoltage.Checked)
                    {
                        cmds.Add(EDownParam.Voltage);
                    }
                    if (this.chkStationCmdID.Checked)
                    {
                        cmds.Add(EDownParam.StationCmdID);
                    }
                    if (this.chkNormalState.Checked)
                    {
                        cmds.Add(EDownParam.NormalState);
                    }
                    if (this.chkVersionNum.Checked)
                    {
                        cmds.Add(EDownParam.VersionNum);
                    }
                    if (this.chkWorkStatus.Checked)
                    {
                        cmds.Add(EDownParam.WorkStatus);
                    }
                    if (this.chkTimePeriod.Checked)
                    {
                        cmds.Add(EDownParam.TimePeriod);
                    }
                    if (this.chkTimeChoice.Checked)
                    {
                        cmds.Add(EDownParam.TimeChoice);
                    }
                }
                else if (tabControl1.SelectedIndex == 1)
                {

                    if (this.chkMainChannel.Checked)
                    {
                        cmds.Add(EDownParam.StandbyChannel);
                    }
                    if (this.chkDestPhoneNum.Checked)
                    {
                        cmds.Add(EDownParam.DestPhoneNum);
                    }
                    if (this.chkTeleNum.Checked)
                    {
                        cmds.Add(EDownParam.TeleNum);
                    }
                    if (this.chkTerminalNum.Checked)
                    {
                        cmds.Add(EDownParam.TerminalNum);
                    }
                    if (this.chkRespBeam.Checked)
                    {
                        cmds.Add(EDownParam.RespBeam);
                    }
                    if (this.chkRingsNum.Checked)
                    {
                        cmds.Add(EDownParam.RingsNum);
                    }
                }
                else if (tabControl1.SelectedIndex == 2)
                {

                    if (this.chkAvegTime.Checked)
                    {
                        cmds.Add(EDownParam.AvegTime);
                    }
                    if (this.chkStationType.Checked)
                    {
                        cmds.Add(EDownParam.StationType);
                    }
                    if (this.chkWater.Checked)
                    {
                        if (cmbWater.Text == "存储水位")
                        {
                            cmds.Add(EDownParam.storeWater);
                        }
                        else
                        {
                            cmds.Add(EDownParam.realWater);
                        }
                    }
                    if (this.chkWaterPlusReportedValue.Checked)
                    {
                        cmds.Add(EDownParam.WaterPlusReportedValue);
                    }
                    if (this.chkRain.Checked)
                    {
                        cmds.Add(EDownParam.Rain);
                    }
                    if (this.chkRainPlusReportedValue.Checked)
                    {
                        cmds.Add(EDownParam.RainPlusReportedValue);
                    }
                    if (this.chkK.Checked)
                    {
                        cmds.Add(EDownParam.KC);
                    }
                    if (this.chkSelectCollectionParagraphs.Checked)
                    {
                        cmds.Add(EDownParam.SelectCollectionParagraphs);
                    }
                    if (this.chkFlashClear.Checked)
                    {
                        cmds.Add(EDownParam.FlashClear);
                    }
                    if (this.chkSensor.Checked)
                    {
                        cmds.Add(EDownParam.SensorType);
                    }
                }
                //if (this.chkUserName.Checked)
                //{
                //    cmds.Add(EDownParam.UserName);
                //}
                //if (this.chkStationName.Checked)
                //{
                //    cmds.Add(EDownParam.StationName);
                //}
                if (cmds.Count == 0)
                {
                    MessageBox.Show("请选择参数!");
                    return;
                }
                #endregion
                string gprsNum = this.txtGprs.Text;
                if (String.IsNullOrEmpty(gprsNum))
                {
                    MessageBox.Show(this.panel_Station.Text + "号码不能为空!");
                    return;
                }
                if (this.m_channelType == EChannelType.GPRS)
                {
                    if (!HasUserOnLine())
                    {
                        return;
                    }
                }
                string query = null;
                if (gprsNum.Length == 8)
                {
                    query = CPortDataMgr.Instance.SendReadMsg(gprsNum, sid, cmds, this.m_channelType);
                }
                if (gprsNum.Length == 11)
                {
                    query = CPortDataMgr.Instance.SendReadHDMsg(gprsNum, sid, cmds, this.m_channelType);
                }
               
                m_RSStatus = EReadOrSetStatus.Read;
                //  日志记录
                string logMsg = String.Format("--------读取参数    目标站点（{0:D4}）--------- ", int.Parse(sid));
                m_CmdStr = "读取";
                m_StatusIDStr = sid;
                SetWarningInfo(string.Format("{0}{1}命令发出", m_CmdStr, m_StatusIDStr), Color.Blue);
                // 写入系统日志
                CSystemInfoMgr.Instance.AddInfo(logMsg);
                AddLog(logMsg);
                AddLog(String.Format("[{0}] Send: {1,-10}  {2}", m_channelType, "", query));
            }
            catch (Exception exp)
            {
                Debug.WriteLine("参数设置模块:读取数据失败！" + exp.Message);
            }
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            try
            {
                ResetAllControlsBackColor();
                //  站点id
                var station = (this.cmbStation as CStationComboBox).GetStation();
                m_CurrentStation = station;
                if (station == null)
                {
                    MessageBox.Show("请选择站点！");
                    return;
                }
                string sid = station.StationID;

                var cmds = new List<EDownParam>();
                CDownConf down = new CDownConf();
                //  配置参数
                #region 配置参数
                if (tabControl1.SelectedIndex == 0)
                {
                    if (this.chkClock.Checked)
                    {
                        cmds.Add(EDownParam.Clock);
                        down.Clock = this.chkLocalTime.Checked ? DateTime.Now : this.vClock.Value;
                    }
                    if (this.chkVoltage.Checked)
                    {
                        MessageBox.Show("电压不允许设置");
                        this.chkVoltage.Checked = false;
                        return;
                    }
                    if (this.chkStationCmdID.Checked)
                    {
                        MessageBox.Show("站号不允许设置");
                        this.chkStationCmdID.Checked = false;
                        return;
                    }
                    if (this.chkNormalState.Checked)
                    {
                        cmds.Add(EDownParam.NormalState);
                        var normalState = this.vNormalState.Text.Trim();
                        if (this.m_vNormalStateLists.Contains(normalState))
                        {
                            down.NormalState = ProtocolMaps.NormalState4UI.FindKey(normalState);
                        }
                        else
                        {
                            MessageBox.Show("常规状态 参数不是合法的!");
                            return;
                        }
                    }
                    if (this.chkVersionNum.Checked)
                    {
                        MessageBox.Show("版本号不允许设置");
                        this.chkVersionNum.Checked = false;
                        return;
                    }
                    if (this.chkFlashClear.Checked)
                    {
                        MessageBox.Show("清除Flash不允许设置");
                        this.chkFlashClear.Checked = false;
                        return;
                    }
                    if (this.chkWorkStatus.Checked)
                    {
                        cmds.Add(EDownParam.WorkStatus);
                        string workStatus = this.vWorkStatus.Text.Trim();
                        if (this.m_vWorkStatusLists.Contains(workStatus))
                        {
                            down.WorkStatus = ProtocolMaps.WorkStatus4UI.FindKey(workStatus);
                        }
                        else
                        {
                            MessageBox.Show("工作状态 参数不是合法的!");
                            return;
                        }
                    }
                    if (this.chkTimePeriod.Checked)
                    {
                        cmds.Add(EDownParam.TimePeriod);
                        string temp = String.Format("{0:D2}", Int16.Parse(this.vTimePeriod.Text));
                        if (this.m_vTimePeriodLists.Contains(Int16.Parse(this.vTimePeriod.Text.Trim())))
                        {
                            down.TimePeriod = ProtocolMaps.TimePeriodMap.FindKey(temp);
                        }
                        else
                        {
                            MessageBox.Show("定时段次 参数不是合法的!");
                            return;
                        }
                    }
                    if (this.chkTimeChoice.Checked)
                    {
                        cmds.Add(EDownParam.TimeChoice);
                        string temp = this.vTimeChoice.Text.Trim();
                        if (this.m_vTimeChoiceLists.Contains(temp))
                        {
                            down.TimeChoice = ProtocolMaps.TimeChoice4UI.FindKey(temp);
                        }
                        else
                        {
                            MessageBox.Show("对时选择 参数不是合法的!");
                            return;
                        }
                    }
                }
                else if (tabControl1.SelectedIndex == 1)
                {
                    if (this.chkMainChannel.Checked)
                    {
                        cmds.Add(EDownParam.StandbyChannel);

                        string mainChannel = this.vMainChannel.Text.Trim();
                        string viceChannel = this.vViceChannel.Text.Trim();
                        if (!this.m_vMainChannelLists.Contains(mainChannel))
                        {
                            MessageBox.Show("主用信道 参数不是合法的!");
                            return;
                        }
                        if (!this.m_vViceChannelLists.Contains(viceChannel))
                        {
                            MessageBox.Show("备用信道 参数不是合法的!");
                            return;
                        }
                        if (mainChannel == viceChannel)
                        {
                            MessageBox.Show("主用信道和备用信道不能同时设为" + vMainChannel.Text);
                            return;
                        }
                        down.MainChannel = ProtocolMaps.ChannelType4UIMap.FindKey(mainChannel);
                        down.ViceChannel = ProtocolMaps.ChannelType4UIMap.FindKey(viceChannel);
                    }
                    if (this.chkDestPhoneNum.Checked)
                    {
                        cmds.Add(EDownParam.DestPhoneNum);
                        string temp = this.vDestPhoneNum.Text.Trim();
                        if (!CStringUtil.IsDigitStrWithSpecifyLength(temp, 11))
                        {
                            MessageBox.Show("目的地手机号码 长度必须为11位，而且是数字!");
                            return;
                        }
                        down.DestPhoneNum = temp;
                    }
                    if (this.chkTeleNum.Checked)
                    {
                        cmds.Add(EDownParam.TeleNum);
                        string temp = this.vTeleNum.Text.Trim();
                        if (!CStringUtil.IsDigit(temp))
                        {
                            MessageBox.Show("SIM卡号 所有位必须全部为数字!");
                            return;
                        }
                        down.TeleNum = temp;
                    }
                    if (this.chkTerminalNum.Checked)
                    {
                        cmds.Add(EDownParam.TerminalNum);
                        string temp = this.vTerminalNum.Text.Trim();
                        if (!CStringUtil.IsDigit(temp))
                        {
                            MessageBox.Show("终端机号 所有位必须全部为数字!");
                            return;
                        }
                        down.TerminalNum = temp;
                    }
                    if (this.chkRespBeam.Checked)
                    {
                        cmds.Add(EDownParam.RespBeam);
                        string temp = this.vRespBeam.Text.Trim();
                        if (!CStringUtil.IsDigit(temp))
                        {
                            MessageBox.Show("响应波束 所有位必须全部为数字!");
                            return;
                        }
                        down.RespBeam = temp;
                    }
                    if (this.chkRingsNum.Checked)
                    {
                        cmds.Add(EDownParam.RingsNum);
                        decimal temp = this.vRingsNum.Value;
                        if (temp <= 0)
                        {
                            MessageBox.Show("振铃次数 必须大于0!");
                            return;
                        }
                        down.RingsNum = temp;
                    }
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    if (this.chkAvegTime.Checked)
                    {
                        cmds.Add(EDownParam.AvegTime);
                        down.AvegTime = Decimal.Parse(this.vAvegTime.Text);
                    }
                    if (this.chkStationType.Checked)
                    {
                        // MessageBox.Show("测站类型不允许设置");
                        cmds.Add(EDownParam.StationType);
                        if (this.vStationType.Text == "雨量站")
                            down.StationType = EStationTypeProto.ERainFall;
                        if (this.vStationType.Text == "并行水位站")
                            down.StationType = EStationTypeProto.EParallelRiverWater;
                        if (this.vStationType.Text == "并行水文站")
                            down.StationType = EStationTypeProto.EParallelEHydrology;
                        if (this.vStationType.Text == "串行水位站")
                            down.StationType = EStationTypeProto.ESerialRiverWater;
                        if (this.vStationType.Text == "串行水文站")
                            down.StationType = EStationTypeProto.ESerialEHydrology;
                        //this.chkStationType.Checked = false;
                        //return;

                    }
                    if (this.chkWater.Checked)
                    {
                        if(this.cmbWater.Text == "存储水位")
                        {
                            MessageBox.Show("存储水位不允许设置");
                            this.chkWater.Checked = false;
                            return;
                        }
                        cmds.Add(EDownParam.realWater);
                        down.realWater = this.vWater.Value * (decimal)100;
                    }
                    if (this.chkWaterPlusReportedValue.Checked)
                    {
                        cmds.Add(EDownParam.WaterPlusReportedValue);
                        down.WaterPlusReportedValue = Decimal.Parse(this.vWaterPlusReportedValue.Text);
                    }
                    if (this.chkRain.Checked)
                    {
                        cmds.Add(EDownParam.Rain);
                        decimal rainAcc = (decimal)m_CurrentStation.DRainAccuracy;
                        if (rainAcc == 0)
                        {
                            MessageBox.Show("站点的雨量精度不能为0!");
                            return;
                        }
                        down.Rain = this.vRain.Value / rainAcc;
                    }
                    if (this.chkRainPlusReportedValue.Checked)
                    {
                        cmds.Add(EDownParam.RainPlusReportedValue);
                        decimal rainAcc = (decimal)m_CurrentStation.DRainAccuracy;
                        if (rainAcc == 0)
                        {
                            MessageBox.Show("站点的雨量精度不能为0!");
                            return;
                        }
                        down.RainPlusReportedValue = this.vRainPlusReportedValue.Value / rainAcc;
                    }
                    if (this.chkK.Checked)
                    {
                        string k = this.vK.Text.Trim();
                        string c = this.vC.Text.Trim();
                        if (!CStringUtil.IsDigitStrWithSpecifyLength(k, 10))
                        {
                            MessageBox.Show("K值长度必须为10位数字!");
                            return;
                        }
                        if (!CStringUtil.IsDigitStrWithSpecifyLength(c, 10))
                        {
                            MessageBox.Show("C值长度必须为10位数字!");
                            return;
                        }
                        cmds.Add(EDownParam.KC);
                        down.KC = this.vK.Text + this.vC.Text;
                    }
                    if (this.chkSelectCollectionParagraphs.Checked)
                    {
                        string temp = this.vSelectCollectionParagraphs.Text;
                        if (!this.m_vSelectCollectionParagraphsLists.Contains(temp))
                        {
                            MessageBox.Show("采集段次 参数不是合法的!");
                            return;
                        }
                        cmds.Add(EDownParam.SelectCollectionParagraphs);
                        down.SelectCollectionParagraphs = ProtocolMaps.SelectCollectionParagraphs4UIMap.FindKey(temp);
                    }
                    if (this.chkSensor.Checked)
                    {
                        cmds.Add(EDownParam.SensorType);
                        string temp = this.vSensorType.Text;
                        if (this.m_vSensorLists.Contains(temp))
                        {
                            down.SensorType = ProtocolMaps.SensorType4UIMap.FindKey(temp);
                        }
                        else
                        {
                            MessageBox.Show("传感器配置 参数不是合法的!");
                            return;
                        }
                    }
                }
                //if (this.chkUserName.Checked)
                //{
                //    string temp = this.vUserName.Text;
                //    if (!CStringUtil.IsDigitOrAlpha(temp))
                //    {
                //        MessageBox.Show("用户名只能为字母或者数字！");
                //        return;
                //    }
                //    cmds.Add(EDownParam.UserName);
                //    down.UserName = temp;
                //}
                //if (this.chkStationName.Checked)
                //{
                //    string temp = this.vStationName.Text;
                //    if (!CStringUtil.IsDigitOrAlpha(temp))
                //    {
                //        MessageBox.Show("测站名只能为字母或者数字！");
                //        return;
                //    }
                //    cmds.Add(EDownParam.StationName);
                //    down.StationName = temp;
                //}
                if (cmds.Count == 0)
                {
                    MessageBox.Show("请选择参数!");
                    return;
                }
                #endregion
                string gprsNum = this.txtGprs.Text;
                if (String.IsNullOrEmpty(gprsNum))
                {
                    MessageBox.Show(this.panel_Station.Text + "号码不能为空!");
                    return;
                }
                if (this.m_channelType == EChannelType.GPRS)
                {
                    if (!HasUserOnLine())
                    {
                        return;
                    }
                }
                string query = null;
                if (gprsNum.Length == 8)
                {
                    query = CPortDataMgr.Instance.SendSetMsg(gprsNum, sid, cmds, down, this.m_channelType);
                }
                if(gprsNum.Length == 11)
                {
                    query = CPortDataMgr.Instance.SendSetHDMsg(gprsNum, sid, cmds, down, this.m_channelType);
                }
                

                m_RSStatus = EReadOrSetStatus.Set;
                m_CmdStr = "设置";
                m_StatusIDStr = sid;
                SetWarningInfo(string.Format("{0}{1}命令发出", m_CmdStr, m_StatusIDStr), Color.Blue);
                // 写入系统日志
                string logMsg = String.Format("--------设置参数    目标站点（{0:D4}）--------- ", int.Parse(sid));
                CSystemInfoMgr.Instance.AddInfo(logMsg);
                this.listView1.Items.Add(logMsg);
                AddLog(String.Format("[{0}] Send: {1,-10}  {2}", m_channelType, "", query));
                m_RSStatus = EReadOrSetStatus.Set;

            }
            catch (Exception exp)
            {
                Debug.WriteLine("参数设置模块:设置参数失败！" + exp.Message);
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private bool HasUserOnLine()
        {
            if (this.panel_Station.Text == "GPRS" && CPortDataMgr.Instance.GPRSCurrentOnlineUserCount == 0)
            {
                MessageBox.Show("GPRS ： 当前用户登录个数为0，请耐心等待用户登录！");
                return false;
            }
            return true;
        }

        /**********  CheckBox Event Handler **********/
        private void AllSelect_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;
            bool isChecked = chk.Checked;

            foreach (var control in this.WorkGroupBox.Controls)
            {
                var chkbox = control as CheckBox;
                if (chkbox == null || chkbox.Tag == null)
                    continue;
                if (chkbox.Tag.ToString().Equals("key"))
                {
                    chkbox.Checked = isChecked;
                }
            }
            foreach (var control in this.CommGroupBox.Controls)
            {
                var chkbox = control as CheckBox;
                if (chkbox == null || chkbox.Tag == null)
                    continue;
                if (chkbox.Tag.ToString().Equals("key"))
                {
                    chkbox.Checked = isChecked;
                }
            }
            foreach (var control in this.SensorGroupBox.Controls)
            {
                var chkbox = control as CheckBox;
                if (chkbox == null || chkbox.Tag == null)
                    continue;
                if (chkbox.Tag.ToString().Equals("key"))
                {
                    chkbox.Checked = isChecked;
                }
            }
        }
        private void chkLocalTime_CheckedChanged(object sender, EventArgs e)
        {
            this.vClock.Enabled = !this.chkLocalTime.Checked;
        }

        /**********  ComboBox Event Handler **********/
        private void CReadAndSettingMgrForm_StationSelected(object sender, CEventSingleArgs<CEntityStation> e)
        {
            var station = e.Value;
            if (station == null)
                return;

            string txt = this.cmbMsgType.Text;
            if (txt == EChannelType.GPRS.ToString())
                this.txtGprs.Text = station.GPRS;
            else if (txt == EChannelType.GSM.ToString())
                this.txtGprs.Text = station.GSM;
            else if (txt == EChannelType.BeiDou.ToString())
                this.txtGprs.Text = station.BDSatellite;
            else if (txt == EChannelType.PSTN.ToString())
#pragma warning disable CS0642 // 空语句可能有错误
                ;
#pragma warning restore CS0642 // 空语句可能有错误
            //      this.txtGprs.Text = station.PSTV;
        }
        private void cmbMsgType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var station = (this.cmbStation as CStationComboBox).GetStation();
            if (station == null)
                return;
            string sid = station.StationID;

            var txt = this.cmbMsgType.Text;
            if (txt == EChannelType.GPRS.ToString())
            {
                this.lblGprs.Text = "GPRS号码:";
                m_channelType = EChannelType.GPRS;
                this.txtGprs.Text = station.GPRS;
            }
            else if (txt == EChannelType.GSM.ToString())
            {
                this.lblGprs.Text = "GSM号码:";
                m_channelType = EChannelType.GSM;
                this.txtGprs.Text = station.GSM;
            }
            else if (txt == EChannelType.BeiDou.ToString())
            {
                this.lblGprs.Text = "北斗卫星号码:";
                m_channelType = EChannelType.BeiDou;
                this.txtGprs.Text = station.BDSatellite;
            }
            else if (txt == EChannelType.PSTN.ToString())
            {
                this.lblGprs.Text = "PSTN号码:";
                this.m_channelType = EChannelType.PSTN;
                //   this.txtGprs.Text = station.PSTV;
            }
        }
        #endregion

        private void SetWarningInfo(string text, Color color)
        {
            if (!this.IsHandleCreated)
                return;
            this.lblWarning.Invoke((Action)delegate
            {
                try
                {
                    this.lblWarning.Text = text;
                    this.lblWarning.ForeColor = color;
                }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
                catch (Exception exp) { }
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过
            });
        }
        private void AddLog(string text)
        {
            if (!this.IsHandleCreated)
                return;
            this.listView1.Invoke((Action)delegate
            {
                try
                {
                    this.listView1.Items.Add(text);
                    int index = this.listView1.Items.Count - 1;
                    if (index >= 0)
                        this.listView1.Items[index].EnsureVisible();
                }
                catch (Exception exp) { Debug.WriteLine(exp.Message); }
            });
        }

        private void ResetAllControlsBackColor()
        {
            UnHighlightGroupBox(this.SensorGroupBox);
            UnHighlightGroupBox(this.CommGroupBox);
            UnHighlightGroupBox(this.WorkGroupBox);
            this.label19.Hide();
        }

        private void UnHighlightGroupBox(GroupBox grpBox)
        {
            foreach (var item in grpBox.Controls)
            {
                Control ctl = item as Control;
                if (item != null)
                {
                    UnHighlightControl(ctl);

                }
            }
        }

        private void UnHighlightControl(Control control)
        {
            var cmb = control as CheckBox;
            if (cmb == null)
            {
                control.BackColor = System.Drawing.SystemColors.Window;
            }
            else
            {
                control.BackColor = System.Drawing.SystemColors.Control;
            }
            control.ForeColor = System.Drawing.SystemColors.WindowText;

            var lbl = control as Label;
            if (lbl != null)
            {
                lbl.BackColor = System.Drawing.SystemColors.ButtonFace;
            }

            if (Object.ReferenceEquals(control, this.vVoltage))
                this.vVoltage.Enabled = false;
            if (Object.ReferenceEquals(control, this.vStationCmdID))
                this.vStationCmdID.Enabled = false;
            if (Object.ReferenceEquals(control, this.vVersionNum))
                this.vVersionNum.Enabled = false;
            //if (Object.ReferenceEquals(control, this.vStationType))
            //    this.vStationType.Enabled = false;
        }

        private void HighlightControl(Control control)
        {
            control.BackColor = Color.Green;
            control.ForeColor = Color.White;
            if (Object.ReferenceEquals(control, this.vVoltage))
                this.vVoltage.Enabled = true;
            if (Object.ReferenceEquals(control, this.vStationCmdID))
                this.vStationCmdID.Enabled = true;
            if (Object.ReferenceEquals(control, this.vVersionNum))
                this.vVersionNum.Enabled = true;
            if (Object.ReferenceEquals(control, this.vStationType))
                this.vStationType.Enabled = true;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = sender as ComboBox;
            UnHighlightControl(cmb);
        }

        internal enum EReadOrSetStatus
        {
            Read,
            Set,
            None
        }

        private void btn_Send_Click(object sender, EventArgs e)
        {
            string cmd = this.vCustomCmd.Text;
            if (String.IsNullOrEmpty(cmd))
            {
                MessageBox.Show("发送命令不能为空!");
                return;
            }
            cmd += "\r\n";
            string gprsNum = this.txtGprs.Text;
            if (String.IsNullOrEmpty(gprsNum))
            {
                MessageBox.Show(this.lblGprs.Text.Replace(":", "") + "号码不能为空!");
                return;
            }

            var station = (this.cmbStation as CStationComboBox).GetStation();
            m_CurrentStation = station;
            if (station == null)
            {
                MessageBox.Show("请选择站点！");
                return;
            }
            string sid = station.StationID;

            string logMsg = String.Format("发送命令 :  {0}", cmd);
            // 写入系统日志
            CSystemInfoMgr.Instance.AddInfo(logMsg);
            AddLog(logMsg);

            CPortDataMgr.Instance.SendMsg(gprsNum, sid, cmd, this.m_channelType);
        }
        private void btn_ClearAll_Click(object sender, EventArgs e)
        {

        }
    }
}
