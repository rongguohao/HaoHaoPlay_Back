﻿using Hao.Core;
using Hao.Library;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hao.AppService
{
    public class LogoutAppService : ApplicationService, ILogoutAppService
    {
        private readonly AppSettingsInfo _appsettings;

        public LogoutAppService(IOptionsSnapshot<AppSettingsInfo> appsettingsOptions)
        {
            _appsettings = appsettingsOptions.Value;
        }

        /// <summary>
        /// 注销当前登录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public async Task Logout(long userId, string jwt)
        {
            var value = await RedisHelper.GetAsync($"{_appsettings.RedisPrefixOptions.LoginInfo}{userId}_{jwt}");
            var cacheUser = JsonSerializer.Deserialize<RedisCacheUserInfo>(value);
            cacheUser.LoginStatus = LoginStatus.Offline;
            await RedisHelper.SetAsync($"{_appsettings.RedisPrefixOptions.LoginInfo}{userId}_{jwt}",JsonSerializer.Serialize(cacheUser));
        }
    }
}
