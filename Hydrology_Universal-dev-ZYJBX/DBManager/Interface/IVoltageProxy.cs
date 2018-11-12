using System;
using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    /// <summary>
    /// 电压表的代理接口抽象
    /// </summary>
    public interface IVoltageProxy : IMultiThread
    {
        void AddNewRow(CEntityVoltage voltage);

        // 异步添加新的一个电压记录
        void AddNewRows(List<CEntityVoltage> voltages);

        // 异步添加新的一个电压记录,不需等待1分钟
        void AddNewRows_1(List<CEntityVoltage> voltages);

        bool DeleteRows(List<String> voltages_StationId, List<String> voltages_StationDate);

        /// <summary>
        /// 更新数据行
        /// </summary>
        /// <param name="rains"></param>
        /// <returns></returns>
        bool UpdateRows(List<CEntityVoltage> voltages);

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
        /// <returns>-1 表示查询失败</returns>
        int GetPageCount();

        /// <summary>
        /// 获取当前选择条件下，总共的行数
        /// </summary>
        /// <returns>-1 表示查询失败</returns>
        int GetRowCount();

        /// <summary>
        /// 获取某一页的数据,pageIndex从1开始
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        List<CEntityVoltage> GetPageData(int pageIndex);

        /// <summary>
        /// 获取数据库中电压表的最新一条记录
        /// </summary>
        /// <param name="lastVoltage"></param>
        /// <param name="lastDayTime"></param>
        /// <param name="lastChannelType"></param>
        /// <param name="lastMessageType"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        bool GetLastData(ref Nullable<Decimal> lastVoltage,  ref Nullable<DateTime> lastDayTime, ref Nullable<EChannelType> lastChannelType, ref Nullable<EMessageType> lastMessageType, string stationId);


        List<CEntityVoltage> QueryForRateTable(CEntityStation station, DateTime date);

        List<CEntityVoltage> QueryForRateMonthTable(CEntityStation station, DateTime startTime, DateTime endTime);

        bool createTable(string name);
    }
}
