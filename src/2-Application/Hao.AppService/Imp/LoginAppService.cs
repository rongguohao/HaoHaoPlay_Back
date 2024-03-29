﻿using DotNetCore.CAP;
using Hao.Core;
using Hao.Encrypt;
using Hao.Enum;
using Hao.EventData;
using Hao.Library;
using Hao.Model;
using Hao.Redis;
using Hao.Service;
using Hao.Utility;
using Mapster;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hao.AppService
{
    /// <summary>
    /// 登录应用服务
    /// </summary>
    public class LoginAppService : ApplicationService, ILoginAppService
    {
        private readonly ICapPublisher _publisher;

        private readonly AppSettings _appSettings;

        private readonly IUserDomainService _userDomainService;

        private readonly IModuleDomainService _moduleDomainService;

        public LoginAppService(
            ICapPublisher publisher,
            IOptionsSnapshot<AppSettings> appSettingsOptions, IUserDomainService userDomainService,
            IModuleDomainService moduleDomainService)
        {
            _appSettings = appSettingsOptions.Value; //IOptionsSnapshot动态获取配置
            _publisher = publisher;
            _userDomainService = userDomainService;
            _moduleDomainService = moduleDomainService;
        }


        /// <summary>
        /// 账号密码登录
        /// </summary>
        /// <param name="input"></param>
        /// <param name="fromIP"></param>
        /// <returns></returns>
        [CapUnitOfWork]
        public async Task<LoginOutput> LoginByAccountPwd(LoginByAccountPwdInput input, string fromIP)
        {
            //rsa解密
            var password = H_EncryptProvider.RsaDecrypt(_appSettings.Key.RsaPrivateKey, input.Password);

            //sha256加密
            password = H_EncryptProvider.HMACSHA256(password, _appSettings.Key.Sha256Key);

            //根据账号密码登录
            var user = await _userDomainService.LoginByAccountPwd(input.Account, password);

            return await Login(user, fromIP, input.IsRememberLogin);
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fromIP"></param>
        /// <param name="isRememberLogin"></param>
        /// <returns></returns>
        private async Task<LoginOutput> Login(SysUser user, string fromIP, bool isRememberLogin)
        {
            var timeNow = DateTime.Now;
            var expireTime = timeNow.AddDays(isRememberLogin ? 3 : 1);

            var authNums = JsonConvert.DeserializeObject<List<long>>(user.AuthNumbers);

            //查询用户菜单
            var modules = await _moduleDomainService.GetMenuTree(authNums);

            //jwt的唯一身份标识，避免重复
            var jti = Guid.NewGuid();
            var jwt = CreateJwt(timeNow, expireTime, jti, user);

            //存入redis
            var cacheUser = user.Adapt<H_CacheUser>();
            cacheUser.AuthNums = authNums;
            cacheUser.Jti = jti.ToString();
            cacheUser.LoginIp = fromIP;
            cacheUser.LoginTime = timeNow;
            cacheUser.LoginStatus = LoginStatus.Online;


            int expireSeconds = (int)expireTime.Subtract(timeNow).Duration().TotalSeconds + 1;
            RedisHelper.Cli.Set($"{_appSettings.RedisPrefix.Login}{user.Id}_{jti}", JsonConvert.SerializeObject(cacheUser), expireSeconds);

            var result = user.Adapt<LoginOutput>();
            result.Jwt = jwt;
            result.AuthNums = authNums;
            result.Menus = modules.Adapt<List<MenuOutput>>();

            await _publisher.PublishAsync(nameof(LoginEventData), new LoginEventData
            {
                UserId = user.Id,
                LoginTime = timeNow,
                LoginIP = fromIP,
                JwtExpireTime = expireTime,
                JwtJti = jti
            });

            return result;
        }


        /// <summary>
        /// 生成Jwt
        /// </summary>
        /// <param name="timeNow"></param>
        /// <param name="expireTime"></param>
        /// <param name="jti"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateJwt(DateTime timeNow, DateTime expireTime, Guid jti, SysUser user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _appSettings.Jwt.Subject), //主题
                new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()), //针对当前 token 的唯一标识 jwt的唯一身份标识，避免重复
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, H_Util.GetUnixTimestamp(timeNow).ToString(), ClaimValueTypes.Integer64), //token 创建时间
            };

            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Jwt.SecretKey));

            var jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: _appSettings.Jwt.Issuer,
                audience: _appSettings.Jwt.Audience,
                claims: claims,
                notBefore: timeNow, //生效时间
                expires: expireTime,//过期时间
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            ));

            return jwt;
        }
    }
}
