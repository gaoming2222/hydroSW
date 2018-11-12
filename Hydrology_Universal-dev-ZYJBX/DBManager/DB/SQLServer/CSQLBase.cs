using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace Hydrology.DBManager.DB.SQLServer
{
    public abstract class CSQLBase
    {
        protected enum EDBOperationType { Insert, Delete, Update, QueryAll }; //数据库操作枚举
        protected static readonly string CN_RowId = "rowid";         //分页用来区别的列名

        protected Dictionary<int, DataTable> m_mapDataTable = null;       // 查询数据缓存区域,int起始索引
        protected DataTable m_tableDataAdded;                            // 添加的数据值的表
        protected DataTable m_tableDataUpdated;                         // 更新操作的数据值的表

        protected System.Timers.Timer m_addTimer;     // 添加数据的定时器
        protected System.Timers.Timer m_updateTimer;  // 更新数据的定时器
        protected DateTime m_dateTimePreAddTime;      // 上一次添加记录的时间
        protected DateTime m_dateTimePreUpdateTime;   // 上一次更新记录的时间


        // 线程安全成员变量
        protected Mutex m_mutexDataTable; //增加记录的内存互斥量
        public /*static*/ Mutex m_mutexWriteToDB; //提交到数据库线程锁
        protected Mutex m_mutexListTask;  // ListOfTask的互斥锁
        protected List<Task> m_listTask;  //当前的所有任务
        protected CancellationTokenSource m_cancelTokenSource; //取消任务的标记

        // 分页查询相关
        protected int m_iPageCount;       //总页面数
        protected int m_iRowCount;        //总行数

        // 定义事件
        public event EventHandler DBWorkDone; // 数据库空闲事件

        /// <summary>
        /// 静态构造函数，用于初始化静态变量
        /// </summary>
        static CSQLBase()
        {
            //S_MutexWriteToDB = new Mutex();
        }

        public CSQLBase()
        {
            m_tableDataAdded = new DataTable();
            m_tableDataUpdated = new DataTable();
            m_mapDataTable = new Dictionary<int, DataTable>();
            m_listTask = new List<Task>();

            // 初始化互斥量
            m_mutexDataTable = new Mutex();
            // m_mutexWriteToDB = new Mutex(); //各自自己初始化
            m_mutexListTask = new Mutex();
            m_cancelTokenSource = new CancellationTokenSource();

            // 初始化定时器
            m_addTimer = new System.Timers.Timer();
            m_addTimer.Elapsed += new System.Timers.ElapsedEventHandler(EHTimer);
            m_addTimer.Enabled = false;
            m_addTimer.Interval = CDBParams.GetInstance().AddToDbDelay;
        }

        /// <summary>
        /// 支持多线程的关闭方法
        /// </summary>
        public virtual void Close()
        {
            // 关闭当前服务
            // 停止定时器
            m_addTimer.Stop();
            // 终止Task
            m_cancelTokenSource.Cancel(); //终止线程
            // 写入数据
            AddDataToDB();
        }

        #region 事件处理
        /// <summary>
        /// 定时器事件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void EHTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            //定时器事件，将所有的记录都写入数据库
            m_addTimer.Stop();  //停止定时器
            m_dateTimePreAddTime = DateTime.Now;
            //将数据写入数据库
            NewTask(() => { AddDataToDB(); });
        }

        /// <summary>
        /// 开启一个线程，使用任务模式完成某件事情
        /// </summary>
        /// <param name="action"></param>
        protected virtual void NewTask(Action action)
        {
            m_mutexListTask.WaitOne();
            //Task task = new Task(action);
            Task task = Task.Factory.StartNew(action, m_cancelTokenSource.Token);
            m_listTask.Add(task);
            task.ContinueWith(this.TaskEndAction, TaskContinuationOptions.OnlyOnRanToCompletion);
            //task.Start();
            m_mutexListTask.ReleaseMutex();
        }


        #endregion

        /// <summary>
        /// 将内存中的数据写入数据库
        /// </summary>
        protected abstract bool AddDataToDB();

        // 将时间转换成SQLSERVER中的时间字符串
        protected string DateTimeToDBStr(DateTime time)
        {
            return " CAST(\'" + time.ToString(CDBParams.GetInstance().DBDateTimeFormat) + "\' as datetime) ";
        }

        // 执行当前的SQL命令
        protected bool ExecuteSQLCommand(string cmd)
        {
            // 获取对数据的的实例唯一访问权
            m_mutexWriteToDB.WaitOne();
            SqlConnection conn = CDBManager.GetInstacne().GetConnection();
            try
            {
                conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = cmd;
                int lines = command.ExecuteNonQuery();
                Debug.WriteLine("{0}行收到影响", lines);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                m_mutexWriteToDB.ReleaseMutex();
                conn.Close();
            }
            return false;
        }

        // 当前数据库任务完成的通知
        protected virtual void TaskEndAction(Task task)
        {
            m_mutexListTask.WaitOne();
            if (m_listTask.Contains(task))
            {
                m_listTask.Remove(task);
            }
            if (0 == m_listTask.Count)
            {
                // 发消息，通知空闲
                if (DBWorkDone != null)
                {
                    DBWorkDone.Invoke(this, new EventArgs());
                }
            }
            Debug.Write("-");
            m_mutexListTask.ReleaseMutex();
        }

        protected void ExecuteNonQuery(String sqlText)
        {
            var sqlConn = CDBManager.GetInstacne().GetConnection();
            try
            {
                m_mutexWriteToDB.WaitOne();         // 取对数据库的唯一访问权
                //m_mutexDataTable.WaitOne();         // 获取内存表的访问权
                sqlConn.Open();                     // 建立数据库连接

                //  查询
                SqlCommand sqlCmd = new SqlCommand(sqlText, sqlConn);
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                sqlConn.Close();                    //  关闭数据库连接
                m_mutexDataTable.ReleaseMutex();    //  释放内存表的访问权
                m_mutexWriteToDB.ReleaseMutex();    //  释放数据库的访问权
            }
        }

        /// <summary>
        /// 生成字符串如：{0}={1},{2}={3},不带空格
        /// </summary>
        /// <returns></returns>
        protected string GenerateSQL(int iColumnCount, int iStartNumber = 0)
        {
            StringBuilder sql = new StringBuilder();
            for (int i = 0; i < iColumnCount; ++i)
            {
                sql.AppendFormat("{{{0}}}={{{1}}}", i * 2 + iStartNumber, i * 2 + 1 + iStartNumber);
                if (i != iColumnCount - 1)
                {
                    sql.Append(",");
                }
            }
            return sql.ToString();
        }

        /// <summary>
        /// 获取Nullable的SQL字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetNullableSQLString(Nullable<decimal> value)
        {
            return value.HasValue ? value.Value.ToString() : "null";
        }

        /// <summary>
        /// 获取Nullable的SQL字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetNullableSQLString(Nullable<float> value)
        {
            return value.HasValue ? value.Value.ToString() : "null";
        }

        protected Nullable<decimal> GetCellDecimalValue(object cell)
        {
            Nullable<decimal> result = null;
            if (!cell.ToString().Equals(""))
            {
                result = Decimal.Parse(cell.ToString());
            }
            return result;
        }

        protected Nullable<float> GetCellFloatValue(object cell)
        {
            Nullable<float> result = null;
            if (!cell.ToString().Equals(""))
            {
                result = float.Parse(cell.ToString());
            }
            return result;
        }
    }
}
