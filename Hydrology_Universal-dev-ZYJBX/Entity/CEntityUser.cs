
namespace Hydrology.Entity
{
    public class CEntityUser
    {
        #region PROPERTY

        /// <summary>
        /// 用户编号,自动增长的唯一ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///  用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///  密码,保存的时候使用MD5进行加密，添加的时候明文传输
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///  是否拥有管理员权限，可以添加删除用户，并且可以操作数据库原始数据等等
        /// </summary>
        public bool Administrator { get; set; }
        #endregion
    }
}
