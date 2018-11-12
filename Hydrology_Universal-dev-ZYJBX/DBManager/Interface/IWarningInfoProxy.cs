using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface IWarningInfoProxy : IMultiThread
    {
        /// <summary>
        /// 异步写入数据库，考虑延时以及数据量
        /// </summary>
        /// <param name="entity"></param>
        void AddNewRow(CEntityWarningInfo entity);

        /// <summary>
        /// 添加告警信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Add(CEntityWarningInfo entity);

        /// <summary>
        /// 添加告警信息集合
        /// </summary>
        /// <param name="listEntitys"></param>
        /// <returns></returns>
        bool AddRange(List<CEntityWarningInfo> listEntitys);

        /// <summary>
        /// 查询告警信息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        List<CEntityWarningInfo> QueryWarningInfo(DateTime startTime, DateTime endTime);

        /// <summary>
        /// 删除告警信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteWarningInfo(long id);

        /// <summary>
        /// 删除系列告警信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteRange(List<long> id);

        /// <summary>
        /// 删除时间段内的所有报警信息
        /// </summary>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        bool DeleteRange(DateTime timeStart, DateTime timeEnd);
    }
}
