using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Wechat.tools
{
    public class Util
    {
        public static string getMD5(byte[] data) {

            MD5 md5Hash = MD5.Create();
            var hash = md5Hash.ComputeHash(data);

            // 创建一个 Stringbuilder 来收集字节并创建字符串  
            StringBuilder sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串  
            for (int i = 0; i < hash.Length; i++) {
                sBuilder.Append(hash[i].ToString("x2"));
            }

            // 返回十六进制字符串  
            return sBuilder.ToString();

        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }
}
