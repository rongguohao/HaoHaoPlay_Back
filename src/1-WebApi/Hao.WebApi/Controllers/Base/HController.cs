﻿using Hao.Core;
using Hao.Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;

namespace Hao.Core.AppController
{
    public class HController : Controller
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        protected IConfiguration _config;

        protected ICurrentUser _currentUser;

        public HController(IConfiguration config, ICurrentUser currentUser)
        {
            _config = config;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 在进入方法之前 获取用户jwt中用户信息
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            #region 若需要Claims里面其他信息，则取消注释
            //var tokenHeader = HttpContext.Request.Headers["Authorization"];

            //var strToken = tokenHeader.ToString();
            //if (strToken.Contains("Bearer "))
            //{
            //    var jwtHandler = new JwtSecurityTokenHandler();
            //    JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(strToken.Remove(0, 7)); //去除"Bearer "
            //    var identity = new ClaimsIdentity(jwtToken.Claims);
            //    var principal = new ClaimsPrincipal(identity);
            //    HttpContext.User = principal;
            //}
            #endregion

            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid); //Security Identifiers安全标识符

            var traceId = HttpContext.TraceIdentifier;
            var path = HttpContext.Request.Path.Value;

            //用户未登录
            if (userId == null)
            {
                //输入日志
                _logger.Info(new LogInfo()
                {
                    Method = path,
                    Argument = new
                    {
                        TraceIdentifier = traceId,
                        HttpContext.Request.Query,
                        HttpContext.Request.Body
                    },
                    Description = "用户未登录"
                });

                throw new HException(ErrorCode.E005006, nameof(ErrorCode.E005006).GetCode());
            }


            _logger.Info(new LogInfo()
            {
                Method = path,
                Argument = new
                {
                    TraceIdentifier = traceId,
                    UserId = userId.Value,
                    HttpContext.Request.Query,
                    HttpContext.Request.Body
                },
                Description = "获取jwt用户信息"
            });

            var value = RedisHelper.Get(_config["LoginCachePrefix"] + userId.Value);

            if (value == null)
            {
                throw new HException(ErrorCode.E100002, nameof(ErrorCode.E100002).GetCode());
            }

            RedisCacheUser cacheUser = JsonExtensions.DeserializeFromJson<RedisCacheUser>(value);

            //当前用户信息
            _currentUser.UserID = cacheUser.ID;
            _currentUser.UserName = cacheUser.UserName;

            base.OnActionExecuting(context);
        }



        protected async Task<HttpResponseMessage> DownFile(string filePath, string fileName)
        {
            return await Task.Factory.StartNew(() =>
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
#if DEBUG

                Stream stream = new FileStream(filePath, FileMode.Open);
                response.Content = new StreamContent(stream);
#else
                response.Content = new StringContent("");
#endif
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileName;

#if DEBUG
#else
                response.Content.Headers.Add("X-Accel-Redirect", $"/Api/ExportExcel/{fileName}");
#endif
                return response;
            });

        }

        private string ReadBody(HttpRequest request)
        {
            var result = string.Empty;
            string method = request.Method.ToLower();
            if (method.Equals("post") || method.Equals("put"))
            {
                request.EnableRewind();
                request.Body.Seek(0, 0);
                using (var reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                //Stream stream = request.Body;
                //byte[] buffer = new byte[request.ContentLength.Value];
                //stream.Read(buffer, 0, buffer.Length);
                //var result = Encoding.UTF8.GetString(buffer);
                return result;
            }
            return result;
        }
    }

}
