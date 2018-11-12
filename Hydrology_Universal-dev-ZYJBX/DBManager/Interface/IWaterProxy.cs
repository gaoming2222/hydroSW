using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface IWaterProxy : IMultiThread
    {
        /// <summary>
        /// 添加新的一个水量记录
        /// </summary>
        /// <param name="rains"></param>
        void AddNewRow(CEntityWater water);
        /// <summary>
        /// 添加新的一个水量记录,需要等待1分钟
        /// </summary>
        /// <param name="rains"></param>
        void AddNewRows(List<CEntityWater> waters);
        /// <summary>
        /// 添加新的一个水量记录,不需要等待一分钟
        /// </summary>
        /// <param name="rains"></param>
        void AddNewRows_1(List<CEntityWater> waters);

        /// <summary>
        /// 删除对应ID的水量记录
        /// </summary>
        /// <param name="rains"></param>
        /// <returns></returns>
        bool DeleteRows(List<String> waters_StationId, List<String> waters_StationDate);

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="rains"></param>
        /// <returns></returns>
        bool UpdateRows(List<CEntityWater> waters);

        /// <summary>
        /// 设置筛选条件,只有在设置了筛选条件以后
        /// </summary>
        /// <param name="stationId">测站ID</param>
        /// <param name="timeStart">起始日期</param>
        /// <param name="timeEnd">结束日期</param>
        void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect);

        /// <summary>
        /// 获取当前选择条件下，总共页面数
        /// </summary>
        /// <returns></returns>
        int GetPageCount();

        /// <summary>
        /// 获取当前选择条件下，总共的行数
        /// </summary>
        /// <returns>-1表示查询失败</returns>
        int GetRowCount();

        /// <summary>
        /// 获取某一页的数据,pageIndex从1开始
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        List<CEntityWater> GetPageData(int pageIndex);

        /// <summary>
        /// 获取最新的上一条记录
        /// </summary>
        /// <param name="lastWaterStage"></param>
        /// <param name="lastDayTime"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        bool GetLastData(ref Nullable<Decimal> lastWaterStage, ref Nullable<Decimal> lastWaterFlow, ref Nullable<DateTime> lastDayTime, ref Nullable<EChannelType> lastChannelType, ref Nullable<EMessageType> lastMessageType, string stationId);

        //1009gm
        List<CEntityWater> QueryA(string station, DateTime time);

        List<CEntityWater> QueryForYear(string station, DateTime time);

        bool checkWaterIsExists(string stationid, DateTime dt);
        void AddOrUpdate(List<CEntityWater> listWaters);
        List<DateTime> getExistsTime(string stationid, DateTime startTime, DateTime endTime);

        bool createTable(string name);
    }
}
