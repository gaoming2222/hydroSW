using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Hydrology.Forms
{
    public partial class CMessageBox : Form
    {
        // 动态显示文本的定时器
        private System.Timers.Timer m_timer;
        private string m_messageInfo;
        private int m_iDotCount = 0;
        private Control m_controlPre;
        public CMessageBox()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1 * 1000; // 1s
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(EH_TimerElapsed);
            btn_Cancel.Enabled = false;
            btn_Cancel.Visible = false;
        }
        // 析构函数
        ~CMessageBox()
        {
            m_timer.Stop();
        }
        #region 属性
        // 消息文本
        public String MessageInfo
        {
            get { return m_messageInfo; }
            set { m_messageInfo = value; m_label.Text = m_messageInfo; }
        }
        public bool CancelEnable
        {
            get { return btn_Cancel.Enabled; }
            set { btn_Cancel.Enabled = value; }
        }
        #endregion 属性

        private void EH_TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 显示点点号
            m_iDotCount = (m_iDotCount + 1) % 7;
            string str = m_messageInfo;
            for (int i = 0; i < m_iDotCount; ++i)
            {
                str += ". ";
            }
            // 更新
            m_label.Invoke((Action)delegate { m_label.Text = str; });
        }


        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // 取消按钮按下，关闭对话框，并返回DialogCancel结果
            m_timer.Stop();
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }
        /// <summary>
        /// 覆盖基类的方法
        /// </summary>
        /// <returns></returns>
        public new void ShowDialog()
        {
            //return;
            m_timer.Start();
            m_iDotCount = 1;
            base.StartPosition = FormStartPosition.CenterScreen;
            Thread t = new Thread(() => { base.ShowDialog(); });
            t.Start();
        }
        /// <summary>
        /// 覆盖基类的方法
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public void ShowDialog(Control parent)
        {
            //return;
            m_timer.Start();
            m_iDotCount = 1;
            m_controlPre = parent;
            //base.Parent = parent;
            //1110
            base.StartPosition = FormStartPosition.Manual;
            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度
            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
            base.Top = (int)(yHeight * 0.3);
            base.Left = (int)(xWidth * 0.4); 

           // base.StartPosition = FormStartPosition.CenterParent;
            Thread t = new Thread(() => { base.ShowDialog(); });
            t.Start();
        }

        public void CloseDialog()
        {
            //return;
            if (this.IsHandleCreated)
            {
                this.Invoke((Action)delegate 
                {
                    m_timer.Stop();
                    this.Close();
                    //if (m_controlPre != null)
                    //{
                    //    m_controlPre.Invoke(new Action(() => { m_controlPre.Visible = true; }));
                    //}
                });
            }
            else
            {
                m_timer.Stop();
                this.Close();
                //if (m_controlPre != null)
                //{
                //    m_controlPre.Invoke(new Action(() => { m_controlPre.Visible = true; }));
                //}
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visible = false;
            base.OnClosing(e);
            //m_timer.Stop();
            if (m_controlPre != null)
            {
                Control control = m_controlPre;
                //while (control.Parent != null)
                //{
                //    control = control.Parent;
                //}
                //Task.Factory.StartNew(new Action(()=>
                //{
                //    control.Invoke(new Action(() => 
                //    {
                //        //if (!control.Visible)
                //        //{
                //        //    control.Show();
                //        //}
                //        //control.Focus();
                //    }));
                //}));
            }
        }

    }
}
