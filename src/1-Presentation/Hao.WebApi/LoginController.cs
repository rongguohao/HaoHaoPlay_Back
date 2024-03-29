﻿using System.Threading.Tasks;
using Hao.AppService;
using Hao.Core.Extensions;
using Hao.Log;
using Microsoft.AspNetCore.Mvc;

namespace Hao.WebApi
{
    /// <summary>
    /// 登录
    /// </summary>
    [ApiController]
    [Route("[action]")]
    //1.参数绑定策略的自动推断,可以省略[FromBody] 
    //2.行为自定义 像MVC框架的大部分组件一样，ApiControllerAttribute的行为是高度可自定义的。首先，上面说的大部分内容都是可以简单的用 on/off 来切换。具体的设置是在startup方法里面通过ApiBehaviorOptions来实现
    public class LoginController : ControllerBase
    {
        private readonly ILoginAppService _loginAppService;

        public LoginController(ILoginAppService loginService)
        {
            _loginAppService = loginService; //顺序2
        }

        /// <summary>
        /// 账号密码登录 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<LoginOutput> LoginByAccountPwd(LoginByAccountPwdInput input)
        {
            string ip = HttpContext.GetIp();

            H_Log.Info(new LogNote()
            {
                Location = "LoginByAccountPwd",
                Data = input,
                TraceId = HttpContext.TraceIdentifier,
                IP = ip,
                Extra = "登录请求"
            });

            var result = await _loginAppService.LoginByAccountPwd(input, ip);

            H_Log.Info(new LogNote()
            {
                Location = "LoginByAccountPwd",
                Data = result,
                TraceId = HttpContext.TraceIdentifier,
                IP = ip,
                Extra = "登录返回"
            });

            return result;
        }
    }
}
