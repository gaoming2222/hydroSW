using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.Diagnostics;
using Hydrology.DBManager.Interface;
using System.Windows.Forms;

namespace Hydrology.CControls
{
    /// <summary>
    /// 电压的图表控件，继承自自定义的图标控件
    /// </summary>
    public class CChartVoltage : CExChart
    {
        #region 静态常量
        // 数据源中电压的列名
        public static readonly string CS_CN_Voltage = "voltage";

        // 电压的坐标名字
        public static readonly string CS_AsixY_Voltage = "电压";

        // 图表名字
        public static readonly string CS_Chart_Name = "电压过程线";

        // 线条名字
        public static readonly string CS_Serial_Name_Volatege = "Serial_Voltage";
        #endregion 静态常量

        private Nullable<decimal> m_dMinVoltage; //最小的时间值
        private Nullable<decimal> m_dMaxVoltage; //最大的时间值

        private Nullable<DateTime> m_maxDateTime;   //最大的日期
        private Nullable<DateTime> m_minDateTime;   //最小的日期

        private Series m_serialVoltage;         //电压过程线

        private IVoltageProxy m_proxyVoltage;

        public CChartVoltage()
            : base()
        {
            // 设定数据表的列
            base.m_dataTable.Columns.Add(CS_CN_DateTime, typeof(DateTime));
            base.m_dataTable.Columns.Add(CS_CN_Voltage, typeof(Decimal));
        }

        // 外部添加电压接口
        public bool AddVoltages(List<CEntityVoltage> voltages)
        {
            bool result = true;
            m_dMinVoltage = null;
            m_dMaxVoltage = null;
            foreach (CEntityVoltage voltage in voltages)
            {
                if (voltage.Voltage >= 5 && voltage.Voltage <= 18)
                {
                }
                else
                {
                    result = false;
                }
                // 判断电压最大值和最小值
                if (m_dMinVoltage.HasValue)
                {
                    m_dMinVoltage = m_dMinVoltage > voltage.Voltage ? voltage.Voltage : m_dMinVoltage;
                }
                else
                {
                    m_dMinVoltage = voltage.Voltage;
                }
                if (m_dMaxVoltage.HasValue)
                {
                    m_dMaxVoltage = m_dMaxVoltage < voltage.Voltage ? voltage.Voltage : m_dMaxVoltage;
                }
                else
                {
                    m_dMaxVoltage = voltage.Voltage;
                }
                // 判断日期, 更新日期最大值和最小值
                if (m_maxDateTime.HasValue)
                {
                    m_maxDateTime = m_maxDateTime < voltage.TimeCollect ? voltage.TimeCollect : m_maxDateTime;
                }
                else
                {
                    m_maxDateTime = voltage.TimeCollect;
                }
                if (m_minDateTime.HasValue)
                {
                    m_minDateTime = m_minDateTime > voltage.TimeCollect ? voltage.TimeCollect : m_minDateTime;
                }
                else
                {
                    m_minDateTime = voltage.TimeCollect;
                }

                //赋值到内部数据表中
                m_dataTable.Rows.Add(voltage.TimeCollect, voltage.Voltage);
            }
            if (voltages.Count >= 3)
            {
                //更新最大值和最小值
                m_chartAreaDefault.AxisX.Maximum = m_maxDateTime.Value.ToOADate();
                m_chartAreaDefault.AxisX.Minimum = m_minDateTime.Value.ToOADate();
                //电压最大值和最小值
                m_dMaxVoltage = m_dMaxVoltage == null ? 18 : m_dMaxVoltage;
                m_dMinVoltage = m_dMinVoltage == null ? 5 : m_dMinVoltage;
                //if (m_dMaxVoltage == m_dMinVoltage)
                //{
                //    double offset = (double)m_dMaxVoltage * (double)0.1;
                //    if (offset == 0)
                //    {
                //        // 如果最大值和最小值没偏差，而且都为0
                //        // 人为赋值
                //        m_chartAreaDefault.AxisY.Maximum = 1;
                //        m_chartAreaDefault.AxisY.Minimum = -1;
                //    }
                //    else
                //    {
                //        m_chartAreaDefault.AxisY.Maximum = (double)((double)m_dMaxVoltage + offset);
                //        m_chartAreaDefault.AxisY.Minimum = (double)((double)m_dMaxVoltage - offset);
                //    }
                //}
                //else
                //{
                //    decimal offset = (m_dMaxVoltage.Value - m_dMinVoltage.Value) * (decimal)0.1;
                //    m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxVoltage + offset);
                //    m_chartAreaDefault.AxisY.Minimum = (double)((m_dMinVoltage - offset) < 0 ? 5 : (m_dMinVoltage - offset));
                //}
                m_chartAreaDefault.AxisY.Maximum = 18;
                m_chartAreaDefault.AxisY.Minimum = 5;

                m_chartAreaDefault.AxisX.Minimum = m_minDateTime.Value.ToOADate();
                m_chartAreaDefault.AxisX.Maximum = m_maxDateTime.Value.ToOADate();

                this.DataBind(); //更新数据到图表
            }
            return result;

        }

        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect)
        {
            bool result = false;
            m_annotation.Visible = false;
            base.ClearAllDatas();
            m_proxyVoltage.SetFilter(iStationId, timeStart, timeEnd, TimeSelect);
            if (-1 == m_proxyVoltage.GetPageCount())
            {
                // 查询失败
                // MessageBox.Show("数据库忙，查询失败，请稍后再试！");
                return result;
            }
            else
            {
                int rowcount = m_proxyVoltage.GetRowCount();
                if (rowcount > CI_Chart_Max_Count)
                {
                    // 数据量太大，退出绘图
                    MessageBox.Show("查询结果集太大，自动退出绘图");
                    return result;
                }
                // 并查询数据，显示第一页
                int iTotalPage = m_proxyVoltage.GetPageCount();
                for (int i = 0; i < iTotalPage; ++i)
                {
                    // 查询所有的数据
                    result = this.AddVoltages(m_proxyVoltage.GetPageData(i + 1));
                    if (result == false)
                    {
                        MessageBox.Show("数据超限，自动退出绘图");
                    }
                }
                return result;
            }
        }

        public void InitDataSource(IVoltageProxy proxy)
        {
            m_proxyVoltage = proxy;
        }

        #region 重载

        // 重写UI,设置XY轴名字
        protected override void InitUI()
        {
            base.InitUI();
            // 设置图表标题
            m_title.Text = CS_Chart_Name;

            // 设置电压的格式
            m_chartAreaDefault.AxisY.LabelStyle.Format = "0.00";

            // m_chartAreaDefault.AxisX.Title = CS_Asix_DateTime; //不显示名字
            m_chartAreaDefault.AxisY.Title = CS_AsixY_Voltage;
            m_chartAreaDefault.AxisY.IsStartedFromZero = true;

            m_serialVoltage = this.Series.Add(CS_Serial_Name_Volatege);
            m_serialVoltage.Name = "电压"; //用来显示图例的
            m_serialVoltage.ChartArea = CS_ChartAreaName_Default;
            m_serialVoltage.ChartType = SeriesChartType.Line; //如果点数过多， 画图很慢，初步测试不能超过2000个
            m_serialVoltage.BorderWidth = 1;
            //m_serialVoltage.Color = Color.FromArgb(22, 99, 1);
            m_serialVoltage.Color = Color.Red;
            //m_serialVoltage.BorderColor = Color.FromArgb(120, 147, 190);
            //m_serialVoltage.ShadowColor = Color.FromArgb(64, 0, 0, 0);
            //m_serialVoltage.ShadowOffset = 2;
            //  设置时间类型,对于serial来说
            m_serialVoltage.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            m_serialVoltage.IsXValueIndexed = false; // 自己计算X值，以及边界值,否则翻译不出正确的值

            //  绑定数据
            m_serialVoltage.XValueMember = CS_CN_DateTime;
            m_serialVoltage.YValueMembers = CS_CN_Voltage;

            m_chartAreaDefault.AxisX.TextOrientation = TextOrientation.Horizontal;
            m_chartAreaDefault.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //m_chartAreaDefault.AxisX.a
            m_chartAreaDefault.AxisX.LabelStyle.Format = "MM-dd HH";
            m_chartAreaDefault.AxisX.LabelStyle.Angle = 90;
        }

        // 显示提示，并重新定位,xPosition有效
        protected override void UpdateAnnotationByDataPoint(DataPoint point)
        {
            if (null == point)
            {
                Debug.WriteLine("CChartVoltage UpdateAnnotationByXValue Failed");
                return;
            }
            DateTime dateTimeX = DateTime.FromOADate(point.XValue);
            String prompt = string.Format("电压：{0}\n日期：{1}\n时间：{2}", point.YValues[0],
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
    }
}
