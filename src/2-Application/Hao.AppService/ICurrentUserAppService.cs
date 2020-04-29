﻿using Hao.AppService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hao.AppService
{
    public interface ICurrentUserAppService
    {

        /// <summary>
        /// 当前用户信息
        /// </summary>
        /// <returns></returns>
        Task<CurrentUserVM> GetUser();

        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateHeadImg(UpdateHeadImgRequest request);

        /// <summary>
        /// 更新当前用户基本信息
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        Task UpdateBaseInfo(CurrentUserUpdateRequest vm);

        /// <summary>
        /// 更新当前用户密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task UpdatePassword(string oldPassword, string newPassword);

        /// <summary>
        /// 当前用户的安全信息
        /// </summary>
        /// <returns></returns>
        Task<UserSecurityVM> GetSecurityInfo();
    }
}