﻿using Hao.Core;
using Hao.Enum;
using Hao.Utility;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hao.Model
{
    /// <summary>
    /// 用户查询
    /// </summary>
    public class UserQuery : Query<SysUser>
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 姓名 模糊查询
        /// </summary>
        public string LikeName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// 手机号码 模糊查询
        /// </summary>
        public string LikePhone { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? Enabled { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTimeStart { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTimeEnd { get; set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        public int? RoleLevel { get; set; }
        /// <summary>
        /// 当前用户角色等级
        /// </summary>
        public int? CurrentRoleLevel { get; set; }

        public override List<Expression<Func<SysUser, bool>>> QueryExpressions
        {
            get
            {
                List<Expression<Func<SysUser, bool>>> expressions = new List<Expression<Func<SysUser, bool>>>();

                if (Account.HasValue()) expressions.Add(x => x.Account == Account);

                if (LikeName.HasValue()) expressions.Add(x => x.Name.Contains(LikeName));

                if (LikePhone.HasValue()) expressions.Add(x => x.Phone.Contains(LikePhone));

                if (Gender.HasValue) expressions.Add(x => x.Gender == Gender);

                if (Enabled.HasValue) expressions.Add(x => x.Enabled == Enabled);

                if (LastLoginTimeStart.HasValue) expressions.Add(x => x.LastLoginTime >= LastLoginTimeStart);

                if (LastLoginTimeEnd.HasValue) expressions.Add(x => x.LastLoginTime <= LastLoginTimeEnd);

                if (RoleLevel.HasValue) expressions.Add(x => x.RoleLevel == RoleLevel);

                if (CurrentRoleLevel.HasValue) expressions.Add(x => x.RoleLevel > CurrentRoleLevel);

                return expressions;
            }
        }

        //public override string QuerySql
        //{
        //    get
        //    {
        //        StringBuilder sb = new StringBuilder(" 1=1 ");


        //        if (!string.IsNullOrWhiteSpace(Code))
        //            sb.Append($" And st.Code like '%{Code}%'");

        //        if (!string.IsNullOrWhiteSpace(Name))
        //            sb.Append($" And st.Name like '%{Name}%'");

        //        if (!string.IsNullOrWhiteSpace(Gender))
        //            sb.Append($" And st.Gender like '%{Gender}%'");

        //        if (!string.IsNullOrWhiteSpace(Mobile))
        //            sb.Append($" And st.Mobile like'%{Mobile}%'");

        //        if (Sta.HasValue)
        //            sb.Append($" And st.Status={(int)Sta}");


        //        return sb.ToString();
        //    }
        //}
    }
}
