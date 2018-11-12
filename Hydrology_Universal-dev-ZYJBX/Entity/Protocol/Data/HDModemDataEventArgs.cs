using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrology.Entity
{
    public class HDModemDataEventArgs : EventArgs
    {
        public HDModemDataStruct Value { get; set; }
        public String Msg { get; set; }
    }
}
