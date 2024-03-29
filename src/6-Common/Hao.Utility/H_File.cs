﻿using System.IO;
using System.Text;

namespace Hao.Utility
{
    public class H_File
    {
        #region 文件相关操作
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        public static void CreateFile(string filePath, string text, Encoding encoding)
        {
            if (IsExistFile(filePath))
            {
                DeleteFile(filePath);
            }
            if (!IsExistFile(filePath))
            {
                string directoryPath = GetDirectoryFromFilePath(filePath);
                CreateDirectory(directoryPath);

                FileInfo file = new FileInfo(filePath);
                using (FileStream stream = file.Create())
                {
                    using (StreamWriter writer = new StreamWriter(stream, encoding))
                    {
                        writer.Write(text);
                        writer.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// 是否存在文件夹
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void CreateDirectory(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 获取文件上级路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDirectoryFromFilePath(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            DirectoryInfo directory = file.Directory;
            return directory.FullName;
        }

        /// <summary>
        /// 是否存在文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }


        /// <summary>
        /// 将文件转化为内存流
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static MemoryStream ToMemoryStream(string filePath)
        {
            return new MemoryStream(File.ReadAllBytes(filePath));
        }

        #endregion
    }
}
