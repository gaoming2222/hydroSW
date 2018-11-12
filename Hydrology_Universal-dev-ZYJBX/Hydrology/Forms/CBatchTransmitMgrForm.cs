using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrology.Forms
{
    public partial class CBatchTransmitMgrForm : Form
    {
        public enum ETrainmitMode { EUPan, EFlash };
        public CBatchTransmitMgrForm(ETrainmitMode mode)
        {
            InitializeComponent();
            SetTransmiteMode(mode);
        }

        #region 帮助方法
        private void SetTransmiteMode(ETrainmitMode mode)
        {
            switch (mode)
            {
                case ETrainmitMode.EUPan:
                    {
                        lbl_EndTime.Hide();
                        dtp_EndTime.Hide();
                        this.Height = this.Height - 40;
                        this.Text = "批量传输 - U盘数据";
                        dtp_StartTime.Format = DateTimePickerFormat.Custom;
                        dtp_StartTime.CustomFormat = "yyyy-MM-dd HH";
                        // 默认都是小时传输
                        radioBtn_HourTransmit.Checked = true;
                    }break;
                case ETrainmitMode.EFlash:
                    {
                        this.Text = "批量传输 - Flash数据";
                        dtp_EndTime.Format = DateTimePickerFormat.Custom;
                        dtp_EndTime.CustomFormat = "yyyy-MM-dd HH";
                        dtp_StartTime.Format = DateTimePickerFormat.Custom;
                        dtp_StartTime.CustomFormat = "yyyy-MM-dd HH";
                        radioBtn_HourTransmit.Checked = true;
                    }break;
            }
        }
        #endregion

        private void radioBtn_HourTransmit_CheckedChanged(object sender, EventArgs e)
        {
            if (radioBtn_HourTransmit.Checked)
            {
                // 小时选择模式
                dtp_StartTime.CustomFormat = "yyyy-MM-dd HH";
                dtp_EndTime.CustomFormat = "yyyy-MM-dd HH";
            }
            else
            {
                // 整天选择模式
                dtp_StartTime.CustomFormat = "yyyy-MM-dd";
                dtp_EndTime.CustomFormat = "yyyy-MM-dd";
            }
        }
    }
}
