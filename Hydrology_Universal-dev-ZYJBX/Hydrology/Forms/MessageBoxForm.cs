using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.DataMgr;

namespace Hydrology.Forms
{
    public partial class MessageBoxForm : Form
    {
        public MessageBoxForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
        }

        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            CVoicePlayer.Instance.Stop();
            CDBDataMgr.flag = 0;
            this.Close();
        }

        //禁用窗体的关闭按钮
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        //private void MessageBoxForm_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    CVoicePlayer.Instance.Stop();
        //    CDBDataMgr.flag = 0;
        //}
    }
}
