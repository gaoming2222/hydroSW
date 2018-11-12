using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

namespace Hydrology.CControls
{
    /// <summary>
    /// 继承自Windows.Form中得自定义图标控件，不包含图例 ，以及Serials
    /// </summary>
    public class CExChart : Chart
    {
        #region 静态常量
        // 默认的绘图区域的名字
        public static readonly string CS_ChartAreaName_Default = "Default";
        
        // 时间名字,用于数据源表列名
        public static readonly string CS_CN_DateTime = "DateTime";

        // 时间的坐标名字
        public static readonly string CS_Asix_DateTime = "日期时间";

        /// <summary>
        /// 最大的数据点位,1万条记录
        /// </summary>
        public static readonly int CI_Chart_Max_Count = 10000;
        #endregion 静态常量

        #region 成员变量

        // 默认的绘图区域
        protected ChartArea m_chartAreaDefault;
        // 保存文件的对话框
        protected SaveFileDialog m_dlgSaveFile;
        // 图标的标题
        protected Title m_title;
        // 提示文本
        protected CalloutAnnotation m_annotation = new CalloutAnnotation();

        // 右键菜单
        protected ContextMenu m_contextMenu;
        // 保存图片的菜单项
        protected MenuItem m_MISaveImage;

        // chart的数据源
        protected DataTable m_dataTable;

        #endregion 成员变量

        public CExChart():base()
        {
            // 初始化对话框
            m_dlgSaveFile = new SaveFileDialog();
            m_dlgSaveFile.Filter = "图片文件(*.bmp)|*.bmp|所有文件(*.*)|*.*";
            m_dataTable = new DataTable();

            // 设置数据源
            this.DataSource = m_dataTable;

            // 初始化界面以及右键
            InitUI();
            InitContextMenu();
        }

        /// <summary>
        /// 清空所有内容
        /// </summary>
        protected virtual void ClearAllDatas()
        {
            m_dataTable.Rows.Clear();
            this.DataBind();
        }

        #region 事件处理
        /// <summary>
        /// 保存图片事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EH_SaveImage_Click(object sender, EventArgs e)
        {
            DialogResult result = m_dlgSaveFile.ShowDialog();
            if (DialogResult.OK == result)
            {
                // 获取文件名
                string filename = m_dlgSaveFile.FileName;
                // 只输出BMP格式
                this.SaveImage(filename, ChartImageFormat.Bmp);
                MessageBox.Show("保存成功");
            }
        }

        /// <summary>
        /// 鼠标滚轮事件，用来缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EH_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    m_chartAreaDefault.AxisX.ScaleView.ZoomReset();
                    m_chartAreaDefault.AxisY.ScaleView.ZoomReset();
                    m_chartAreaDefault.AxisY2.ScaleView.ZoomReset();
                }

                if (e.Delta > 0)
                {
                    // 不做放大处理
                    return;
                }
            }
            catch { }
        }

        /// <summary>
        /// 一般用于Y,Y1z轴同步放大缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EH_AxisViewChanged(object sender, ViewEventArgs e)
        {
           
        }

        /// <summary>
        /// 位置发生改变，应该将点定位到数据点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void EH_CursorPositionChanged(object sender, CursorEventArgs e)
        {
            if (double.IsNaN(e.NewPosition))
                return;

            if (e.Axis.AxisName == AxisName.X)
            {
                // 如果X的值发生了变化，更新注释以及Y的值
                //UpdateAnnotationByDataPoint(e.NewPosition);
            }
            else
            {
            }
        }

        protected virtual void EH_MouseLeave(object sender, EventArgs e)
        {
            if (this.Focused)
            {
                // 释放焦点
                if (this.Parent != null)
                {
                    this.Parent.Focus();
                }
            }
        }

        protected virtual void EH_MouseEnter(object sender, EventArgs e)
        {
            if (!this.Focused)
            {
                // 获取焦点
                this.Focus();
            }
        }

        protected virtual void EH_MouseClick(object sender, MouseEventArgs e)
        {
            // 不显示注释
            m_annotation.Visible = false;
            var pos = e.Location;
            Point clickPosition = pos;
            var results = this.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    // 函数内部显示注释
                    UpdateAnnotationByDataPoint((result.Object as DataPoint));
                    break;
                }
            }
        }
        #endregion 事件处理

        #region 帮助方法
        /// <summary>
        /// 初始化界面UI
        /// </summary>
        protected virtual void InitUI()
        {
            #region 基本属性

            // 设置背景色
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));

            // 设置位置
            this.Location = new System.Drawing.Point(0, 0);
            // 设置边框颜色
            this.BorderlineDashStyle = ChartDashStyle.Solid;
            this.BorderlineWidth = 3;
            #endregion 基本属性

            #region 标题

            m_title = new Title();
            m_title.Font = new System.Drawing.Font("楷体", 14.25F, System.Drawing.FontStyle.Bold);
            m_title.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            //m_title.Name = "Title1";
            m_title.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            m_title.ShadowOffset = 1;
            m_title.Text = "图表标题";
            this.Titles.Add(m_title);

            #endregion 标题

            #region 绘图背景色ChartAreas

            // 初始化chartArea
            m_chartAreaDefault = new ChartArea();
            m_chartAreaDefault.Name = CS_ChartAreaName_Default;

            m_chartAreaDefault.BackColor = System.Drawing.Color.OldLace;
            m_chartAreaDefault.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            m_chartAreaDefault.BackSecondaryColor = System.Drawing.Color.White;
            m_chartAreaDefault.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

            // 设置最小值 ，防止崩溃
            m_chartAreaDefault.AxisX.ScaleView.MinSize = 0.001;
            m_chartAreaDefault.AxisY.ScaleView.MinSize = 0.001;
            m_chartAreaDefault.AxisY2.ScaleView.MinSize = 0.001;

            m_chartAreaDefault.ShadowColor = System.Drawing.Color.Transparent;

            #region 鼠标
            // 显示坐标
            m_chartAreaDefault.CursorX.IsUserEnabled = true;
            m_chartAreaDefault.CursorX.IsUserSelectionEnabled = true;
            //m_chartAreaDefault.CursorX.Position = 2; //设置初始光标位置，默认不设置
            m_chartAreaDefault.CursorX.SelectionColor = System.Drawing.Color.Gray;
            m_chartAreaDefault.CursorX.Interval = 1D;
            m_chartAreaDefault.CursorX.IntervalType = DateTimeIntervalType.Milliseconds;

            m_chartAreaDefault.CursorY.IsUserEnabled = true;
            m_chartAreaDefault.CursorY.IsUserSelectionEnabled = true;
            //m_chartAreaDefault.CursorY.Position = 2;
            m_chartAreaDefault.CursorY.SelectionColor = System.Drawing.Color.Gray;
            m_chartAreaDefault.CursorY.Interval = 1D;
            m_chartAreaDefault.CursorY.IntervalType = DateTimeIntervalType.Milliseconds;

            this.ChartAreas.Add(m_chartAreaDefault);

            #endregion 鼠标

            #endregion 绘图背景色

            #region XY轴设置

            m_chartAreaDefault.AxisX.IsLabelAutoFit = false;
            m_chartAreaDefault.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            // 设置横轴下面一条的颜色
            m_chartAreaDefault.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //m_chartAreaDefault.AxisX.Interval = 1;
            //m_chartAreaDefault.AxisX.MajorTickMark.IntervalType = DateTimeIntervalType.Milliseconds;
            //m_chartAreaDefault.AxisX.MajorTickMark.Interval = 1D;
            //m_chartAreaDefault.AxisX.IntervalType =  DateTimeIntervalType.Milliseconds;
            //m_chartAreaDefault.AxisX.Interval = 1D;
            m_chartAreaDefault.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //m_chartAreaDefault.AxisX.IntervalType = DateTimeIntervalType.Milliseconds;
            //m_chartAreaDefault.AxisX.MajorTickMark.Enabled = false;
            //m_chartAreaDefault.AxisX.ScrollBar.Size = 10; //滚动条宽度
            // 下面一行代码有效哟
            m_chartAreaDefault.AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Milliseconds;

            m_chartAreaDefault.AxisY.IsLabelAutoFit = false;
            m_chartAreaDefault.AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            m_chartAreaDefault.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            m_chartAreaDefault.AxisY.MajorTickMark.IntervalType =  DateTimeIntervalType.Auto;
            m_chartAreaDefault.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            m_chartAreaDefault.AxisY.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Milliseconds;
            m_chartAreaDefault.AxisY.LabelStyle.IsEndLabelVisible = true;
            m_chartAreaDefault.AxisY.LabelStyle.TruncatedLabels = true;

            //  设置Y2
            m_chartAreaDefault.AxisY2.IsLabelAutoFit = false;
            m_chartAreaDefault.AxisY2.LabelStyle.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold);
            m_chartAreaDefault.AxisY2.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            m_chartAreaDefault.AxisY2.MajorGrid.LineColor = Color.Transparent;
            m_chartAreaDefault.AxisY2.LabelStyle.IsEndLabelVisible = true;
            m_chartAreaDefault.AxisY2.LabelStyle.TruncatedLabels = true;
            m_chartAreaDefault.AxisY2.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Milliseconds;

            // 设置X轴箭头，以及文字
            m_chartAreaDefault.AxisX.ArrowStyle = AxisArrowStyle.None;
            m_chartAreaDefault.AxisX.TitleAlignment = StringAlignment.Center;
            m_chartAreaDefault.AxisX.TitleFont = new System.Drawing.Font("宋体", 10F);
            //默认X轴都是时间，格式都是年月日时
            m_chartAreaDefault.AxisX.LabelStyle.Format = "yy/MM/dd HH";

            // 设置Y轴箭头，以及文字
            m_chartAreaDefault.AxisY.ArrowStyle = AxisArrowStyle.Triangle;
            m_chartAreaDefault.AxisY.TitleAlignment = StringAlignment.Far;
            m_chartAreaDefault.AxisY.TitleFont = new System.Drawing.Font("楷体", 10F);

            // 设置Y2轴箭头，以及文字
            m_chartAreaDefault.AxisY2.ArrowStyle = AxisArrowStyle.Triangle;
            m_chartAreaDefault.AxisY2.TitleAlignment = StringAlignment.Near;
            m_chartAreaDefault.AxisY2.TitleFont = new System.Drawing.Font("宋体", 10F);

            // 设置缩放
            m_chartAreaDefault.AxisX.ScaleView.Zoomable = true;
            m_chartAreaDefault.AxisY.ScaleView.Zoomable = true;
            m_chartAreaDefault.AxisY2.ScaleView.Zoomable = true;
            m_chartAreaDefault.AxisY2.ScrollBar.Enabled = false; //Y2轴的滚动条是不可见的，Y2的缩放比例跟Y轴的保持一致

            #endregion XY轴设置

            #region 提示文本

            m_annotation.AllowAnchorMoving = false;
            m_annotation.AllowSelecting = true;
            m_annotation.AnchorDataPointName = "Default";
            m_annotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(240)))));
            m_annotation.Font = new System.Drawing.Font("宋体", 9F);
            m_annotation.Name = "Callout1";
            m_annotation.Visible = false;
            m_annotation.Alignment = ContentAlignment.TopLeft;
            this.Annotations.Add(m_annotation);

            #endregion 提示文本

            // 绑定消息

            //this.CursorPositionChanged += new EventHandler<CursorEventArgs>(EH_CursorPositionChanged);
            this.AxisViewChanged += new EventHandler<ViewEventArgs>(EH_AxisViewChanged);
            this.MouseClick += new MouseEventHandler(EH_MouseClick);

            // 滚轮事件,不一定自动获取焦点
            this.MouseWheel += new MouseEventHandler(EH_MouseWheel);
            this.MouseEnter += new EventHandler(EH_MouseEnter);
            this.MouseLeave += new EventHandler(EH_MouseLeave);

        }

        /// <summary>
        /// 初始化右键菜单
        /// </summary>
        protected virtual void InitContextMenu()
        {
            m_contextMenu = new ContextMenu();

            m_MISaveImage = new MenuItem() { Text = "保存图片..." };
            m_contextMenu.MenuItems.Add(m_MISaveImage);

            this.ContextMenu = m_contextMenu;

            m_MISaveImage.Click += new EventHandler(EH_SaveImage_Click);
        }

        /// <summary>
        /// 根据X的值，更新Y的值，以及注释内容并显示
        /// </summary>
        /// <param name="xPosition"></param>
        protected virtual void UpdateAnnotationByDataPoint(DataPoint point)
        {
            
        }
        #endregion 帮助方法
    }//class CExChart
}
