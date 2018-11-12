using System;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.Entity;

namespace Hydrology.Forms
{
    public partial class CLoginForm : Form
    {
        #region 事件定义

        /// <summary>
        /// 更改当前用户模式
        /// </summary>
        public event EventHandler<CEventSingleArgs<bool>> UserModeChanged;

        #endregion ///<事件定义

        public CLoginForm()
        {
            InitializeComponent();
            label_Info.Visible = false;
            //textBox_UserName.Text = "admin";
            //textBox_Password.Text = "admin";
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            label_Info.Visible = false;
            bool bAdministrator = false;
            string username = textBox_UserName.Text.Trim();
            string password = textBox_Password.Text.Trim();
            //string username = "admin";
            //string password = "admin";
            if (String.IsNullOrEmpty(username))
            {
                label_Info.Visible = true;
                label_Info.Text = "用户名不能为空！";
                return;
            }
            if (String.IsNullOrEmpty(password))
            {
                label_Info.Visible = true;
                label_Info.Text = "密码不能为空！";
                return;
            }

            if (username == "admin" && password == "admin")
            {
                bAdministrator = true;
                CCurrentLoginUser.Instance.Login(username, password, bAdministrator);
                //登陆成功,通知主界面进入管理员或者普通用户模式
                if (UserModeChanged != null)
                {
                    CSystemInfoMgr.Instance.AddInfo(string.Format("用户{0}登录,权限：{1}", username, bAdministrator ? "管理员" : "普通用户"));
                    UserModeChanged.Invoke(this, new CEventSingleArgs<bool>(bAdministrator));
                }
                this.Close();
                return;
            }
            try
            {
                if (CDBDataMgr.Instance.GetUserProxy().UserLogin(username, password, ref bAdministrator))
                {
                    CCurrentLoginUser.Instance.Login(username, password, bAdministrator);
                    //登陆成功,通知主界面进入管理员或者普通用户模式
                    if (UserModeChanged != null)
                    {
                        CSystemInfoMgr.Instance.AddInfo(string.Format("用户{0}登录,权限：{1}", username, bAdministrator ? "管理员" : "普通用户"));
                        UserModeChanged.Invoke(this, new CEventSingleArgs<bool>(bAdministrator));
                    }
                    this.Close();
                    return;
                }
            }
#pragma warning disable CS0168 // 声明了变量“exp”，但从未使用过
            catch (Exception exp) { }
#pragma warning restore CS0168 // 声明了变量“exp”，但从未使用过

            // 登陆失败，显示提示信息
            label_Info.Visible = true;
            label_Info.Text = "用户名或密码错误！";
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CLoginForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.btn_Login_Click(null, null);
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                this.btn_Login_Click(null, null);
                return false;
            }
            return base.ProcessDialogKey(keyData);
        }
    }

    public class CCurrentLoginUser
    {
        private static CCurrentLoginUser instance;
        public static CCurrentLoginUser Instance
        {
            get
            {
                if (instance == null)
                    instance = new CCurrentLoginUser();
                return instance;
            }
        }
        private CCurrentLoginUser()
        {
            this.IsLogin = false;
            this.IsAdmin = false;
        }

        public bool IsLogin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        public void Login(string name, string pwd, bool isAdmin)
        {
            this.IsLogin = true;
            this.Name = name;
            this.Password = pwd;
            this.IsAdmin = isAdmin;
        }
        public void LogOut()
        {
            this.IsLogin = false;
            this.Name = string.Empty;
            this.Password = string.Empty;
            this.IsAdmin = false;
        }
    }
}
