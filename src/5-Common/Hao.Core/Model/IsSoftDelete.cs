﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.Core.Model
{
    /// <summary>
    /// 逻辑删除
    /// </summary>
    public interface IsSoftDelete
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        bool? IsDeleted { get; set; }
    }
}
