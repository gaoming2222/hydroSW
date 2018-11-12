using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Hydrology.Entity
{
    /// <summary>
    /// 多线程的封装类，可用于子类继承，提供一个管理数组，也可作为成员变量
    /// </summary>
    public class CMultiTaskWrapper
    {
        private List<Task> m_listCurrentTask; //当前所有的任务列表，便于退出时候等待
        private Mutex m_mutexTaskList; //任务列表互斥量 
        private bool m_bStopServer; // 停止服务

        public CMultiTaskWrapper()
        {
            m_listCurrentTask = new List<Task>();
            m_mutexTaskList = new Mutex();
            m_bStopServer = false;
        }

        // 当前数据库任务完成的通知
        public virtual void TaskEndAction(Task task)
        {
            m_mutexTaskList.WaitOne();
            if (m_listCurrentTask.Contains(task))
            {
                //task.Wait();
                m_listCurrentTask.Remove(task);
                //Debug.WriteLine(string.Format("处理数据线程结束{0},状态{1}", m_listCurrentTask.Count, task.Status));
                System.Diagnostics.Debug.Write("|");
            }
            m_mutexTaskList.ReleaseMutex();
        }

        /// <summary>
        /// 开启一个线程，使用任务模式完成某件事情
        /// </summary>
        /// <param name="action"></param>
        public virtual void NewTask(Action action)
        {
            if (m_bStopServer)
            {
                // 不再接受任务
                System.Diagnostics.Debug.WriteLine("drop new task");
                return;
            }
            m_mutexTaskList.WaitOne();
            //Task task = new Task(action);
            Task task = Task.Factory.StartNew(action);
            m_listCurrentTask.Add(task);
            //task.Start();
            task.ContinueWith(this.TaskEndAction, TaskContinuationOptions.OnlyOnRanToCompletion);
            m_mutexTaskList.ReleaseMutex();
        }

        /// <summary>
        /// 阻塞被调用线程，直至所有的任务都已经完成
        /// </summary>
        public virtual void Close()
        {
            System.Diagnostics.Debug.WriteLine("尝试停止当前任务");
            m_bStopServer = true; //停止接收任何数据
            // 先等待当前的任务完成,此时不再接受任何其它消息
            while (m_listCurrentTask.Count > 0)
            {
                Task.WaitAll(m_listCurrentTask.ToArray());
            }
            System.Diagnostics.Debug.WriteLine("成功停止当前任务");
        }
    }
}
