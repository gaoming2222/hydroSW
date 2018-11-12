using System;
using System.Drawing;
using System.Windows.Forms;
using Hydrology.Entity;

namespace Hydrology.CControls
{
    /// <summary>
    /// 默认实现文字内容的信息添加，如果自己设定了ListView，那么就应该实现自己信息更新和添加方法
    /// </summary>
    class CListFormTabPage : Form, ITabPage, IDisposable
    {
        #region  ITABPAGE
        private string m_strTitle;
        private Panel panel1;
        private PictureBox m_pitureBoxClose;
        private CExListView m_listView;
        private Label m_labelTitle;

        // 页面关闭事件
        public event EventHandler TabClosed;
        public string Title
        {
            get
            {
                return m_strTitle;
            }
            set
            {
                m_strTitle = value;
                m_labelTitle.Text = value;  //设置标题的值
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

        #region PROPERTY
        private bool m_bCloseButton;
        public bool BCloseButton
        {
            get { return m_bCloseButton; }
            set
            {
                m_bCloseButton = value;
                m_pitureBoxClose.Hide(); //隐藏关闭按钮
            }
        }

        public CExListView ListView
        {
            get { return m_listView; }
            set 
            {
                this.SuspendLayout();
                this.Controls.Remove(m_listView);
                m_listView = value;
                //InitializeComponent();
                this.m_listView.BackColor = System.Drawing.SystemColors.Info;
                this.m_listView.BHorizentalScroolVisible = true;
                this.m_listView.BVertialScrollVisible = true;
                this.m_listView.Dock = System.Windows.Forms.DockStyle.Bottom;
                this.m_listView.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.m_listView.FullRowSelect = true;
                this.m_listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
                this.m_listView.Location = new System.Drawing.Point(0, 164);
                this.m_listView.Name = "m_listView";
                this.m_listView.Size = new System.Drawing.Size(284, 97);
                this.m_listView.TabIndex = 2;
                this.m_listView.UseCompatibleStateImageBehavior = false;
                //this.m_listView.View = System.Windows.Forms.View.Details;

                m_listView.Columns.Add("表头", -2, HorizontalAlignment.Left);

                // 设置ListView的行高
                ImageList imgList = new ImageList();
                imgList.ImageSize = new Size(1, 20);//设置 ImageList 的宽和高
                m_listView.SmallImageList = imgList;

                this.Controls.Add(m_listView);

                this.ResumeLayout(false);
                
            }
        }
        #endregion PROPERTY



        public CListFormTabPage()
            : base()
        {
           
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Padding = new System.Windows.Forms.Padding(0);
            //this.ControlBox = false; //去掉最大化，最小化和关闭按钮
            //this.Dock = DockStyle.Fill;
            //初始化提示信息
            m_toolTip = new ToolTip();
            m_toolTip.AutoPopDelay = 0;
            m_toolTip.UseAnimation = true;
            m_toolTip.ShowAlways = true;

            // m_listView
            m_listView.Columns.Add("表头", -2, HorizontalAlignment.Left);

            // 设置ListView的行高
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 20);//设置 ImageList 的宽和高
            m_listView.SmallImageList = imgList;
            // m_listView.edi
            // m_listView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.None);

        }


        // 添加显示内容
        public void AddText(string str, ETextMsgState state)
        {
            try
            {
                //if (!this.IsHandleCreated)
                //{
                //    return;
                //}
                ListViewItem item = new ListViewItem() { Text = str };
                Color forcolor = Color.Black;
                switch (state)
                {
                    case ETextMsgState.ENormal: { item.ImageIndex = 0; } break;
                    case ETextMsgState.EWarning: { item.ForeColor = Color.Yellow; item.ImageIndex = 1; } break;
                    case ETextMsgState.EError: { item.ForeColor = Color.Red; item.ImageIndex = 1; } break;
                }
                m_listView.AddItem(item);
            }
            catch (System.Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
            }
            
        }

        // 设置显示样式
        private void SetMsgViewMode(bool bImageOn)
        {
            if (bImageOn)
            {
                //图片显示
                ImageList imgList = new ImageList();
                imgList.ImageSize = new Size(20, 20);
                imgList.Images.Add(Hydrology.Properties.Resources.COM_NORMAL);
                imgList.Images.Add(Hydrology.Properties.Resources.COM_ERROR);
                m_listView.LargeImageList = imgList;
                m_listView.View = View.LargeIcon;
            }
        }

        #region PRIVATE_REGION
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CListFormTabPage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_labelTitle = new System.Windows.Forms.Label();
            this.m_pitureBoxClose = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pitureBoxClose)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Silver;
            this.panel1.BackgroundImage = global::Hydrology.Properties.Resources.标签头部;
            this.panel1.Controls.Add(this.m_labelTitle);
            this.panel1.Controls.Add(this.m_pitureBoxClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.panel1.Size = new System.Drawing.Size(284, 20);
            this.panel1.TabIndex = 1;
            // 
            // m_labelTitle
            // 
            this.m_labelTitle.AutoSize = true;
            this.m_labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.m_labelTitle.ForeColor = System.Drawing.Color.White;
            this.m_labelTitle.Location = new System.Drawing.Point(3, 4);
            this.m_labelTitle.Name = "m_labelTitle";
            this.m_labelTitle.Size = new System.Drawing.Size(77, 12);
            this.m_labelTitle.TabIndex = 1;
            this.m_labelTitle.Text = "m_labelTitle";
            // 
            // m_pitureBoxClose
            // 
            this.m_pitureBoxClose.BackColor = System.Drawing.Color.Transparent;
            this.m_pitureBoxClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.m_pitureBoxClose.Image = ((System.Drawing.Image)(resources.GetObject("m_pitureBoxClose.Image")));
            this.m_pitureBoxClose.Location = new System.Drawing.Point(266, 2);
            this.m_pitureBoxClose.Name = "m_pitureBoxClose";
            this.m_pitureBoxClose.Size = new System.Drawing.Size(16, 16);
            this.m_pitureBoxClose.TabIndex = 0;
            this.m_pitureBoxClose.TabStop = false;
            this.m_pitureBoxClose.Click += new System.EventHandler(this.m_pitureBoxClose_Click);
            this.m_pitureBoxClose.MouseLeave += new System.EventHandler(this.m_pitureBoxClose_MouseLeave);
            this.m_pitureBoxClose.MouseHover += new System.EventHandler(this.m_pitureBoxClose_MouseHover);
            // 
            // m_listView
            // 
            this.m_listView = new Hydrology.CControls.CExListView();
            this.m_listView.BackColor = System.Drawing.SystemColors.Info;
            this.m_listView.BHorizentalScroolVisible = true;
            this.m_listView.BVertialScrollVisible = true;
            this.m_listView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_listView.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_listView.FullRowSelect = true;
            this.m_listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.m_listView.Location = new System.Drawing.Point(0, 164);
            this.m_listView.Name = "m_listView";
            this.m_listView.Size = new System.Drawing.Size(284, 97);
            this.m_listView.TabIndex = 2;
            this.m_listView.UseCompatibleStateImageBehavior = false;
            this.m_listView.View = System.Windows.Forms.View.Details;
            // 
            // CListFormTabPage
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.m_listView);
            this.Controls.Add(this.panel1);
            this.Name = "CListFormTabPage";
            this.Load += new System.EventHandler(this.CListFormTabPage_Load);
            this.SizeChanged += new System.EventHandler(this.CListFormTabPage_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pitureBoxClose)).EndInit();
            this.ResumeLayout(false);

        }
        private ToolTip m_toolTip; //关闭按钮的提示信息


        private void CListFormTabPage_SizeChanged(object sender, EventArgs e)
        {
            int width = this.Width;
            int height = this.Height - panel1.Height;
            //m_listBox.Size = new System.Drawing.Size(width, height);
            m_listView.Size = new System.Drawing.Size(width, height);
            if (m_listView.Columns.Count > 0)
            {
                m_listView.Columns[0].Width = width /*- 25*/;
                //if( m_listView. )
            }
        }

        private void m_pitureBoxClose_Click(object sender, EventArgs e)
        {
            //关闭事件
            this.TabClosed(this, new EventArgs());
        }

        private void m_pitureBoxClose_MouseHover(object sender, EventArgs e)
        {
            //显示提示信息
            m_pitureBoxClose.BackColor = Color.LightYellow;
            m_toolTip.Show("关闭", m_pitureBoxClose, 0, m_pitureBoxClose.Height + 5, 1000); //显示1秒
        }

        private void m_pitureBoxClose_MouseLeave(object sender, EventArgs e)
        {
            m_pitureBoxClose.BackColor = Color.Transparent;
        }

        private void CListFormTabPage_Load(object sender, EventArgs e)
        {
            //this.m_listView.Scrollable = true;
            //CListFormTabPage.ShowScrollBar( (int)this.m_listView.Handle, SB_HORZ, 0); //只显示垂直滚动条
        }
        #endregion PRIVATE_REGION



        //         private void m_listBox_DrawItem(object sender, DrawItemEventArgs e)
        //         {
        //             //自定义的颜色来实例化
        //             object item = (((ListBox)sender).Items[e.Index]);
        //             //转换成自定义的类型
        //             CExTextString ownItem = item as CExTextString;
        //             e.Graphics.FillRectangle(new SolidBrush(ownItem.BGColor), e.Bounds);
        //             e.Graphics.DrawString(ownItem.ToString(), ownItem.TxFont, new SolidBrush(ownItem.ForeColor), e.Bounds);
        //             e.DrawFocusRectangle();
        //         }

        /*private void m_listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item = (((ListBox)sender).Items[m_listBox.SelectedIndex]);
            //转换成自定义的类型
            CExTextString ownItem = item as CExTextString;
            ownItem.BGColor = Color.BurlyWood;
        }*/
        //private Form m_form;

    }
}
