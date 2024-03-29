﻿using Hao.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.Model
{
    /// <summary>
    /// 用户拥有的应用菜单
    /// </summary>
    public class MenuTreeDto
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
        /// 父级id
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public ModuleType? Type { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 层级（一层64个)
        /// </summary>
        public int? Layer { get; set; }

        /// <summary>
        /// 权限数字
        /// </summary>
        public long? Number { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 父级别名
        /// </summary>
        public string ParentAlias { get; set; }

        /// <summary>
        /// 子应用菜单
        /// </summary>
        public List<MenuTreeDto> ChildMenus { get; set; }
    }
}
