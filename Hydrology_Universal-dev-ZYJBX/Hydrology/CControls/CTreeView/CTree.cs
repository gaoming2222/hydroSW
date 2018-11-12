using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.Entity;
using Protocol.Manager;
using System.IO.Ports;
using System.Xml;
using System.IO;

namespace Hydrology.CControls
{
    public class CTree : TreeView
    {
        #region 单例模式
        private static CTree instance;
        public static CTree Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region 构造函数
        static CTree()
        {
            if (instance == null)
            {
                instance = new CTree();
            }
        }
        private CTree()
            : base()
        {
            //  修改TreeView样式
            this.ShowLines = true;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("宋体", 12);
            this.Dock = DockStyle.Fill;
            //  加载图片资源列表
            this.ImageList = CImageList.GetImageList();
        }
        #endregion

        public void LoadTree()
        {
            if (instance.Nodes.Count > 0)
                instance.Nodes.Clear();

            InitSystem();
            InitUser();
            InitStation();

            int firstLevelNodeCount = GetNodeCount(false);
            for (int i = 0; i < firstLevelNodeCount; i++)
            {
                instance.Nodes[i].Expand();
            }
        }

        #region 树形控件添加结点
        /// <summary>
        /// 初始化系统设置结点
        /// </summary>
        private void InitSystem()
        {
            var sysNode = BuildTreeNode(CTreeType.SystemSetting);
            sysNode.Nodes.Add(BuildTreeNode(CTreeType.ChannelProtocolCfg));//  添加通讯方式配置结点
            sysNode.Nodes.Add(BuildTreeNode(CTreeType.DataProtocolCfg));//  添加数据协议配置结点
            sysNode.Nodes.Add(BuildTreeNode(CTreeType.CommunicationPort));//  添加通讯口配置结点

            instance.Nodes.Add(sysNode);
        }
        /// <summary>
        /// 初始化用户管理结点
        /// </summary>
        private void InitUser()
        {
            var userNode = BuildTreeNode(CTreeType.UserSetting);
            userNode.Nodes.Add(BuildTreeNode(CTreeType.UserLogin));//  添加通讯方式配置结点

            instance.Nodes.Add(userNode);
        }


        /// <summary>
        /// 初始化分中心、站点信息
        /// </summary>
        //private void InitStation()
        //{
        //    var m_listStation = CDBDataMgr.GetInstance().GetAllStation();
        //    var m_listSubCenter = CDBDataMgr.GetInstance().GetAllSubCenter();

        //    var dics = new Dictionary<CEntitySubCenter, IList<CEntityStation>>();

        //    var listSubcenters = new List<int>();
        //    foreach (var center in m_listSubCenter)
        //    {
        //        int subcenterId = center.SubCenterID;

        //        var listStations = new List<CEntityStation>();
        //        foreach (var station in m_listStation)
        //        {
        //            if (station.SubCenterID.HasValue && station.SubCenterID == subcenterId)
        //            {
        //                listStations.Add(station);
        //            }
        //        }

        //        if (!listSubcenters.Contains(subcenterId))
        //        {
        //            dics.Add(center, listStations);
        //        }
        //    }
        //    InitStation(dics);
        //}
        private void InitStation()
        {
            var m_listStation = CDBDataMgr.GetInstance().GetAllStation();
            var m_listSubCenter = CDBDataMgr.GetInstance().GetAllSubCenter();
            var m_listSoilStation = CDBSoilDataMgr.GetInstance().GetAllSoilStation();

            //   var dics = new Dictionary<CEntitySubCenter, IList<CEntityStation>>();
            var dics = new Dictionary<CEntitySubCenter, IList<Object>>();
            var dicsSoil = new Dictionary<CEntitySubCenter, IList<CEntitySoilStation>>();

            var listSubcenters = new List<int>();
            foreach (var center in m_listSubCenter)
            {
                int subcenterId = center.SubCenterID;

                var listStations = new List<Object>();
                var listSoilStations = new List<Object>();
                foreach (var station in m_listStation)
                {
                    if (station.SubCenterID.HasValue && station.SubCenterID == subcenterId)
                    {
                        listStations.Add(station);

                    }
                }

                foreach (var soilstation in m_listSoilStation)
                {
                    if (soilstation.SubCenterID.HasValue && soilstation.SubCenterID == subcenterId)
                    {
                        listStations.Add(soilstation);
                    }
                }

                if (!listSubcenters.Contains(subcenterId))
                {
                    dics.Add(center, listStations);
                    //  dics.Add(center, listSoilStations);
                    //  dicsSoil.Add(center, listSoilStations);
                }

            }
            InitStation(dics);
        }

        //private void InitStation(Dictionary<CEntitySubCenter, IList<CEntityStation>> dics)
        //{
        //    //  初始化一级节点，遥测站结点
        //    var nodeRoot = BuildTreeNode(CTreeType.CeZhan1);

        //    //  初始化二级节点，分中心结点
        //    foreach (var dic in dics)
        //    {
        //        var nodeSubCenter = BuildTreeNode(CTreeType.CeZhan2, dic.Key);

        //        foreach (var item in dic.Value)
        //        {
        //            //  初始化三级节点，站点结点
        //            var nodeStation = BuildTreeNode(CTreeType.CeZhan3, item);

        //            nodeSubCenter.Nodes.Add(nodeStation);
        //        }
        //        nodeRoot.Nodes.Add(nodeSubCenter);
        //    }
        //    instance.Nodes.Add(nodeRoot);
        //}
        private void InitStation(Dictionary<CEntitySubCenter, IList<Object>> dics)
        {
            //  初始化一级节点，遥测站结点
            var nodeRoot = BuildTreeNode(CTreeType.CeZhan1);

            //  初始化二级节点，分中心结点
            foreach (var dic in dics)
            {
                var nodeSubCenter = BuildTreeNode(CTreeType.CeZhan2, dic.Key);

                foreach (var item in dic.Value)
                {
                    //  初始化三级节点，站点结点
                    var nodeStation = BuildTreeNode(CTreeType.CeZhan3, item);

                    nodeSubCenter.Nodes.Add(nodeStation);
                }

                nodeRoot.Nodes.Add(nodeSubCenter);
            }
            
            instance.Nodes.Add(nodeRoot);
        }

        #endregion

        private CTreeNode BuildTreeNode(CTreeType treeType, object obj = null)
        {
            return BuildTreeNodeMap[treeType](obj);
        }

        /// <summary>
        /// 结点类型和生成结点方法的Map
        ///     根据节点类型，快速查找构建结点的方法
        /// </summary>
        private CDictionary<CTreeType, BuildTreeNodeDelegate> BuildTreeNodeMap = new CDictionary<CTreeType, BuildTreeNodeDelegate>()
        {
            { CTreeType.SystemSetting, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildSysNode)  },
            { CTreeType.CommunicationPort, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildPortNode)  },
            { CTreeType.ChannelProtocolCfg, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildChannelNode)  },
            { CTreeType.DataProtocolCfg, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildDataNode)  },
            { CTreeType.UserSetting, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildUserManageNode)  },
            { CTreeType.UserLogin, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildUserNode)  },
            { CTreeType.CeZhan1, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildRemoteStationNode)  },
            { CTreeType.CeZhan2, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildSubCenterNode)  },
            { CTreeType.CeZhan3, new BuildTreeNodeDelegate( CBuildTreeNodeHelper.BuildStationNode)  }
        };

        private delegate CTreeNode BuildTreeNodeDelegate(object obj);

        /// <summary>
        /// 树形控件结点定义
        /// </summary>
        internal class CBuildTreeNodeHelper
        {
            /// <summary>
            /// 系统设置结点
            /// </summary>
            public static CTreeNode BuildSysNode(object obj)
            {
                var node = new CTreeNode("系统管理", CTreeType.SystemSetting);
                node.SetImageIndex(0);
                return node;
            }
            /// <summary>
            /// 串口信息结点
            /// </summary>
            public static CTreeNode BuildPortNode(object obj)
            {
                var node = new CTreeNode("串口配置", CTreeType.CommunicationPort);
                node.SetImageIndex(2);
                //var ports = CDBDataMgr.Instance.GetAllSerialPort();
                //foreach (var port in ports)
                //{
                //    string msg = String.Format("COM{0} [{1}]", port.PortNumber, CEnumHelper.SerialTransTypeToUIStr(port.TransType));
                //    var portLevel2 = new CTreeNode(msg, CTreeType.PortSettings2);
                //    //  设置小图标

                //    //  添加二级菜单
                //    node.Nodes.Add(portLevel2);
                //}
                var coms = XmlDocManager.Instance.GetAllComPorts();

                var sysComs = new List<string>(SerialPort.GetPortNames());
                var comsInDB = CDBDataMgr.Instance.GetAllSerialPortName();
                foreach (var com in coms)
                {
                    var dllInfo = XmlDocManager.Instance.GetChannelDllByComOrPort(com, true);
                    if (dllInfo == null)
                        continue;
                    string comName = "COM" + com;
                    if (!sysComs.Contains(comName))
                        continue;
                    if (!comsInDB.Contains(comName))
                        continue;
                    string msg = String.Format("COM{0} [{1}]", com, dllInfo.Name);
                    var portLevel2 = new CTreeNode(msg, CTreeType.CommunicationPort2);
                    //  设置小图标

                    //  添加二级菜单
                    node.Nodes.Add(portLevel2);
                }

                return node;
            }
            /// <summary>
            /// 信道协议结点
            /// </summary>
            public static CTreeNode BuildChannelNode(object obj)
            {
                delnode();
                //  添加信道协议
                var node = new CTreeNode("通讯方式", CTreeType.ChannelProtocolCfg);
                node.SetImageIndex(6);

                var m_dllCollections = Protocol.Manager.XmlDocManager.Deserialize();
                foreach (var item in m_dllCollections.Infos)
                {
                    if (item.Type == "channel" && item.Enabled)
                    {
                        var level2 = new CTreeNode(item.Name, CTreeType.ChannelProtocolCfg2);

                        node.Nodes.Add(level2);
                    }
                }
               
                return node;
            }

            private static void delnode()
            {        
                var m_dllCollections = Protocol.Manager.XmlDocManager.Deserialize();
                XmlDllCollections m_dllCollections_1=new XmlDllCollections();
                foreach (XmlDllInfo info in m_dllCollections.Infos)
                {
                    ////  不显示已经被禁用的协议
                    //if (!info.Enabled)
                    //    continue;
                    // 每一个类，以及实现的接口

                        // 显示信道协议
                        if (info.Type == "channel")
                        {                      
                           if (string.IsNullOrEmpty(info.BaseDir) ||
                                string.IsNullOrEmpty(info.FileName) ||
                                !File.Exists(info.BaseDir + "\\" + info.FileName) ||
                                info.Members.Count <= 0)
                           {
                          
                              }else
                           {
                                    m_dllCollections_1.Infos.Add(info);
                           }
                        }             
                    else
                    {
                        // 显示数据协议
                        if (info.Type == "data")
                        {              
                            if (string.IsNullOrEmpty(info.BaseDir) ||
                             string.IsNullOrEmpty(info.FileName) ||
                             !File.Exists(info.BaseDir + "\\" + info.FileName) ||
                             info.Members.Count <= 0)
                            {

                            }
                            else
                            {
                                m_dllCollections_1.Infos.Add(info);
                            }
                        }
                    }
                }

          
                XmlDocManager.Instance.DllInfo = m_dllCollections_1;
                XmlDocManager.Instance.WriteToXml();
            }
            /// <summary>
            /// 数据协议结点
            /// </summary>
            public static CTreeNode BuildDataNode(object obj)
            {
                var node = new CTreeNode("数据协议", CTreeType.DataProtocolCfg);
                node.SetImageIndex(4);

                var m_dllCollections = Protocol.Manager.XmlDocManager.Deserialize();
                foreach (var item in m_dllCollections.Infos)
                {
                    if (item.Type == "data" && item.Enabled)
                    {
                        var level2 = new CTreeNode(item.Name, CTreeType.DataProtocolCfg2);

                        node.Nodes.Add(level2);
                    }
                }
                return node;
            }
            /// <summary>
            /// 用户结点
            /// </summary>
            public static CTreeNode BuildUserManageNode(object obj)
            {
                var node = new CTreeNode("用户管理", CTreeType.UserSetting);
                node.SetImageIndex(8);
                return node;
            }
            /// <summary>
            /// 用户结点
            /// </summary>
            public static CTreeNode BuildUserNode(object obj)
            {
                var node = new CTreeNode("登录用户", CTreeType.UserLogin);
                node.SetImageIndex(8);
                return node;
            }
            /// <summary>
            /// 遥测站结点
            /// </summary>
            /// <returns></returns>
            public static CTreeNode BuildRemoteStationNode(object obj)
            {
                var node = new CTreeNode("遥测站管理", CTreeType.CeZhan1);
                node.SetImageIndex(10);
                return node;
            }
            /// <summary>
            /// 分中心结点
            /// </summary>
            /// <param name="subcenter"></param>
            /// <returns></returns>
            public static CTreeNode BuildSubCenterNode(object obj)
            {
                var subcenter = obj as CEntitySubCenter;
                var node = new CTreeNode(subcenter.SubCenterName, CTreeType.CeZhan2, subcenter.SubCenterID.ToString());
                node.SetImageIndex(12);
                return node;
            }
            /// <summary>
            /// 监测站点结点
            /// </summary>
            /// <param name="station"></param>
            /// <returns></returns>
            //public static CTreeNode BuildStationNode(object obj)
            //{
            //    var station = obj as CEntityStation;
            //    string text = string.Format("{0} ({1})", station.StationName, station.StationID);
            //    var node = new CTreeNode(text, CTreeType.CeZhan3, station.StationID);
            //    //node.SetImageIndex(14);
            //    if(station.StationType == EStationType.ERainFall)
            //    {
            //        node.SetImageIndex(18);
            //    }else if (station.StationType == EStationType.ERiverWater)
            //    {
            //        node.SetImageIndex(20);
            //    }else if (station.StationType == EStationType.EHydrology)
            //    {
            //        node.SetImageIndex(22);
            //    }

            //    return node;
            //}
            
            public static CTreeNode BuildStationNode(object obj)
            {
                //  if (obj.ToString() == "Hydrology.Entity.CEntityStation")
                if (obj is CEntityStation)
                {
                    var station = obj as CEntityStation;

                    string text = string.Format("{0} ({1})", station.StationName, station.StationID);
                    var node = new CTreeNode(text, CTreeType.CeZhan3, station.StationID);

                    //node.SetImageIndex(14);
                    if (station.StationType == EStationType.ERainFall)
                    {
                        node.SetImageIndex(18);
                    }
                    else if (station.StationType == EStationType.ERiverWater)
                    {
                        node.SetImageIndex(20);
                    }
                    else if (station.StationType == EStationType.EHydrology)
                    {
                        node.SetImageIndex(22);
                    }
                    return node;
                }
                else
                {
                    var station = obj as CEntitySoilStation;

                    string text = string.Format("{0} ({1})", station.StationName, station.StationID);
                    var node = new CTreeNode(text, CTreeType.CeZhan3, station.StationID);

                    node.SetImageIndex(14);
                    return node;
                }

            }

        }
    }
}
