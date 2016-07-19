using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System;
using System.Collections.Specialized;

namespace Wechat.API.Http
{
    public class HttpClient
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        private  CookieContainer mCookiesContainer;
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] GET(string url)
        {
            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";

                if (mCookiesContainer == null) {
                    mCookiesContainer = new CookieContainer();
                }

                request.CookieContainer = mCookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return new byte[] {0};
            }
        }

        public string GET_UTF8String(string url)
        {
            byte[] bytes = this.GET(url);
            string utf8str = Encoding.UTF8.GetString(bytes);
            return utf8str;
        }


        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public  byte[] POST(string url, string body)
        {
            try {
                byte[] request_body = Encoding.UTF8.GetBytes(body);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.ContentLength = request_body.Length;

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (mCookiesContainer == null) {
                    mCookiesContainer = new CookieContainer();
                }
                request.CookieContainer = mCookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }
                return buf;
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e);
                return new byte[] { 0 };
            }
        }

        public string POST_UTF8String(string url, string body)
        {
            byte[] bytes = this.POST(url,body);
            string utf8str = Encoding.UTF8.GetString(bytes);
            return utf8str;
        }




        /// <summary>
        /// 获取指定cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Cookie GetCookie(string name)
        {
            List<Cookie> cookies = GetAllCookies(mCookiesContainer);
            foreach (Cookie c in cookies) {
                if (c.Name == name) {
                    return c;
                }
            }
            return null;
        }

        private static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values) {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }


        public byte[] UploadFile(string url, byte[] fileBuf,string fileName,string mime_type,NameValueCollection data, Encoding encoding)
        {
            string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            request.Accept = "*/*";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6,zh-TW;q=0.4,ja;q=0.2");


            using (Stream stream = request.GetRequestStream()) {
                //1.1 key/value
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (data != null) {
                    foreach (string key in data.Keys) {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, data[key]);
                        byte[] formitembytes = encoding.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                //1.2 file
                string headerTemplate = "Content-Disposition: form-data; name=\"filename\"; filename=\"{0}\"\r\nContent-Type: "+mime_type+"\r\n\r\n";

                stream.Write(boundarybytes, 0, boundarybytes.Length);
                string header = string.Format(headerTemplate,fileName);
                byte[] headerbytes = encoding.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);
                stream.Write(fileBuf, 0, fileBuf.Length);

                //1.3 form end
                stream.Write(endbytes, 0, endbytes.Length);
            }
            //2.WebResponse
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream response_stream = response.GetResponseStream();

            int count = (int)response.ContentLength;
            int offset = 0;
            byte[] buf = new byte[count];
            while (count > 0)  //读取返回数据
            {
                int n = response_stream.Read(buf, offset, count);
                if (n == 0) break;
                count -= n;
                offset += n;
            }
            return buf;
        }


        public string UploadFile_UTF8String(string url, byte[] fileBuf, string fileName,string mime_type, NameValueCollection data, Encoding encoding)
        {
            var bytes = UploadFile(url,fileBuf,fileName,mime_type,data,encoding);
            string utf8str = Encoding.UTF8.GetString(bytes);
            return utf8str;
        }

    }
}
