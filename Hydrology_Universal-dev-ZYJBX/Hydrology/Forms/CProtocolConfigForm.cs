using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Protocol.Manager;

namespace Hydrology.Forms
{
    public partial class CProtocolConfigForm : Form
    {
        public event EventHandler ProtocolConfigChanged;

        private XmlDllCollections m_dllCollections;
        private XmlDllCollections m_dllCollectionsBak;  // 内存备份
        private Dictionary<string, XmlDllInfo> m_mapChannelInfo; //协议类型名和详细信息的映射
        private Dictionary<string, string> m_mapClassInterface;

        private string m_currentProtocolName; //  当前协议名
        private bool m_bIsInChannelType;            // 是配置信道协议还是配置数据协议，默认配置信道协议
        private int m_iPreProtocolSelectedIndex; // 上一次选中的协议索引
        //获得项目exe文件路径
        string stringEXEPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        private bool IsChanged(XmlDllCollections origion, XmlDllCollections bak)
        {
            return !(origion.Equals(bak));
        }

        public CProtocolConfigForm(bool bIsInChannelType = true)
        {
            InitializeComponent();
            m_bIsInChannelType = bIsInChannelType;
            this.txt_ChannelProtocolName.Enabled = false;
            this.txt_ChannelProtocolName.ReadOnly = true;
            this.txt_DataProtocolName.ReadOnly = true;
            this.txt_DataProtocolName.Enabled = false;
            m_openFileDialog.Filter = "库文件(*.dll)|*.dll|所有文件(*.*)|*.*";
            m_openFileDialog.RestoreDirectory = true;
            if (m_bIsInChannelType)
            {
                m_openFileDialog.Title = "选择通讯方式";
                this.lbl_ClassNames.Visible = false;
                this.lbl_Interfaces.Visible = false;
                this.cmb_ClassNames.Visible = false;
                this.cmb_Interfaces.Visible = false;

                this.txt_ChannelClassName.Visible = false;
                this.lbl_ChannelClassName.Visible = false;
                this.lbl_ToolTip1.Visible = false;
                this.Text = "通讯方式配置";
            }
            else
            {
                m_openFileDialog.Title = "选择数据协议";
                this.lbl_ClassNames.Visible = false;
                this.lbl_Interfaces.Visible = false;
                this.cmb_ClassNames.Visible = false;
                this.cmb_Interfaces.Visible = false;


                this.lbl_ToolTip2.Visible = false;
                this.lbl_DataFlash.Visible = false;
                this.lbl_DataUDisk.Visible = false;
                this.lbl_DataUp.Visible = false;
                this.lbl_DataDown.Visible = false;

                this.txt_DataUDisk.Visible = false;
                this.txt_DataUp.Visible = false;
                this.txt_DataFlash.Visible = false;
                this.txt_DataDown.Visible = false;

                this.Text = "数据协议配置";
            }

            //m_openFileDialog.InitialDirectory = @"E:\陈乐宁相关\水文监测\HelloWorld.1228\MDITest\bin\Debug";

            m_mapChannelInfo = new Dictionary<string, XmlDllInfo>();
            m_mapClassInterface = new Dictionary<string, string>();

            panel4DataAdd.Visible = false;
            panel4ChannelAdd.Visible = false;

            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_GSM));
            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_GPRS));
            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_HDGPRS));
            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_BEIDOU_NORMAL));
            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_BEIDOU_500));
            this.cmb_ChannelInterfaceNames.Items.Add(GetInterfaceNameWithoutNamespace(CS_DEFINE.I_CHANNEL_CABLE));

            this.cmb_ChannelInterfaceNames.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmb_ChannelInterfaceNames.SelectedIndex = 0;
            m_iPreProtocolSelectedIndex = -1;

            LoadData();

            FormHelper.InitUserModeEvent(this);

        }

        public void SetDefaultText(string str)
        {
            if (listBox_ChannelList.Items.Contains(str))
            {
                listBox_ChannelList.SelectedItem = str;
            }
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            if (this.listBox_ChannelList.SelectedIndex < 0)
            {
                MessageBox.Show("请选择协议!");
                return;
            }
            // 浏览本地dll文件
            //m_openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();// 设置起始目录
            if (!textBox_DllPath.Text.Equals(""))
            {
                m_openFileDialog.InitialDirectory = textBox_DllPath.Text;
            }
            //m_openFileDialog.InitialDirectory = @"E:\陈乐宁相关\水文监测\HelloWorld.1228\MDITest\bin\Debug";

            m_openFileDialog.FileName = textBox_DllFileName.Text;

            DialogResult result = m_openFileDialog.ShowDialog();

            if (result == DialogResult.OK && (!m_openFileDialog.FileName.ToString().Equals("")))
            {
                // 显示路径
                string path = Path.GetDirectoryName(m_openFileDialog.FileName);
                string filename = Path.GetFileName(m_openFileDialog.FileName);

                UpdatePathAndFileName(path, filename);

                // 设置成已经更改
                SetModified(true);
            }
        }

        private void listBox_ChannelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txt_ChannelProtocolName.ReadOnly = true;
            this.txt_ChannelProtocolName.Enabled = false;
            // 判断当前是否未保存
            if (btn_Save.Enabled)
            {
                MessageBox.Show("请先完成当前编辑");
                this.listBox_ChannelList.SelectedIndexChanged -= listBox_ChannelList_SelectedIndexChanged;
                this.listBox_ChannelList.SelectedIndex = m_iPreProtocolSelectedIndex;
                this.listBox_ChannelList.SelectedIndexChanged += listBox_ChannelList_SelectedIndexChanged;
                return;
            }
            if (listBox_ChannelList.SelectedIndex < 0)
                return;
            // 选择了某个协议
            string channel = listBox_ChannelList.SelectedItem.ToString();
            m_mapClassInterface.Clear();
            cmb_ClassNames.Items.Clear();
            cmb_Interfaces.Items.Clear();
            if (m_mapChannelInfo.ContainsKey(channel))
            {
                // 初始化界面
                // 建立关联表，类名和实现的接口名进绑定
                UpdatePathAndFileName(m_mapChannelInfo[channel].BaseDir, m_mapChannelInfo[channel].FileName);
                foreach (XmlMember member in m_mapChannelInfo[channel].Members)
                {
                    if (member.ClassName != "")
                    {
                        cmb_ClassNames.Items.Add(GetInterfaceNameWithoutNamespace(member.ClassName));
                        cmb_Interfaces.Items.Add(member.Tag);
                        m_mapClassInterface.Add(member.ClassName, member.Tag); //类名和实现的接口名进绑定
                    }
                }
                cmb_ClassNames.SelectedIndex = 0;
                cmb_Interfaces.SelectedIndex = 0;

                //     this.txtProtocolName.Enabled = true;//编程可编辑状态

                //   更新协议名称，和是否使用该协议
                var info = m_mapChannelInfo[channel];
                //  解除绑定
                //    this.txtProtocolName.TextChanged -= txtProtocolName_TextChanged;
                this.txtProtocolName.Text = info.Name;
                //  重新绑定
                //      this.txtProtocolName.TextChanged += txtProtocolName_TextChanged;

                this.m_currentProtocolName = info.Name;

                m_iPreProtocolSelectedIndex = this.listBox_ChannelList.SelectedIndex;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定保存配置信息？", "保存修改", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 保存当前所做的修改到内存m_mapChannelInfo
                string keyChannel = listBox_ChannelList.SelectedItem.ToString();
                if (m_mapChannelInfo.ContainsKey(keyChannel))
                {
                    string newChannelName = txtProtocolName.Text.Trim();
                    List<string> arrayIndex = m_mapChannelInfo.Keys.ToList();
                    if (newChannelName != keyChannel && m_mapChannelInfo.ContainsKey(newChannelName))
                    {
                        // 如果协议名字发生了改变，并且在协议列表中存在与它名字相同的协议，说明已经重复了
                        MessageBox.Show(string.Format("\"{0}\"已存在!不能重复添加！", newChannelName));
                    }
                    else
                    {
                        // 判断当前的配置是否是正确的配置
                        XmlDllInfo info = m_mapChannelInfo[keyChannel];
                        info.Name = txtProtocolName.Text.Trim();
                        info.BaseDir = textBox_DllPath.Text.Trim();
                        info.FileName = textBox_DllFileName.Text.Trim();
                        info.Members = m_mapChannelInfo[keyChannel].Members;
                        //info.Type = m_mapChannelInfo[keyChannel].Type;
                        if (Protocol.Manager.ProtocolManager.AssertDllValid(info))
                        {
                            SetModified(false);
                            // dll有效
                            m_mapChannelInfo.Remove(keyChannel);
                            // 并添加到当前的Map中
                            m_mapChannelInfo.Add(newChannelName, info);
                            // 并更新ListView中的显示名字
                            listBox_ChannelList.Items[listBox_ChannelList.Items.IndexOf(keyChannel)] = newChannelName;
                            // 同时更新m_dllCollections内的内容,由于都是引用类型，map和collection中的内容一致
                            //for (int i = 0; i < m_dllCollections.Infos.Count; ++i)
                            //{
                            //    if (m_dllCollections.Infos[i].Name == keyChannel)
                            //    {
                            //        // 更新
                            //        m_dllCollections.Infos[i] = info;
                            //        break;
                            //    }
                            //}

                        }
                        else
                        {
                            // dll无效
                            MessageBox.Show("无效的dll文件");
                        }
                    }
                }
            }
        }

        private void tsButSave_Click(object sender, EventArgs e)
        {
            if (!IsChanged(m_dllCollections, m_dllCollectionsBak))
            {
                MessageBox.Show("未做任何修改，无需保存！");
                return;
            }
            // btn_Save_Click(null,null);
            // 做了修改，才需要保存
            SaveConfigToXml();
        }



        private void tsButExit_Click(object sender, EventArgs e)
        {
            // 退出，检查是否未保存
            this.Close();
        }

        private void cmb_ClassNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 选择的类型名发生了改变
            string classname = this.cmb_ClassNames.Text;
            if (m_mapClassInterface.ContainsKey(classname))
            {
                // 更改接口
                cmb_Interfaces.Text = m_mapClassInterface[classname];
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定放弃修改？", "放弃修改", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 放弃修改，从内存备份中恢复出来
                foreach (XmlDllInfo dll in m_dllCollectionsBak.Infos)
                {
                    if (dll.Name == listBox_ChannelList.SelectedItem.ToString())
                    {
                        // 找到匹配，修改为原来的路径和文件名
                        m_mapChannelInfo[dll.Name].BaseDir = dll.BaseDir;
                        m_mapChannelInfo[dll.Name].FileName = dll.FileName;
                        UpdatePathAndFileName(dll.BaseDir, dll.FileName);
                        SetModified(false);
                    }
                }
                this.txtProtocolName.Text = string.Empty;
                this.textBox_DllPath.Text = string.Empty;
                this.textBox_DllFileName.Text = string.Empty;
            }
        }

        private void CChannelConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //// 关闭事件，检查当前是否已经保存，或者正在编辑状态
            if (IsChanged(m_dllCollections, m_dllCollectionsBak))
            {
                DialogResult result = MessageBox.Show("当前所做修改尚未保存，是否保存？", "提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult.Cancel == result)
                {
                    // 不保存，退出对话框
                    e.Cancel = true;
                }
                else if (DialogResult.Yes == result)
                {
                    //// 保存当前修改
                    //btn_Save_Click(this, new EventArgs());
                    // 写入XML文件
                    //tsButSave_Click(this, new EventArgs());
                    bool bSavedOk = SaveConfigToXml();
                    // 如果保存失败，不允许退出对话框
                    e.Cancel = !bSavedOk;
                }
                else if (DialogResult.No == result)
                {
                    e.Cancel = false;
                }
            }
            //SaveConfigToXml();
        }


        private void tsButRevert_Click(object sender, EventArgs e)
        {
            // 判断是否做了修改
            if (!IsChanged(m_dllCollections, m_dllCollectionsBak))
            {
                // 如果没做修改，就不需要撤销
                // 在判断是否属于添加模式，如果是的话， 提示
                if (panel4ChannelAdd.Visible == true || panel4DataAdd.Visible == true)
                {
                    if (MessageBox.Show("确定要取消添加？", "撤销", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (m_bIsInChannelType)
                        {
                            ResetChannelPanel();
                        }
                        else
                        {
                            ResetDataPanel();
                        }
                        SetModified(false);
                        Init(); //初始化
                    }
                }
                else
                {
                    MessageBox.Show("没做任何修改，无需撤销");
                }
                return;
            }
            if (MessageBox.Show("确定要撤销已修改的配置信息？", "撤销修改", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 菜单栏上面的撤销按钮
                if (m_bIsInChannelType)
                {
                    ResetChannelPanel();
                }
                else
                {
                    ResetDataPanel();
                }
                SetModified(false);
                m_dllCollections = m_dllCollectionsBak.Clone() as XmlDllCollections;
                Init(); //初始化
            }
        }

        //private void txtProtocolName_TextChanged(object sender, EventArgs e)
        //{
        //    if (this.listBox_ChannelList.SelectedIndex < 0)
        //        return;
        //    string text = this.txtProtocolName.Text;
        //    if (text != m_currentProtocolName)
        //    {
        //        m_currentProtocolName = text;
        //        SetModified(true);
        //    }
        //}

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!listBox_ChannelList.Enabled)
            {
                MessageBox.Show("当前操作不允许删除协议！");
                return;
            }

            if (listBox_ChannelList.SelectedIndex < 0)
            {
                MessageBox.Show("请选择要删除的协议!");
                return;
            }
            // 保存当前所做的修改到内存m_mapChannelInfo
            string channel = listBox_ChannelList.SelectedItem.ToString();
            if (m_mapChannelInfo.ContainsKey(channel))
            {
                m_mapChannelInfo[channel].Enabled = false;
                //if (MessageBox.Show(String.Format("确认删除协议\"{0}\"?", channel), "删除协议", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                //{
                //    m_bIsDeleteProtocol = true;
                //    m_bModifiedWithoutSave = true;
                //    //this.tsButSave_Click(null, null);

                //}
                if (MessageBox.Show("确定删除" + channel + "?", "删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.m_mapChannelInfo.Remove(channel);
                    var temp = new XmlDllCollections() { Infos = new XmlDllInfos() };
                    foreach (var item in this.m_dllCollections.Infos)
                    {
                        if (item.Name != channel)
                        {
                            temp.Infos.Add(item);
                        }
                    }
                    this.m_dllCollections = temp;

                    // 初始化界面
                    listBox_ChannelList.Items.Clear();
                    if (m_mapChannelInfo.Count > 0)
                    {
                        foreach (KeyValuePair<string, XmlDllInfo> dll in m_mapChannelInfo)
                        {
                            //  添加合法的协议名称
                            var dllInfo = dll.Value;
                            if (string.IsNullOrEmpty(dllInfo.BaseDir) ||
                                string.IsNullOrEmpty(dllInfo.FileName) ||
                                !File.Exists(dllInfo.BaseDir + "\\" + dllInfo.FileName) ||
                                dllInfo.Members.Count <= 0)
                                continue;
                            listBox_ChannelList.Items.Add(dll.Key);
                        }
                    }
                    // 清空cmb的内容
                    textBox_DllFileName.Text = "";
                    textBox_DllPath.Text = "";
                    cmb_Interfaces.Items.Clear();
                    cmb_ClassNames.Items.Clear();
                    txtProtocolName.Text = "";
                    txtProtocolName.Enabled = false; //不可用
                    SetModified(false);
                }
            }
        }

        #region 帮助方法
        private void SetModified(bool modified)
        {
            if (modified)
            {
                // 启用按钮放弃和保存
                btn_Cancel.Enabled = true;
                btn_Save.Enabled = true;
            }
            else
            {
                // 关闭按钮放弃和保存
                btn_Cancel.Enabled = false;
                btn_Save.Enabled = false;
            }
        }

        private void Init()
        {
            // 初始化界面
            if (null == m_dllCollections)
            {
                return;
            }
            // 获取信道协议或者数据协议
            m_mapChannelInfo.Clear();

            //EDllType4Xml dllInfoType1 = EDllType4Xml.gprs;
            //XmlMemberInfos members1 = new XmlMemberInfos();
            //members1.Add(new XmlMember()
            //{
            //    ClassName = "Protocol.Channel.Gprs.GprsParser",
            //    Tag = "IGprs",
            //    InterfaceName = "Protocol.Channel.Interface.IGprs"
            //});
            //XmlDllInfo info1 = new XmlDllInfo()
            //{
            //    BaseDir = stringEXEPath,
            //    FileName = "Protocol.Channel.Gprs.dll",
            //    Enabled = true,
            //    DllType = dllInfoType1,
            //    Name = "Gprs",
            //    Type = "channel",
            //    Members = members1,
            //    Coms = new List<int>(),
            //    Ports = new List<CXMLPort>()
            //};
            //if (m_bIsInChannelType)
            //{
            //    m_mapChannelInfo.Add(info1.Name, info1);
            //}


            //string fileName = "辽宁-数据协议.dll";      
            //string tag = this.cmb_ChannelInterfaceNames.Text.Trim();

            //string path1 = string.Format(@"{0}\{1}", stringEXEPath, fileName);
            //string up1, down1, udisk1, flash1, soil1;
            //up1 = down1 = udisk1 = flash1 = soil1 = string.Empty;
            //ProtocolManager.AssertDataProtocolDllValid(path1, out up1, out down1, out udisk1, out flash1, out soil1);

            //XmlMemberInfos members2 = new XmlMemberInfos();
            //members2.Add(new XmlMember()
            //{
            //    ClassName = udisk1,
            //    Tag = CS_DEFINE.TAG_DATA_UBatch,
            //    InterfaceName = CS_DEFINE.I_DATA_UDISK_BATCH
            //});
            //members2.Add(new XmlMember()
            //{
            //    ClassName = down1,
            //    Tag = CS_DEFINE.TAG_DATA_Down,
            //    InterfaceName = CS_DEFINE.I_DATA_DOWN
            //});
            //members2.Add(new XmlMember()
            //{
            //    ClassName = flash1,
            //    Tag = CS_DEFINE.TAG_DATA_FlashBatch,
            //    InterfaceName = CS_DEFINE.I_DATA_FLASH_BATCH
            //});
            //members2.Add(new XmlMember()
            //{
            //    ClassName = up1,
            //    Tag = CS_DEFINE.TAG_DATA_Up,
            //    InterfaceName = CS_DEFINE.I_DATA_UP
            //});
            //members2.Add(new XmlMember()
            //{
            //    ClassName = soil1,
            //    Tag = CS_DEFINE.Tag_Data_Soil,
            //    InterfaceName = CS_DEFINE.I_DATA_SOIL
            //});

            //XmlDllInfo info2 = new XmlDllInfo()
            //{
            //    BaseDir = stringEXEPath,
            //    Coms = new List<int>(),
            //    FileName = "辽宁-数据协议.dll",
            //    Enabled = true,
            //    DllType = EDllType4Xml.none,
            //    Name = "辽宁",
            //    Type = "data",
            //    Members = members2,
            //    Ports = new List<CXMLPort>()
            //};
            //if (!m_bIsInChannelType)
            //{
            //    m_mapChannelInfo.Add(info2.Name, info2);
            //}


            foreach (XmlDllInfo info in m_dllCollections.Infos)
            {
                //  不显示已经被禁用的协议
                if (!info.Enabled)
                    continue;
                // 每一个类，以及实现的接口
                if (m_bIsInChannelType)
                {
                    // 显示信道协议
                    if (info.Type == "channel")
                    {
                        m_mapChannelInfo.Add(info.Name, info);

                    }
                }
                else
                {
                    // 显示数据协议
                    if (info.Type == "data")
                    {
                        m_mapChannelInfo.Add(info.Name, info);

                    }
                }
            }
            // 初始化界面
            listBox_ChannelList.Items.Clear();
            if (m_mapChannelInfo.Count > 0)
            {
                //   this.listBox_ChannelList.Items.Add(dll.Key);

                foreach (KeyValuePair<string, XmlDllInfo> dll in m_mapChannelInfo)
                {
                    //  添加合法的协议名称
                    var dllInfo = dll.Value;
                    if (string.IsNullOrEmpty(dllInfo.BaseDir) ||
                        string.IsNullOrEmpty(dllInfo.FileName) ||
                        !File.Exists(dllInfo.BaseDir + "\\" + dllInfo.FileName) ||
                        dllInfo.Members.Count <= 0)
                    {
                        //   continue;
                        //删除XML
                    }
                    else
                    {
                        this.listBox_ChannelList.Items.Add(dll.Key);
                    }
                }

            }
            SetModified(false); // 没做任何更改
            // 清空cmb的内容
            this.txtProtocolName.Text = string.Empty;
            this.txtProtocolName.Enabled = false;//默认不可编辑状态
            textBox_DllFileName.Text = "";
            textBox_DllPath.Text = "";
            cmb_Interfaces.Items.Clear();
            cmb_ClassNames.Items.Clear();
        }

        private void UpdatePathAndFileName(string dir, string filename)
        {
            // 更新界面
            textBox_DllPath.Text = dir;
            textBox_DllFileName.Text = filename;
        }
        private void LoadData()
        {
            XmlDocManager.Instance.ReadFromXml();
            m_dllCollections = Protocol.Manager.XmlDocManager.Instance.DllInfo;
            m_dllCollectionsBak = m_dllCollections.Clone() as XmlDllCollections;
            Init(); //初始化
        }

        private void RefreshPageData()
        {
            //  更新txtProtocolName
            //this.txtProtocolName.TextChanged -= txtProtocolName_TextChanged;
            //this.txtProtocolName.Text = string.Empty;
            //this.txtProtocolName.TextChanged += txtProtocolName_TextChanged;
            //  更新cmb_ClassNames
            if (this.cmb_ClassNames.Items.Count > 0)
                this.cmb_ClassNames.Items.Clear();
            //  更新cmb_Interfaces
            if (this.cmb_Interfaces.Items.Count > 0)
                this.cmb_Interfaces.Items.Clear();
            //  更新textBox_DllPath
            this.textBox_DllPath.Text = string.Empty;
            //  更新textBox_DllFileName
            this.textBox_DllFileName.Text = string.Empty;
            //  更新listBox_ChannelList
            if (this.listBox_ChannelList.Items.Count > 0)
                this.listBox_ChannelList.Items.Clear();


            SetModified(false);

            //  加载页面数据
            LoadData();
        }
        #endregion 帮助方法

        private void btnAddNewProtocol_Click(object sender, EventArgs e)
        {
            // 如果当前正在添加模式，或者修改模式，不允许添加，提示
            if (btn_Save.Enabled == true || panel4ChannelAdd.Visible || panel4DataAdd.Visible)
            {
                MessageBox.Show("请先完成当前编辑");
                return;
            }
            if (m_bIsInChannelType)
            {
                panel4ChannelAdd.Visible = true;
            }
            else
            {
                panel4DataAdd.Visible = true;
            }
            listBox_ChannelList.Enabled = false;
        }

        #region 添加信道协议
        private void btn_ChannelBrowse_Click(object sender, EventArgs e)
        {
            // 浏览本地dll文件
            //m_openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();// 设置起始目录
            if (!txt_ChannelDllPath.Text.Equals(""))
            {
                m_openFileDialog.InitialDirectory = txt_ChannelDllPath.Text;
            }

            //m_openFileDialog.InitialDirectory = @"E:\陈乐宁相关\水文监测\HelloWorld.1228\MDITest\bin\Debug";

            m_openFileDialog.FileName = txt_ChannelDllFileName.Text;
            //  m_openFileDialog.InitialDirectory = Application.StartupPath;
            string stringThePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            DialogResult result = m_openFileDialog.ShowDialog();

            if (result == DialogResult.OK && (!m_openFileDialog.FileName.ToString().Equals("")))
            {
                // 显示路径
                this.txt_ChannelDllFileName.Text = Path.GetFileName(m_openFileDialog.FileName);
                this.txt_ChannelDllPath.Text = Path.GetDirectoryName(m_openFileDialog.FileName);
                //  this.txt_ChannelDllPath.Text = "..";

                //  更新接口实现类型
                string tag = this.cmb_ChannelInterfaceNames.Text;
                // string baseDir = this.txt_ChannelDllPath.Text.Trim();
                string baseDir = stringThePath;
                string fileName = this.txt_ChannelDllFileName.Text.Trim();
                string path = string.Format(@"{0}\{1}", baseDir, fileName);

                string className = string.Empty;
                string interfaceName = string.Empty;
                string dllInfoTag = string.Empty;
                EDllType4Xml dllInfoType = EDllType4Xml.none;
                if (!ProtocolManager.AssertChannelProtocolDllValid(path,
                    tag,
                    out className,
                    out interfaceName,
                    out dllInfoTag,
                    out dllInfoType))
                {
                    MessageBox.Show("不是合法的DLL！请重新添加！");
                    this.txt_ChannelDllPath.Text = string.Empty;
                    this.txt_ChannelDllFileName.Text = string.Empty;
                    return;
                }
                this.txt_ChannelClassName.Text = className;
                // this.txt_ChannelProtocolName.Text = this.txt_ChannelDllFileName.Text.Replace(".dll", "");
                if (this.txt_ChannelDllFileName.Text == "Protocol.Channel.Gprs.dll")
                {
                    this.txt_ChannelProtocolName.Text = "SX-GPRS";
                }
                else if (this.txt_ChannelDllFileName.Text == "Protocol.Channel.HDGprs.dll")
                {
                    this.txt_ChannelProtocolName.Text = "HD-GPRS";
                }

                else if (this.txt_ChannelDllFileName.Text == "Protocol.Channel.Gsm.dll")
                {
                    this.txt_ChannelProtocolName.Text = "GSM";
                }
                else if (this.txt_ChannelDllFileName.Text == "Protocol.Channel.Beidou.dll")
                {
                    this.txt_ChannelProtocolName.Text = "Beidou-Normal";
                }
                else if (this.txt_ChannelDllFileName.Text == "Protocol.Channel.Beidou500.dll")
                {
                    this.txt_ChannelProtocolName.Text = "Beidou-500";
                }

                else
                {
                    MessageBox.Show("不是合法的DLL！请重新添加！");
                    this.txt_ChannelDllPath.Text = string.Empty;
                    this.txt_ChannelDllFileName.Text = string.Empty;
                    return;
                }
            }
        }
        private void btnChannelSaveAddNew_Click(object sender, EventArgs e)
        {
            string protocolName = this.txt_ChannelProtocolName.Text.Trim();

            if (String.IsNullOrEmpty(protocolName))
            {
                MessageBox.Show("协议名不能为空!");
                return;
            }
            //  string stringThePath1 = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            // string baseDir = stringEXEPath;
            // string baseDir = this.txt_ChannelDllFileName.Text.Trim();
            string baseDir = this.txt_ChannelDllPath.Text.Trim();
            string fileName = this.txt_ChannelDllFileName.Text.Trim();
            if (String.IsNullOrEmpty(fileName) || String.IsNullOrEmpty(baseDir))
            {
                MessageBox.Show("请选择DLL!");
                return;
            }
            string tag = this.cmb_ChannelInterfaceNames.Text.Trim();

            string path = string.Format(@"{0}\{1}", baseDir, fileName);

            string className = string.Empty;
            string interfaceName = string.Empty;
            string dllInfoTag = string.Empty;
            EDllType4Xml dllInfoType = EDllType4Xml.none;
            if (!ProtocolManager.AssertChannelProtocolDllValid(path,
                tag,
                out className,
                out interfaceName,
                out dllInfoTag,
                out dllInfoType))
            {
                MessageBox.Show("不是合法的DLL！请重新添加！");
                this.txt_ChannelDllPath.Text = string.Empty;
                this.txt_ChannelDllFileName.Text = string.Empty;
                return;
            }
            this.txt_ChannelClassName.Text = className;

            XmlMemberInfos members = new XmlMemberInfos();
            members.Add(new XmlMember()
            {
                ClassName = className,
                Tag = dllInfoTag,
                InterfaceName = interfaceName
            });
            // baseDir = "..";
            XmlDllInfo info = new XmlDllInfo()
            {
                BaseDir = baseDir,
                FileName = fileName,
                Enabled = true,
                DllType = dllInfoType,
                Name = protocolName,
                Type = "channel",
                Members = members,
                Coms = new List<int>(),
                Ports = new List<CXMLPort>()
            };
            if (m_mapChannelInfo.ContainsKey(protocolName))
            {
                MessageBox.Show(string.Format("通讯方式{0}已存在，不能重复添加!", protocolName));
                return;
            }
            if (MessageBox.Show("确定添加通讯方式？", "添加", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                m_dllCollections.Infos.Add(info);
                ResetChannelPanel();
            }
        }
        private void btnChannelCancelAddNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定取消添加通讯方式？", "取消", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ResetChannelPanel();
            }
        }
        private void ResetChannelPanel()
        {
            this.txt_ChannelProtocolName.Text = string.Empty;
            this.cmb_ChannelInterfaceNames.SelectedIndex = 0;
            this.txt_ChannelClassName.Text = string.Empty;
            this.txt_ChannelDllFileName.Text = string.Empty;
            this.txt_ChannelDllPath.Text = string.Empty;

            this.panel4ChannelAdd.Visible = false;
            this.listBox_ChannelList.Enabled = true;

            //  更新txtProtocolName
            //this.txtProtocolName.TextChanged -= txtProtocolName_TextChanged;
            //this.txtProtocolName.Text = string.Empty;
            //this.txtProtocolName.TextChanged += txtProtocolName_TextChanged;
            //  更新cmb_ClassNames
            if (this.cmb_ClassNames.Items.Count > 0)
                this.cmb_ClassNames.Items.Clear();
            //  更新cmb_Interfaces
            if (this.cmb_Interfaces.Items.Count > 0)
                this.cmb_Interfaces.Items.Clear();
            //  更新textBox_DllPath
            this.textBox_DllPath.Text = string.Empty;
            //  更新textBox_DllFileName
            this.textBox_DllFileName.Text = string.Empty;
            //  更新listBox_ChannelList
            if (this.listBox_ChannelList.Items.Count > 0)
                this.listBox_ChannelList.Items.Clear();

            SetModified(false);
            Init();
        }
        #endregion

        #region 添加数据协议
        private void btn_DataBrowse_Click(object sender, EventArgs e)
        {
            this.txt_DataProtocolName.ReadOnly = true;
            this.txt_DataProtocolName.Enabled = false;
            // 浏览本地dll文件
            if (!this.txt_DataDllPath.Text.Equals(""))
            {
                m_openFileDialog.InitialDirectory = txt_ChannelDllPath.Text;
            }

            m_openFileDialog.FileName = this.txt_DataDllFileName.Text;

            DialogResult result = m_openFileDialog.ShowDialog();

            if (result == DialogResult.OK && (!m_openFileDialog.FileName.ToString().Equals("")))
            {
                // 显示路径
                this.txt_DataDllFileName.Text = Path.GetFileName(m_openFileDialog.FileName);
                this.txt_DataDllPath.Text = Path.GetDirectoryName(m_openFileDialog.FileName);

                //  更新接口实现类型
                string path = string.Format(@"{0}\{1}", this.txt_DataDllPath.Text.Trim(), this.txt_DataDllFileName.Text.Trim());
                string up, down, udisk, flash, soil;
                up = down = udisk = flash = soil = string.Empty;
                if (!ProtocolManager.AssertDataProtocolDllValid(path, out up, out down, out udisk, out flash, out soil))
                {
                    MessageBox.Show("不是合法的DLL！请重新添加！");
                    this.txt_DataDllPath.Text = string.Empty;
                    this.txt_DataDllFileName.Text = string.Empty;
                    return;
                }
                //  更新界面ui
                this.txt_DataUp.Text = up;
                this.txt_DataDown.Text = down;
                this.txt_DataUDisk.Text = udisk;
                this.txt_DataFlash.Text = flash;
                if (this.txt_DataDllFileName.Text == "Protocol.Data.Lib.dll")
                {
                    this.txt_DataProtocolName.Text = "LN";
                }
                else if (this.txt_DataDllFileName.Text == "Protocol.Data.XYJBX.dll")
                {
                    this.txt_DataProtocolName.Text = "XYJBX";
                }
                else if (this.txt_DataDllFileName.Text == "Protocol.Data.ZYJBX.dll")
                {
                    this.txt_DataProtocolName.Text = "ZYJBX";
                }
                else if (this.txt_DataDllFileName.Text == "Protocol.Data.SXDZ.dll")
                {
                    this.txt_DataProtocolName.Text = "SXDZ";
                }
                else if (this.txt_DataDllFileName.Text == "Protocol.Data.HJJBX.dll")
                {
                    this.txt_DataProtocolName.Text = "HJJBX";
                }
            }
        }
        private void btnDataSaveAddNew_Click(object sender, EventArgs e)
        {
            string protocolName = this.txt_DataProtocolName.Text.Trim();
            if (String.IsNullOrEmpty(protocolName))
            {
                MessageBox.Show("协议名不能为空!");
                return;
            }
            string filename = this.txt_DataDllFileName.Text.Trim();
            string basedir = this.txt_DataDllPath.Text.Trim();
            if (String.IsNullOrEmpty(filename) || String.IsNullOrEmpty(basedir))
            {
                MessageBox.Show("请选择DLL!");
                return;
            }
            string path = string.Format(@"{0}\{1}", basedir, filename);
            string up, down, udisk, flash, soil;
            up = down = udisk = flash = soil = string.Empty;
            if (!ProtocolManager.AssertDataProtocolDllValid(path, out up, out down, out udisk, out flash, out soil))
            {
                MessageBox.Show("不是合法的DLL！请重新添加！");
                this.txt_DataDllPath.Text = string.Empty;
                this.txt_DataDllFileName.Text = string.Empty;
                return;
            }
            //  更新界面ui
            this.txt_DataUp.Text = up;
            this.txt_DataDown.Text = down;
            this.txt_DataUDisk.Text = udisk;
            this.txt_DataFlash.Text = flash;

            XmlMemberInfos members = new XmlMemberInfos();
            members.Add(new XmlMember()
            {
                ClassName = this.txt_DataUDisk.Text.Trim(),
                Tag = CS_DEFINE.TAG_DATA_UBatch,
                InterfaceName = CS_DEFINE.I_DATA_UDISK_BATCH
            });
            members.Add(new XmlMember()
            {
                ClassName = this.txt_DataDown.Text.Trim(),
                Tag = CS_DEFINE.TAG_DATA_Down,
                InterfaceName = CS_DEFINE.I_DATA_DOWN
            });
            members.Add(new XmlMember()
            {
                ClassName = this.txt_DataFlash.Text.Trim(),
                Tag = CS_DEFINE.TAG_DATA_FlashBatch,
                InterfaceName = CS_DEFINE.I_DATA_FLASH_BATCH
            });
            members.Add(new XmlMember()
            {
                ClassName = this.txt_DataUp.Text.Trim(),
                Tag = CS_DEFINE.TAG_DATA_Up,
                InterfaceName = CS_DEFINE.I_DATA_UP
            });
            members.Add(new XmlMember()
            {
                ClassName = soil.Trim(),
                Tag = CS_DEFINE.Tag_Data_Soil,
                InterfaceName = CS_DEFINE.I_DATA_SOIL
            });

            XmlDllInfo info = new XmlDllInfo()
            {
                BaseDir = basedir,
                Coms = new List<int>(),
                FileName = filename,
                Enabled = true,
                DllType = EDllType4Xml.none,
                Name = protocolName,
                Type = "data",
                Members = members,
                Ports = new List<CXMLPort>()
            };
            if (this.m_mapChannelInfo.ContainsKey(protocolName))
            {
                MessageBox.Show(string.Format("数据协议{0}已存在，不能重复添加!", protocolName));
                return;
            }
            if (MessageBox.Show("确定添加通讯方式？", "添加", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                m_dllCollections.Infos.Add(info);
                ResetDataPanel();
            }
        }
        private void btnDataCancelAddNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定取消添加数据协议？", "取消", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ResetDataPanel();
            }
        }
        private void ResetDataPanel()
        {
            this.txt_DataProtocolName.Text = string.Empty;
            this.txt_DataUDisk.Text = string.Empty;
            this.txt_DataFlash.Text = string.Empty;
            this.txt_DataDown.Text = string.Empty;
            this.txt_DataUp.Text = string.Empty;
            this.txt_DataDllFileName.Text = string.Empty;
            this.txt_DataDllPath.Text = string.Empty;

            this.panel4DataAdd.Visible = false;
            this.listBox_ChannelList.Enabled = true;

            //  更新txtProtocolName
            //this.txtProtocolName.TextChanged -= txtProtocolName_TextChanged;
            //this.txtProtocolName.Text = string.Empty;
            //this.txtProtocolName.TextChanged += txtProtocolName_TextChanged;
            //  更新cmb_ClassNames
            if (this.cmb_ClassNames.Items.Count > 0)
                this.cmb_ClassNames.Items.Clear();
            //  更新cmb_Interfaces
            if (this.cmb_Interfaces.Items.Count > 0)
                this.cmb_Interfaces.Items.Clear();
            //  更新textBox_DllPath
            this.textBox_DllPath.Text = string.Empty;
            //  更新textBox_DllFileName
            this.textBox_DllFileName.Text = string.Empty;
            //  更新listBox_ChannelList
            if (this.listBox_ChannelList.Items.Count > 0)
                this.listBox_ChannelList.Items.Clear();

            SetModified(false);

            Init();
        }
        #endregion

        /// <summary>
        /// 将配置信息写入配置文件中
        /// </summary>
        private bool SaveConfigToXml()
        {
            //  未修改任何配置信息
            //if (!IsChanged(m_dllCollections, m_dllCollectionsBak))
            //{
            //    MessageBox.Show("未做任何修改，无需保存！");
            //    return;
            //}
            //  用户不选择保存配置信息
            //if (MessageBox.Show("是否保存配置信息到配置文件中?", "保存", MessageBoxButtons.OKCancel) != DialogResult.OK)
            //    return;

            try
            {
                //  不允许写入的配置信息中包含相同的名称
                //  如果包含相同的名字，则提醒用户重新配置
                var keys = new List<string>();
                foreach (var item in m_dllCollections.Infos)
                {
                    keys.Add(item.Name);
                }

                foreach (var item in keys)
                {
                    var count = (from r in keys where r == item select r).Count();
                    if (count != 1)
                    {
                        MessageBox.Show("协议列表中不允许有相同的名称!请重新配置！");
                        return false;
                    }
                }
                //  写入配置文件
                XmlDocManager.Instance.DllInfo = m_dllCollections;
                XmlDocManager.Instance.WriteToXml();
                MessageBox.Show("保存成功!");

                #region 日志文件
                // 写入日志文件
                string loginfo = "协议配置变更：";
                string dlltype = "";
                if (m_bIsInChannelType)
                {
                    loginfo += "通讯方式:";
                    dlltype = "channel";
                }
                else
                {
                    loginfo += "数据协议：";
                    dlltype = "data";
                }
                loginfo += "修改前：<";
                for (int i = 0; i < m_dllCollectionsBak.Infos.Count; ++i)
                {
                    if (m_dllCollectionsBak.Infos[i].Type == dlltype)
                    {
                        loginfo += m_dllCollectionsBak.Infos[i].Name;
                        if (i != m_dllCollectionsBak.Infos.Count - 1)
                        {
                            loginfo += ",";
                        }
                    }
                }
                loginfo += "> 修改后：<";
                for (int i = 0; i < m_dllCollections.Infos.Count; ++i)
                {
                    if (m_dllCollections.Infos[i].Type == dlltype)
                    {
                        loginfo += m_dllCollections.Infos[i].Name;
                        if (i != m_dllCollections.Infos.Count - 1)
                        {
                            loginfo += ",";
                        }
                    }
                }
                loginfo += ">";
                CSystemInfoMgr.Instance.AddInfo(loginfo);
                #endregion 日志文件

                //  更新界面
                RefreshPageData();

                //  通知系统数据协议、信道协议配置信息改变
                if (ProtocolConfigChanged != null)
                    ProtocolConfigChanged(null, null);


                return true;
            }
            catch (System.Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                MessageBox.Show("保存失败!");
                return false;
            }
        }

        /// <summary>
        /// 将配置信息写入配置文件
        /// </summary>
        /// <param name="config"></param>
        //private void SaveConfigInfoToXml(XmlDllCollections config)
        //{
        //    try
        //    {
        //        //  需要提示是否保存
        //        if (MessageBox.Show("是否保存配置信息到配置文件中?", "保存", MessageBoxButtons.OKCancel) == DialogResult.OK)
        //        {
        //            var keys = new List<string>();
        //            foreach (var item in config.Infos)
        //            {
        //                keys.Add(item.Name);
        //            }

        //            foreach (var item in keys)
        //            {
        //                var count = (from r in keys where r == item select r).Count();
        //                if (count != 1)
        //                {
        //                    MessageBox.Show("协议列表中不允许有相同的名称!请重新配置！");
        //                    return;
        //                }
        //            }
        //            XmlDocManager.Instance.DllInfo = config;
        //            XmlDocManager.Instance.WriteToXml();
        //            MessageBox.Show("保存成功");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        MessageBox.Show("保存失败");
        //    }
        //}

        private string GetInterfaceNameWithoutNamespace(string fullName)
        {
            if (fullName.Contains("."))
            {
                return fullName.Substring(fullName.LastIndexOf('.') + 1);
            }
            return fullName;
        }

        private void cmb_ChannelInterfaceNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_ChannelInterfaceNames.SelectedIndex == 4)
            {
                this.txt_ChannelProtocolName.Text = "Cable";
            }
        }
    }// end of class
}
