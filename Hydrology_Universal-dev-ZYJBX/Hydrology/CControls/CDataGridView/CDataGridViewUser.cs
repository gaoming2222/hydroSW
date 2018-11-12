using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;

namespace Hydrology.CControls
{
    /// <summary>
    /// 用户管理表格控件
    /// </summary>
    class CDataGridViewUser : CExDataGridView
    {
        #region 事件定义

        public event EventHandler<CEventSingleArgs<int>> UserCountChanged;

        #endregion ///<事件定义

        #region 静态常量
        public static readonly string CS_UserID = "用户ID";
        public static readonly string CS_Delete = "删除";
        public static readonly string CS_UserName = "用户名";
        public static readonly string CS_PassWord = "密码";
        public static readonly string CS_Authority = "权限";
        public static readonly string CS_Administrator = "管理员";
        public static readonly string CS_NormalUser = "普通用户";
        #endregion  ///<静态常量

        #region 属性
        private int m_iUserCount = 0;
        public int UserCount
        {
            get { return m_iUserCount; }
        }
        #endregion ///<属性

        #region 成员变量
        private IUserProxy m_proxyUser;
        private List<CEntityUser> m_listUpdatedUser;    //更新用户的集合
        private List<int> m_listDeletedUser;            //删除用户的集合
        private List<CEntityUser> m_listAddedUser;      //添加用户的集合
        #endregion ///<成员变量

        #region 公共方法
        public CDataGridViewUser()
            : base()
        {
            InitializeComponent();
            // 默认编辑模式
            // SetEditMode(true);
            m_listUpdatedUser = new List<CEntityUser>();
            m_listDeletedUser = new List<int>();
            m_listAddedUser = new List<CEntityUser>();
        }
        /// <summary>
        /// 绑定与数据源的关联
        /// </summary>
        /// <param name="proxy"></param>
        public void InitDataSource(IUserProxy proxy)
        {
            m_proxyUser = proxy;
        }
        /// <summary>
        /// 将数据标记为删除，从集合中删除当前行
        /// </summary>
        public void DoDelete()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            //GetUpdatedData();
            // 获取数据
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                // -1表示新添加的用户
                int userId = Int32.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_UserID].Value.ToString());
                // 并且从编辑项中减去这一行
                m_listEditedRows.Remove(m_listMaskedDeletedRows[i]);
                if (userId != -1)
                {
                    m_listDeletedUser.Add(Int32.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_UserID].Value.ToString()));
                }
            }
            // 将某些行标记为不可见
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                //base.Rows[i].Visible = false;
                base.DeleteRowData(m_listMaskedDeletedRows[i]);
            }
            m_listMaskedDeletedRows.Clear(); //清空
            base.UpdateDataToUI();
            this.UpdateUserCount(base.m_dataTable.Rows.Count);

        }

        public override bool DoSave()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            if (!AssertSaveData())
            {
                return false;
            }

            GetUpdatedData();
            GetDeletedData();

            if (m_listAddedUser.Count > 0 || m_listUpdatedUser.Count > 0 || m_listDeletedUser.Count > 0)
            {
                // 应该作为一个事物一起处理
                //return base.DoSave();
                // 增加
                bool bResults = m_proxyUser.AddUserRange(m_listAddedUser);

                // 更新
                bResults = bResults && m_proxyUser.UpdateUserRange(m_listUpdatedUser);
                // 删除

                bResults = bResults && m_proxyUser.DeleteUserRange(m_listDeletedUser);

                if (bResults)
                {
                    this.Hide();
                    MessageBox.Show("保存成功");
                    this.Show();
                }
                else
                {
                    this.Hide();
                    MessageBox.Show("保存失败");
                    this.Show();
                }
                // 重新加载
                ClearAllState();
                LoadData();
                UpdateDataToUI();
            }
            else
            {
                this.Revert();
                this.Hide();
                MessageBox.Show("没有任何修改，无需保存");
                this.Show();
            }
            return true;
        }

        public bool LoadData()
        {
            if (m_proxyUser != null)
            {
                this.SetUser(m_proxyUser.QueryAllUser());
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭事件，是否取消关闭
        /// </summary>
        /// <returns>false的话，表示取消关闭</returns>
        public bool Close()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            // 关闭方法
            if (base.m_listEditedRows.Count > 0 || m_listDeletedUser.Count > 0 || base.m_listMaskedDeletedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("当前所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    return false;
                }
                else if (DialogResult.Yes == result)
                {
                    // 保存当前修改
                    return DoSave();
                }
                else if (DialogResult.No == result)
                {
                    return true;
                }
            }
            return true;
        }

        public void Revert()
        {
            this.Hide();
            // 清空所有状态
            this.ClearAllState();
            this.LoadData();
            this.Show();
        }

        /// <summary>
        /// 添加新的用户，新用户的ID默认都是-1
        /// </summary>
        public void AddNewUser()
        {
            base.AddRow(new string[]
                {
                    "False",
                    "",
                    "",
                    CS_NormalUser,
                    "-1"
                }, EDataState.ENormal);
            base.ClearSelection();
            //base.Update();
            //base.Rows[base.Rows.Count - 1].Selected = true;
            base.UpdateDataToUI();
            //base.Rows[base.Rows.Count - 1].Selected = true;
            // 清空选择
            this.UpdateUserCount(base.m_dataTable.Rows.Count);
            base.m_listEditedRows.Add(m_dataTable.Rows.Count - 1);
        }

        /// <summary>
        /// 是否开启编辑模式
        /// </summary>
        /// <param name="bEnable"></param>
        public void SetEditMode(bool bEnable)
        {
            if (bEnable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[]
                {
                    CS_Delete, CS_UserName, CS_PassWord, CS_Authority,CS_UserID
                };
                this.HideColomns = new int[]
                {
                    4 /*CS_UserID*/
                };

                //开启编辑模式,设置可编辑列
                DataGridViewCheckBoxColumn deleteCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, deleteCol);

                // 设置用户名编辑列
                DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(1, nameCol);

                // 设置用户名编辑列
                DataGridViewTextBoxColumn passCol = new DataGridViewTextBoxColumn();

                base.SetColumnEditStyle(2, passCol);

                // 设置权限编辑列
                DataGridViewComboBoxColumn authorityCol = new DataGridViewComboBoxColumn();
                authorityCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                authorityCol.Items.AddRange(new object[] { CS_Administrator, CS_NormalUser });
                base.SetColumnEditStyle(3, authorityCol);

                // 设置删除列的宽度
                this.Columns[0].Width = 15; //删除列宽度为20
            }
            else
            {

            }

        }

        //1111 gm
        //private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    // 编辑第4列时，把第4列显示为*号
        //    TextBox t = e.Control as TextBox;
        //    if (t != null)
        //    {
        //        if (this.CurrentCell.ColumnIndex == 3)
        //            t.PasswordChar = '*';
        //        else
        //            t.PasswordChar = new char();
        //    }
        //}
        #endregion 公共方法

        #region 重写
        // 重写Cell值改变事件
        protected override void EHValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
            {
                // 删除列
                if (base.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals("True"))
                {
                    // 删除项
                    base.MarkRowDeletedOrNot(e.RowIndex, true);
                }
                else
                {
                    base.MarkRowDeletedOrNot(e.RowIndex, false);
                }
            }
            else
            {
                // 非删除列
                base.EHValueChanged(sender, e);
            }
            base.UpdateDataToUI();
        }

        // 重写显示事件
        //protected override void ExCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        //{
        //    // 如果是密码行，则显示为*号
        //    if (m_arrayStrHeader[e.ColumnIndex].Equals(CS_PassWord))
        //    {
        //        if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "")
        //        {
        //            this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "******";
        //        }
        //    }
        //    base.ExCellFormatting(sender, e);
        //}



        // 重写双击事件
        protected override void OnCellMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellMouseDoubleClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellMouseDoubleClick(e);
            }
        }

        // 单击事件
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            if (base.m_listMaskedDeletedRows.Contains(e.RowIndex))
            {
                if (base.m_arrayStrHeader[e.ColumnIndex] == CS_Delete)
                {
                    //开启编辑
                    base.OnCellClick(e);
                }
                else
                {
                    //不编辑
                }
            }
            else
            {
                // 开启编辑
                base.OnCellClick(e);
            }
        }
        #endregion 重写

        #region  帮助方法

        private void SetUser(List<CEntityUser> listUser)
        {
            UpdateUserCount(listUser.Count);
            base.m_dataTable.Rows.Clear();
            // 将用户信息显示到表格上
            for (int i = 0; i < listUser.Count; ++i)
            {
                string authority = CS_NormalUser;
                if (listUser[i].Administrator)
                {
                    authority = CS_Administrator;
                }
                base.AddRow(new string[]
                {
                    "False",
                    listUser[i].UserName,
                    listUser[i].Password,
                    //"******",
                    authority,
                    listUser[i].UserID.ToString()
                }, EDataState.ENormal);
            }
            UpdateDataToUI();
        }

        /// <summary>
        /// 获取更新过的数据,包括增加的用户记录
        /// </summary>
        private void GetUpdatedData()
        {
            // 标记为删除的就不需要添加的修改或者添加的分中心中了
            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 将去重后的项赋给编辑项
            base.m_listEditedRows = listEditRows;
            for (int i = 0; i < base.m_listEditedRows.Count; ++i)
            {
                CEntityUser user = new CEntityUser();
                user.UserID = Int32.Parse(base.Rows[m_listEditedRows[i]].Cells[CS_UserID].Value.ToString());
                user.UserName = base.Rows[m_listEditedRows[i]].Cells[CS_UserName].Value.ToString();
                user.Password = base.Rows[m_listEditedRows[i]].Cells[CS_PassWord].Value.ToString();
                string administrator = base.Rows[m_listEditedRows[i]].Cells[CS_Authority].Value.ToString();
                if (administrator.Equals(CS_Administrator))
                {
                    user.Administrator = true;
                }
                else
                {
                    user.Administrator = false;
                }
                if (user.UserID == -1)
                {
                    // 添加的新用户
                    m_listAddedUser.Add(user);
                }
                else
                {
                    m_listUpdatedUser.Add(user);
                }

            }
            m_listEditedRows.Clear();   //清空此次记录
        }

        /// <summary>
        /// 获取删除过的数据
        /// </summary>
        private void GetDeletedData()
        {
            for (int i = 0; i < base.m_listMaskedDeletedRows.Count; ++i)
            {
                // -1是新增加的用户
                int userId = Int32.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_UserID].Value.ToString());
                if (userId != -1)
                {
                    m_listDeletedUser.Add(Int32.Parse(base.Rows[m_listMaskedDeletedRows[i]].Cells[CS_UserID].Value.ToString()));
                }
            }
            m_listMaskedDeletedRows.Clear();   //清空此次记录
        }

        private void UpdateUserCount(int userCount)
        {
            m_iUserCount = userCount;
            if (UserCountChanged != null)
            {
                UserCountChanged(this, new CEventSingleArgs<int>(userCount));
            }
        }

        // 清空所有数据，恢复到刚加载的状态
        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listAddedUser.Clear();
            m_listDeletedUser.Clear();
            m_listUpdatedUser.Clear();
        }

        // 判断用户数据是否合法，用户名和密码不能为空
        private bool AssertSaveData()
        {
            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 判断有没有不合法的数据
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                if (base.Rows[m_listEditedRows[i]].Cells[CS_UserName].Value.ToString().Equals(""))
                {
                    this.Hide();
                    MessageBox.Show("用户名不能为空");
                    this.Show();
                    return false;
                }
                if (base.Rows[m_listEditedRows[i]].Cells[CS_PassWord].Value.ToString().Equals(""))
                {
                    this.Hide();
                    MessageBox.Show("密码不能为空");
                    this.Show();
                    return false;
                }
            }
            // 用户名存在是否重复
            List<string> allUserName = new List<string>();
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                allUserName.Add(m_dataTable.Rows[i][CS_UserName].ToString());
            }

            // 判断分中心名字是否重复
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                string name = base.Rows[listEditRows[i]].Cells[CS_UserName].Value.ToString();
                if (allUserName.IndexOf(name) !=
                    allUserName.LastIndexOf(name))
                {
                    this.Hide();
                    MessageBox.Show(string.Format("用户名\"{0}\"不能重复", name));
                    this.Show();
                    return false;
                }
            }
            return true;
        }

        #endregion ///<HELP_METHOD

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this.m_dataTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataTableBak)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // CDataGridViewUser
            // 
            this.RowTemplate.Height = 23;
            this.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.CDataGridViewUser_CellFormatting);
            this.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.CDataGridViewUser_EditingControlShowing);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataTableBak)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void CDataGridViewUser_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 把第4列显示*号，*号的个数和实际数据的长度相同
            if (e.ColumnIndex == 2)
            {
                if (e.Value != null && e.Value.ToString().Length > 0)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }

        private void CDataGridViewUser_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // 编辑第4列时，把第4列显示为*号
            TextBox t = e.Control as TextBox;
            if (t != null)
            {
                if (this.CurrentCell.ColumnIndex == 2)
                    t.PasswordChar = '*';
                else
                    t.PasswordChar = new char();
            }
        }
    }
}
