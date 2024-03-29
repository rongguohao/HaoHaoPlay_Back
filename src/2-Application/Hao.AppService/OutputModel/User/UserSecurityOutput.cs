﻿namespace Hao.AppService
{
    /// <summary>
    /// 用户安全信息
    /// </summary>
    public class UserSecurityOutput
    {
        /// <summary>
        /// 密码强度 0：弱，1：中，2：强
        /// </summary>
        public string PasswordLevel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
