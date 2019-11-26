﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.Utility
{
    /// <summary>
    /// 泛型扩展类
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// 是否是默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }
    }
}
