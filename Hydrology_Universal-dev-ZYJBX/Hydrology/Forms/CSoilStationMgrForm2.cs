using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrology.Entity;
using Hydrology.DataMgr;
using Hydrology.DBManager.Interface;
using Hydrology.Utils;
using Protocol.Manager;

namespace Hydrology.Forms
{
    //定义一个需要string类型参数的委托
    public delegate void MyDelegateSoil(CEntitySoilStation soilstation);
    public partial class CSoilStationMgrForm2 : Form
    {
        List<CEntitySubCenter> m_listSubCenter;

        public List<CEntitySoilStation> m_listSoilStationCombination;    //根据增加和修改以及删除，拼接的内存的数据表
        List<CEntityStation> m_listStation;             //数据库中水情站点列表
        List<CEntitySoilStation> m_listSoilStation;             //数据库中墒情站点列表
        public List<CEntitySoilStation> m_listSoilStationAdded;     //新增的站点列表
        /// <summary>
        /// 所有数据协议列表
        /// </summary>
        private List<string> m_listProtocolData;
        private XmlDllCollections m_dllCollections;

        //定义该委托的事件
        public event MyDelegateSoil AddSoilStationEvent;
        public CSoilStationMgrForm2()
        {
            try
            {
                InitializeComponent();
                InitDataSource();
                InitUI();
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e) { }
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitDataSource()
        {
            try
            {
                m_listSubCenter = CDBDataMgr.Instance.GetAllSubCenter();
                m_listStation = CDBDataMgr.GetInstance().GetAllStation();
                m_listSoilStation = CDBSoilDataMgr.GetInstance().GetAllSoilStation();
                m_listSoilStationCombination = new List<CEntitySoilStation>(m_listSoilStation.ToArray()); //可以的啦
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        public void InitUI()
        {
            try
            {
                //// 初始化分中心
                for (int i = 0; i < m_listSubCenter.Count; ++i)
                {
                    cmb_SubCenter.Items.Add(m_listSubCenter[i].SubCenterName);
                }
                cmb_SubCenter.SelectedIndex = 0;

                //ESoil = 3,         //  04墒情站
                //ESoilRain = 4,     //  05墒情雨量站
                //ESoilWater = 5,     //  06，16墒情水位站
                //ESoilHydrology = 6  //  07，17墒情水文站
                // 初始化站点类型
                cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoil));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilRain));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilWater));
                //cmb_StationType.Items.Add(CEnumHelper.StationTypeToUIStr(EStationType.ESoilHydrology));
                cmb_StationType.SelectedIndex = 0;

                textBox_StationID.Text = "0001";
                textBox_StationName.Text = "测站1";
                //textBox_deviceNumber.Text = "21208890";
                textBox_Voltage.Text = "11";

                textBox_10a.Text = "1.0000";
                textBox_10b.Text = "2.0000";
                textBox_10c.Text = "3.0000";
                textBox_10d.Text = "4.0000";
                textBox_10m.Text = "1.0000";
                textBox_10n.Text = "2.0000";

                textBox_20a.Text = "1.0000";
                textBox_20b.Text = "2.0000";
                textBox_20c.Text = "3.0000";
                textBox_20d.Text = "4.0000";
                textBox_20m.Text = "1.0000";
                textBox_20n.Text = "2.0000";

                textBox_30a.Text = "1.0000";
                textBox_30b.Text = "2.0000";
                textBox_30c.Text = "3.0000";
                textBox_30d.Text = "4.0000";
                textBox_30m.Text = "1.0000";
                textBox_30n.Text = "2.0000";

                textBox_40a.Text = "1.0000";
                textBox_40b.Text = "2.0000";
                textBox_40c.Text = "3.0000";
                textBox_40d.Text = "4.0000";
                textBox_40m.Text = "1.0000";
                textBox_40n.Text = "2.0000";

                textBox_60a.Text = "1.0000";
                textBox_60b.Text = "2.0000";
                textBox_60c.Text = "3.0000";
                textBox_60d.Text = "4.0000";
                textBox_60m.Text = "1.0000";
                textBox_60n.Text = "2.0000";

                //自报段次
                comb_Paragraph.Items.Add("1");
                comb_Paragraph.Items.Add("4");
                comb_Paragraph.Items.Add("8");
                comb_Paragraph.Items.Add("12");
                comb_Paragraph.Items.Add("24");
                comb_Paragraph.Items.Add("48");
                comb_Paragraph.SelectedIndex = 4;

                //通讯参数初始化
                textBox_GSM.Text = "";
                textBox_GSM.Enabled = false;

                textBox_GPRS.Text = "";
                textBox_GPRS.Enabled = false;

                textBox_Beidou.Text = "";
                textBox_Beidou.Enabled = false;

                textBox_BeidouMember.Text = "";
                textBox_BeidouMember.Enabled = false;

                m_listProtocolData = XmlDocManager.Instance.DataProtocolNames;
                for (int i = 0; i < m_listProtocolData.Count; i++)
                {
                    comb_DataProtocol.Items.Add(m_listProtocolData[i]);
                }
                comb_DataProtocol.SelectedIndex = 0;

                XmlDocManager.Instance.ReadFromXml();
                m_dllCollections = Protocol.Manager.XmlDocManager.Instance.DllInfo;
                foreach (XmlDllInfo info in m_dllCollections.Infos)
                {
                    //  不显示已经被禁用的协议
                    if (!info.Enabled)
                        continue;
                    // 显示信道协议
                    if (info.Type == "channel")
                    {
                        //   m_mapChannelInfo.Add(info.Name, info);
                        comb_MainRoad.Items.Add(info.Name);
                        comb_PrepareRoad.Items.Add(info.Name);
                    }

                }
                comb_PrepareRoad.Items.Add("无");
                comb_MainRoad.SelectedIndex = 0;
                comb_PrepareRoad.SelectedIndex = 1;

                if (comb_MainRoad.Text == "SX-GPRS" || comb_PrepareRoad.Text == "SX-GPRS")
                {
                    //通讯参数初始化
                    textBox_GPRS.Enabled = true;
                    textBox_GPRS.Text = "00000001";
                }
                if (comb_MainRoad.Text == "GSM" || comb_PrepareRoad.Text == "GSM")
                {
                    textBox_GSM.Text = "13211111111";
                    textBox_GSM.Enabled = true;
                }
                if (comb_MainRoad.Text == "Beidou-Normal" || comb_PrepareRoad.Text == "Beidou-Normal")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
                if (comb_MainRoad.Text == "Beidou-500" || comb_PrepareRoad.Text == "Beidou-500")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;

                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        /// <summary>
        /// 判断数据数据是否合法
        /// </summary>
        /// <returns></returns>
        private bool AssertInputData()
        {
            try
            {
                // 1. stationId 不能为空, 也不能重复,只能占用四个字节
                string stationId = textBox_StationID.Text.Trim();
                if (stationId.Equals(""))
                {
                    MessageBox.Show("测站站号不能为空");
                    return false;
                }
                // 判断测站编号是否为4位数字
                if (stationId.Length != 4)
                {
                    MessageBox.Show("请输入正确的4位测站编号");
                    return false;
                }
                // 判断编号是否为负数
                try
                {
                    int.Parse(stationId);
                }
                catch (System.Exception)
                {
                    MessageBox.Show("站点编号不能含有非法字符");
                    return false;
                }

                string gprs = textBox_GPRS.Text.Trim();
                if (gprs.Equals(""))
                {
                    //MessageBox.Show("GPRS号码不能为空!");
                    //return false;
                }
                else
                {
                    //GPRS号码
                    //  string gprsNum = textBox_GPRS.Text.Trim();
                    if (!CStringUtil.IsDigit(gprs))
                    {
                        MessageBox.Show("GPRS号码参数不合法，必须全部是数字!");
                        return false;
                    }
                    else
                    {

                    }
                }
                for (int i = 0; i < m_listStation.Count; ++i)
                {
                    if (m_listStation[i].StationID.Trim() == stationId)
                    {
                        //MessageBox.Show(string.Format("测站站号不能重复！编号 \"{0}\" 与测站 \"{1}\" 重复",
                        //    stationId, m_listStationCombination[i].StationName.Trim()));
                        MessageBox.Show(string.Format("墒情情测站站号不能重复！已存在水情测站{0}",
                         stationId));
                        return false;
                    }
                }
                for (int i = 0; i < m_listSoilStation.Count; ++i)
                {
                    if (m_listSoilStation[i].StationID.Trim() == stationId)
                    {
                        MessageBox.Show(string.Format("墒情情测站站号不能与墒情测站编号一样！已存在墒情测站{0}",
                        stationId));
                        return false;
                    }
                 }

                if (comb_MainRoad.Text.ToString() == "SX-GPRS" || comb_PrepareRoad.Text.ToString() == "SX-GPRS")
                {
                    if (gprs.Equals(""))
                    {
                        MessageBox.Show("GPRS号码不能为空!");
                        return false;
                    }
                    else
                    {
                        //GPRS号码
                        //  string gprsNum = textBox_GPRS.Text.Trim();
                        if (!CStringUtil.IsDigit(gprs))
                        {
                            MessageBox.Show("GPRS号码参数不合法，必须全部是数字!");
                            return false;
                        }
                        else
                        {

                        }
                     }
                    for (int i = 0; i < m_listStation.Count; ++i)
                    {
                        if (m_listStation[i].GPRS.Trim() == gprs)
                        {
                            MessageBox.Show(string.Format("墒情测站Gprs不能重复！与水情站{0} gprs号码一样 ",
                           m_listStation[i].StationID.Trim()));
                            return false;
                        }
                    }

                    for (int i = 0; i < m_listSoilStation.Count; ++i)
                    {
                        if (m_listSoilStation[i].GPRS.Trim() == gprs)
                        {
                            MessageBox.Show(string.Format("墒情测站Gprs不能重复！与墒情站{0} gprs号码一样 ",
                            m_listSoilStation[i].StationID.Trim()));
                            return false;
                        }
                    }
                }
                if (System.Text.Encoding.Default.GetByteCount(stationId) > 4)
                {
                    MessageBox.Show("测站ID字符数不能超过4个");
                    return false;
                }
                // 2. 站名不能为空，不能超过50个字符
                if (textBox_StationName.Text.Trim().Equals(""))
                {
                    MessageBox.Show("站名不能为空");
                    return false;
                }
                if (System.Text.Encoding.Default.GetByteCount(textBox_StationName.Text.Trim()) > 50)
                {
                    MessageBox.Show("站名不能超过50个字符");
                    return false;
                }
                // 3. 站类不能为空
                if (cmb_StationType.Text.Equals(""))
                {
                    MessageBox.Show("站点类型不能为空");
                    return false;
                }
                // 4. 分中心不能为空
                if (cmb_SubCenter.Text.Equals(""))
                {
                    MessageBox.Show("分中心不能为空");
                    return false;
                }
                //if (textBox_StationID.Text.Equals(""))
                //{
                //    MessageBox.Show("测站不能为空");
                //    return false;
                //}
                //    String strDeviceNumber = base.Rows[m_listEditedRows[i]].Cells[CS_DeviceNumber].Value.ToString();
                //    if (String.IsNullOrEmpty(strDeviceNumber))
                //    {
                //        MessageBox.Show("设备号不能为空");
                //        return false;
                //    }
                //    // 判断设备号是否为8位并且都是数据
                //    if (strDeviceNumber.Length != 8)
                //    {
                //        MessageBox.Show("设备号长度错误");
                //        return false;
                //    }
                //    if (!CStringFromatHelper.IsDigit(strDeviceNumber))
                //    {
                //        MessageBox.Show("设备号不能包含非数字字符");
                //        return false;
                //    }
                //}
                // 测站是否重复
                //List<CEntitySoilStation> allSoilStation = new List<CEntitySoilStation>();
                //allSoilStation = m_listSoilStation;

                //for (int i = 0; i < allSoilStation.Count; ++i)
                //{
                //    string name = m_listSoilStation[i].StationID.ToString();
                //    if (name.Equals(textBox_StationID.Text))
                //    {
                //        MessageBox.Show(string.Format("不能重复设置站点\"{0}\"的参数", name));
                //        return false;
                //    }
                //}
                return true;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { return false; }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                // 完成添加
                CEntitySoilStation soilstation = GenerateSoilStationFromUI();
                if (null == soilstation)
                {
                    return;
                }
                CDBSoilDataMgr.Instance.m_mapStaionSoilInfo.Add(soilstation.StationID, soilstation);
                AddSoilStationEvent(soilstation);
                this.Hide();
                MessageBox.Show("测站添加完成！ ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex)
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
            {
                MessageBox.Show("测站添加失败！ ", "提示 ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private CEntitySoilStation GenerateSoilStationFromUI()
        {
            try
            {
                if (!AssertInputData())
                {
                    return null;
                }
                // 读取界面数据，并构建一个站点实体类
                CEntitySoilStation soilStation = new CEntitySoilStation();
                soilStation.StationID = textBox_StationID.Text.Trim(); //去掉空格
                soilStation.StationName = textBox_StationName.Text.Trim();
                soilStation.SubCenterID = CDBDataMgr.Instance.GetSubCenterByName(cmb_SubCenter.Text).SubCenterID;
                soilStation.StationType = CEnumHelper.UIStrToStationType(cmb_StationType.Text);

                //   soilStation.StrDeviceNumber=textBox_deviceNumber.Text.Trim();
                soilStation.VoltageMin = CStringFromatHelper.ConvertToNullableDecimal(textBox_Voltage.Text.ToString());
                soilStation.A10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10a.Text.ToString());
                soilStation.A20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20a.Text.ToString());
                soilStation.A30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30a.Text.ToString());
                soilStation.A40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40a.Text.ToString());
                soilStation.A60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60a.Text.ToString());

                soilStation.B10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10b.Text.ToString());
                soilStation.B20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20b.Text.ToString());
                soilStation.B30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30b.Text.ToString());
                soilStation.B40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40b.Text.ToString());
                soilStation.B60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60b.Text.ToString());

                soilStation.C10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10c.Text.ToString());
                soilStation.C20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20c.Text.ToString());
                soilStation.C30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30c.Text.ToString());
                soilStation.C40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40c.Text.ToString());
                soilStation.C60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60c.Text.ToString());

                soilStation.D10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10d.Text.ToString());
                soilStation.D20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20d.Text.ToString());
                soilStation.D30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30d.Text.ToString());
                soilStation.D40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40d.Text.ToString());
                soilStation.D60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60d.Text.ToString());

                soilStation.M10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10m.Text.ToString());
                soilStation.M20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20m.Text.ToString());
                soilStation.M30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30m.Text.ToString());
                soilStation.M40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40m.Text.ToString());
                soilStation.M60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60m.Text.ToString());

                soilStation.N10 = CStringFromatHelper.ConvertToNullableDecimal(textBox_10n.Text.ToString());
                soilStation.N20 = CStringFromatHelper.ConvertToNullableDecimal(textBox_20n.Text.ToString());
                soilStation.N30 = CStringFromatHelper.ConvertToNullableDecimal(textBox_30n.Text.ToString());
                soilStation.N40 = CStringFromatHelper.ConvertToNullableDecimal(textBox_40n.Text.ToString());
                soilStation.N60 = CStringFromatHelper.ConvertToNullableDecimal(textBox_60n.Text.ToString());

                if (!textBox_GSM.Text.Trim().Equals(""))
                {
                    string gsmNum = textBox_GSM.Text.Trim();
                  //  if (!CStringUtil.IsDigitStrWithSpecifyLength(gsmNum, 11))
                   // {
                      //  MessageBox.Show("GSM号码参数不合法，长度必须为11位，必须全部是数字!");
                     if (!CStringUtil.IsDigit(gsmNum)){
                        MessageBox.Show("GSM号码参数不合法，必须全部是数字!");
                        return null;
                    }
                    // 设置了GSM号码
                    soilStation.GSM = gsmNum;
                }

                if (!textBox_GPRS.Text.Trim().Equals(""))
                {
                    //GPRS号码
                    string gprsNum = textBox_GPRS.Text.Trim();
                    if (!CStringUtil.IsDigit(gprsNum))
                    {
                        MessageBox.Show("GPRS号码参数不合法，必须全部是数字!");
                        return null;
                    }
                    soilStation.GPRS = gprsNum;
                }

                if (!textBox_Beidou.Text.Trim().Equals(""))
                {
                    string bdNum = textBox_Beidou.Text.Trim();
                    if (!CStringUtil.IsDigit(bdNum))
                    {
                        MessageBox.Show("北斗卫星终端号码参数不合法，必须全部是数字!");
                        return null;
                    }
                    // 北斗卫星号码
                    soilStation.BDSatellite = bdNum;
                }

                if (!textBox_BeidouMember.Text.Trim().Equals(""))
                {
                    string bdNum = textBox_BeidouMember.Text.Trim();
                    if (!CStringUtil.IsDigit(bdNum))
                    {
                        MessageBox.Show("北斗卫星成员号码参数不合法，必须全部是数字!");
                        return null;
                    }
                    // 北斗卫星号码
                    soilStation.BDMemberSatellite = bdNum;
                }

                soilStation.Maintran = comb_MainRoad.Text;
                soilStation.Subtran = comb_PrepareRoad.Text;

                soilStation.Datapotocol = comb_DataProtocol.Text;
                soilStation.Reportinterval = comb_Paragraph.Text;
                return soilStation;
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { return null; }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        private void comb_MainRoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox_GPRS.Enabled = false;
                textBox_GPRS.Text = "";
                textBox_GSM.Text = "";
                textBox_GSM.Enabled = false;
                textBox_Beidou.Text = "";
                textBox_Beidou.Enabled = false;
                textBox_BeidouMember.Text = "";
                textBox_BeidouMember.Enabled = false;
                if (comb_PrepareRoad.Text == "SX-GPRS")
                {
                    textBox_GPRS.Enabled = true;
                    textBox_GPRS.Text = "00000001";
                }
                if (comb_PrepareRoad.Text == "GSM")
                {
                    textBox_GSM.Enabled = true;
                    textBox_GSM.Text = "13211111111";
                }
                if (comb_PrepareRoad.Text == "Beidou-Normal")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
                if (comb_PrepareRoad.Text == "Beidou-500")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }

                if (comb_MainRoad.Text == "SX-GPRS")
                {
                    textBox_GPRS.Enabled = true;
                    textBox_GPRS.Text = "00000001";
                }
                if (comb_MainRoad.Text == "GSM")
                {
                    textBox_GSM.Enabled = true;
                    textBox_GSM.Text = "13211111111";
                }
                if (comb_MainRoad.Text == "Beidou-Normal")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
                if (comb_MainRoad.Text == "Beidou-500")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

        private void comb_PrepareRoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox_GPRS.Enabled = false;
                textBox_GPRS.Text = "";
                textBox_GSM.Text = "";
                textBox_GSM.Enabled = false;
                textBox_Beidou.Text = "";
                textBox_Beidou.Enabled = false;
                textBox_BeidouMember.Text = "";
                textBox_BeidouMember.Enabled = false;
                if (comb_MainRoad.Text == "SX-GPRS")
                {
                    textBox_GPRS.Enabled = true;
                    textBox_GPRS.Text = "00000001";
                }
                if (comb_MainRoad.Text == "GSM")
                {
                    textBox_GSM.Enabled = true;
                    textBox_GSM.Text = "13211111111";
                }
                if (comb_MainRoad.Text == "Beidou-Normal")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
                if (comb_MainRoad.Text == "Beidou-500")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }

                if (comb_PrepareRoad.Text == "SX-GPRS")
                {
                    textBox_GPRS.Enabled = true;
                    textBox_GPRS.Text = "00000001";
                }
                if (comb_PrepareRoad.Text == "GSM")
                {
                    textBox_GSM.Enabled = true;
                    textBox_GSM.Text = "13211111111";
                }
                if (comb_PrepareRoad.Text == "Beidou-Normal")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
                if (comb_PrepareRoad.Text == "Beidou-500")
                {
                    textBox_Beidou.Text = "212880";
                    textBox_Beidou.Enabled = true;
                    textBox_BeidouMember.Text = "0";
                    textBox_BeidouMember.Enabled = true;
                }
            }
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            catch (Exception ex) { }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过
        }

    }
}



