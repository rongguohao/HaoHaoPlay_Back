﻿using Hao.Core;
using Hao.EventData;
using Hao.Library;
using Hao.Utility;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Hao.EventBus
{
    /// <summary>
    /// 注销
    /// </summary>
    public class LogoutEventConsumer : EventBusService, ILogoutEventConsumer
    {
        private readonly H_AppSettingsConfig _appsettings;

        public LogoutEventConsumer(IOptionsSnapshot<H_AppSettingsConfig> appsettingsOptions)
        {
            _appsettings = appsettingsOptions.Value;
        }

        public async Task LogoutForUpdateAuth(LogoutForUpdateAuthEventData data)
        {
            foreach(var userId in data.UserIds)
            {
                var keys = await RedisHelper.KeysAsync($"{_appsettings.RedisPrefix.GlobalKey}{_appsettings.RedisPrefix.Login}{userId}_*"); //不会自动加globalkey
                keys = keys.Select(a => a.Split('_', 2)[1]).ToArray(); //去除globalkey
                foreach (var key in keys)
                {
                    var value = await RedisHelper.GetAsync(key); //会自动加globkey
                    var cacheUser = H_JsonSerializer.Deserialize<H_RedisCacheUser>(value);
                    cacheUser.IsAuthUpdate = true;
                    cacheUser.LoginStatus = LoginStatus.Offline;
                    await RedisHelper.SetAsync(key, H_JsonSerializer.Serialize(cacheUser));
                }
            }
        }

        /// <summary>
        /// 注销登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Logout(LogoutEventData data)
        {
            foreach (var userId in data.UserIds)
            {
                var keys = await RedisHelper.KeysAsync($"{_appsettings.RedisPrefix.GlobalKey}{_appsettings.RedisPrefix.Login}{userId}_*");
                keys = keys.Select(a => a.Split('_', 2)[1]).ToArray(); //去除globalkey
                foreach (var key in keys)
                {
                    await RedisHelper.DelAsync(key);
                }
            }
        }
    }
}
