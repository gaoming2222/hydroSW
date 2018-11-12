using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using Hydrology.DBManager.Interface;

namespace Hydrology.CControls
{
    public class CChartWaterStage : CExChart
    {
        #region 静态常量
        // 数据源中水位的列名
        public static readonly string CS_CN_Water = "water";

        // 数据源中流量的列名
        public static readonly string CS_CN_Flow = "flow";

        // 水位的坐标名字
        public static readonly string CS_AsixY_Name = "水位(m)";

        // 流量的坐标名字
        public static readonly string CS_AsixY2_Name = "流量(m3/s)";

        // 图表名字
        public static readonly string CS_Chart_Name = "水位流量过程线";

        // 水位线条名字
        public static readonly string CS_Serial_Name_Water = "Serial_Water";

        // 流量线条名字
        public static readonly string CS_Serial_Name_Flow = "Serial_Flow";

        #endregion 静态常量

        private Nullable<decimal> m_dMinWaterStage; //最小的水位值,实际值，不是计算后的值
        private Nullable<decimal> m_dMaxWaterStage; //最大的水位值，实际值，不是计算后的值

        private Nullable<decimal> m_dMinWaterFlow; //最小的流量，实际值，不是计算后的值
        private Nullable<decimal> m_dMaxWaterFlow; //最大的流量，实际值，不是计算后的值

        private Nullable<DateTime> m_maxDateTime;   //最大的日期
        private Nullable<DateTime> m_minDateTime;   //最小的日期

        private Series m_serialWaterState;         //水位过程线
        private Series m_serialWaterFlow;         //流量过程线

        private Legend m_legend;     //图例

        private MenuItem m_MIWaterSerial; //水位
        private MenuItem m_MIFlowSerial;  //流量

        private IWaterProxy m_proxyWaterFlow;

        public CChartWaterStage()
            : base()
        {
            // 设定数据表的列
            base.m_dataTable.Columns.Add(CS_CN_DateTime, typeof(DateTime));
            base.m_dataTable.Columns.Add(CS_CN_Water, typeof(Decimal));
            base.m_dataTable.Columns.Add(CS_CN_Flow, typeof(Decimal));
        }
        // 外部添加水位流量接口
        public void AddWaters(List<CEntityWater> waters)
        {
            m_dMinWaterStage = null;
            m_dMaxWaterStage = null;
            foreach (CEntityWater water in waters)
            {
            //    if (water.WaterStage > 0 && water.WaterFlow > 0)
                if (water.WaterStage !=-200 )
                {

                    // 判断水位最大值和最小值
                    if (m_dMinWaterStage.HasValue)
                    {
                        m_dMinWaterStage = m_dMinWaterStage > water.WaterStage ? water.WaterStage : m_dMinWaterStage;
                    }
                    else
                    {
                        m_dMinWaterStage = water.WaterStage;
                    }
                    if (m_dMaxWaterStage.HasValue)
                    {
                        m_dMaxWaterStage = m_dMaxWaterStage < water.WaterStage ? water.WaterStage : m_dMaxWaterStage;
                    }
                    else
                    {
                        m_dMaxWaterStage = water.WaterStage;
                    }
                }
                if (water.WaterFlow > 0)
                {
                    // 判断流量的最大值和最小值
                    if (m_dMinWaterFlow.HasValue)
                    {
                        m_dMinWaterFlow = m_dMinWaterFlow > water.WaterFlow ? water.WaterFlow : m_dMinWaterFlow;
                    }
                    else
                    {
                        m_dMinWaterFlow = water.WaterFlow;
                    }
                    if (m_dMaxWaterFlow.HasValue)
                    {
                        m_dMaxWaterFlow = m_dMaxWaterFlow < water.WaterFlow ? water.WaterFlow : m_dMaxWaterFlow;
                    }
                    else
                    {
                        m_dMaxWaterFlow = water.WaterFlow;
                    }
                }
                else
                {
                    water.WaterFlow = 0;
                }
                    // 判断日期, 更新日期最大值和最小值
                    if (m_maxDateTime.HasValue)
                    {
                        m_maxDateTime = m_maxDateTime < water.TimeCollect ? water.TimeCollect : m_maxDateTime;
                    }
                    else
                    {
                        m_maxDateTime = water.TimeCollect;
                    }
                    if (m_minDateTime.HasValue)
                    {
                        m_minDateTime = m_minDateTime > water.TimeCollect ? water.TimeCollect : m_minDateTime;
                    }
                    else
                    {
                        m_minDateTime = water.TimeCollect;
                    }

                    if (water.WaterStage != -200 && water.WaterFlow >=0)
                    {
                        //赋值到内部数据表中
                        m_dataTable.Rows.Add(water.TimeCollect, water.WaterStage, water.WaterFlow);
                       // m_dataTable.Rows.Add(water.TimeCollect, water.WaterStage);
                    }
                  //  if( water.WaterFlow != -200)
                  //{
                  //    m_dataTable.Rows.Add(water.TimeCollect, water.WaterFlow);
                  //}

                
            }
            if (waters.Count >= 3)
            {
                // 水位和流量最大值和最小值
                decimal offset = 0;
                m_dMaxWaterStage = m_dMaxWaterStage == null ? 0 : m_dMaxWaterStage;
                m_dMinWaterStage = m_dMinWaterStage == null ? 0 : m_dMinWaterStage;
                if (m_dMaxWaterStage != m_dMinWaterStage)
                {
                    offset = (m_dMaxWaterStage.Value - m_dMinWaterStage.Value) * (decimal)0.1;
                }
                else
                {
                    // 如果相等的话
                    offset = (decimal)m_dMaxWaterStage * (decimal)0.1;
                }
                m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxWaterStage + offset);
                m_chartAreaDefault.AxisY.Minimum = (double)(m_dMinWaterStage - offset);
                if (offset == 0)
                {
                    // 人为赋值
                    m_chartAreaDefault.AxisY.Maximum = m_chartAreaDefault.AxisY.Minimum + 10;
                }

                if (m_dMaxWaterFlow.HasValue && m_dMinWaterFlow.HasValue)
                {
                    if (m_dMaxWaterFlow != m_dMinWaterFlow)
                    {
                        offset = (m_dMaxWaterFlow.Value - m_dMinWaterFlow.Value) * (decimal)0.1;
                    }
                    else
                    {
                        offset = (decimal)m_dMaxWaterFlow / 2;
                    }
                    m_chartAreaDefault.AxisY2.Maximum = (double)(m_dMaxWaterFlow + offset);
                    m_chartAreaDefault.AxisY2.Minimum = (double)(m_dMinWaterFlow - offset);

                    if (offset == 0)
                    {
                        m_chartAreaDefault.AxisY2.Maximum = m_chartAreaDefault.AxisY2.Minimum + 10; //人为赋值
                    }
                    m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.True;
                }
                else
                {
                    // 没有流量数据
                    // 人为流量最大最小值
                    m_chartAreaDefault.AxisY2.Maximum = (double)100;
                    m_chartAreaDefault.AxisY2.Minimum = (double)0;
                    m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.False;
                }
                // 设置日期最大值和最小值
                m_chartAreaDefault.AxisX.Minimum = m_minDateTime.Value.ToOADate();
                m_chartAreaDefault.AxisX.Maximum = m_maxDateTime.Value.ToOADate();

                this.DataBind(); //更新数据到图表
            }
        }

        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            m_annotation.Visible = false;
            ClearAllDatas();
            m_proxyWaterFlow.SetFilter(iStationId, timeStart, timeEnd, TimeSelect);
            if (-1 == m_proxyWaterFlow.GetPageCount())
            {
                // 查询失败
                // MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                return false;
            }
            else
            {
                // 并查询数据，显示第一页
                m_dMaxWaterFlow = null;
                m_dMaxWaterStage = null;
                m_dMinWaterFlow = null;
                m_dMinWaterStage = null;
                int iTotalPage = m_proxyWaterFlow.GetPageCount();
                int rowcount = m_proxyWaterFlow.GetRowCount();
                if (rowcount > CI_Chart_Max_Count)
                {
                    // 数据量太大，退出绘图
                    MessageBox.Show("查询结果集太大，自动退出绘图");
                    return false;
                }
                for (int i = 0; i < iTotalPage; ++i)
                {
                    // 查询所有的数据
                    this.AddWaters(m_proxyWaterFlow.GetPageData(i + 1));
                }
                return true;
            }
        }

        public void InitDataSource(IWaterProxy proxy)
        {
            m_proxyWaterFlow = proxy;
        }

        //流量
        private void EH_MI_FlowSerial(object sender, EventArgs e)
        {
            m_MIFlowSerial.Checked = !m_MIFlowSerial.Checked;
            m_serialWaterFlow.Enabled = m_MIFlowSerial.Checked;
            //m_serialWaterFlow.Enabled = true;
            //m_serialWaterState.Enabled = true;
            if (m_MIFlowSerial.Checked && (!m_MIWaterSerial.Checked))
            {
                // 开启右边的滚动条，当且仅当流量可见的时候
                //m_chartAreaDefault.AxisY2.ScaleView.Zoomable = false;
                //m_chartAreaDefault.AxisY2.ScrollBar.Enabled = true;
                //m_chartAreaDefault.CursorY.AxisType = AxisType.Secondary;
                //m_serialWaterFlow.YAxisType = AxisType.Primary;
                m_chartAreaDefault.CursorY.IsUserEnabled = false;
                m_chartAreaDefault.CursorY.IsUserSelectionEnabled = false;
            }
            else
            {
                // 关闭右边的滚动条
                m_chartAreaDefault.CursorY.IsUserEnabled = true;
                m_chartAreaDefault.CursorY.IsUserSelectionEnabled = true;
                //m_chartAreaDefault.AxisY2.ScrollBar.Enabled = false;
                //m_chartAreaDefault.AxisY.ScrollBar.Enabled = true;
                //m_serialWaterFlow.YAxisType = AxisType.Secondary;
                //m_serialWaterState.YAxisType = AxisType.Primary;
            }
            //流量过程线
            if (m_serialWaterFlow.Enabled)
            {
                // 流量可见
                m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.True;
            }
            else
            {
                // 流量不可见
                m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.False;
            }
            //水位过程线
            if (m_serialWaterState.Enabled)
            {
                // 水位可见
                m_chartAreaDefault.AxisY.Enabled = AxisEnabled.True;
            }
            else
            {
                // 水位不可见
                m_chartAreaDefault.AxisY.Enabled = AxisEnabled.False;
            }
        }

        //水位
        private void EH_MI_WaterSerial(object sender, EventArgs e)
        {
            m_MIWaterSerial.Checked = !m_MIWaterSerial.Checked;
            //水位
            m_serialWaterState.Enabled = m_MIWaterSerial.Checked;
            if (m_MIFlowSerial.Checked && (!m_MIWaterSerial.Checked))
            {
                // 开启右边的滚动条，当且仅当流量可见的时候
                m_chartAreaDefault.CursorY.IsUserEnabled = false;
                m_chartAreaDefault.CursorY.IsUserSelectionEnabled = false;
                //m_chartAreaDefault.CursorY.AxisType = AxisType.Secondary;
                //m_serialWaterFlow.YAxisType = AxisType.Primary;
            }
            else
            {
                // 关闭右边的滚动条
                m_chartAreaDefault.CursorY.IsUserEnabled = true;
                m_chartAreaDefault.CursorY.IsUserSelectionEnabled = true;
                //m_chartAreaDefault.AxisY2.ScrollBar.Enabled = false;
                //m_chartAreaDefault.AxisY2.ScaleView.Zoomable = true;
                //m_serialWaterFlow.YAxisType = AxisType.Secondary;
                //m_serialWaterState.YAxisType = AxisType.Primary;
            }
            if (m_serialWaterFlow.Enabled)
            {
                // 流量可见
                m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.True;
            }
            else
            {
                // 流量不可见
                m_chartAreaDefault.AxisY2.Enabled = AxisEnabled.False;
            }
            if (m_serialWaterState.Enabled)
            {
                // 水位可见
                m_chartAreaDefault.AxisY.Enabled = AxisEnabled.True;
            }
            else
            {
                // 水位不可见
                m_chartAreaDefault.AxisY.Enabled = AxisEnabled.False;
            }
        }


        #region 重载

        // 重新右键菜单
        protected override void InitContextMenu()
        {
            base.InitContextMenu();
            m_MIFlowSerial = new MenuItem() { Text = "流量线" };
            m_MIWaterSerial = new MenuItem() { Text = "水位线" };
            base.m_contextMenu.MenuItems.Add(0, m_MIWaterSerial);
            base.m_contextMenu.MenuItems.Add(0, m_MIFlowSerial);
            m_MIFlowSerial.Checked = true;
            m_MIWaterSerial.Checked = true;

            m_MIWaterSerial.Click += new EventHandler(EH_MI_WaterSerial);
            m_MIFlowSerial.Click += new EventHandler(EH_MI_FlowSerial);
        }


        // 重写UI,设置XY轴名字
        protected override void InitUI()
        {
            base.InitUI();
            // 设置图表标题
            m_title.Text = CS_Chart_Name;

            // 设置水位和流量格式
            m_chartAreaDefault.AxisY.LabelStyle.Format = "0.00";
            m_chartAreaDefault.AxisY2.LabelStyle.Format = "0.00";

            // m_chartAreaDefault.AxisX.Title = CS_Asix_DateTime; //不显示名字
            m_chartAreaDefault.AxisY.Title = CS_AsixY_Name;
            m_chartAreaDefault.AxisY2.Title = CS_AsixY2_Name;

            m_chartAreaDefault.AxisX.TextOrientation = TextOrientation.Horizontal;
            m_chartAreaDefault.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //m_chartAreaDefault.AxisX.a
            m_chartAreaDefault.AxisX.LabelStyle.Format = "MM-dd HH";
            m_chartAreaDefault.AxisX.LabelStyle.Angle = 90;

            #region 水位
            m_serialWaterState = this.Series.Add(CS_Serial_Name_Water);
            m_serialWaterState.Name = "水位"; //用来显示图例的
            m_serialWaterState.ChartArea = CS_ChartAreaName_Default;
            m_serialWaterState.ChartType = SeriesChartType.Line; //如果点数过多， 画图很慢，初步测试不能超过2000个
            m_serialWaterState.BorderWidth = 1;
            //m_serialWaterState.Color = Color.FromArgb(22,99,1);
            m_serialWaterState.Color = Color.Red;
            //m_serialWaterState.BorderColor = Color.FromArgb(120, 147, 190);
            //m_serialWaterState.ShadowColor = Color.FromArgb(64, 0, 0, 0);
            //m_serialWaterState.ShadowOffset = 2;
            //  设置时间类型,对于serial来说
            m_serialWaterState.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            m_serialWaterState.IsXValueIndexed = false; // 自己计算X值，以及边界值,否则翻译不出正确的值

            //  绑定数据
            m_serialWaterState.XValueMember = CS_CN_DateTime;
            m_serialWaterState.YValueMembers = CS_CN_Water;

            m_serialWaterState.YAxisType = AxisType.Primary;
            #endregion 水位

            #region 流量
            m_serialWaterFlow = this.Series.Add(CS_Serial_Name_Water);
            m_serialWaterFlow.Name = "流量"; //用来显示图例的
            m_serialWaterFlow.ChartArea = CS_ChartAreaName_Default;
            m_serialWaterFlow.ChartType = SeriesChartType.Line; //如果点数过多， 画图很慢，初步测试不能超过2000个
            m_serialWaterFlow.BorderWidth = 1;
            //m_serialWaterFlow.BorderColor = Color.FromArgb(120, 147, 190);
            m_serialWaterFlow.Color = Color.Blue;
            //m_serialWaterFlow.ShadowOffset = 2;
            //  设置时间类型,对于serial来说
            m_serialWaterFlow.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            m_serialWaterFlow.IsXValueIndexed = false; // 自己计算X值，以及边界值,否则翻译不出正确的值

            //  绑定数据
            m_serialWaterFlow.XValueMember = CS_CN_DateTime;
            m_serialWaterFlow.YValueMembers = CS_CN_Flow;
            m_serialWaterFlow.YAxisType = AxisType.Secondary;
            #endregion 流量

            #region 图例
            m_legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            m_legend.Alignment = System.Drawing.StringAlignment.Center;
            m_legend.BackColor = System.Drawing.Color.Transparent;
            m_legend.DockedToChartArea = CS_ChartAreaName_Default;
            m_legend.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            m_legend.IsDockedInsideChartArea = false;
            this.Legends.Add(m_legend);
            #endregion 图例
        }

        // 显示提示，并重新定位,xPosition有效
        protected override void UpdateAnnotationByDataPoint(DataPoint point)
        {
            if (null == point)
            {
                Debug.WriteLine("CChartWaterStage UpdateAnnotationByDataPoint Failed");
                return;
            }
            String prompt = "";
            DateTime dateTimeX = DateTime.FromOADate(point.XValue);
            if (m_serialWaterState.Points.Contains(point))
            {
                // 水位
                prompt = string.Format("水位：{0:0.00}\n日期：{1}\n时间：{2}", point.YValues[0],
                            dateTimeX.ToString("yyyy-MM-dd"),
                            dateTimeX.ToString("HH:mm:ss"));
            }
            else
            {
                // 就是流量了
                prompt = string.Format("流量：{0:0.00}\n日期：{1}\n时间：{2}", point.YValues[0],
                            dateTimeX.ToString("yyyy-MM-dd"),
                            dateTimeX.ToString("HH:mm:ss"));
            }

            m_chartAreaDefault.CursorY.Position = point.YValues[0]; // 重新设置Y的值
            m_chartAreaDefault.CursorX.Position = point.XValue; //重新设置X的值

            // 显示注释
            m_annotation.Text = prompt;
            //m_annotation.X = point.XValue;
            //m_annotation.Y = point.YValues[0];
            m_annotation.AnchorDataPoint = point;
            m_annotation.Visible = true;
        }

        // 同步放大和缩小
        protected override void EH_AxisViewChanged(object sender, ViewEventArgs e)
        {
            // 同步放大
            double iYMin = m_chartAreaDefault.AxisY.Minimum;
            double iYMax = m_chartAreaDefault.AxisY.Maximum;
            double iY2Min = m_chartAreaDefault.AxisY2.Minimum;
            double iY2Max = m_chartAreaDefault.AxisY2.Maximum;
            double yPosition = m_chartAreaDefault.AxisY.ScaleView.Position;
            m_chartAreaDefault.AxisY2.ScaleView.Position = iY2Min + ((yPosition - iYMin) / (iYMax - iYMin)) * (iY2Max - iY2Min);

            double size = m_chartAreaDefault.AxisY.ScaleView.Size;
            m_chartAreaDefault.AxisY2.ScaleView.Size = size * (iY2Max - iY2Min) / (iYMax - iYMin);
        }

        // 重载清空所有数据
        protected override void ClearAllDatas()
        {
            base.ClearAllDatas();
            m_maxDateTime = null;
            m_minDateTime = null;
            m_dMaxWaterFlow = null;
            m_dMinWaterFlow = null;
            m_dMaxWaterStage = null;
            m_dMinWaterStage = null;
        }

        #endregion 重载
    }
}
