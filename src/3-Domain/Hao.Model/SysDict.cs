﻿using Hao.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.Model
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class SysDict : FullAuditedEntity<long>
    {
        /// <summary>
        /// 字典编号
        /// </summary>
        public string DictCode { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        public string DictName { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; }

        /// <summary>
        /// 数据值
        /// </summary>
        public int? DataValue { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int? Sort { get; set; }
    }
}