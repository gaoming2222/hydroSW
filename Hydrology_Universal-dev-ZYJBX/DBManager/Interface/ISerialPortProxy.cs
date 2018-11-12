using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface ISerialPortProxy : IMultiThread
    {
        /// <summary>
        /// 添加串口集合
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool AddRange(List<CEntitySerialPort> listPort);

        /// <summary>
        /// 更新串口信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool UpdateRange(List<CEntitySerialPort> listPort);

        /// <summary>
        /// 删除串口信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool DeleteRange(List<int> listPortID);

        /// <summary>
        /// 查询所有串口
        /// </summary>
        /// <returns></returns>
        List<CEntitySerialPort> QueryAll();
    }
}
