﻿using FluentValidation;
using Hao.Enum;
using Hao.Utility;

namespace Hao.AppService
{
    /// <summary>
    /// 更新模块请求
    /// </summary>
    public class ModuleUpdateInput
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 应用图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string RouterUrl { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType? Type { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
    }

    /// <summary>
    /// 验证
    /// </summary>
    public class ModuleUpdateValidator : AbstractValidator<ModuleUpdateInput>
    {
        public ModuleUpdateValidator()
        {

            RuleFor(x => x.Name).MustHasValue("模块名称");

            RuleFor(x => x.Type).MustHasValue("模块类型");

            RuleFor(x => x.Icon).MustHasValue("模块图标").When(a => a.Type == ModuleType.Main);

            RuleFor(x => x.RouterUrl).MustHasValue("子应用路由地址").When(a => a.Type == ModuleType.Sub);

            RuleFor(x => x.Sort).MustHasValue("排序值");

            RuleFor(x => x.Alias).MustHasValue("别名").Must(a => H_Validator.IsLetter(a)).WithMessage("别名只能输入英文");
        }
    }
}
