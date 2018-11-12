using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hydrology.Entity;

namespace Hydrology.DataMgr
{
    /// <summary>
    /// 系统信息的管理类
    /// </summary>
    class CSystemInfoMgr
    {
        #region 事件定义
        /// <summary>
        /// 收到新的系统信息事件
        /// </summary>
        public event EventHandler<CEventSingleArgs<CTextInfo>> RecvedNewSystemInfo;

        #endregion 事件定义

        #region 成员变量
        private List<CTextInfo> m_listTextInfo;

        // 定时器,写入延迟
        private System.Timers.Timer m_timer = null;

        // 信息列表的互斥量
        private Mutex m_mutexListInfo;

        // 文件的互斥量
        private Mutex m_mutexWriteToFile;

        // 内存中记录的最大值
        private int m_iMaxListCount;

        // 文件夹名
        private string m_strLogFileName;

        #endregion 成员变量

        #region 单例模式
        private CSystemInfoMgr()
        {
            //初始化互斥量
            m_mutexListInfo = new Mutex();
            m_mutexWriteToFile = new Mutex();

            m_strLogFileName = "log";
            m_iMaxListCount = 1000;     // 1000条内存缓存

            m_listTextInfo = new List<CTextInfo>();

            //初始化定时器
            m_timer = new System.Timers.Timer();
            m_timer.Elapsed += new System.Timers.ElapsedEventHandler(EHTimer);
            m_timer.Interval = 1 * 60 * 1000; //5分钟的写入延迟
            m_timer.Enabled = false;

        }
        private static CSystemInfoMgr m_instance;
        public static CSystemInfoMgr Instance
        {
            get
            {
                if (null == m_instance)
                {
                    m_instance = new CSystemInfoMgr();
                }
                return m_instance;
            }
        }
        #endregion 单例模式

        #region 公共方法
        public void AddInfo(string info, DateTime time, ETextMsgState state = ETextMsgState.ENormal, bool bNotity = true)
        {
            m_mutexListInfo.WaitOne();
            m_listTextInfo.Add(new CTextInfo() { Time = time, Info = info });
            m_mutexListInfo.ReleaseMutex();
            if (m_listTextInfo.Count > m_iMaxListCount)
            {
                Task task = new Task(() => { WriteInfoToFile(); });
                task.Start();
            }
            else
            {
                // 开启定时器
                m_timer.Start();
            }
            if (bNotity)
            {
                if (null != RecvedNewSystemInfo)
                {
                    // 通知其它订阅者
                    Task.Factory.StartNew(() =>
                    {
                        RecvedNewSystemInfo.Invoke(this, new CEventSingleArgs<CTextInfo>(new CTextInfo() { Time = time, Info = info, EState = state }));
                    });
                }
            }
        }

        public void AddInfo(string info, bool bnotify = true)
        {
            AddInfo(info, DateTime.Now, ETextMsgState.ENormal, bnotify);
        }

        public void Close()
        {
            // 关闭,将当前的所有修改都写入文件
            m_timer.Stop();
            WriteInfoToFile_all();
        }

        /// <summary>
        /// 收到系统信息消息，由外部程序发出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void EHRecvSystemInfo(object sender, CEventSingleArgs<CTextInfo> args)
        {
            this.AddInfo(args.Value.Info, args.Value.Time);
        }
        #endregion  公共方法

        #region 帮助方法

        private void EHTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            //定时器事件，由于初始化的时候是5秒刷新一次，所以，这里直接刷新界面，并停止定时器
            m_timer.Stop();
            try
            {
                WriteInfoToFile();
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message.ToString());
            }
        }

        // 写入文件
        private void WriteInfoToFile()
        {
            DateTime dt1 = DateTime.Now;
            int year = dt1.Year;
            int month = dt1.Month;
            int day = dt1.Day;
            int hour = dt1.Hour;
            DateTime dt2 = new DateTime(year, month, day, hour, 0, 0);
            TimeSpan ts = dt1 - dt2;
            int minute = ts.Minutes;
            //if (minute > 10 && minute < 55)
            //{
            List<CTextInfo> listInfo = null;
            m_mutexListInfo.WaitOne();
            if (m_listTextInfo.Count > 0)
            {
                listInfo = m_listTextInfo;
                m_listTextInfo = new List<CTextInfo>();
            }
            else
            {
                // 没有任何东西可以写入
                m_mutexListInfo.ReleaseMutex();
                return;
            }
            m_mutexListInfo.ReleaseMutex();

            try
            {
                m_mutexWriteToFile.WaitOne();
                // 判断log文件夹是否存在
                if (!Directory.Exists(m_strLogFileName))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(m_strLogFileName);
                }
                string filename = "Log" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                string path = m_strLogFileName + "/" + filename;
                if (!File.Exists(path))
                {
                    // 不存在文件，新建一个
                    FileStream fs = new FileStream(path, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (CTextInfo info in listInfo)
                    {
                        //开始写入
                        sw.WriteLine(String.Format("{0}  {1}", info.Time.ToString("HH:mm:ss"), info.Info));
                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    // 添加到现有文件
                    FileStream fs = new FileStream(path, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    foreach (CTextInfo info in listInfo)
                    {
                        //开始写入
                        sw.WriteLine(String.Format("{0}  {1}", info.Time.ToString("HH:mm:ss"), info.Info));
                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                m_mutexWriteToFile.ReleaseMutex();
            }
            //}
            //else
            //{
            //    Debug.Write("稍后在写入");
            //}

        }


        private void WriteInfoToFile_all()
        {
            List<CTextInfo> listInfo = null;
            m_mutexListInfo.WaitOne();
            if (m_listTextInfo.Count > 0)
            {
                listInfo = m_listTextInfo;
                m_listTextInfo = new List<CTextInfo>();
            }
            else
            {
                // 没有任何东西可以写入
                m_mutexListInfo.ReleaseMutex();
                return;
            }
            m_mutexListInfo.ReleaseMutex();

            try
            {
                m_mutexWriteToFile.WaitOne();
                // 判断log文件夹是否存在
                if (!Directory.Exists(m_strLogFileName))
                {
                    // 创建文件夹
                    Directory.CreateDirectory(m_strLogFileName);
                }
                string filename = "Log" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                string path = m_strLogFileName + "/" + filename;
                if (!File.Exists(path))
                {
                    // 不存在文件，新建一个
                    FileStream fs = new FileStream(path, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (CTextInfo info in listInfo)
                    {
                        //开始写入
                        sw.WriteLine(String.Format("{0}  {1}", info.Time.ToString("HH:mm:ss"), info.Info));
                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    // 添加到现有文件
                    FileStream fs = new FileStream(path, FileMode.Append);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    foreach (CTextInfo info in listInfo)
                    {
                        //开始写入
                        sw.WriteLine(String.Format("{0}  {1}", info.Time.ToString("HH:mm:ss"), info.Info));
                    }
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                m_mutexWriteToFile.ReleaseMutex();
            }
        }
        #endregion
    }
}
