
namespace Hydrology.DBManager.Interface
{
    /// <summary>
    /// 多线程的数据库接口
    /// </summary>
    public interface IMultiThread
    {
        /// <summary>
        /// 关闭事件，阻塞调用该方法的线程，直到数据库操作玩曾
        /// </summary>
        void Close();
    }
}
