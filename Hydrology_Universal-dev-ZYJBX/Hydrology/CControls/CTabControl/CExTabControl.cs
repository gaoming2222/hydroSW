using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Hydrology.CControls
{
    public class CExTabControl : TabControl
    {
        // 保存当前的所有页面
        private List<ITabPage> m_listPage;

        // 关闭图标相关
        private int m_iIconCloseH = 16;
        private int m_iIconCloseW = 16;
        private Bitmap m_imageClose = null;
        private const string S_C_IMAGEHOLDER = "    ";
        private ToolTip m_toolTip;
        private Boolean m_bHasTip;

        #region PROPERTY
        private Color m_itemSelectedColor;
        public Color ItemSelectedColor
        {
            get { return m_itemSelectedColor; }
            set { m_itemSelectedColor = value; }
        }

        private Color m_itemUnSelectedColor;
        public Color ItemUnSelectedColor
        {
            get { return m_itemUnSelectedColor; }
            set { m_itemUnSelectedColor = value; }
        }
        private Color m_colorWhiteSpaceFill;
        public Color WhitePlaceFillColor
        {
            get { return m_colorWhiteSpaceFill; }
            set { m_colorWhiteSpaceFill = value; }
        }
        // 标签页与内容的宽度
        private int m_iTabContentSpacePixel;
        public int TabContentSpacePixel
        {
            get { return m_iTabContentSpacePixel; }
            set { m_iTabContentSpacePixel = value; }
        }
        // 右边线条的空间区域
        private int m_iTabRightSpacePixel;
        public int TabRightSpacePixel
        {
            get { return m_iTabRightSpacePixel; }
            set { m_iTabRightSpacePixel = value; }
        }

        #endregion ///<PROPERTY

        //private PictureBox m_pitureBoxClose; //关闭图标
        public CExTabControl()
        {
            // 设置样式
            // base.SetStyle(ControlStyles.UserPaint, true);
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);

            m_listPage = new List<ITabPage>();

            //this.Appearance = TabAppearance.Buttons; //选项卡的显示模式
            this.Dock = DockStyle.Fill;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Margin = new Padding(0);
            this.Padding = new Point(0, 0);
            this.Appearance = TabAppearance.Normal;
            m_imageClose = global::Hydrology.Properties.Resources.IconClose.ToBitmap();
            this.ItemSize = new Size(50, m_iIconCloseH + 4); //设置选项卡标签的大小,可改变高不可改变宽
            m_toolTip = new ToolTip();
            m_toolTip.AutoPopDelay = 0;
            m_toolTip.UseAnimation = true;
            //m_toolTip.ReshowDelay = 1000;
            //m_toolTip.ShowAlways = true;
            m_bHasTip = false;

            // 设置默认颜色
            m_itemSelectedColor = Color.FromArgb(255, 232, 166);
            m_itemUnSelectedColor = Color.FromArgb(70, 235, 235, 235);
            m_colorWhiteSpaceFill = Color.FromArgb(255, 232, 166);
            m_iTabContentSpacePixel = 3;
            m_iTabRightSpacePixel = 3;
        }
        // 必须是控件,System.Windows.Forms.Controls
        public void AddPage(ITabPage page)
        {
            if (page is Control)
            {
                TabPage tabpage = new TabPage();
                tabpage.BorderStyle = BorderStyle.None;
                tabpage.BackColor = Color.Transparent;
                tabpage.BackgroundImage = Hydrology.Properties.Resources.状态栏;
                tabpage.SuspendLayout();
                this.Controls.Add(tabpage);
                tabpage.Controls.Add((Control)page);
                tabpage.Name = page.Title;
                if (page.BTabRectClosable)
                {
                    tabpage.Text = page.Title + S_C_IMAGEHOLDER;
                }
                else
                {
                    tabpage.Text = page.Title;
                }
                tabpage.UseVisualStyleBackColor = true;
                tabpage.TabIndex = m_listPage.Count;
                tabpage.ResumeLayout(false);
                page.TabPageIndex = m_listPage.Count;
                m_listPage.Add(page); //添加到集合，便于管理
                //绑定消息
                page.TabClosed -= new EventHandler(this.EHTabClosed);
                page.TabClosed += new EventHandler(this.EHTabClosed);
            }//end of if
        }

        public void RemovePage(ITabPage page)
        {
            this.TabPages.RemoveAt(page.TabPageIndex);
            m_listPage.RemoveAt(page.TabPageIndex);
            // 更新其它页面的索引吧？
            for (int i = 0; i < m_listPage.Count; ++i)
            {
                m_listPage[i].TabPageIndex = i;
            }
        }

        //public void AddPage(ITabPage page, Padding pad, Padding margin)
        //{
        //    if (page is Control)
        //    {
        //        TabPage tabpage = new TabPage();
        //        tabpage.BorderStyle = BorderStyle.None;
        //        tabpage.BackColor = Color.Transparent;
        //        tabpage.BackgroundImage = Hydrology.Properties.Resources.状态栏;
        //        tabpage.Padding = pad;
        //        tabpage.Margin = margin;
        //        tabpage.Location = new System.Drawing.Point(4, 22);
        //        tabpage.SuspendLayout();
        //        this.Controls.Add(tabpage);
        //        tabpage.Controls.Add((Control)page);
        //        tabpage.Name = page.Title;
        //        if (page.BTabRectClosable)
        //        {
        //            tabpage.Text = page.Title + S_C_IMAGEHOLDER;
        //        }
        //        else
        //        {
        //            tabpage.Text = page.Title;
        //        }
        //        tabpage.UseVisualStyleBackColor = true;
        //        tabpage.TabIndex = m_listPage.Count;
        //        tabpage.ResumeLayout(false);
        //        page.TabPageIndex = m_listPage.Count;
        //        m_listPage.Add(page); //添加到集合，便于管理
        //        //绑定消息
        //        page.TabClosed += new EventHandler(this.EHTabClosed);
        //    }//end of if
        //}
        #region EVENT_HANDER
        private void EHTabClosed(object sender, EventArgs args)
        {
            //某个页面关闭事件
            ITabPage page = sender as ITabPage;
            this.TabPages.RemoveAt(page.TabPageIndex);
            m_listPage.RemoveAt(page.TabPageIndex);

            // 更新其它页面的索引吧？
            for (int i = 0; i < m_listPage.Count; ++i)
            {
                m_listPage[i].TabPageIndex = i;
            }

        }
        #endregion EVENT_HANDER

        #region OVERRIDE
        //重写绘制事件
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Brush selected_bgcolor = new SolidBrush(m_itemSelectedColor); //选中的项的背景色
            Pen selected_pen = new Pen(Brushes.Gray, 1);
            Brush noselected_bgcolor = new SolidBrush(m_itemUnSelectedColor); //未选中的项的背景色
            Pen unselected_pen = new Pen(Brushes.Gray, 1);
            Brush sel_text_color = Brushes.Black;
            Brush nosel_text_color = Brushes.White;

            Graphics g = e.Graphics;
            Color backColor = Color.Red; // 背景色
            // 绘制背景图片，达到BGIMG的效果
            //using (SolidBrush brush = new SolidBrush(backColor))
            //{
            //Graphics tmp = Graphics.FromImage(MDITest.Properties.Resources.WhiteSpace);
            //g.FillRectangle(brush, ClientRectangle);

            //g.DrawImage(MDITest.Properties.Resources.WhiteSpace, ClientRectangle);
            ImageAttributes imageAttrib = new ImageAttributes();

            //imageAttrib.SetWrapMode(WrapMode.TileFlipXY);
            imageAttrib.SetWrapMode(WrapMode.TileFlipXY);

            // 设置标签页的空白背景
            g.DrawImage(Hydrology.Properties.Resources.状态栏,
                ClientRectangle, 0, 0,
                Hydrology.Properties.Resources.状态栏.Width, Hydrology.Properties.Resources.状态栏.Height,
                GraphicsUnit.Pixel, imageAttrib);

            imageAttrib.Dispose();

            //g.FillRectangle(selected_color, r);
            //g.FillRectangle(brush, headerRect);
            //}
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                string title = this.TabPages[i].Text;
                Rectangle r = GetTabRect(i);
                //r.Width = r.Width + 8;
                r.Height = r.Height + m_iTabContentSpacePixel;
                switch (this.Alignment)
                {
                    case TabAlignment.Top: { r.Y = r.Y - m_iTabContentSpacePixel + 1; ; } break;
                    case TabAlignment.Bottom: { r.Y = r.Y - 1 /*+ m_iTabContentSpacePixel - 1*/; } break;
                }


                //GraphicsPath path = new GraphicsPath();
                //path.AddLine(r.X, r.Y + r.Height, r.X - 8, r.Y);
                //path.AddLine(r.X - 8, r.Y, r.X + r.Width - 8, r.Y);
                //path.AddLine(r.X + r.Width - 8, r.Y, r.X + r.Width, r.Y + r.Height);
                if (i == e.Index)
                {
                    //g.DrawPath(selected_pen, path);
                    //g.FillPath(selected_bgcolor, path); 
                    g.DrawRectangle(selected_pen, r);
                    g.FillRectangle(selected_bgcolor, r); //改变选项卡标签的背景色  
                    g.DrawString(title, this.Font, sel_text_color, new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置
                }
                else
                {
                    //g.DrawRectangle(unselected_pen, r);
                    g.FillRectangle(noselected_bgcolor, r); //改变选项卡标签的背景色 
                    g.DrawString(title, this.Font, nosel_text_color, new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置
                }
                r.Offset(r.Width - m_iIconCloseW - 0, 0);
                if (m_listPage[i].BTabRectClosable)
                {
                    g.DrawImage(m_imageClose, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);//选项卡上的图标的位置
                    //this.TabPages[i].Controls.Add(m_pitureBoxClose);
                    //m_pitureBoxClose.Location = new Point(r.X - 2, r.Y + 1);
                    //m_pitureBoxClose.Show();
                }
            }
            Rectangle rLink = ClientRectangle;
            switch (this.Alignment)
            {
                case TabAlignment.Top: { rLink.Y = this.ItemSize.Height + m_iTabContentSpacePixel; } break;
                case TabAlignment.Bottom: { rLink.Y = 0; rLink.Height -= (this.ItemSize.Height + m_iTabContentSpacePixel); } break;
            }
            SolidBrush rLinkBrush = new SolidBrush(m_colorWhiteSpaceFill);

            //rLink.Height = 3;
            g.FillRectangle(rLinkBrush, rLink); //标签页下面或者上面的黄色线条
            #region  BEFORE

            //             if (e.Index == this.SelectedIndex)
            //             {
            //                 //当前选中的Tab页，设置不同的样式以示选中  
            //                 Brush selected_color = Brushes.White; //选中的项的背景色  
            //                 g.FillRectangle(selected_color, r); //改变选项卡标签的背景色  
            //                 string title = this.TabPages[e.Index].Text;
            //                 g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置
            //                 r.Offset(r.Width - m_iIconCloseW - 0, 0);
            //                 if (m_listPage[e.Index].BTabRectClosable)
            //                 {
            //                     g.DrawImage(m_imageClose, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);//选项卡上的图标的位置
            //                 }
            //                 //this.ShowToolTips = true;
            //             }
            //             else
            //             {
            //                 Brush noselected_color = Brushes.Tomato; //选中的项的背景色 
            //                 string title = this.TabPages[e.Index].Text;
            //                 g.FillRectangle(noselected_color, r); //改变选项卡标签的背景色  
            //                 g.DrawString(title, this.Font, new SolidBrush(Color.Black), new PointF(r.X + 3, r.Y + 6));//PointF选项卡标题的位置   
            //                 r.Offset(r.Width - m_iIconCloseW - 0, 0);
            //                 if (m_listPage[e.Index].BTabRectClosable)
            //                 {
            //                     g.DrawImage(m_imageClose, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);//选项卡上的图标的位置
            //                 }
            //             }
            #endregion BEFORE

        }
        // 单击关闭
        protected override void OnMouseClick(MouseEventArgs e)
        {
            //左键判断是否在关闭区域
            if (e.Button == MouseButtons.Left)
            {
                if (this.SelectedIndex < m_listPage.Count)
                {
                    if (m_listPage[this.SelectedIndex].BTabRectClosable)
                    {
                        Point p = e.Location;
                        Rectangle r = GetTabRect(this.SelectedIndex);
                        r.Offset(r.Width - m_iIconCloseW - 3, 2);
                        r.Width = m_iIconCloseW;
                        r.Height = m_iIconCloseH;
                        if (r.Contains(p)) //点击特定区域时才发生   
                        {
                            //MessageBox.Show("Hello world");
                            //this.TabPages.Remove(this.SelectedTab);
                            //this.RemovePage(this.SelectedTab as ITabPage);
                            m_listPage[this.SelectedIndex].CloseTab();
                            //m_listPage.RemoveAt(this.SelectedIndex);
                        }
                    }//if closable
                }// if index okey
            }// if button left
        }
        // 提示
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //if ((this.TabPages[SelectedIndex] as ITabPage).BClosable)
            //{
            // 当前页面可以关闭
            Point p = e.Location;
            Rectangle r = GetTabRect(this.SelectedIndex);
            if (this.SelectedIndex < m_listPage.Count)
            {
                if (m_listPage[this.SelectedIndex].BTabRectClosable)
                {
                    r.Offset(r.Width - m_iIconCloseW - 3, 2);
                    r.Width = m_iIconCloseW;
                    r.Height = m_iIconCloseH;
                    if (!m_bHasTip && r.Contains(p)) //点击特定区域时才发生   
                    {
                        Graphics g = this.CreateGraphics();
                        r.Offset(r.Width - m_iIconCloseW - 0, 0);
                        Pen pen = new Pen(Color.White, 1);
                        g.DrawRectangle(pen, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);
                        m_toolTip.Show("关闭", this, p.X + 10, p.Y + 15, 1000);
                        m_bHasTip = true;
                    }
                    else if (!r.Contains(p))
                    {
                        //m_toolTip.StopTim
                        Debug.WriteLine("##move out");
                        Graphics g = this.CreateGraphics();
                        Pen pen = new Pen(Color.FromArgb(255, 232, 166), 1);
                        g.DrawRectangle(pen, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);
                        m_toolTip.Hide(this);
                        m_bHasTip = false;
                    }
                }//end of if closable
            }
            #region NEW
            //             base.OnMouseMove(e);
            //             //if ((this.TabPages[SelectedIndex] as ITabPage).BClosable)
            //             //{
            //             if (this.SelectedIndex > -1/* && m_listPage[SelectedIndex].BClosable*/)
            //             {
            //                 // 当前页面可以关闭
            //                 Point p = e.Location;
            //                 bool bMeet =false;
            //                 Rectangle r = new Rectangle();
            //                 for(  int i= 0;i< m_listPage.Count; ++i )
            //                 {
            //                     if (m_listPage[i].BTabRectClosable)
            //                     {
            //                         /*Rectangle*/ r = GetTabRect(i);
            //                         r.Offset(r.Width - m_iIconCloseW - 3, 2);
            //                         r.Width = m_iIconCloseW;
            //                         r.Height = m_iIconCloseH;
            //                         if (!m_bHasTip && r.Contains(p)) //点击特定区域时才发生   
            //                         {
            //                             m_bHasTip = true;
            //                             bMeet = true;
            //                             Debug.WriteLine("---->move in - bMeet:{0}",bMeet);
            //                             i = m_listPage.Count; // for break;
            //                         }
            //                     }//end of if closable
            //                 }//index < count
            //                 if ( !bMeet)
            //                 {
            //                     m_bHasTip = false;
            //                     //m_toolTip.Hide(this);
            //                     Pen pen = new Pen(Color.FromArgb(255, 232, 166), 1);
            //                     Graphics g = this.CreateGraphics();
            //                     g.DrawRectangle(pen, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);
            //                     Debug.WriteLine("####move out bMeet:{0}",bMeet);
            //                 }
            //                 else
            //                 {
            //                     // 显示提示
            //                     Graphics g = this.CreateGraphics();
            //                     r.Offset(r.Width - m_iIconCloseW - 0, 0);
            //                     //g.DrawImage(m_imageClose, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);//选项卡上的图标的位置
            //                     Pen pen = new Pen(Color.White, 1);
            //                     g.DrawRectangle(pen, r.X-2,r.Y + 1,m_iIconCloseW,m_iIconCloseH);
            //                     //g.FillRectangle(Brushes.Gray, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);
            //                     //pen.Brush = Brushes.Yellow;
            //                     //g.DrawRectangle(pen, r.X - 2, r.Y + 1, m_iIconCloseW, m_iIconCloseH);
            //                     //this.ShowToolTips = true;
            //                     m_toolTip.Show("关闭", this, p.X + 10, p.Y + 15, 1000);
            //                 }
            //             }// end of index > -1
            #endregion
        }

        // 用户闪动
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // 加了这句话以后，会导致不停的刷新？什么原因呢？？
                // cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        // 去掉边距，3,3,3,3,并且消除外边线
        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = base.DisplayRectangle;
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        {
                            rect.X = rect.X - m_iTabRightSpacePixel - 1;
                            rect.Y = rect.Y + m_iTabContentSpacePixel;
                            rect.Width = rect.Width + m_iTabRightSpacePixel + 1;
                            rect.Height += 0;
                        } break;
                    case TabAlignment.Bottom:
                        {
                            rect.X = rect.X - 3 - 1;
                            rect.Y = rect.Y - 3 - 1;
                            rect.Width = rect.Width + 3 + 3 - m_iTabRightSpacePixel + 1;
                            rect.Height += 3 - m_iTabContentSpacePixel;
                        } break;
                }
                return rect;
            }
        }

        #endregion OVERIDE

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CExTabControl
            // 
            this.Margin = new System.Windows.Forms.Padding(0);
            this.ResumeLayout(false);


            //this.Controls.Add(m_pitureBoxClose);
            //this.m_pitureBoxClose.Click += new System.EventHandler(this.m_pitureBoxClose_Click);
            //this.m_pitureBoxClose.MouseLeave += new System.EventHandler(this.m_pitureBoxClose_MouseLeave);
            //this.m_pitureBoxClose.MouseHover += new System.EventHandler(this.m_pitureBoxClose_MouseHover);

        }
    }
}
