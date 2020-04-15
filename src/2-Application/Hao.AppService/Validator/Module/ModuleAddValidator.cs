﻿using FluentValidation;
using Hao.AppService.ViewModel;
using Hao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.AppService
{
    public class ModuleAddValidator : AbstractValidator<ModuleAddRequest>
    {
        public ModuleAddValidator()
        {

            RuleFor(x => x.Name).NotEmpty().WithMessage("模块名称不能为空");

            RuleFor(x => x.Icon).NotEmpty().WithMessage("模块图标不能为空").When(a => a.Type == ModuleType.Main);

            RuleFor(x => x.RouterUrl).NotEmpty().WithMessage("子应用路由地址不能为空").When(a => a.Type == ModuleType.Sub);
            
            RuleFor(x => x.Type).NotEmpty().WithMessage("模块类型不能为空");

            RuleFor(x => x.ParentId).NotEmpty().WithMessage("父节点Id不能为空");
        }
    }
}