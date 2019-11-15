﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Hao.Core;
using Hao.EventData;
using Hao.Repository;

namespace Hao.Event
{
    public class LoginEventHandler : HEventHandler,ILoginEventHandler, ICapSubscribe
    {

        private readonly ISysUserRepository _userRep;

        public LoginEventHandler(ISysUserRepository userRepository)
        {
            _userRep = userRepository;
        }

        [CapSubscribe(nameof(LoginEventData))]
        public async Task UpdateLoginTimeAndIP(LoginEventData person)
        {
            var user = await _userRep.GetAysnc(person.UserId);
            if (user != null)
            {
                user.LastLoginTime = person.LastLoginTime;
                user.LastLoginIP = person.LastLoginIP;
                await _userRep.UpdateAsync(user);
            }
        }
    }
}
