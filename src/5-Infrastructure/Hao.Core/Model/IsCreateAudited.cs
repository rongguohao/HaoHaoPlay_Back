﻿using System;

namespace Hao.Core
{
    public interface IsCreateAudited
    {
        /// <summary>
        /// 创建人
        /// </summary>
        long? CreatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? CreateTime { get; set; }
    }
}