using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
     public interface ITSRainProxy : IMultiThread
    {
        /// <summary>
        /// 异步添加雨量记录
        /// </summary>
        /// <param name="rain"></param>
        void AddNewRow(CEntityTSRain rain);

        /// <summary>
        /// 异步添加新的雨量记录
        /// </summary>
        /// <param name="rains"></param>
        void AddNewRows(List<CEntityTSRain> rains);

        List<CEntityTSRain> QueryForAll(string stationid, DateTime start, DateTime end);

        void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect);

        List<CEntityTSRain> GetPageData(int pageIndex);
    }
}
