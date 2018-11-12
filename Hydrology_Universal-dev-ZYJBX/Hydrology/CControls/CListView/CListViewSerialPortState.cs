using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.Entity;

namespace Hydrology.CControls
{
    /// <summary>
    /// 显示串口状态的ListView
    /// </summary>
    class CListViewSerialPortState : CExListView
    {
        public enum EMsgState { ENormal, EWarning, EError };
        public CListViewSerialPortState()
            : base()
        {
            // 设置图片，默认样式
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(20, 20);
            imgList.Images.Add(Hydrology.Properties.Resources.COM_NORMAL);
            imgList.Images.Add(Hydrology.Properties.Resources.COM_ERROR);
            this.LargeImageList = imgList;
            this.View = View.LargeIcon;
            LoadSerialPort();
        }
        public void LoadSerialPort()
        {
            // 导入串口， 默认都是正常的，清空以前的串口
            List<CEntitySerialPort> list = CDBDataMgr.Instance.GetAllSerialPort();
            List<ListViewItem> listItems = new List<ListViewItem>();
            foreach (CEntitySerialPort port in list)
            {
                ListViewItem item = new ListViewItem() { Text = GetCommUIString(port.PortNumber) };
                Color forcolor = Color.Black;
                item.ImageIndex = 1; //默认错误
                listItems.Add(item);
            }
            // 更新界面
            // 多线程，调用Invoke更新界面
            if (this.IsHandleCreated)
            {
                this.Invoke((Action)delegate()
                {
                    this.Items.Clear();   //清空以前的额串口
                    this.Items.AddRange(listItems.ToArray());
                });

            }
            else
            {
                this.Items.Clear();   //清空以前的额串口
                this.Items.AddRange(listItems.ToArray());
            }
        }

        public void UpdateSerialPortState(int portNumber, EMsgState state)
        {
            // 更新某个串口状态，如果没找到，写入系统日志
            this.SuspendLayout();
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (ListViewItem item in this.Items)
                    {
                        if (item.Text == GetCommUIString(portNumber))
                        {
                            //找到匹配，更新图片颜色
                            switch (state)
                            {
                                case EMsgState.ENormal: { item.ImageIndex = 0; item.ForeColor = Color.Black; } break;
                                case EMsgState.EWarning: { item.ForeColor = Color.Yellow; item.ImageIndex = 1; } break;
                                case EMsgState.EError: { item.ForeColor = Color.Red; item.ImageIndex = 1; } break;
                            }
                            CSystemInfoMgr.Instance.AddInfo(string.Format("更新串口COM{0}状态为{1}", portNumber, GetLogStrOfMsgState(state)),false);
                            return;
                        }
                    }
                    CSystemInfoMgr.Instance.AddInfo(string.Format("更新串口COM{0}失败，找不到串口COM{1}", portNumber, portNumber), false);
                }));
            }
            else
            {
                foreach (ListViewItem item in this.Items)
                {
                    if (item.Text == GetCommUIString(portNumber))
                    {
                        //找到匹配，更新图片颜色
                        switch (state)
                        {
                            case EMsgState.ENormal: { item.ImageIndex = 0; item.ForeColor = Color.Black; } break;
                            case EMsgState.EWarning: { item.ForeColor = Color.Yellow; item.ImageIndex = 1; } break;
                            case EMsgState.EError: { item.ForeColor = Color.Red; item.ImageIndex = 1; } break;
                        }
                        CSystemInfoMgr.Instance.AddInfo(string.Format("更新串口COM{0}状态为{1}", portNumber, GetLogStrOfMsgState(state)), false);
                        return;
                    }
                }
                CSystemInfoMgr.Instance.AddInfo(string.Format("更新串口COM{0}失败，找不到串口COM{1}", portNumber, portNumber), false);
            }
            
            this.ResumeLayout(false);
            
        }

        public void EHSerialPortChanged(object sender, EventArgs args)
        {
            // 重新加载数据
            this.LoadSerialPort();
        }
        public void EHSerialPortStateUpdated(object sender, CEventSingleArgs<CSerialPortState> arg)
        {
            if (arg.Value.PortType == EListeningProtType.SerialPort)
            {
                //  更新串口    
                if (arg.Value.BNormal)
                {
                    this.UpdateSerialPortState(arg.Value.PortNumber, EMsgState.ENormal);
                }
                else
                {
                    this.UpdateSerialPortState(arg.Value.PortNumber, EMsgState.EError);
                }
            }
            else if (arg.Value.PortType == EListeningProtType.Port)
            {
                //  更新端口

            }
        }

        #region 帮助方法

        private string GetCommUIString(int portNumber)
        {
            return string.Format("COM{0}", portNumber);
        }
        private string GetLogStrOfMsgState(EMsgState state)
        {
            switch (state)
            {
                case EMsgState.ENormal: { return "正常"; }
                case EMsgState.EWarning: { return "警告"; }
                case EMsgState.EError: { return "掉线"; }
            }
            throw new Exception("GetLogStrOfMsgState Error");
        }
        #endregion 帮助方法
    }
}
