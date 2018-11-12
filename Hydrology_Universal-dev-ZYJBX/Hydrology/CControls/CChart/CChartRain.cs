using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using Hydrology.Entity;
using System.Drawing;
using System.Diagnostics;
using Hydrology.DBManager.Interface;
using System.Windows.Forms;

namespace Hydrology.CControls
{
    public class CChartRain : CExChart
    {
        #region 静态常量
        // 数据源中时段的列名
        public static readonly string CS_CN_PeriodRain = "periodRain";

        // 时段雨量的坐标名字
        public static readonly string CS_AsixY_Name = "时段雨量(mm)";

        // 图表名字
        public static readonly string CS_Chart_Name = "雨量柱状图";

        // 线条名字
        public static readonly string CS_Serial_Name_Volatege = "Serial_PeriodRain";
        #endregion 静态常量

        private Nullable<decimal> m_dMinPeriodRain; //最小的时段雨量
        private Nullable<decimal> m_dMaxPeriodRain; //最大的时段雨量

        private Nullable<decimal> m_dMinDifferenceRain; //最小的时段雨量
        private Nullable<decimal> m_dMaxDifferenceRain; //最大的时段雨量

        private Nullable<decimal> m_dMinDayRain; //最小的时段雨量
        private Nullable<decimal> m_dMaxDayRain; //最大的时段雨量

        private Nullable<DateTime> m_timeMin;       //z的时间点
        private Nullable<DateTime> m_timeMax;       //最大的时间点
        private Series m_serialRain;         //雨量柱状图

        private IRainProxy m_proxyRain;

        public CChartRain()
            : base()
        {
            // 设定数据表的列
            base.m_dataTable.Columns.Add(CS_CN_DateTime, typeof(DateTime));
            base.m_dataTable.Columns.Add(CS_CN_PeriodRain, typeof(Decimal));

        }
        // 外部添加电压接口
        public void AddRains(List<CEntityRain> rainLists, int m)
        {
            List<CEntityRain> rains = rainLists.OrderBy(i => i.TimeCollect).ToList();
            if (m == 0)
            {
                m_dMinPeriodRain = 0;
                m_dMaxPeriodRain = 0;
                foreach (CEntityRain rain in rains)
                {
                    // 判断雨量的最大值和最小值
                    if (m_dMinPeriodRain.HasValue)
                    {
                        m_dMinPeriodRain = m_dMinPeriodRain > rain.PeriodRain ? rain.PeriodRain : m_dMinPeriodRain;
                    }
                    else
                    {
                        m_dMinPeriodRain = rain.PeriodRain;
                    }
                    if (m_dMaxPeriodRain.HasValue)
                    {
                        m_dMaxPeriodRain = m_dMaxPeriodRain < rain.PeriodRain ? rain.PeriodRain : m_dMaxPeriodRain;
                    }
                    else
                    {
                        m_dMaxPeriodRain = rain.PeriodRain;
                    }

                    // 时间的最大值和最小值
                    if (m_timeMax.HasValue)
                    {
                        m_timeMax = m_timeMax < rain.TimeCollect ? rain.TimeCollect : m_timeMax;
                    }
                    else
                    {
                        m_timeMax = rain.TimeCollect;
                    }
                    if (m_timeMin.HasValue)
                    {
                        m_timeMin = m_timeMin > rain.TimeCollect ? rain.TimeCollect : m_timeMin;
                    }
                    else
                    {
                        m_timeMin = rain.TimeCollect;
                    }

                    //赋值到内部数据表中
                    m_dataTable.Rows.Add(rain.TimeCollect, rain.PeriodRain < 0 ? 0 : rain.PeriodRain);
                }
                if (rains.Count >= 3)
                {
                    // 雨量最大值和最小值
                    if (m_dMaxPeriodRain != m_dMinPeriodRain)
                    {
                        decimal offset = (m_dMaxPeriodRain.Value - m_dMinPeriodRain.Value) * (decimal)0.1;
                        m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxPeriodRain + offset);
                        m_chartAreaDefault.AxisY.Minimum = (double)m_dMinPeriodRain;
                    }
                    else
                    {
                        // 如果相等的话
                        m_chartAreaDefault.AxisY.Minimum = 0;
                        m_chartAreaDefault.AxisY.Maximum = (double)(2 * m_dMaxPeriodRain);
                        if (m_chartAreaDefault.AxisY.Maximum == 0)
                        {
                            m_chartAreaDefault.AxisY.Maximum = 100.0; //人为默认值
                        }
                    }
                    //m_chartAreaDefault.AxisX.Minimum = m_timeMin.Value.ToOADate();
                    //m_chartAreaDefault.AxisX.Maximum = m_timeMax.Value.ToOADate();
                    //m_chartAreaDefault.AxisY.Minimum = (double)(m_dMinPeriodRain - offset);
                    this.DataBind(); //更新数据到图表
                }
            }
            else if (m == 1)
            {
                m_chartAreaDefault.AxisY.Title = "差值雨量(mm)";
                m_dMinDifferenceRain = 0;
                m_dMaxDifferenceRain = 0;
                foreach (CEntityRain rain in rains)
                {
                    // 判断雨量的最大值和最小值
                    if (m_dMinDifferenceRain.HasValue)
                    {
                        m_dMinDifferenceRain = m_dMinDifferenceRain > rain.DifferneceRain ? rain.DifferneceRain : m_dMinDifferenceRain;
                    }
                    else
                    {
                        m_dMinDifferenceRain = rain.DifferneceRain;
                    }
                    if (m_dMaxDifferenceRain.HasValue)
                    {
                        m_dMaxDifferenceRain = m_dMaxDifferenceRain < rain.DifferneceRain ? rain.DifferneceRain : m_dMaxDifferenceRain;
                    }
                    else
                    {
                        m_dMaxDifferenceRain = rain.DifferneceRain;
                    }

                    // 时间的最大值和最小值
                    if (m_timeMax.HasValue)
                    {
                        m_timeMax = m_timeMax < rain.TimeCollect ? rain.TimeCollect : m_timeMax;
                    }
                    else
                    {
                        m_timeMax = rain.TimeCollect;
                    }
                    if (m_timeMin.HasValue)
                    {
                        m_timeMin = m_timeMin > rain.TimeCollect ? rain.TimeCollect : m_timeMin;
                    }
                    else
                    {
                        m_timeMin = rain.TimeCollect;
                    }

                    //赋值到内部数据表中
                    m_dataTable.Rows.Add(rain.TimeCollect, rain.DifferneceRain < 0 ? 0 : rain.DifferneceRain);
                }
                if (rains.Count >= 3)
                {
                    // 雨量最大值和最小值
                    if (m_dMaxDifferenceRain != m_dMinDifferenceRain)
                    {
                        decimal offset = (m_dMaxDifferenceRain.Value - m_dMinDifferenceRain.Value) * (decimal)0.1;
                        m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxDifferenceRain + offset);
                        m_chartAreaDefault.AxisY.Minimum = (double)m_dMinDifferenceRain;
                    }
                    else
                    {
                        // 如果相等的话
                        m_chartAreaDefault.AxisY.Minimum = 0;
                        m_chartAreaDefault.AxisY.Maximum = (double)(2 * m_dMaxDifferenceRain);
                        if (m_chartAreaDefault.AxisY.Maximum == 0)
                        {
                            m_chartAreaDefault.AxisY.Maximum = 100.0; //人为默认值
                        }
                    }
                    //m_chartAreaDefault.AxisX.Minimum = m_timeMin.Value.ToOADate();
                    //m_chartAreaDefault.AxisX.Maximum = m_timeMax.Value.ToOADate();
                    //m_chartAreaDefault.AxisY.Minimum = (double)(m_dMinPeriodRain - offset);
                    this.DataBind(); //更新数据到图表
                }

            }
            else if (m == 2)
            {
                m_chartAreaDefault.AxisY.Title = "日雨量(mm)";
                m_dMinDayRain = 0;
                m_dMaxDayRain = 0;
                foreach (CEntityRain rain in rains)
                {
                    // 判断雨量的最大值和最小值
                    if (m_dMinDayRain.HasValue)
                    {
                        m_dMinDayRain = m_dMinDayRain > rain.DayRain ? rain.DayRain : m_dMinDayRain;
                    }
                    else
                    {
                        m_dMinDayRain = rain.DayRain;
                    }
                    if (m_dMaxDayRain.HasValue)
                    {
                        m_dMaxDayRain = m_dMaxDayRain < rain.DayRain ? rain.DayRain : m_dMaxDayRain;
                    }
                    else
                    {
                        m_dMaxDayRain = rain.DayRain;
                    }

                    // 时间的最大值和最小值
                    if (m_timeMax.HasValue)
                    {
                        m_timeMax = m_timeMax < rain.TimeCollect ? rain.TimeCollect : m_timeMax;
                    }
                    else
                    {
                        m_timeMax = rain.TimeCollect;
                    }
                    if (m_timeMin.HasValue)
                    {
                        m_timeMin = m_timeMin > rain.TimeCollect ? rain.TimeCollect : m_timeMin;
                    }
                    else
                    {
                        m_timeMin = rain.TimeCollect;
                    }

                    //赋值到内部数据表中
                    m_dataTable.Rows.Add(rain.TimeCollect, rain.DayRain < 0 ? 0 : rain.DayRain);
                }
                if (rains.Count >= 3)
                {
                    // 雨量最大值和最小值
                    if (m_dMaxDayRain != m_dMinDayRain)
                    {
                        decimal offset = (m_dMaxDayRain.Value - m_dMinDayRain.Value) * (decimal)0.1;
                        m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxDayRain + offset);
                        m_chartAreaDefault.AxisY.Minimum = (double)m_dMinDayRain;
                    }
                    else
                    {
                        // 如果相等的话
                        m_chartAreaDefault.AxisY.Minimum = 0;
                        m_chartAreaDefault.AxisY.Maximum = (double)(2 * m_dMaxDayRain);
                        if (m_chartAreaDefault.AxisY.Maximum == 0)
                        {
                            m_chartAreaDefault.AxisY.Maximum = 100.0; //人为默认值
                        }
                    }
                    //m_chartAreaDefault.AxisX.Minimum = m_timeMin.Value.ToOADate();
                    //m_chartAreaDefault.AxisX.Maximum = m_timeMax.Value.ToOADate();
                    //m_chartAreaDefault.AxisY.Minimum = (double)(m_dMinPeriodRain - offset);
                    this.DataBind(); //更新数据到图表
                }
            }

        }

        //m=0,时段雨量；m=1,差值雨量；m=2,日雨量
        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd, int m, bool TimeSelect)
        {
            m_annotation.Visible = false;
            base.ClearAllDatas();
            m_proxyRain.SetFilter(iStationId, timeStart, timeEnd, TimeSelect);
            if (-1 == m_proxyRain.GetPageCount())
            {
                // 查询失败
                // MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                return false;
            }
            else
            {
                int rowcount = m_proxyRain.GetRowCount();
                if (rowcount > CI_Chart_Max_Count)
                {
                    // 数据量太大，退出绘图
                    MessageBox.Show("查询结果集太大，自动退出绘图");
                    return false;
                }
                // 并查询数据，显示第一页
                int iTotalPage = m_proxyRain.GetPageCount();
                for (int i = 0; i < iTotalPage; ++i)
                {
                    // 查询所有的数据
                    this.AddRains(m_proxyRain.GetPageData(i + 1), m);
                }
                return true;
            }
        }

        public void InitDataSource(IRainProxy proxy)
        {
            m_proxyRain = proxy;
        }

        #region 重载

        // 重写UI,设置XY轴名字
        protected override void InitUI()
        {
            base.InitUI();
            // 设置图表标题
            m_title.Text = CS_Chart_Name;

            // 设置雨量的格式
            m_chartAreaDefault.AxisY.LabelStyle.Format = "0.0"; //保留一位小数

            // m_chartAreaDefault.AxisX.Title = CS_Asix_DateTime; //不显示名字
            m_chartAreaDefault.AxisY.Title = CS_AsixY_Name;

            m_serialRain = this.Series.Add(CS_Serial_Name_Volatege);
            m_serialRain.Name = "雨量"; //用来显示图例的
            m_serialRain.ChartArea = CS_ChartAreaName_Default;
            m_serialRain.ChartType = SeriesChartType.Column; //如果点数过多， 画图很慢，初步测试不能超过2000个
            m_serialRain.BorderWidth = 0;
            //m_serialRain.BorderColor = Color.FromArgb(120, 147, 190);
            //m_serialRain.ShadowColor = Color.FromArgb(64, 0, 0, 0);
            //m_serialRain.ShadowOffset = 2;
            //  设置时间类型,对于serial来说
            m_serialRain.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            m_serialRain.IsXValueIndexed = true; // 自己计算X值，以及边界值,否则翻译不出正确的值

            //  绑定数据
            m_serialRain.XValueMember = CS_CN_DateTime;
            m_serialRain.YValueMembers = CS_CN_PeriodRain;
            m_chartAreaDefault.AxisX.TextOrientation = TextOrientation.Horizontal;
            m_chartAreaDefault.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //m_chartAreaDefault.AxisX.a
            m_chartAreaDefault.AxisX.LabelStyle.Format = "MM-dd HH";
            m_chartAreaDefault.AxisX.LabelStyle.Angle = 90;
            //m_chartAreaDefault.AxisX.Interval = 2;   //设置X轴坐标的间隔为1
            //m_chartAreaDefault.AxisX.IntervalOffset = 2;  //设置X轴坐标偏移为1
        }

        // 显示提示，并重新定位,xPosition有效
        protected override void UpdateAnnotationByDataPoint(DataPoint point)
        {
            if (null == point)
            {
                Debug.WriteLine("CChartRain UpdateAnnotationByDataPoint Failed");
                return;
            }
            DateTime dateTimeX = DateTime.FromOADate(point.XValue);
            String prompt = string.Format("雨量：{0:0.00}\n日期：{1}\n时间：{2}", point.YValues[0],
                        dateTimeX.ToString("yyyy-MM-dd"),
                        dateTimeX.ToString("HH:mm:ss"));
            m_chartAreaDefault.CursorY.Position = point.YValues[0]; // 重新设置Y的值
            m_chartAreaDefault.CursorX.Position = point.XValue; //重新设置X的值

            // 显示注释
            m_annotation.Text = prompt;
            //m_annotation.X = point.XValue;
            //m_annotation.Y = point.YValues[0];
            m_annotation.AnchorDataPoint = point;
            m_annotation.Visible = true;
        }

        #endregion 重载
    } // class CChartRain
}
