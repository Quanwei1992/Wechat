using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Wechat.API
{
    public class LoginResult
    {
        /// <summary>
        /// 201: 已扫描但是未确认登录
        /// 200: 已确认登录
        /// </summary>
        public int code;
        /// <summary>
        /// 以base64编码的用户头像图片
        /// code=201 时可用
        /// </summary>
        public string UserAvatar;
        /// <summary>
        /// 登录重定向Url
        /// code=200 时可用
        /// </summary>
        public string redirect_uri;
    }
}
