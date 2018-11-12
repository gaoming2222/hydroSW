using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface ISubCenterProxy : IMultiThread
    {
        /// <summary>
        /// 批量添加分中心
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool AddRange(List<CEntitySubCenter> items);

        /// <summary>
        /// 更新分中心信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool UpdateRange(List<CEntitySubCenter> items);

        /// <summary>
        /// 删除分中心
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool DeleteRange(List<int> items);

        /// <summary>
        /// 查询所有分中心信息
        /// </summary>
        /// <returns></returns>
        List<CEntitySubCenter> QueryAll();

        //gm2017_02
        //bool AddSubRain(List<CEntitySubCenter> items);
    }
}
