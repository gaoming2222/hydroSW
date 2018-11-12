using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{
    public class CSDStruct
    {
        public String StationID;
        public String Cmd;

        public List<CTimeAndAllData> Datas;
    }
    public class CTimeAndAllData
    {
        public DateTime Time;
        public string water;
        public string rain;
        public string voltage;
    }
}

