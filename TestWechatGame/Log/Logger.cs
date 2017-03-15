using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechatGame
{
    public class Logger
    {

        public static void LogInfo(string format, params object[] argus)
        {
            format = "[INFO]" + format;
            string log = string.Format(format, argus);
            System.Diagnostics.Debug.Write(log);
        }
        public static void LogError(string format, params object[] argus)
        {
            format = "[ERROR]" + format;
            string log = string.Format(format, argus);
            System.Diagnostics.Debug.Write(log);
        }
    }
}
