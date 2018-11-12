using System;
using System.Windows.Forms;
using Hydrology.DataMgr;
using Hydrology.Entity;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

namespace Hydrology.Forms
{
    public class FormHelper
    {

        public static MainForm MainFormRef;

        /// <summary>
        /// 当前的弹出窗体
        /// </summary>
        public static Form CurrentForm = null;
        /// <summary>
        /// 用户退出登录的菜单项事件
        /// </summary>
        public static void UserLogOut(object sender, EventArgs e)
        {
            CSystemInfoMgr.Instance.AddInfo("用户退出登录");
            MainFormRef.Logout();
        }

        /// <summary>
        /// 退出系统
        /// </summary>
        public static void SysExit(object sender, EventArgs e)
        {
            MainFormRef.Close();
        }

        /// <summary>
        /// 弹出系统菜单窗体
        /// </summary>
        public static void ShowForm(object sender, EventArgs e)
        {
            try
            {
                var item = sender as ToolStripItem;
                if (item != null && item.Tag != null)
                {
                    var form = GetForm(item.Tag.ToString());

                    if (form != null)
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (System.OverflowException ex)
            {
                //MessageBox.Show("操作非法,请参考帮助手册");
                CSystemInfoMgr.Instance.AddInfo(ex.ToString(), false);
                Debug.WriteLine(ex.ToString());
                CPortDataMgr.Instance.StopGprs();
                CPortDataMgr.Instance.StartGprs();
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("操作非法,请参考帮助手册");
                CSystemInfoMgr.Instance.AddInfo(ex.ToString(), false);
                Debug.WriteLine(ex.ToString());
                CPortDataMgr.Instance.StopGprs();
                CPortDataMgr.Instance.StartGprs();
            }
        }

        private static Form GetForm(String tag)
        {
            Form form = null;
            switch (tag)
            {
                case "数据协议配置":
                    form = new CProtocolConfigForm(false);
                    (form as CProtocolConfigForm).ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                    break;
                case "通讯方式配置":
                    form = new CProtocolConfigForm();
                    (form as CProtocolConfigForm).ProtocolConfigChanged += MainForm.ProtocolConfigChanged;
                    break;
                case "串口配置":
                    form = new CSerialPortConfigForm();
                    break;
                case "数据库配置":
                    form = new CDatabaseConfigForm();
                    break;
                case "用户登陆":
                    {
                        form = new CLoginForm();
                        (form as CLoginForm).UserModeChanged += new EventHandler<CEventSingleArgs<bool>>(MainFormRef.EHUserModeChanged);
                    }
                    break;
                case "用户管理":
                    form = new CUserMgrForm();
                    break;
                case "分中心管理":
                    form = new CSubStationMgrForm();
                    break;
                case "测站管理":
                    form = new CStationMgrForm();
                    break;
                case "墒情站配置":
                    {
                        form = new CSoilStationMgrForm();
                        break;
                    }
                case "历史数据查询":
                    form = new CStationDataMgrForm()
                    {
                        Editable = false
                    };
                    break;
                case "读取与设置":
                    form = new CReadAndSettingMgrFormNew();
                    break;
                //       case "U盘传输":
                //           form = new CBatchUDiskMgrForm();
                //           break;
                case "批量传输":
                    form = new CBatchFlashMgrForm();
                    break;
                case "历史数据校正":
                    form = new CStationDataMgrForm()
                    {
                        Editable = true
                    };
                    break;
                case "告警信息查询":
                    {
                        form = new CWarningInfoQuery();
                        //判断是不是管理员
                        if (CCurrentLoginUser.Instance.IsAdmin)
                        {
                            // 是管理员可以编辑
                            (form as CWarningInfoQuery).Editable = true;
                        }
                    }
                    break;
                case "调试信息查询":
                    form = new DebugInfoSearch();
                    break;
                case "GPRS":
                    form = new CGprsNewMgrForm();
                    break;
                case "北斗卫星":
                    form = new CBeidouNewMgrForm();
                    break;
                case "水位流量管理":
                    form = new CWaterFlowMapForm();
                    break;
                //case "畅通率报表":
                //    //form = new FlowRateReport(); // 畅通率报表
                //    //form = new MoreStationsDayForm(); //多站日报表
                //    //form = new Form2();    //单站月报表
                //    form = new MoreStationsMonthForm(); //多站月报表
                //    break;
                case "声音配置":
                    form = new CVoiceConfigForm();
                    break;

                case "批量远程":
                    form = new CBatchManagement();
                    break;

                case "北斗卫星指挥机终端配置":
                    form = new CBeidou500MgrForm();
                    break;
                case "系统对时":
                  //  form = new CStationSystemClockForm();
                    form = new CBatchManagement();
                 //   form = new Form1();
                    break;
                case "多站日":
                    //form = new CVoiceConfigForm();
                    form = new MoreStationsDayForm();
                    break;
                case "多站月":
                    //form = new CVoiceConfigForm();
                    form = new MoreStationsMonthForm();
                    break;
                case "单站月":
                    //form = new CVoiceConfigForm();
                    form = new Form2();
                    break;
                case "畅通率":
                    //form = new CVoiceConfigForm();
                    form = new FlowRateReport();
                    break;
                case "单站年":
                    //form = new CVoiceConfigForm();
                    form = new OneStationYear();
                    break;
                //gm 1203
                case "关于":
                    MessageBox.Show("水文自动监测管理系统 v1.0" + "\r\n" + "湖北一方科技发展有限责任公司");
                    break;
                case "帮助":
                    System.Diagnostics.Process.Start(Application.StartupPath.ToString() + "\\help.pdf");
                    break;

                default:
                    form = null;
                    break;
            }
            if (form != null)
            {
                //  设置窗体相关属性
                form.StartPosition = FormStartPosition.CenterParent;
            }

            return form;
        }

        #region 焦点相关
        /// <summary>
        /// 用来获取焦点的帮助方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void EHControlClickAndFocus(object sender, EventArgs e)
        {
            try
            {
                (sender as Control).Focus();
            }
            catch (System.Exception)
            {

            }
        }
        /// <summary>
        /// 初始化控件的单击事件,用于焦点切换
        /// </summary>
        /// <param name="control"></param>
        public static void InitControlFocusLoop(Control control)
        {
            // 避免空值
            if (control == null)
            {
                return;
            }
            if (control.HasChildren)
            {
                for (int i = 0; i < control.Controls.Count; ++i)
                {
                    try
                    {
                        if (control.Controls[i] is Panel)
                        {
                            control.Controls[i].Click += FormHelper.EHControlClickAndFocus;
                        }
                        else if (control.Controls[i] is Label)
                        {
                            control.Controls[i].Click += FormHelper.EHControlClickAndFocus;
                        }
                        else if (control.Controls[i] is GroupBox)
                        {
                            control.Controls[i].Click += FormHelper.EHControlClickAndFocus;
                        }
                        else if (control.Controls[i] is Form)
                        {
                            control.Controls[i].Click += FormHelper.EHControlClickAndFocus;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    // 递归循环每个控件
                    InitControlFocusLoop(control.Controls[i]);
                }
            }
        }
        #endregion 焦点相关

        #region 用户权限相关
        public static void InitUserModeEvent(Form form)
        {
            CurrentForm = form;
        }
        /// <summary>
        /// 用户权限发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void EHUserModeChanged(object sender, CEventSingleArgs<bool> args)
        {
            if (args.Value)
            {
                // 只处理由管理员退出为普通用户的情况
                return;
            }
            try
            {
                if (CurrentForm != null && CurrentForm.IsHandleCreated)
                {
                    // 提示，并强行退出当前对话框
                    string eventname = "FormClosing";

                    BindingFlags mPropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                    Type controlType = CurrentForm.GetType();
                    PropertyInfo propertyInfo = controlType.GetProperty("Events", mPropertyFlags);
                    BindingFlags mFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;
                    EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(CurrentForm, null);
                    // From必须这样写EVENT_
                    FieldInfo fieldInfo = typeof(Form).GetField("EVENT_" + eventname.ToUpper(), mFieldFlags);

                    Delegate d = eventHandlerList[fieldInfo.GetValue(CurrentForm)];

                    if (d != null)
                    {
                        // 如果绑定了事件
                        EventInfo eventInfo = controlType.GetEvent(eventname);

                        //解除绑定
                        foreach (Delegate dx in d.GetInvocationList())
                        {
                            eventInfo.RemoveEventHandler(CurrentForm, dx);
                        }
                    }

                    MessageBox.Show("管理员权限过期,请重新登录");
                    CurrentForm.Close();
                    CurrentForm.DialogResult = DialogResult.Abort;
                    CurrentForm = null;
                    return;
                    #region 解绑定事件


                    //FieldInfo[] fields = typeof(Control).GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                    //foreach (FieldInfo fi in fields)
                    //{
                    //    Debug.WriteLine(string.Format("FieldName: {0}, Accessibility: {1}, Has Attributes: {2}.", fi.Name, fi.Attributes, fi.GetCustomAttributes(true).Length != 0));
                    //}

                    ////var fi = CurrentForm.GetType().GetEventField("FormClosing");
                    ////if (fi == null) return;
                    ////fi.SetValue(obj, null);
                    //
                    //if (CurrentForm == null) return;
                    //if (string.IsNullOrEmpty(eventname)) return;

                    //BindingFlags mPropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                    //BindingFlags mFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;
                    //Type controlType = CurrentForm.GetType();
                    //PropertyInfo propertyInfo = controlType.GetProperty("Events", mPropertyFlags);
                    //EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(CurrentForm, null);
                    //FieldInfo fieldInfo = typeof(Form).GetField("Event" + "Load", mFieldFlags);
                    //Delegate d = eventHandlerList[fieldInfo.GetValue(CurrentForm)];



                    //if (d == null) return;
                    //EventInfo eventInfo = controlType.GetEvent(eventname);

                    //foreach (Delegate dx in d.GetInvocationList())
                    //    eventInfo.RemoveEventHandler(CurrentForm, dx);

                    ////EventInfo eventInfo = CurrentForm.GetType().GetEvent("FormClosing");


                    //FieldInfo f1 = typeof(Form).GetField("EventFormClosing", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                    ////object obj = f1.GetValue(CurrentForm);
                    //PropertyInfo pi = CurrentForm.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance );
                    //EventHandlerList list = (EventHandlerList)pi.GetValue(CurrentForm, null);
                    ////list.RemoveHandler(obj, list[obj]);

                    //EventInfo[] events = CurrentForm.GetType().GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    //for (int i = 0; i < events.Length; i++)
                    //{
                    //    EventInfo ei = events[i];

                    //    if (ei.Name == eventname)
                    //    {
                    //        FieldInfo fi = ei.DeclaringType.GetField("printPageHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    //        if (fi != null)
                    //        {
                    //            fi.SetValue(CurrentForm, null);
                    //        }

                    //        break;
                    //    }
                    //}

                    #endregion 解绑定事件

                }
            }
            catch
            {
            }
        }
        #endregion 用户权限相关
    }

}
