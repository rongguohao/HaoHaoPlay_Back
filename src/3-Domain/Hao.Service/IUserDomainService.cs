﻿using Hao.Model;
using System.Threading.Tasks;

namespace Hao.Service
{
    /// <summary>
    /// 用户领域服务接口
    /// </summary>
    public interface IUserDomainService
    {
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task Add(SysUser user);

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<SysUser> Get(long userId);

        /// <summary>
        /// 检测用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        void CheckUser(long userId);

        /// <summary>
        /// 根据账号密码登录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<SysUser> LoginByAccountPwd(string account, string password);

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        Task UpdatePwd(long userId, string oldPwd, string newPwd);
    }
}
