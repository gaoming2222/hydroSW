using System;

namespace Hydrology.Entity
{
    public class CEntityWarningInfo
    {
        /// <summary>
        /// 告警信息的唯一ID
        /// </summary>
        public long WarningInfoID { get; set; }
        /// <summary>
        /// 警告信息的时间
        /// </summary>
        public DateTime DataTime { get; set; }

        /// <summary>
        /// 错误信息类型
        /// </summary>
        public Nullable<EWarningInfoCodeType> WarningInfoCodeType { get; set; }

        /// <summary>
        /// 站点ID
        /// </summary>
        public string StrStationId { get; set; }

        /// <summary>
        /// 警告信息的内容
        /// </summary>
        private string m_strInfoDetail;
        public string InfoDetail
        {
            get { return m_strInfoDetail; }
            set
            {
                // 如果长度越界，抛出异常，数据库400个字符
                int length = System.Text.Encoding.Default.GetBytes(value).Length;
                if (length > 400)
                {
                    throw new Exception("警告信息过长");
                }
                m_strInfoDetail = value;
            }
        }
    }
}
