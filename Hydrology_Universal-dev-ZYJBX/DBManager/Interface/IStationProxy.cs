using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface IStationProxy : IMultiThread
    {
        /// <summary>
        /// 批量添加测站
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool AddRange(List<CEntityStation> items);

        /// <summary>
        /// 更新测站信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool UpdateRange(List<CEntityStation> items);

        /// <summary>
        /// 删除测站
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool DeleteRange(List<string> items);

        /// <summary>
        /// 查询所有分中心信息
        /// </summary>
        /// <returns></returns>
        List<CEntityStation> QueryAll();

        string QueryGPRSById(string id);

        CEntityStation QueryByGprs(string gprsID);

        CEntityStation QueryById(string stationid);

        List<string> getAllGprs();

        List<CEntityStation> getAllGprs_1();
    }
}
