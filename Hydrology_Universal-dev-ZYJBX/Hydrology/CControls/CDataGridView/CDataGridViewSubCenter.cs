using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Entity;

namespace Hydrology.CControls.CDataGridView
{
    class CDataGridViewSubCenter : CExDataGridView
    {
        public event EventHandler<CEventSingleArgs<int>> SubCenterCountChanged;

        private static readonly string CS_SubCenterID = "分中心ID";
        private static readonly string CS_Delete = "删除";
        private static readonly string CS_SubCenterName = "分中心名称";
        private static readonly string CS_Comment = "备注";

        private int m_iPortCount = 0;
        public int PortCount
        {
            get { return m_iPortCount; }
        }
        private ISubCenterProxy m_proxy;
        private List<CEntitySubCenter> m_listUpdated; //  更新串口集合
        private List<int> m_listDeleted;               //  删除串口集合
        private List<CEntitySubCenter> m_listAdded;    //  添加串口集合

        public CDataGridViewSubCenter()
            : base()
        {
            this.m_listAdded = new List<CEntitySubCenter>();
            this.m_listDeleted = new List<int>();
            this.m_listUpdated = new List<CEntitySubCenter>();
        }

        public void InitDataSource(ISubCenterProxy proxy)
        {
            this.m_proxy = proxy;
        }

        public bool LoadData()
        {
            if (m_proxy != null)
            {
                this.Set(m_proxy.QueryAll());
                return true;
            }
            return false;
        }

        public void DoDelete()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            //GetUpdatedData();
            foreach (var item in base.m_listMaskedDeletedRows)
            {
                int subcenterId = Int32.Parse(base.Rows[item].Cells[CS_SubCenterID].Value.ToString());
                // 并且从编辑项中减去这一行
                m_listEditedRows.Remove(item);
                if (-1 != subcenterId)
                {
                    m_listDeleted.Add(subcenterId);
                }
                // -1是新增加的用户
            }
            for (int i = 0; i < m_listMaskedDeletedRows.Count; ++i)
            {
                base.DeleteRowData(m_listMaskedDeletedRows[i]);
            }
            m_listMaskedDeletedRows.Clear(); //清空
            base.UpdateDataToUI();
            this.UpdatePortsCount(base.m_dataTable.Rows.Count);
        }

        public override bool DoSave()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            // 判断数据是否合法
            if (!AssertSaveData())
            {
                return false;
            }
            GetUpdatedData();
            GetDeletedData();
            
            if (m_listAdded.Count > 0 || m_listDeleted.Count > 0 || m_listUpdated.Count > 0)
            {
                // 应该作为一个事物一起处理
                //return base.DoSave();
                // 增加
                bool result = m_proxy.AddRange(m_listAdded);
                //bool addResult = 0;

                //gm2017_02
                //if(m_listAdded.Count > 0)
                //{
                //    addResult = 
                
                //}
                // 更新
                result = result && m_proxy.UpdateRange(m_listUpdated);

                // 删除
                result = result && m_proxy.DeleteRange(m_listDeleted);

                if (result)
                {
                    MessageBox.Show("保存成功");
                }
                else
                {
                 //   MessageBox.Show("保存失败,请检查数据库连接或者分中心依赖关系");
                    MessageBox.Show("保存失败,该分中心下有所属测站，删除该分中心需改变分中心所属关系或删除该分中心所有测站");
                }
                // 通知其它界面重新加载
                CDBDataMgr.Instance.UpdateAllSubCenter();
                // 重新加载
                ClearAllState();
                LoadData();
                UpdateDataToUI();

                
            }
            else
            {
                this.Set(CDBDataMgr.Instance.GetAllSubCenter());
                UpdateDataToUI();
                MessageBox.Show("没有任何修改，无需保存");
            }
            return true;
        }

        public bool Close()
        {
            if (this.IsCurrentCellInEditMode)
            {
                //MessageBox.Show("请完成当前的编辑");
                this.EndEdit();
                //return false;
            }
            // 关闭方法
            if (base.m_listEditedRows.Count > 0 || m_listDeleted.Count > 0 || base.m_listMaskedDeletedRows.Count > 0)
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

        public void SetEditMode(bool bEnable)
        {
            if (bEnable)
            {
                // 设定标题栏,默认有个隐藏列
                this.Header = new string[] 
                { 
                    CS_Delete ,
                    CS_SubCenterName,
                    CS_Comment,  
                    CS_SubCenterID
                };
                this.HideColomns = new int[] { 3 };

                var delCol = new DataGridViewCheckBoxColumn();
                base.SetColumnEditStyle(0, delCol);

                var nameCol = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(1, nameCol);

                var commentCol = new DataGridViewTextBoxColumn();
                base.SetColumnEditStyle(2, commentCol);

                // 设置删除框的宽度
                base.Columns[0].Width = 30;
            }
        }

        public void AddNew()
        {
            base.AddRow(new string[] 
                {
                    "False",
                    "",
                    "",
                    "-1"
                }, EDataState.ENormal);
            base.m_listEditedRows.Add(m_dataTable.Rows.Count - 1);
            base.ClearSelection();
            base.UpdateDataToUI();
            this.UpdatePortsCount(base.m_dataTable.Rows.Count);
        }

        public void Revert()
        {
            // 撤销当前所做的所有更改
            this.ClearAllState();
            this.LoadData();
        }

        private void Set(IList<CEntitySubCenter> ports)
        {
            UpdatePortsCount(ports.Count);
            base.m_dataTable.Rows.Clear();

            foreach (var item in ports)
            {
                base.AddRow(new string[] 
                {
                    "False",
                    item.SubCenterName,
                    item.Comment,
                    item.SubCenterID.ToString()    
                }, EDataState.ENormal);
            }
            UpdateDataToUI();
        }

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
            foreach (var item in base.m_listEditedRows)
            {
                var port = new CEntitySubCenter();

                port.SubCenterID = base.RowCount;
                port.SubCenterName = base.Rows[item].Cells[CS_SubCenterName].Value.ToString();
                port.Comment = base.Rows[item].Cells[CS_Comment].Value.ToString();

                string tag = base.Rows[item].Cells[CS_SubCenterID].Value.ToString();
                if (tag == "-1")
                {
                    m_listAdded.Add(port);
                }
                else
                {
                    port.SubCenterID = Int32.Parse(base.Rows[item].Cells[CS_SubCenterID].Value.ToString());
                    m_listUpdated.Add(port);
                }
            }
            m_listEditedRows.Clear();
        }

        private void GetDeletedData()
        {
            foreach (var item in base.m_listMaskedDeletedRows)
            {
                int subcenterId = Int32.Parse(base.Rows[item].Cells[CS_SubCenterID].Value.ToString());
                if (-1 != subcenterId)
                {
                    m_listDeleted.Add(subcenterId);
                    // -1表示新的增加的分中心
                }
            }
            m_listMaskedDeletedRows.Clear();
        }

        private void UpdatePortsCount(int count)
        {
            m_iPortCount = count;
            if (SubCenterCountChanged != null)
            {
                SubCenterCountChanged(this, new CEventSingleArgs<int>(count));
            }
        }

        private bool AssertSaveData()
        {
            // 判断当前数据是否合法
            List<string> allSubCenterName = new List<string>();

            List<int> listEditRows = new List<int>();
            foreach (int item in base.m_listEditedRows)
            {
                if (!m_listMaskedDeletedRows.Contains(item))
                {
                    listEditRows.Add(item);
                }
            }
            // 判断分中心名字不能为空
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                if (base.Rows[listEditRows[i]].Cells[CS_SubCenterName].Value.ToString().Equals(""))
                {
                    MessageBox.Show("分中心名字不能为空");
                    return false;
                }
            }

            // 获取所有分中心的名字
            for (int i = 0; i < m_dataTable.Rows.Count; ++i)
            {
                allSubCenterName.Add(m_dataTable.Rows[i][CS_SubCenterName].ToString());
            }

            // 判断分中心名字是否重复
            for (int i = 0; i < listEditRows.Count; ++i)
            {
                string name = base.Rows[listEditRows[i]].Cells[CS_SubCenterName].Value.ToString();
                if (allSubCenterName.IndexOf(name) !=
                    allSubCenterName.LastIndexOf(name))
                {
                    MessageBox.Show(string.Format("分中心名字\"{0}\"不能重复", name));
                    return false;
                }
            }

            return true;
        }


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

        // 清空所有数据，恢复到刚加载的状态
        protected override void ClearAllState()
        {
            base.ClearAllState();
            m_listUpdated.Clear();
            m_listDeleted.Clear();
            m_listAdded.Clear();
        }

        #endregion ///<#region 重写
    }
}
