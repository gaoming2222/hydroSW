using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface ITSWater
    {
        /// <summary>
        /// 异步添加雨量记录
        /// </summary>
        /// <param name="rain"></param>
        void AddNewRow(CEntityTSWater water);

        /// <summary>
        /// 异步添加新的雨量记录
        /// </summary>
       // /// <param name="rains"></param>
        void AddNewRows(List<CEntityTSWater> waters);

        void SetFilter(string stationId, DateTime timeStart, DateTime timeEnd, bool TimeSelect);

        List<CEntityTSWater> GetPageData(int pageIndex);
    }
}
