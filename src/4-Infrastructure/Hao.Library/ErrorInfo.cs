﻿using System;

namespace Hao.Library
{
    public static class ErrorInfo
    {

        public const string E100001 = "用户未登录或令牌已过期，请重新登录";
        public const string E100002 = "用户未登录，请重新登录";
        public const string E100011 = "提交数据类型错误，请检查";
        //基础异常
        public const string E005001 = "用户不存在";
        public const string E005002 = "用户名已存在";
        public const string E005003 = "附件不存在";
        public const string E005004 = "两次输入的密码不相同";
        public const string E005005 = "用户名或密码错误";
        public const string E005006 = "用户未登录";
        public const string E005007 = "请选择要上传的Excel文件";
        public const string E005008 = "只能上传Excel格式文件";
    }

    public static class StringExtensions
    {
        /// <summary>
        /// 返回错误Code
        /// </summary>
        /// <param name="errorName"></param>
        /// <returns></returns>
        public static int GetErrorCode(this string errorName)
        {
            int result = 100000;
            if (!string.IsNullOrWhiteSpace(errorName))
            {
                int.TryParse(errorName.Substring(1), out result);
            }
            return result;
        }
    }
}
