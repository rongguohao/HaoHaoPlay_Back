﻿using Hao.AppService;
using Hao.AppService.ViewModel;
using Hao.Core;
using Hao.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hao.AppService
{
    public interface IUserAppService 
    {
        Task<LoginOut> Login(UserQuery query);

        Task<long> AddUser(UserIn vm);
        
        Task AddUsers(List<UserIn> vms);
        
        Task EditUser(long userId, UserIn vm);

        Task<PagedList<UserOut>> GetUsers(UserQuery query);

        Task<UserOut> GetUser(long id);

        Task<CurrentUserOut> GetCurrentUser();

        Task UpdateLogin(long userId, DateTime lastLoginTime, string ip);

        Task DeleteUser(long userId);

        Task UpdateUserEnabled(long userId,bool enabled);

        Task<string> ExportUsers(UserQuery query);
    }
}
