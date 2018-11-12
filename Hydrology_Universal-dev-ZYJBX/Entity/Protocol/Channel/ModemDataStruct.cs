using System.Runtime.InteropServices;

namespace Hydrology.Entity
{
    public struct ModemDataStruct
    {
        /// <summary>
        /// Modem模块的ID号
        /// </summary>
        public uint m_modemId;

        /// <summary>
        /// 接收到数据包的时间
        /// </summary>
        public uint m_recv_time;

        /// <summary>
        /// 存储接收到的数据
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1451)]
        public byte[] m_data_buf;

        /// <summary>
        /// 接收到的数据包长度
        /// </summary>
        public ushort m_data_len;

        /// <summary>
        /// 接收到的数据包类型
        ///     0x01：用户数据包 
        ///     0x02：对控制命令帧的回应
        /// </summary>
        public byte m_data_type;
    }
}
