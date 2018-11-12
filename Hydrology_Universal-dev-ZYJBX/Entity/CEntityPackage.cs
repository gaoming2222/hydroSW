using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Hydrology.Entity
{
    public class CEntityPackage
    {
        // 测站编号
        [XmlElement("sid")]
        public string StrStationID { get; set; }

        // 包序号
        [XmlElement("PackageNum")]
        public string PackageNum { get; set; }

        // 时间
        [XmlElement("time")]
        public DateTime time { get; set; }
    }
}
