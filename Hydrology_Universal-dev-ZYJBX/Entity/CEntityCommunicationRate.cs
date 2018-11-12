using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{

    public class CEntityCommunicationRate
    {
        /// <summary>
        /// 站点ID
        /// </summary>
        public string StationID { get; set; }

        /// <summary>
        /// 记录的时间，整点，2014030312,分钟和秒钟为0
        /// </summary>
        public DateTime Time { get; set; }
    }
}
