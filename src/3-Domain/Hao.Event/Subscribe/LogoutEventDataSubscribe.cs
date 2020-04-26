﻿using DotNetCore.CAP;
using Hao.EventData;
using System.Threading.Tasks;

namespace Hao.Event
{
    /// <summary>
    /// 注销
    /// </summary>
    public interface ILogoutEventDataSubscribe
    {
        Task Logout(LogoutEventData data);
    }

    public class LogoutEventDataSubscribe : ILogoutEventDataSubscribe, ICapSubscribe
    {

        private readonly ILogoutEventDataHandler _handler;

        public LogoutEventDataSubscribe(ILogoutEventDataHandler handler)
        {
            _handler = handler;
        }

        [CapSubscribe(nameof(LogoutEventData))]
        public async Task Logout(LogoutEventData data)
        {
            await _handler.Logout(data);
        }
    }
}
