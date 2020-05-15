﻿using System.Threading.Tasks;
using Hao.AppService;
using Hao.AppService.ViewModel;
using Hao.Log;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Hao.WebApi.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //1.参数绑定策略的自动推断,可以省略[FromBody] 
    //2.行为自定义 像MVC框架的大部分组件一样，ApiControllerAttribute的行为是高度可自定义的。首先，上面说的大部分内容都是可以简单的用 on/off 来切换。具体的设置是在startup方法里面通过ApiBehaviorOptions来实现
    public class LoginController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private readonly ILoginAppService _loginAppService;

        public LoginController(ILoginAppService loginService)
        {
            _loginAppService = loginService;
        }

        /// <summary>
        /// 登录 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<LoginVM> Login(LoginRequest request)
        {
            _logger.Info(new H_Log() { Method = "Login", Argument = request, Description = "登录请求" });

            var result = await _loginAppService.Login(request.LoginName, request.Password, request.IsRememberLogin);

            _logger.Info(new H_Log() { Method = "Login", Argument = request, Description = "登录成功" });

            return result;
        }
    }
}
