using System.Runtime.InteropServices;

namespace Hydrology.Entity
{
    public struct HDModemInfoStruct
    {
        /// <summary>
        /// Modem模块的ID号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] m_modemId;


        /// <summary>
        /// DTU进入Internet的代理主机IP地址
        /// </summary>

        public uint  m_sin_addr;

        /// <summary>
        ///DTU进入Internet的代理主机IP端口
        /// </summary>
        public ushort m_sin_port;

        /// <summary>
        /// DTU在移动网内IP地址
        /// </summary>
       
        public uint  m_local_addr;

        /// <summary>
        ///DTU在移动网内IP端口
        /// </summary>
        public ushort m_local_port;

        /// <summary>
        /// DTU登录时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] m_logon_date;

        /// <summary>
        ///DTU包更新时间，DSC接收到该DTU最近一个包的时间，使用前四字节，time_t类型，后16字节未使用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] m_update_time;

        /// <summary>
        /// TU状态, 1 在线 ，0 不在线
        /// </summary>
        public byte m_status;

        /// <summary>
        /// Modem的11位SIM卡号，必须以'\0'字符结尾 
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        //public byte[] m_phoneno;

        ///// <summary>
        ///// Modem的4位动态ip地址   
        ///// </summary>
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        //public byte[] m_dynip;

        ///// <summary>
        ///// Modem模块最后一次建立TCP连接的时间 
        ///// </summary>
        //public uint m_conn_time;

        ///// <summary>
        ///// Modem模块最后一次收发数据的时间  
        ///// </summary>
        //public uint m_refresh_time;

        public override bool Equals(object obj)
        {
            try
            {
                var item = (HDModemInfoStruct)obj;

                return (this.m_modemId == item.m_modemId && this.m_local_addr == item.m_local_addr && this.m_local_port == item.m_local_port && this.m_logon_date == item.m_logon_date && this.m_update_time == item.m_update_time);
            }
            catch
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
