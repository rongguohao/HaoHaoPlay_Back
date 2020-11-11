using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Hao.Utility
{
    public class H_Check
    {
        internal H_Check()
        {
        }

        public class Argument
        {
            internal Argument()
            {
            }

            public static void NotNull(object argument, string argumentName, string message = "")
            {
                if (argument == null)
                {
                    throw new ArgumentNullException(argumentName, message); //异常中件捕捉到异常信息，记录到日志中
                }
            }

            public static void NotEmpty<T>(ICollection<T> argument, string argumentName)
            {
                NotNull(argument, argumentName, "集合不能为Null");

                if (argument.Count == 0)
                {
                    throw new ArgumentException("集合不能为空", argumentName);
                }
            }
        }
    }
}
