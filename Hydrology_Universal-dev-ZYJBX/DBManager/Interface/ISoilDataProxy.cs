using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    /// <summary>
    /// 墒情站数据接口定义
    /// </summary>
    public interface ISoilDataProxy : IMultiThread
    {
        /// <summary>
        /// 异步添加一行墒情数据
        /// </summary>
        /// <param name="entity"></param>
        void AddNewRow(CEntitySoilData entity);

        /// <summary>
        /// 异步添加系列墒情数据
        /// </summary>
        /// <param name="listData"></param>
        void AddSoilDataRange(List<CEntitySoilData> listData);

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="rains"></param>
        /// <returns></returns>
        bool UpdateRows(List<CEntitySoilData> listData);

        bool DeleteRows(List<String> soildatas_StationId, List<String> soildatas_StationDate);

        /// <summary>
        /// 查询墒情历史数据
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <returns></returns>
        List<CEntitySoilData> QueryByStationAndTime(string stationId, DateTime timeStart, DateTime timeEnd);

        /// <summary>
        /// 获取最新的历史数据，如果lastData为null, 表示没有最新的数据
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="lastData"></param>
        /// <returns></returns>
        bool GetLastStationData(string stationId, out CEntitySoilData lastData);

        //1009gm
        List<CEntitySoilData> QueryForMonthTable(string stationId, DateTime date);

    }
}
