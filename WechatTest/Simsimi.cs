using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wechat.API.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace WechatTest
{

    public class SimsimiResponse
    {
        public string response;
        public string id;
        public int result;
        public string msg;
    }

    public static class Simsimi
    {

        public static SimsimiResponse say(string word)
        {
            HttpClient http = new HttpClient();
            string url = "http://api.simsimi.com/request.p?key=your_paid_key&lc=zh&ft=1.0&text=" + word;
            string ret = http.GET_UTF8String(url);
            var rep = JsonConvert.DeserializeObject<SimsimiResponse>(ret);
            return rep;
        }

    }
}
