using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    /// <summary>
    /// 雨量表的代理接口抽象
    /// </summary>
    public interface IRainProxy: IMultiThread
    {
        /// <summary>
        /// 异步添加雨量记录
        /// </summary>
        /// <param name="rain"></param>
        void AddNewRow(CEntityRain rain);

        /// <summary>
        /// 异步添加新的雨量记录
        /// </summary>
        /// <param name="rains"></param>
        void AddNewRows(List<CEntityRain> rains);

        bool DeleteRows(List<String> rains_StationId, List<String> rains_StationDate);

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="rains"></param>
        /// <returns></returns>
        bool UpdateRows(List<CEntityRain> rains);

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
        /// <returns>-1表示查询失败</returns>
        int GetPageCount();
        
        /// <summary>
        /// 获取当前选择条件下，总共的行数
        /// </summary>
        /// <returns></returns>
        int GetRowCount();

        /// <summary>
        /// 获取某一页的数据,pageIndex从1开始
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        List<CEntityRain> GetPageData(int pageIndex);

        /// <summary>
        /// 获取计算新的雨量值所需要的数据
        /// </summary>
        /// <param name="lastTotalRain"></param>
        /// <param name="lastDayRain"></param>
        /// <param name="lastDayTime"></param>
        /// <returns></returns>
        bool GetLastData(ref Nullable<Decimal> lastTotalRain,ref Nullable<DateTime> lastDataTime, ref Nullable<Decimal> lastDayRain, ref Nullable<Decimal> llastDayRain, ref Nullable<Decimal> lastSharpClockTotalRain, ref Nullable<DateTime> lastSharpClockTime, ref Nullable<DateTime> lastDayTime, ref Nullable<EChannelType> lastChannelType, ref Nullable<EMessageType> lastMessageType, string stationId);

        //decimal GetLastDayTotalRain(string stationId, DateTime lastDay);

        ////获取上n条整点数据
        //List<CEntityRain> GetLastClockSharp(string stationId, DateTime TodayTime);
        decimal GetLastDayTotalRain(string stationId, DateTime lastDay);

        CEntityRain GetLastDayRain(string stationId, DateTime lastDay);
        decimal GetLastClockSharpTotalRain(string stationId, DateTime TodayTime);


        //1009gm
        List<CEntityRain> QueryAccTime(DateTime time);
        List<CEntityRain> QueryAccTimeAndStation(string StationId, DateTime time);
        List<CEntityRain> QueryForMonthTable(string StationId, DateTime time);
        List<CEntityRain> QueryForYearTable(string StationId, DateTime time);

        bool checkRainIsExists(string stationid, DateTime dt);

        bool UpdateRows_1(List<Hydrology.Entity.CEntityRain> rains, int index);
        void AddOrUpdate(List<Hydrology.Entity.CEntityRain> rains);
        List<CEntityRain> getListRainForUpdate(string stationid, DateTime start, DateTime end);

        List<string> getUpdateStations(DateTime start, DateTime end);


        //1028
        CEntityRain GetLastRain(string stationId, DateTime dt);
        CEntityRain GetLastSharpRain(string stationId, DateTime dt);

        List<DateTime> getExistsTime(string stationid, DateTime startTime, DateTime endTime);

        //数据校正,1031
        void AddNewRows_DataModify(List<Hydrology.Entity.CEntityRain> rains);
        bool UpdateRows_DataModify(List<CEntityRain> rains);
        bool UpdateOtherRows_DataModify(List<CEntityRain> rains);

        CEntityRain GetNextRain(CEntityRain rain);
        CEntityRain GetNextSharpRain(CEntityRain rain);
        CEntityRain GetNextDayRain(CEntityRain rain);

        bool UpdateNextRain(CEntityRain rain);
        bool UpdateNextSharpRain(CEntityRain rain);
        bool UpdateNextDayRain(CEntityRain rain);
        bool UpdateOtherRows_DataModify_1(List<CEntityRain> rains);

        CEntityRain getRainsForInit(string stationid, DateTime dt);

        //20170524 gm 创建分表
        bool createTable(string namee);

    }
}
