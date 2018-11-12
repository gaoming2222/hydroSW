using System.Collections.Generic;
using Hydrology.Entity;

namespace Hydrology.DBManager.Interface
{
    public interface IUserProxy : IMultiThread
    {
        /// <summary>
        /// 添加用户集
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool AddUserRange(List<CEntityUser> listUser);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool UpdateUserRange(List<CEntityUser> listUser);

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="listUser"></param>
        /// <returns></returns>
        bool DeleteUserRange(List<int> listUserID);

        /// <summary>
        /// 查询所有用户
        /// </summary>
        /// <returns></returns>
        List<CEntityUser> QueryAllUser();

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="bAdministrator"></param>
        /// <returns>登陆是否成功</returns>
        bool UserLogin(string username, string password, ref bool bAdministrator);
    }
}
