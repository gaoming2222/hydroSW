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
using System.Data;

namespace Hydrology.CControls
{
    /// <summary>
    /// 含水率图形
    /// </summary>
    public class CChartSoilData : CExChart
    {
        #region 静态常量
        // 数据源中的列名
        public static readonly string CS_CN_M10 = "M10";
        public static readonly string CS_CN_M20 = "M20";
        public static readonly string CS_CN_M30 = "M30";
        public static readonly string CS_CN_M40 = "M40";
        public static readonly string CS_CN_M60 = "M60";
        public static readonly string CS_CN_Voltage = "V0";

        // 含水率的坐标名字
        public static readonly string CS_AsixY_Name = "含水率";
        public static readonly string CS_AsixY2_name = "电压";

        // 图表名字
        public static readonly string CS_Chart_Name = "土壤含水率过程线";

        // 线条名字
        public static readonly string CS_Serial_Name_M10 = "10CM含水率";
        public static readonly string CS_Serial_Name_M20 = "20CM含水率";
        public static readonly string CS_Serial_Name_M30 = "30CM含水率";
        public static readonly string CS_Serial_Name_M40 = "40CM含水率";
        public static readonly string CS_Serial_Name_M60 = "60CM含水率";
        public static readonly string CS_Serial_Name_Votage = "电压";

        #endregion 静态常量

        private Nullable<float> m_dMinMoistrue; //最小的含水率
        private Nullable<float> m_dMaxMoistrue; //最大的含水率

        private Nullable<decimal> m_dMinVoltage; //最小电压
        private Nullable<decimal> m_dMaxVoltage; //最大电压

        private Nullable<DateTime> m_maxDateTime;   //最大的日期
        private Nullable<DateTime> m_minDateTime;   //最小的日期

        //private Nullable<float> m_maxVoltage;  //最大电压值
        //private Nullable<float> m_minVoltage; // 最小电压值

        // 线条对象
        private Series m_serialM10;
        private Series m_serialM20;
        //  private Series m_serialM30;
        private Series m_serialM40;
        private Series m_serialVoltage;
        //  private Series m_serialM60;

        private Legend m_legend;     //图例

        // 右键菜单
        private MenuItem m_MI_M10;
        private MenuItem m_MI_M20;
        //    private MenuItem m_MI_M30;
        private MenuItem m_MI_M40;
        private MenuItem m_MI_Voltage;
        //    private MenuItem m_MI_M60;

        /// <summary>
        /// 数据库的代理，用于查询数据
        /// </summary>
        private ISoilDataProxy m_proxySoilData;

        //public DataTable dataTable1;
        //public DataTable dataTable2;
        //public DataTable dataTable3;
        //public DataTable dataTable4;
        public CChartSoilData()
            : base()
        {
            // 设定数据表的列
            base.m_dataTable.Columns.Add(CS_CN_DateTime, typeof(DateTime));
            base.m_dataTable.Columns.Add(CS_CN_M10, typeof(float));
            base.m_dataTable.Columns.Add(CS_CN_M20, typeof(float));
            //     base.m_dataTable.Columns.Add(CS_CN_M30, typeof(float));
            base.m_dataTable.Columns.Add(CS_CN_M40, typeof(float));
            base.m_dataTable.Columns.Add(CS_CN_Voltage, typeof(float));
            //dataTable1 = new System.Data.DataTable();
            //dataTable1.Columns.Add(CS_Asix_DateTime, typeof(DateTime));
            //dataTable1.Columns.Add(CS_Serial_Name_Votage, typeof(float));

            //dataTable2 = new System.Data.DataTable();
            //dataTable2.Columns.Add(CS_Asix_DateTime, typeof(DateTime));
            //dataTable2.Columns.Add(CS_Serial_Name_M10, typeof(float));

            //      base.m_dataTable.Columns.Add(CS_CN_M60, typeof(float));
        }
        // 外部添加水位流量接口
        public void AddSoilData(List<CEntitySoilData> soilDatas)
        {
            try{

            #region 含水量最大最小值
                foreach (CEntitySoilData entity in soilDatas)
                {
                    // 判断水位最大值和最小值
                    float maxValue = 0;
                    float minValue = 0;
                    List<Nullable<float>> listMoistrues = new List<Nullable<float>>();
                    if ((entity.Moisture10.HasValue) && (entity.Moisture20.HasValue) && (entity.Moisture40.HasValue))
                    {
                        listMoistrues.Add(entity.Moisture10);
                        listMoistrues.Add(entity.Moisture20);
                        //listMoistrues.Add(entity.Moisture30);
                        listMoistrues.Add(entity.Moisture40);
                        //    listMoistrues.Add(entity.Moisture60);
                    }
                    bool bHasMaxValue = GetMaxValue(listMoistrues, ref maxValue);
                    bool bHasMinValue = GetMinValue(listMoistrues, ref minValue);
                    if (bHasMaxValue)
                    {
                        // 此次有最大值
                        if (m_dMaxMoistrue.HasValue)
                        {
                            m_dMaxMoistrue = maxValue > m_dMaxMoistrue.Value ? maxValue : m_dMaxMoistrue;
                        }
                        else
                        {
                            m_dMaxMoistrue = maxValue;
                        }
                    }
                    if (bHasMinValue)
                    {
                        // 此次有最小值
                        if (m_dMinMoistrue.HasValue)
                        {
                            m_dMinMoistrue = minValue < m_dMinMoistrue ? minValue : m_dMinMoistrue;
                        }
                        else
                        {
                            m_dMinMoistrue = minValue;
                        }
                    }
            #endregion
                    #region 电压最大最小值
                    // 判断水位最大值和最小值
                    float maxValue_1 = 0;
                    float minValue_1 = 0;
                    List<Nullable<float>> listVoltage = new List<Nullable<float>>();
                    if (entity.DVoltage.ToString() != null)
                    {
                        listVoltage.Add(float.Parse(entity.DVoltage.ToString()));
                    }
                    bool bHasMaxValue_1 = GetMaxValue(listVoltage, ref maxValue_1);
                    bool bHasMinValue_1 = GetMinValue(listVoltage, ref minValue_1);
                    if (bHasMaxValue_1)
                    {
                        // 此次有最大值
                        if (m_dMaxVoltage.HasValue)
                        {
                            if (maxValue_1 > m_dMaxMoistrue.Value)
                            {
                                m_dMaxVoltage = decimal.Parse(maxValue_1.ToString());
                            }
                            else
                            {
                                m_dMaxVoltage = decimal.Parse(maxValue_1.ToString());
                            }
                        }
                       
                    }
                    if (bHasMinValue_1)
                    {
                        // 此次有最小值
                        if (m_dMinVoltage.HasValue)
                        {
                            if (minValue_1 < m_dMinMoistrue)
                            {
                                m_dMinVoltage = decimal.Parse(minValue_1.ToString());
                            }
                            else
                            {
                                m_dMinVoltage = decimal.Parse(minValue_1.ToString());
                            }
                        }
                        
                    #endregion
                        #region 日期最大最小值

                        // 判断日期, 更新日期最大值和最小值
                        if (m_maxDateTime.HasValue)
                        {
                            m_maxDateTime = m_maxDateTime < entity.DataTime ? entity.DataTime : m_maxDateTime;
                        }
                        else
                        {
                            m_maxDateTime = entity.DataTime;
                        }
                        if (m_minDateTime.HasValue)
                        {
                            m_minDateTime = m_minDateTime > entity.DataTime ? entity.DataTime : m_minDateTime;
                        }
                        else
                        {
                            m_minDateTime = entity.DataTime;
                        }
                        #endregion 日期最大最小值
                        if ((entity.Moisture10.HasValue) && (entity.Moisture20.HasValue) && (entity.Moisture40.HasValue))
                        {
                            if (entity.DVoltage.ToString() != "")
                            {
                                try
                                {
                                    m_dataTable.Rows.Add(entity.DataTime, entity.Moisture10, entity.Moisture20, entity.Moisture40, entity.DVoltage);
                                }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
                                catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
                                {

                                }
                            }

                        }
                        //if(entity.DVoltage.ToString() != "")
                        //{
                        //    try
                        //    {
                        //        dataTable1.Rows.Add(entity.DataTime, entity.DVoltage);
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //    }
                        // }
                        //if(entity.Moisture10.HasValue){
                        //    try
                        //    {
                        //        dataTable2.Rows.Add(entity.DataTime, entity.Moisture10);
                        //     }
                        //    catch (Exception ex)
                        //    {

                        //    }
                        //}
                    }// end of foraech

                    if (soilDatas.Count >= 3)
                    {
                        // 水位和流量最大值和最小值
                        float offset = 0;
                        if (m_dMaxMoistrue != m_dMinMoistrue)
                        {
                            offset = (m_dMaxMoistrue.Value - m_dMinMoistrue.Value) * (float)0.1;
                        }
                        else
                        {
                            // 如果相等的话
                            offset = (float)m_dMaxMoistrue / 2;
                        }
                        m_chartAreaDefault.AxisY.Maximum = (double)(m_dMaxMoistrue + offset);
                        m_chartAreaDefault.AxisY.Minimum = (double)(m_dMinMoistrue - offset);
                        if (offset == 0)
                        {
                            // 人为赋值
                            m_chartAreaDefault.AxisY.Maximum = m_chartAreaDefault.AxisY.Minimum + 10;
                        }

                        if (m_dMaxVoltage.HasValue && m_dMinVoltage.HasValue)
                        {
                            if (m_dMaxVoltage != m_dMinVoltage)
                            {
                                offset = (float)(m_dMaxVoltage - m_dMinVoltage) * 0.1f;
                            }
                            else
                            {
                                offset = (float)m_dMaxVoltage / 2;
                            }
                            m_chartAreaDefault.AxisY2.Maximum = (double)((float)m_dMaxVoltage + offset);
                            m_chartAreaDefault.AxisY2.Minimum = (double)((float)m_dMinVoltage - offset);
                            if (m_chartAreaDefault.AxisY2.Maximum <= m_chartAreaDefault.AxisY2.Minimum)
                            {
                                double a = m_chartAreaDefault.AxisY2.Minimum;
                                m_chartAreaDefault.AxisY2.Minimum =  m_chartAreaDefault.AxisY2.Maximum;
                                m_chartAreaDefault.AxisY2.Maximum = a;
                            }

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
            }
                catch(Exception e){
                    Debug.WriteLine("gm" + e.Message);
                }

            }

            //this.Series.Clear();
            //Series dataTable1Series = new Series("电压值");
            //Series dataTable2Series = new Series("10CM含水率");
            //Series dataTable3Series = new Series("20CM含水率");
            //Series dataTable4Series = new Series("40CM含水率");
            //dataTable1Series.Points.DataBind(dataTable1.AsEnumerable(), CS_Asix_DateTime, CS_Serial_Name_Votage, "");
            //dataTable1Series.XValueType = ChartValueType.DateTime;//设置X轴类型为时间
            //dataTable1Series.ChartType = SeriesChartType.Line;  //设置Y轴为折线

            //dataTable2Series.Points.DataBind(dataTable2.AsEnumerable(), CS_Asix_DateTime, CS_Serial_Name_M10, "");
            //dataTable2Series.XValueType = ChartValueType.DateTime;//设置X轴类型为时间
            //dataTable2Series.ChartType = SeriesChartType.Line;  //设置Y轴为折线
            //this.Series.Add(dataTable1Series);
           // this.Series.Add(dataTable2Series);
        


        public bool SetFilter(string iStationId, DateTime timeStart, DateTime timeEnd)
        {
            m_annotation.Visible = false;
            ClearAllDatas();
            List<CEntitySoilData> listDatas = m_proxySoilData.QueryByStationAndTime(iStationId, timeStart, timeEnd);
            int rowcount = listDatas.Count;
            if (rowcount > CI_Chart_Max_Count)
            {
                // 数据量太大，退出绘图
                MessageBox.Show("查询结果集太大，自动退出绘图");
                return false;
            }
            AddSoilData(listDatas); //显示图形
            return true;
        }

        public void InitDataSource(ISoilDataProxy proxy)
        {
            m_proxySoilData = proxy;
        }


        #region 重载

        // 重新右键菜单
        protected override void InitContextMenu()
        {
            base.InitContextMenu();
            m_MI_M10 = new MenuItem() { Text = "10CM含水率", Checked = true };
            m_MI_M20 = new MenuItem() { Text = "20CM含水率", Checked = true };
            // m_MI_M30 = new MenuItem() { Text = "30CM含水率", Checked = true };
            m_MI_M40 = new MenuItem() { Text = "40CM含水率", Checked = true };
            m_MI_Voltage = new MenuItem() { Text = "电压值", Checked = true };
            // m_MI_M60 = new MenuItem() { Text = "60CM含水率", Checked = true };
            base.m_contextMenu.MenuItems.Add(0, m_MI_M10);
            base.m_contextMenu.MenuItems.Add(0, m_MI_M20);
            //  base.m_contextMenu.MenuItems.Add(0, m_MI_M30);
            base.m_contextMenu.MenuItems.Add(0, m_MI_M40);
            base.m_contextMenu.MenuItems.Add(0, m_MI_Voltage);
            //  base.m_contextMenu.MenuItems.Add(0, m_MI_M60);

            m_MI_M10.Click += new EventHandler(EH_MI_M10_Click);
            m_MI_M20.Click += new EventHandler(EH_MI_M20_Click);
            //   m_MI_M30.Click += new EventHandler(EH_MI_M30_Click);
            m_MI_M40.Click += new EventHandler(EH_MI_M40_Click);
            m_MI_Voltage.Click += new EventHandler(EH_MI_Voltage_Click);
            //  m_MI_M60.Click += new EventHandler(EH_MI_M60_Click);
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
            m_chartAreaDefault.AxisY2.Title = CS_AsixY2_name;

            m_serialM10 = AddSerial(CS_Serial_Name_M10);
            m_serialM10.YValueMembers = CS_CN_M10;
            m_serialM10.Color = Color.Red;

            m_serialM20 = AddSerial(CS_Serial_Name_M20);
            m_serialM20.YValueMembers = CS_CN_M20;
            m_serialM20.Color = Color.Blue;

            //m_serialM30 = AddSerial(CS_Serial_Name_M30);
            //m_serialM30.YValueMembers = CS_CN_M30;
            //m_serialM30.Color = Color.Green;

            m_serialM40 = AddSerial(CS_Serial_Name_M40);
            m_serialM40.YValueMembers = CS_CN_M40;
            m_serialM40.Color = Color.FromArgb(205, 92, 197);

            m_serialVoltage = AddSerial(CS_Serial_Name_Votage);
            m_serialVoltage.Color = Color.Green;
            //m_serialVoltage.XValueMember = CS_CN_DateTime;
            m_serialVoltage.YValueMembers = CS_CN_Voltage;
            m_serialVoltage.YAxisType = AxisType.Secondary;
            //m_serialM60 = AddSerial(CS_Serial_Name_M60);
            //m_serialM60.YValueMembers = CS_CN_M60;
            //m_serialM60.Color = Color.LightSkyBlue;
            m_chartAreaDefault.AxisX.TextOrientation = TextOrientation.Horizontal;
            m_chartAreaDefault.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            //m_chartAreaDefault.AxisX.a
            m_chartAreaDefault.AxisX.LabelStyle.Format = "MM-dd";
            m_chartAreaDefault.AxisX.LabelStyle.Angle = 90;

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
            //if (m_serialM10.Points.Contains(point))
            //{
            if (m_serialVoltage.Points.Contains(point))
            {
                prompt = string.Format("电压：{0:0.00}\n日期：{1}\n时间：{2}", point.YValues[0],
                        dateTimeX.ToString("yyyy-MM-dd"),
                        dateTimeX.ToString("HH:mm:ss"));
            }
            else
            {
                prompt = string.Format("含水率：{0:0.00}\n日期：{1}\n时间：{2}", point.YValues[0],
                        dateTimeX.ToString("yyyy-MM-dd"),
                        dateTimeX.ToString("HH:mm:ss"));
            }

            m_chartAreaDefault.CursorY.Position = point.YValues[0]; // 重新设置Y的值
            m_chartAreaDefault.CursorX.Position = point.XValue;     //重新设置X的值

            // 显示注释
            m_annotation.Text = prompt;
            // 不设置位置，此时会自动计算
            //m_annotation.X = point.XValue;
            //m_annotation.Y = point.YValues[0];
            m_annotation.AnchorDataPoint = point;
            m_annotation.Visible = true;
        }

        // 重载清空所有数据
        protected override void ClearAllDatas()
        {
            base.ClearAllDatas();
            m_maxDateTime = null;
            m_minDateTime = null;
            m_dMinMoistrue = null;
            m_dMaxMoistrue = null;
        }

        #endregion 重载

        #region 事件响应
        private void EH_MI_M40_Click(object sender, EventArgs e)
        {
            m_MI_M40.Checked = !m_MI_M40.Checked;
            m_serialM40.Enabled = m_MI_M40.Checked;
        }
        private void EH_MI_Voltage_Click(object sender, EventArgs e)
        {
            m_MI_Voltage.Checked = !m_MI_Voltage.Checked;
            m_serialVoltage.Enabled = m_MI_Voltage.Checked;
        }
        private void EH_MI_M20_Click(object sender, EventArgs e)
        {
            m_MI_M20.Checked = !m_MI_M20.Checked;
            m_serialM20.Enabled = m_MI_M20.Checked;
        }

        private void EH_MI_M10_Click(object sender, EventArgs e)
        {
            m_MI_M10.Checked = !m_MI_M10.Checked;
            m_serialM10.Enabled = m_MI_M10.Checked;
        }
        #endregion 事件响应

        #region 帮助方法
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="listValue"></param>
        /// <returns></returns>
        private bool GetMaxValue(List<Nullable<float>> listValue, ref float refMaxValue)
        {
            bool bHasMax = false;
            if (listValue.Count > 0)
            {
                Nullable<float> maxValue = null;
                foreach (Nullable<float> value in listValue)
                {
                    if (value.HasValue)
                    {
                        bHasMax = true;
                        if (maxValue.HasValue)
                        {
                            // 判断最大值
                            maxValue = value > maxValue ? value : maxValue;
                        }
                        else
                        {
                            // 保存第一个值，便于后续比较大小
                            maxValue = value;
                        }
                    }
                }//end of foeach
                if (maxValue.HasValue)
                {
                    refMaxValue = maxValue.Value;
                }
            }
            return bHasMax;
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="listValue"></param>
        /// <returns></returns>
        private bool GetMinValue(List<Nullable<float>> listValue, ref float refMinValue)
        {
            bool bHasMin = false;
            if (listValue.Count > 0)
            {
                Nullable<float> minValue = null;
                foreach (Nullable<float> value in listValue)
                {
                    if (value.HasValue)
                    {
                        bHasMin = true;
                        if (minValue.HasValue)
                        {
                            // 判断最大值
                            minValue = value < minValue ? value : minValue;
                        }
                        else
                        {
                            // 保存第一个值，便于后续比较大小
                            minValue = value;
                        }
                    }
                }//end of foeach
                if (minValue.HasValue)
                {
                    refMinValue = minValue.Value;
                }
            }
            return bHasMin;
        }

        /// <summary>
        /// 添加一条线
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Series AddSerial(string name)
        {
            Series result = this.Series.Add(name);
            result.Name = name; //用来显示图例的
            result.ChartArea = CS_ChartAreaName_Default;
            result.ChartType = SeriesChartType.Line; //如果点数过多， 画图很慢，初步测试不能超过2000个
            result.BorderWidth = 1;
            //result.Color = Color.FromArgb(22,99,1);
            result.Color = Color.Red; // 设置线条颜色
            //result.BorderColor = Color.FromArgb(120, 147, 190);
            //result.ShadowColor = Color.FromArgb(64, 0, 0, 0);
            //result.ShadowOffset = 2;
            //  设置时间类型,对于serial来说
            result.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            result.IsXValueIndexed = false; // 自己计算X值，以及边界值,否则翻译不出正确的值

            //  绑定数据
            result.XValueMember = CS_CN_DateTime;
            // result.YValueMembers = CS_CN_Water;

            result.YAxisType = AxisType.Primary;
            return result;
        }
        #endregion 帮助方法
    }
}
