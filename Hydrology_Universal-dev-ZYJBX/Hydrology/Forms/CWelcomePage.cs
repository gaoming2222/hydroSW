using System.Windows.Forms;
using System;
using System.Threading;
using System.ComponentModel;

namespace Hydrology.Forms
{
    public partial class CWelcomePage : Form
    {
        // 动态显示文本的定时器
        private System.Timers.Timer m_timer;
        private string m_messageInfo;
        private int m_iDotCount = 0;
        private bool m_bInWelcomeMode = true; //是否是欢迎界面，默认是true
        public CWelcomePage()
        {
            InitializeComponent();
            m_timer = new System.Timers.Timer();
            m_timer.Interval = 1 * 1000; // 1s
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(EH_TimerElapsed);
            m_messageInfo = "正在加载,请稍候";
            m_label.Text = m_messageInfo;
        }
        public void SetClosedPicture()
        {
            panel1.SuspendLayout();
            panel1.BackgroundImage = global::Hydrology.Properties.Resources.退出界面;
            m_label.Visible = false;
            m_bInWelcomeMode = false;
            panel1.ResumeLayout(false);
        }
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
        public new void ShowDialog()
        {
            if (m_bInWelcomeMode)
            {
                // 启动定时器
                m_timer.Start();
            }
            Thread t = new Thread(() => { base.ShowDialog(); });
            t.Start();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            // 停止定时器
            m_timer.Stop();
            base.OnClosing(e);
        }
    }
}
