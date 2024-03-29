﻿using FluentValidation;
using Hao.Enum;
using System;

namespace Hao.AppService
{
    /// <summary>
    /// 修改用户请求
    /// </summary>
    public class UserUpdateInput
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender? Gender { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        public string WeChat { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
    }

    /// <summary>
    /// 验证
    /// </summary>
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateInput>
    {
        public UserUpdateRequestValidator()
        {

            RuleFor(x => x.Name).MustHasValue("姓名");

            RuleFor(x => x.Gender).EnumMustHasValue("性别");

            RuleFor(x => x.Birthday).MustHasValue("出生日期");
        }
    }
}
