﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hao.Core
{
    public static class OrderByExtensions
    {
        ///// <summary>
        ///// 组合排序
        ///// </summary>
        ///// <param name="orderByType"></param>
        ///// <param name="sortField"></param>
        ///// <returns></returns>
        //public static string ToOrderByFields<T>(this T?[] sortFields, SortType?[] orderByTypes) where T : struct, Enum
        //{
        //    if (orderByTypes == null || sortFields == null) return null;

        //    if (orderByTypes.Length == 0 || sortFields.Length == 0) return null;

        //    if (orderByTypes.Length != sortFields.Length) return null;

        //    StringBuilder sb = new StringBuilder();

        //    for (int i = 0; i < sortFields.Length; i++)
        //    {
        //        if (!sortFields[i].HasValue || !orderByTypes[i].HasValue) continue;

        //        sb.Append(string.Format("{0} {1},", sortFields[i], orderByTypes[i]));
        //    }

        //    if (sb.Length > 0) return sb.ToString().TrimEnd(',');

        //    return null;
        //}


        /// <summary>
        /// 组合排序
        /// </summary>
        /// <param name="orderByType"></param>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public static List<OrderByInfo> ToOrderByConditions<T>(this T?[] sortFields, SortType?[] orderByTypes) where T : struct, Enum
        {
            if (orderByTypes == null || sortFields == null) return null;

            if (orderByTypes.Length == 0 || sortFields.Length == 0) return null;

            if (orderByTypes.Length != sortFields.Length) return null;

            List<OrderByInfo> list = new List<OrderByInfo>();

            for (int i = 0; i < sortFields.Length; i++)
            {
                if (!sortFields[i].HasValue || !orderByTypes[i].HasValue) continue;

                list.Add(new OrderByInfo (sortFields[i].ToString(), orderByTypes[i] == SortType.Asc ));
            }

            return list;
        }
    }
}
