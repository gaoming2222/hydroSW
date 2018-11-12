using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface IWaterFlowMapProxy:IMultiThread
    {
        /// <summary>
        /// 添加多条水位流量关系表记录
        /// </summary>
        /// <param name="listEntitys"></param>
        /// <returns></returns>
        bool AddRange(List<CEntityWaterFlowMap> listEntitys);

        /// <summary>
        /// 根据站点ID查询该站点的水位流量一体线条
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        List<CEntityWaterFlowMap> QueryMapsByStationId(string stationId);

        /// <summary>
        /// 更新水位流量一体线
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        bool UpdateRange(List<CEntityWaterFlowMap> listEntity); 

        /// <summary>
        /// 删除雨量记录
        /// </summary>
        /// <param name="listIds"></param>
        /// <returns></returns>
        //bool DeleteRange(List<long> listIds);
        bool DeleteRange(List<CEntityWaterFlowMap> listEntity);

        bool DeleteLine(CEntityWaterFlowMap Entity);
    }
}
