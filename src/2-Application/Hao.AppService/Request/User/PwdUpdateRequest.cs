﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.AppService
{
    /// <summary>
    /// 修改密码请求
    /// </summary>
    public class PwdUpdateRequest
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string RePassword { get; set; }
    }

    /// <summary>
    /// 验证
    /// </summary>
    public class PwdUpdateValidator : AbstractValidator<PwdUpdateRequest>
    {
        public PwdUpdateValidator()
        {
            RuleFor(x => x.OldPassword).MustHasValue("旧密码");

            RuleFor(x => x.NewPassword).MustHasValue("新密码").Length(6, 16).WithMessage("新密码长度应在6~16个字符");

            RuleFor(x => x.RePassword).MustHasValue("重复密码").When(a => a.RePassword != a.NewPassword).WithMessage("两次输入密码不匹配");
        }
    }
}
