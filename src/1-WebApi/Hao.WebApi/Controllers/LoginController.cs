﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hao.AppService;
using Hao.AppService.ViewModel;
using Hao.AutoMapper;
using Hao.Core;
using Hao.Encrypt;
using Hao.Library;
using Hao.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NLog;

namespace Hao.WebApi
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        private const string publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnVVB8ECm9XgSAgQ10Gn2dyhFCVissWi2guj62999e/gOaUVjYxKmyEtZoZOKGhRIxgzvh3gD8OGm+cpjp8oTGlvIj2q788miw8GMKcgVa4bmF4eIkSksDI7SyHvr6maVt5Cdnpe/S7fqi9XoDf4hJXshQ7TXrO1EQ8F2/9MAeRiS5+AYNtnyxSLoXtkTGYKWqCYyuqqPPCn5LrXKATrbzZhigrrzptUdGvaRpTmEnQc7hyrGKDAfRwqOG/FIx9BlKiG61pBxqfBmwcoZVm3pJYdulLZ9Nq95Q8J/SGuJ1uFMwQdSq1BcbCUDV5v4wZOeIORwJj98J+FiHvizEA1DrwIDAQAB";

        protected const string privateKey = "MIIEowIBAAKCAQEAnVVB8ECm9XgSAgQ10Gn2dyhFCVissWi2guj62999e/gOaUVjYxKmyEtZoZOKGhRIxgzvh3gD8OGm+cpjp8oTGlvIj2q788miw8GMKcgVa4bmF4eIkSksDI7SyHvr6maVt5Cdnpe/S7fqi9XoDf4hJXshQ7TXrO1EQ8F2/9MAeRiS5+AYNtnyxSLoXtkTGYKWqCYyuqqPPCn5LrXKATrbzZhigrrzptUdGvaRpTmEnQc7hyrGKDAfRwqOG/FIx9BlKiG61pBxqfBmwcoZVm3pJYdulLZ9Nq95Q8J/SGuJ1uFMwQdSq1BcbCUDV5v4wZOeIORwJj98J+FiHvizEA1DrwIDAQABAoIBABf8MmQ1Bv7vAhfKmoWeUdOSkQu+t/0H0KMeb3frl06530CPMnqdEk4AprZqLqiRJRMET9PgKQGk9Papsw2WUdk32th6VhLxT132eK658QIGe7dkkx5GH4/+igXEVo/SecqVQmI2EhSyAhC1WH4hmt4C6mxO+n5DYZ/Os5yGh1Duf9+0UrJxBW2TFKpEUQD0h/8W+X4JUekrwx7Nv3cqltLCVmKmwfC/FbCZlOZj76BXOI/2Dj78nO6JFexPxPxKwFNvJniQwpyN+RuvpQCaSSoZ1G3Dy6LNXJ2ttNLaP8Ng88LWs3PkPIEBvAIBnKYkhD9Slr7h9X1WQOWU5lObk/ECgYEA0WsJPka/gvpLdWGrKzc0q9fplN5JOA4WHubFbnkd/5fCNeW9xIKkGnFJkKyKHv9CMF2n3x2VCBiKbvZdnlOzA4QWX5FAoHX8wQ56X1CDVwkkJSpm89NDWEyE/i/DBof9EqRQ9mAQ/6r15uZBc4yhYKPmRjkhOZMWs3vznAxlDwcCgYEAwFRS/Bl3UtlBHM9J9MPz9Law+07RUI6BaM6+xSBjoaCG5I+8+7P2OEzJBcoOLRw/wUK1Zna9HryUdGJwgN2jJMNyiwOIDibyJObsMGUzeW0MpEJo6zo/BX8wHb3MCUz6IZIUalH3/fFtF8k5jWMJs29xQsS+a5idu7wB8Mwb1BkCgYBSRo83HGyZdOS4lUq+i83xxb5LcpmpoD8onNnscUFL7b+rlTtdPUZj7SZN5LvPOdIzVlhh3Cl1KM8akUhur8uhEgAIQ/T0FebbomiJkgRH6Zt9zo7sNJA83LNTekhRBJR1AoGcilmjWPOLZ+NHFnVAlqQd6swW5qcAlS48nNIHeQKBgGvDk8/fsQVB6ALtfuHfneTDLL1TN2MsxgPku+2WLV0Vph+RDnH2LwLyuzw8L99E0dBGwX+NDoCXZ5MiySjbhxsFTDvqGaTcW5cjsLmZKtNhr9ClyUXsF/3Lezle7GFVuV1wDXJZRVwLl3XZcR7wnDHnQo4H0S5WkCLtwCSdKQaZAoGBALrD/bZxlJjtuy2tZMgB9aOcyi/px/+Kwy10EG3Qo8syEdeX/dKFPzjVeFHNpIIZMgSsYXaeiVLkHaohLcBaNX12r7ImOct3u4GtphsyqMBcOK3fXP5BVf0YIg1jjxlaJx+Y0pEeZJPcgxYAUQMwe+OlAfezhzT+jH+2XguZ+EEI";
  

        protected IUserAppService _userAppService;

        protected JwtOptions _jwtOptions;

        //protected IDistributedCache _cache;

        protected IConfigurationRoot _config;

        protected IAutoMapper _mapper;


        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        public LoginController(IAutoMapper mapper,IOptions<JwtOptions> jwtOptions, IUserAppService userService,IConfigurationRoot config)
        {
            _jwtOptions = jwtOptions.Value;
            _userAppService = userService;
            //_cache = cache;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<LoginVMOut> Login([FromBody]UserQueryInput queryInput)
        {
            var user = new LoginVMOut();
            var query = _mapper.Map<UserQuery>(queryInput);

            var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, privateKey);
            string pwd = rsa.Decrypt(query.Password); //解密

            query.Enabled = true;
            query.Password = EncryptProvider.HMACSHA256(pwd, "haohaoplay");
            user = await _userAppService.Login(query);


            var timeNow = DateTime.Now;
            var validFrom = timeNow.Ticks;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"hao"), //主题
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //针对当前 token 的唯一标识
                new Claim(JwtRegisteredClaimNames.Sid, user.ID.ToString()),
                // new Claim(JwtRegisteredClaimNames.Typ, body.Platform.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, timeNow.AddDays(3).Ticks.ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Iat, validFrom.ToString(), ClaimValueTypes.Integer64), //token 创建时间
                new Claim(JwtRegisteredClaimNames.Nbf, validFrom.ToString(), ClaimValueTypes.Integer64)
            };
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                signingCredentials: _jwtOptions.SigningKey
            );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            user.JwtToken = encodedJwt;

            var cacheValue = await RedisHelper.GetAsync(_config["LoginCachePrefix"] + user.ID.ToString());
            RedisCacheUser cacheUser = new RedisCacheUser();
            if (cacheValue != null)
            {
                try
                {
                    cacheUser = JsonExtensions.DeserializeFromJson<RedisCacheUser>(cacheValue);
                }
                catch (Exception ex)
                {
                    _logger.Info(new LogInfo() { Method = "Login", Argument = ex, Description = "登录异常" });
                }
            }

            var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }

            await _userAppService.UpdateLoginTimeAndIP(user.ID.Value, DateTime.Now, ip);

            //存入redis
            var userValue = new RedisCacheUser
            {
                ID = user.ID,
                UserName = user.UserName,
                LoginName = user.LoginName,
                PCValidFrom = validFrom
            };
            await RedisHelper.SetAsync(_config["LoginCachePrefix"] + user.ID.ToString(), JsonExtensions.SerializeToJson(userValue));

            _logger.Info(new LogInfo() { Method = "Login", Argument = query.LoginName, Description = "登录成功" });

            return user;
        }
    }
}
