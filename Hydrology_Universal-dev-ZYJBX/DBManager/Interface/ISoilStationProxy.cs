using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    /// <summary>
    /// 墒情站附加信息接口定义
    /// </summary>
    public interface ISoilStationProxy
    {
        /// <summary>
        /// 添加一个墒情站记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool AddNewRow(CEntitySoilStation entity);

        /// <summary>
        /// 添加系列上墒情站记录
        /// </summary>
        /// <param name="listStation"></param>
        /// <returns></returns>
        bool AddSoilStationRange(List<CEntitySoilStation> listStation);

        /// <summary>
        /// 删除系列墒情站点
        /// </summary>
        /// <param name="listStationId"></param>
        /// <returns></returns>
        bool DeleteSoilStationRange(List<string> listStationId);

        /// <summary>
        /// 更细系列墒情站点
        /// </summary>
        /// <param name="listStation"></param>
        /// <returns></returns>
        bool UpdateSoilStation(List<CEntitySoilStation> listStation);

        /// <summary>
        /// 查询所有的墒情站点
        /// </summary>
        /// <returns></returns>
        List<CEntitySoilStation> QueryAllSoilStation();

        //List<CEntityStation> QueryByGprs(string gpreID);

        List<string> getAllGprs();

        List<CEntitySoilStation> getAllGprs_1();

    }
}
