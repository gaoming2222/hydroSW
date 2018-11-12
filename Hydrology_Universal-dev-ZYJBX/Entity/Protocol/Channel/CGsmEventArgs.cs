using System;

namespace Hydrology.Entity
{
    public class CGsmEventArgs : EventArgs
    {
        public CGSMStruct Gsm { get; set; }
        public Object Data { get; set; }
    }
}
