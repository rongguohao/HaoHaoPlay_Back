﻿using AutoMapper;
using Hao.AppService.ViewModel;
using Hao.Core;
using Hao.Encrypt;
using Hao.Enum;
using Hao.File;
using Hao.Library;
using Hao.Repository;
using Hao.RunTimeException;
using Hao.Utility;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Hao.AppService
{
    /// <summary>
    /// 当前用户应用服务
    /// </summary>
    public class CurrentUserAppService : ApplicationService, ICurrentUserAppService
    {

        private readonly IMapper _mapper;

        private readonly ISysUserRepository _userRep;

        private readonly ICurrentUser _currentUser;


        private readonly AppSettingsInfo _appsettings;

        public FilePathInfo PathInfo { get; set; }

        public CurrentUserAppService(ISysUserRepository userRepository, IMapper mapper, ICurrentUser currentUser, IOptionsSnapshot<AppSettingsInfo> appsettingsOptions)
        {
            _userRep = userRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _appsettings = appsettingsOptions.Value;
        }
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<CurrentUserVM> GetUser()
        {
            var user = await _userRep.GetAysnc(_currentUser.Id.Value);
            return _mapper.Map<CurrentUserVM>(user);
        }

        /// <summary>
        /// 更新头像地址 (ImageSharp)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task UpdateHeadImg(UpdateHeadImgRequest request)
        {
            string[] str = request.Base64Str.Split(',');  //base64Str为base64完整的字符串，先处理一下得到我们所需要的字符串
            byte[] imageBytes = Convert.FromBase64String(str[1]);

            HFile.CreateDirectory(PathInfo.AvatarPath);
            string imgName = $"{_currentUser.Id}_{HUtil.GetTimeStamp()}.png";
            string imgPath = Path.Combine(PathInfo.AvatarPath, imgName);
            using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(imageBytes))
            {
                image.Save(imgPath);
            }
            var user = await _userRep.GetAysnc(_currentUser.Id.Value);
            string oldImgUrl = user.HeadImgUrl;
            user.HeadImgUrl = imgName;
            await _userRep.UpdateAsync(user, user => new { user.HeadImgUrl });
            if (!string.IsNullOrWhiteSpace(oldImgUrl))
            {
                HFile.DeleteFile(Path.Combine(PathInfo.AvatarPath, oldImgUrl));
            }

        }

        /// <summary>
        /// 更新当前用户基本信息
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public async Task UpdateBaseInfo(CurrentUserUpdateRequest vm)
        {
            var user = await _userRep.GetAysnc(_currentUser.Id.Value);
            user.Name = vm.Name;
            user.Age = vm.Age;
            user.Gender = vm.Gender;
            user.NickName = vm.NickName;
            user.Profile = vm.Profile;
            user.HomeAddress = vm.HomeAddress;
            user.FirstNameSpell = HSpell.GetFirstLetter(user.Name.ToCharArray()[0]);
            await _userRep.UpdateAsync(user,
                user => new { user.Name, user.Age, user.Gender, user.NickName, user.Profile, user.HomeAddress, user.FirstNameSpell });
        }

        /// <summary>
        /// 更新当前用户密码
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task UpdatePassword(string oldPassword, string newPassword)
        {
            var user = await _userRep.GetAysnc(_currentUser.Id.Value);
            oldPassword = EncryptProvider.HMACSHA256(oldPassword, _appsettings.Key.Sha256Key);
            if (user.Password != oldPassword) throw new HException("原密码错误");
            user.PasswordLevel = (PasswordLevel)HUtil.CheckPasswordLevel(newPassword);
            newPassword = EncryptProvider.HMACSHA256(newPassword, _appsettings.Key.Sha256Key);
            user.Password = newPassword;
            await _userRep.UpdateAsync(user, user => new { user.Password, user.PasswordLevel });
        }

        /// <summary>
        /// 当前用户安全信息
        /// </summary>
        public async Task<UserSecurityVM> GetSecurityInfo()
        {
            var user = await _userRep.GetAysnc(_currentUser.Id.Value);
            var result = _mapper.Map<UserSecurityVM>(user);
            return result;
        }
    }
}
