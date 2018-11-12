using System;
using System.Drawing;
using System.Windows.Forms;
using Hydrology.Entity;

namespace Hydrology.CControls
{
    class CDataGridTabPage : Panel, ITabPage
    {
        #region  ITABPAGE
        public event EventHandler TabClosed;
        private string m_strTitle;
        public string Title
        {
            get
            {
                return m_strTitle;
            }
            set
            {
                m_strTitle = value;
            }
        }
        public ETabType TabType
        {
            get;
            set;
        }
        private Boolean m_bClosable;
        public Boolean BTabRectClosable
        {
            get
            {
                return m_bClosable;
            }
            set
            {
                m_bClosable = value;
            }
        }
        private int m_iTabIndex;
        public int TabPageIndex
        {
            get { return m_iTabIndex; }
            set { m_iTabIndex = value; }
        }
        public void CloseTab()
        {
            if (TabClosed != null)
            {
                this.TabClosed(this, new EventArgs());
            }
        }
        #endregion ITABPAGE

        //构造函数
        public CDataGridTabPage()
        {
            this.SuspendLayout();
            this.Dock = DockStyle.Fill;
            this.ResumeLayout(false);
        }

        public void RemoveDataGrid()
        {
            if (m_dataGridView != null)
            {
                m_tableLayoutPanel.Controls.Remove(m_dataGridView);
            }
        }
        #region PROPERTY
        // 设置数据视图
        public CExDataGridView DataGrid
        {
            get
            {
                return m_dataGridView;
            }
            set { m_dataGridView = value; InitUI(); }
        }

        public void DisabledLeftLabel()
        {
            m_labelLeft.Visible = false;
        }

        #endregion PROPERTY

        //数据实体
        #region DATAMEMBER
        private CExDataGridView m_dataGridView; //数据主体

        private Panel m_panelBottom;    //下面的一部分
        private Label m_labelLeft;      //下面的文本框
        private Label m_labelPanelRight;    //右边的一部分
        private TableLayoutPanel m_tableLayoutPanel;    //表格布局
        #endregion DATAMEMBER

        #region PRIVATE_METHOD
        // 初始化界面布局
        private void InitUI()
        {
            // m_tableLayoutPanel
            m_tableLayoutPanel = new TableLayoutPanel();
            m_tableLayoutPanel.SuspendLayout();
            m_tableLayoutPanel.Dock = DockStyle.Fill;
            m_tableLayoutPanel.ColumnCount = 1;
            m_tableLayoutPanel.BackgroundImage = global::Hydrology.Properties.Resources.WhiteSpace;
            //m_tableLayoutPanel.BackColor = Color.Red;
            m_tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); //一列

            //m_dataGridView = new CExDataGridView();
            m_dataGridView.AllowUserToAddRows = false;
            m_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            //m_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //m_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            m_dataGridView.Dock = DockStyle.Fill;
            //m_dataGridView.AutoSize = true;
            //m_dataGridView.ReadOnly = true; //只读
            m_dataGridView.AllowUserToResizeRows = false;
            m_dataGridView.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            m_dataGridView.RowHeadersWidth = 50;
            m_dataGridView.ColumnHeadersHeight = 25;
            m_dataGridView.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);

            m_panelBottom = new Panel();
            m_panelBottom.BackColor = Color.White;
            m_panelBottom.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            m_panelBottom.Size = new System.Drawing.Size(200, 20);
            m_panelBottom.Dock = DockStyle.Bottom;
            //m_panelBottom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);


            m_labelLeft = new Label();
            m_labelLeft.Dock = DockStyle.Left;
            m_labelLeft.TextAlign = ContentAlignment.MiddleLeft;
            //m_textBox.ReadOnly = true;
            //m_textBox.BorderStyle = BorderStyle.FixedSingle;
            //m_textBox.Margin = new System.Windows.Forms.Padding(2);
            m_labelLeft.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            m_labelLeft.Text = "页码：0 / 0";
            //m_textBox.BackColor = Color.FromArgb(219,225,232); //浅灰色
            //m_textBox.TextAlign = VerticalAlign.
            //m_textBox.BackColor = Color.LightGray;
            //m_textBox.
            m_panelBottom.Controls.Add(m_labelLeft);

            m_labelPanelRight = new Label();
            m_labelPanelRight.Dock = DockStyle.Right;
            m_labelPanelRight.TextAlign = ContentAlignment.MiddleRight;
            //m_textBoxPanelRight.BorderStyle = BorderStyle.FixedSingle;
            m_labelPanelRight.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            m_labelPanelRight.Text = "共 0 条记录";
            //m_textBoxPanelRight.TextAlign = HorizontalAlignment.Right;
            m_panelBottom.Controls.Add(m_labelPanelRight);

            //加入到控件中
            //this.Controls.Add(m_dataGridView);
            //this.Controls.Add(m_panelBottom);
            // 加入到布局控件中
            m_tableLayoutPanel.Controls.Add(m_dataGridView, 0, 0);
            m_tableLayoutPanel.Controls.Add(m_panelBottom, 0, 1);
            m_tableLayoutPanel.RowCount = 2;
            m_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            m_tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));

            this.Controls.Add(m_tableLayoutPanel);

            m_tableLayoutPanel.ResumeLayout(false);
            m_tableLayoutPanel.PerformLayout();

            //m_dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.Fill);

            //绑定消息
            m_dataGridView.RowsAdded += new DataGridViewRowsAddedEventHandler(EHDataRowChange);
            m_dataGridView.PageNumberChanged += new EventHandler<CEventSingleArgs<int>>(EVPageNumberChanged);
        }
        #endregion PRIVATE_METHOD

        #region EVENT_HANDLER

        // 相应表格行数变化的事件
        private void EHDataRowChange(object sender, DataGridViewRowsAddedEventArgs e)
        {
            m_labelLeft.Text = String.Format("页码：{0} / {1}", m_dataGridView.CurrentPageIndex, m_dataGridView.TotalPageCount);
            m_labelPanelRight.Text = String.Format("共 {0} 条记录", m_dataGridView.RowCount);
        }

        private void EVPageNumberChanged(object sender, CEventSingleArgs<int> e)
        {
            int separatorIndex = m_labelLeft.Text.IndexOf('/') + 1;
            int totalPage = Int32.Parse(m_labelLeft.Text.Substring(separatorIndex, m_labelLeft.Text.Length - separatorIndex));
            m_labelLeft.Text = String.Format("页码：{0} / {1}", e.Value, totalPage);
        }

        #endregion EVENT_HANDLER
    }
}
