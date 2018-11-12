using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.CControls;
using Hydrology.Entity;

namespace Hydrology.Forms
{
    public partial class CStationDataAddForm : Form
    {
        #region 常量定义
        private static readonly string CS_AddData_Voltage = "电压";
        private static readonly string CS_AddData_Rain = "雨量";
        private static readonly string CS_AddData_Water = "水位";
        #endregion 常量定义

        #region 数据成员

        /// <summary>
        ///  当前站点
        /// </summary>
        private CEntityStation m_currentStation;

        private CEntityRain m_entityRain;
        private CEntityVoltage m_entityVoltage;
        private CEntityWater m_entityWater;
        #endregion 数据成员

        #region 公共方法
        /// <summary>
        /// 获取增加的雨量记录，如果没有，则为空
        /// </summary>
        /// <returns></returns>
        public CEntityRain GetAddedRain()
        {
            return m_entityRain;
        }

        public CEntityWater GetAddedWater()
        {
            return m_entityWater;
        }
        public CEntityVoltage GetAddedVoltage()
        {
            return m_entityVoltage;
        }
        public void SetCurrentStation(CEntityStation station)
        {
            (cmb_StationId as CStationComboBox).SetCurrentStation(station);
        }
        #endregion 公共方法

        public CStationDataAddForm()
        {
            InitializeComponent();
            InitUI();

            m_currentStation = null;
            m_entityRain = null;
            m_entityVoltage = null;
            m_entityWater = null;

            FormHelper.InitUserModeEvent(this);
        }
        #region 事件响应
        private void EHStationChanged(object sender, CEventSingleArgs<CEntityStation> e)
        {
            //如果为空，则不选择所有，如果不为空，更新站点类型复选框
            m_currentStation = e.Value;
            if (null == m_currentStation)
            {
                textBox_StationType.Text = "";
                cmb_AddDataType.Items.Clear(); //清空
                cmb_AddDataType.Text = "";
                chk_Water.Enabled = false;
                chk_Voltage.Enabled = false;
                chk_Rain.Enabled = false;
            }
            else
            {
                cmb_AddDataType.Items.Clear();
                cmb_AddDataType.Text = "";
                textBox_StationType.Text = CEnumHelper.StationTypeToUIStr(m_currentStation.StationType);
                // 根据站点类型，输入可选的数据类型
                switch (m_currentStation.StationType)
                {
                    case EStationType.EHydrology:
                        {
                            chk_Rain.Enabled = true;
                            chk_Voltage.Enabled = true;
                            chk_Water.Enabled = true;
                            // 水位站
                            //cmb_AddDataType.Items.Add(CS_AddData_Rain);
                            //cmb_AddDataType.Items.Add(CS_AddData_Water);
                            //cmb_AddDataType.Items.Add(CS_AddData_Voltage);

                            //cmb_AddDataType.Items.Add(CS_AddData_Rain + " " + CS_AddData_Water);
                            //cmb_AddDataType.Items.Add(CS_AddData_Rain + " " + CS_AddData_Voltage);
                            //cmb_AddDataType.Items.Add(CS_AddData_Water + " " + CS_AddData_Voltage);

                            //cmb_AddDataType.Items.Add(CS_AddData_Rain + " " + CS_AddData_Water + " " + CS_AddData_Voltage);
                        }
                        break;
                    case EStationType.ERainFall:
                        {
                            //雨量站
                            chk_Rain.Enabled = true;
                            chk_Voltage.Enabled = true;
                            //cmb_AddDataType.Items.Add(CS_AddData_Rain);
                            //cmb_AddDataType.Items.Add(CS_AddData_Voltage);
                            //cmb_AddDataType.Items.Add(CS_AddData_Rain + " " + CS_AddData_Voltage);

                        }
                        break;
                    case EStationType.ERiverWater:
                        {
                            //水位站
                            chk_Voltage.Enabled = true;
                            chk_Water.Enabled = true;
                            //cmb_AddDataType.Items.Add(CS_AddData_Water);
                            //cmb_AddDataType.Items.Add(CS_AddData_Voltage);
                            //cmb_AddDataType.Items.Add(CS_AddData_Water + " " + CS_AddData_Voltage);
                        }
                        break;
                }
            }
        }

        private void cmb_AddDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 选择添加的数据类型发生了改变
            string text = cmb_AddDataType.Text;
            if (text.Contains(CS_AddData_Rain))
            {
                // 开启雨量编辑
                number_PeriodRain.Enabled = true;
                number_DayRain.Enabled = true;
                number_TotalRain.Enabled = true;
            }
            else
            {
                number_PeriodRain.Enabled = false;
                number_DayRain.Enabled = false;
                number_TotalRain.Enabled = false;
            }

            if (text.Contains(CS_AddData_Voltage))
            {
                number_Voltage.Enabled = true; //电压
            }
            else
            {
                number_Voltage.Enabled = false; //电压
            }

            if (text.Contains(CS_AddData_Water))
            {
                number_WaterStage.Enabled = true;
                number_WaterFlow.Enabled = true;
            }
            else
            {
                number_WaterStage.Enabled = false;
                number_WaterFlow.Enabled = false;
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // 取消添加
            this.Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            // 完成添加
            if (AssertDataValid())
            {
                // 只有数据合法才能关闭窗口
                GenerateAdddedDate();
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void EHVoltageChecked(object sender, EventArgs e)
        {
            if (chk_Voltage.CheckState == CheckState.Checked)
            {
                // 如果选中
                number_Voltage.Enabled = true;
            }
            else
            {
                // 没有选中
                number_Voltage.Enabled = false;
            }
        }

        private void EHWaterChecked(object sender, EventArgs e)
        {
            if (chk_Water.CheckState == CheckState.Checked)
            {
                // 如果选中
                number_WaterStage.Enabled = true;
                number_WaterFlow.Enabled = true;
            }
            else
            {
                // 没有选中
                number_WaterStage.Enabled = false;
                number_WaterFlow.Enabled = false;
            }
        }

        private void EHRainChecked(object sender, EventArgs e)
        {
            if (chk_Rain.CheckState == CheckState.Checked)
            {
                // 开启雨量编辑
                //number_PeriodRain.Enabled = true;
                //number_DayRain.Enabled = true;
                number_TotalRain.Enabled = true;
            }
            else
            {
                number_PeriodRain.Enabled = false;
                number_DayRain.Enabled = false;
                number_TotalRain.Enabled = false;
            }

        }

        #endregion 事件响应

        #region 帮助方法
        private void InitUI()
        {
            this.SuspendLayout();
            // 
            // cmb_StationType
            // 
            this.groupBox1.Controls.Remove(this.cmb_StationId);
            this.cmb_StationId = new CStationComboBox();
            this.cmb_StationId.FormattingEnabled = true;
            this.cmb_StationId.Location = new System.Drawing.Point(88, 20);
            this.cmb_StationId.Name = "cmb_StationId";
            this.cmb_StationId.Size = new System.Drawing.Size(148, 20);
            this.cmb_StationId.TabIndex = 1;
            (this.cmb_StationId as CStationComboBox).StationSelected += new EventHandler<CEventSingleArgs<CEntityStation>>(EHStationChanged);
            this.groupBox1.Controls.Add(this.cmb_StationId);

            // 接受时间和采集时间
            dtp_CollectTime.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            dtp_TimeReceived.Value = DateTime.Now;

            // 数据协议
            cmb_DataType.Items.Add(CEnumHelper.MessageTypeToUIStr(EMessageType.ETimed));
            cmb_DataType.Items.Add(CEnumHelper.MessageTypeToUIStr(EMessageType.EAdditional));

            // 信道协议
            //cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.None));
            //cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.GPRS));
            //cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.GSM));
            //cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.BeiDou));
            //cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.PSTN));
            cmb_ChannelType.Items.Add(CEnumHelper.ChannelTypeToUIStr(EChannelType.None));
            cmb_ChannelType.SelectedIndex = 0;
            cmb_ChannelType.Enabled = false; //只能为无

            // 数值都是不可编辑的
            number_Voltage.Enabled = false; //电压

            number_PeriodRain.Enabled = false;
            number_DayRain.Enabled = false;
            number_TotalRain.Enabled = false;

            number_WaterStage.Enabled = false;
            number_WaterFlow.Enabled = false;

            cmb_AddDataType.Visible = false;    // 不可见，已经废弃

            // 绑定消息
            chk_Rain.CheckedChanged += new EventHandler(EHRainChecked);
            chk_Water.CheckedChanged += new EventHandler(EHWaterChecked);
            chk_Voltage.CheckedChanged += new EventHandler(EHVoltageChecked);

            // 初始化焦点切换
            FormHelper.InitControlFocusLoop(this);

            this.ResumeLayout(false);
        }



        private bool AssertDataValid()
        {
            // 判断数据是否合法
            // 站点是否为空
            if (null == m_currentStation)
            {
                MessageBox.Show("站点不能为空");
                return false;
            }
            // 数据类型不能为空
            if (cmb_DataType.Text.Equals(""))
            {
                MessageBox.Show("请选择正确的报文类型");
                return false;
            }
            // 数据类型不能为空
            if (chk_Rain.CheckState != CheckState.Checked && chk_Voltage.CheckState != CheckState.Checked && chk_Water.CheckState != CheckState.Checked)
            {
                MessageBox.Show("请选择要添加的数据");
                return false;
            }
            // 协议类型不能为空
            //if (cmb_ChannelType.Text.Equals(""))
            //{
            //    MessageBox.Show("信道类型不能为空");
            //    return false;
            //}
            // 添加的数据类型不能为空
            //if (cmb_AddDataType.Text.Equals(""))
            //{
            //    MessageBox.Show("请选择要添加的数据");
            //    return false;
            //}
            return true;
        }

        /// <summary>
        /// 根据界面的数据，生成添加的实体类
        /// </summary>
        private void GenerateAdddedDate()
        {
            if (chk_Rain.CheckState == CheckState.Checked)
            {
                // 新建雨量记录
                m_entityRain = new CEntityRain();
                m_entityRain.StationID = m_currentStation.StationID;
                m_entityRain.TimeCollect = dtp_CollectTime.Value;
                m_entityRain.TimeRecieved = dtp_TimeReceived.Value;
                m_entityRain.MessageType = CEnumHelper.UIStrToMesssageType(cmb_DataType.Text);
                m_entityRain.ChannelType = CEnumHelper.UIStrToChannelType(cmb_ChannelType.Text);
                //m_entityRain.PeriodRain = number_PeriodRain.Value;
                //m_entityRain.DayRain = number_DayRain.Value;
                m_entityRain.TotalRain = number_TotalRain.Value;
                m_entityRain.BState = 1;//默认是正常的
            }
            if (chk_Water.CheckState == CheckState.Checked)
            {
                // 新建水位记录
                m_entityWater = new CEntityWater();
                m_entityWater.StationID = m_currentStation.StationID;
                m_entityWater.TimeCollect = dtp_CollectTime.Value;
                m_entityWater.TimeRecieved = dtp_TimeReceived.Value;
                m_entityWater.MessageType = CEnumHelper.UIStrToMesssageType(cmb_DataType.Text);
                m_entityWater.ChannelType = CEnumHelper.UIStrToChannelType(cmb_ChannelType.Text);
                m_entityWater.WaterStage = number_WaterStage.Value;
                m_entityWater.WaterFlow = number_WaterFlow.Value;
                m_entityWater.state = 1;
            }
            if (chk_Voltage.CheckState == CheckState.Checked)
            {
                // 新建电压记录
                m_entityVoltage = new CEntityVoltage();
                m_entityVoltage.StationID = m_currentStation.StationID;
                m_entityVoltage.TimeCollect = dtp_CollectTime.Value;
                m_entityVoltage.TimeRecieved = dtp_TimeReceived.Value;
                m_entityVoltage.MessageType = CEnumHelper.UIStrToMesssageType(cmb_DataType.Text);
                m_entityVoltage.ChannelType = CEnumHelper.UIStrToChannelType(cmb_ChannelType.Text);
                m_entityVoltage.Voltage = number_Voltage.Value;
                m_entityVoltage.state = 1;
            }
        }
        #endregion 帮助方法





    }
}
