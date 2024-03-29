﻿using Hao.Library;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Hao.Core.Extensions
{
    internal class StaticFileMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly ITimeLimitedDataProtector _protector;


        public StaticFileMiddleware( RequestDelegate next,IDataProtectionProvider provider, IOptionsSnapshot<H_AppSettings> appSettingsOptions)
        {
            _next = next;
            _protector = provider.CreateProtector(appSettingsOptions.Value.DataProtectorPurpose.FileDownload).ToTimeLimitedDataProtector();
        }

        public async Task Invoke(HttpContext context)
        {
            //https传输 URL参数肯定是安全的 被加密
            //https传递查询参数肯定是没有问题的，但是不要用来传递可能引发安全问题的敏感信息奥。
            if (!context.Request.Query.ContainsKey("Authorization"))
            {
                throw new H_Exception(H_Error.E100001);
            }
            
            // var fileId = _protector.Unprotect(context.Request.Query["FileId"].ToString());
            //
            // var path = context.Request.Path.ToString();

            // if (!path.Contains($"{fileId}"))
            // {
            //     throw new HException(ErrorInfo.E100001, nameof(ErrorInfo.E100001).GetErrorCode());
            // }

            // 验证用户信息 //TODO
            //var tokenHeader = HttpContext.Request.Query["Authorization"];
            //var strToken = tokenHeader.ToString();
            //if (strToken.Contains("Bearer "))
            //{
            //    var jwtHandler = new JwtSecurityTokenHandler();
            //    JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(strToken.Remove(0, 7)); //去除"Bearer "
            //    var identity = new ClaimsIdentity(jwtToken.Claims);
            //    var principal = new ClaimsPrincipal(identity);
            //    HttpContext.User = principal;
            //}

            await _next(context);
        }

    }
}
