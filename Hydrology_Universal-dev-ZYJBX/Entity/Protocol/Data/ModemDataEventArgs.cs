using System;

namespace Hydrology.Entity
{
    public class ModemDataEventArgs : EventArgs
    {
        public ModemDataStruct Value { get; set; }
        public String Msg { get; set; }
    }
}
