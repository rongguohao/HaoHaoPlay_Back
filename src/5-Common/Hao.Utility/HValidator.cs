﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Hao.Utility
{
    public static class HValidator
    {
        #region 类型判断
        public static bool IsInt(string str)
        {
            return new Regex("^(-){0,1}\\d+$").Match(str).Success && long.Parse(str) <= 2147483647L && long.Parse(str) >= -2147483648L;
        }

        public static bool IsDate(this object thisValue)
        {
            if (thisValue == null) return false;
            DateTime outValue = DateTime.MinValue;
            return DateTime.TryParse(thisValue.ToString(), out outValue);
        }

        public static bool IsAnonymousType(this Type type)
        {
            string typeName = type.Name;
            return typeName.Contains("<>") && typeName.Contains("__") && typeName.Contains("AnonymousType");
        }
        public static bool IsCollectionsList(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Collections.Generic.List") || (thisValue + "").StartsWith("System.Collections.Generic.IEnumerable");
        }
        public static bool IsStringArray(this string thisValue)
        {
            return (thisValue + "").IsMatch(@"System\.[a-z,A-Z,0-9]+?\[\]");
        }
        public static bool IsEnumerable(this string thisValue)
        {
            return (thisValue + "").StartsWith("System.Linq.Enumerable");
        }

        public static bool IsClass(this Type thisValue)
        {
            return thisValue != HUtil.StringType && thisValue.IsEntity();
        }
        #endregion

        #region 实用功能判断
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="email">Email地址</param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, "^[A-Za-z0-9](([_\\.\\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证网址是否有效
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns></returns>
        public static bool IsUrl(string url)
        {
            return Regex.IsMatch(url, "^(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\\.))+(([a-zA-Z0-9\\._-]+\\.[a-zA-Z]{2,6})|([0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}))(/[a-zA-Z0-9\\&amp;%_\\./-~-]*)?$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证手机号，目前只支持中国手机号码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            return Regex.IsMatch(mobile, "^1\\d{10}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证IP是否有效
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, "^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$", RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 目前只支持中国电话，格式010-85849685
        /// </summary>
        /// <param name="str">电话号码</param>
        /// <returns></returns>
        public static bool IsTel(string source)
        {
            return Regex.IsMatch(source, "^\\d{3,4}-?\\d{6,8}$", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 目前只支持中国邮政编码，即6个数字
        /// </summary>
        /// <param name="str">邮编号码</param>
        /// <returns></returns>
        public static bool IsPostCode(string str)
        {
            return Regex.IsMatch(str, "^\\d{6}$", RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// 验证身份证是否有效，目前只支持中国身份证号码
        /// </summary>
        /// <param name="Id">身份证号码</param>
        /// <returns></returns>
        public static bool IsIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                return HValidator.IsIDCard18(Id);
            }
            return Id.Length == 15 && HValidator.IsIDCard15(Id);
        }

        private static bool IsIDCard15(string Id)
        {
            long num = 0L;
            if (!long.TryParse(Id, out num) || (double)num < Math.Pow(10.0, 14.0))
            {
                return false;
            }
            if ("11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91".IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime dateTime = default(DateTime);
            return DateTime.TryParse(s, out dateTime);
        }

        private static bool IsIDCard18(string Id)
        {
            long num = 0L;
            if (!long.TryParse(Id.Remove(17), out num) || (double)num < Math.Pow(10.0, 16.0) || !long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out num))
            {
                return false;
            }
            if ("11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91".IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string s = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime dateTime = default(DateTime);
            if (!DateTime.TryParse(s, out dateTime))
            {
                return false;
            }
            string[] array = "1,0,x,9,8,7,6,5,4,3,2".Split(new char[]
            {
                ','
            });
            string[] array2 = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(new char[]
            {
                ','
            });
            char[] array3 = Id.Remove(17).ToCharArray();
            int num2 = 0;
            for (int i = 0; i < 17; i++)
            {
                num2 += int.Parse(array2[i]) * int.Parse(array3[i].ToString());
            }
            int num3 = -1;
            Math.DivRem(num2, 11, out num3);
            return !(array[num3] != Id.Substring(17, 1).ToLower());
        }


        /// <summary>
        /// 验证字符是否为中文汉字
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns></returns>
        public static bool IsChineseChar(string str)
        {
            return Regex.IsMatch(str, "^[\\u4e00-\\u9fa5]+$", RegexOptions.IgnoreCase);
        }


        public static bool hasChineseChar(string str)
        {
            return Regex.IsMatch(str, "[\\u4e00-\\u9fa5]+", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 验证给定的URL是否为图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool IsImage(string url)
        {
            return !string.IsNullOrWhiteSpace(url) && new string[]
            {
                ".jpeg",
                ".jpg",
                ".png",
                ".tif",
                ".tiff",
                ".bmp",
                ".gif"
            }.FirstOrDefault(d => url.ToLower().EndsWith(d)) != null;
        }

        #endregion

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

        public static bool IsInRange(this int thisValue, int begin, int end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        public static bool IsInRange(this DateTime thisValue, DateTime begin, DateTime end)
        {
            return thisValue >= begin && thisValue <= end;
        }

        public static bool IsIn<T>(this T thisValue, params T[] values)
        {
            return values.Contains(thisValue);
        }

        public static bool IsContainsIn(this string thisValue, params string[] inValues)
        {
            return inValues.Any(it => thisValue.Contains(it));
        }

        public static bool IsMatch(this object thisValue, string pattern)
        {
            if (thisValue == null) return false;
            Regex reg = new Regex(pattern);
            return reg.IsMatch(thisValue.ToString());
        }
    }
}
