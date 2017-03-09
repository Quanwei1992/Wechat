using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestWechatGame
{
    public class Utils
    {
        /// 清除文本中Html的标签  
        /// </summary>  
        /// <param name="Content"></param>  
        /// <returns></returns>  
        public static string ClearHtml(string Content)
        {
            Content = ReplaceHtml("&#[^>]*;", "", Content);
            Content = ReplaceHtml("</?marquee[^>]*>", "", Content);
            Content = ReplaceHtml("</?object[^>]*>", "", Content);
            Content = ReplaceHtml("</?param[^>]*>", "", Content);
            Content = ReplaceHtml("</?embed[^>]*>", "", Content);
            Content = ReplaceHtml("</?table[^>]*>", "", Content);
            Content = ReplaceHtml(" ", "", Content);
            Content = ReplaceHtml("</?tr[^>]*>", "", Content);
            Content = ReplaceHtml("</?th[^>]*>", "", Content);
            Content = ReplaceHtml("</?p[^>]*>", "", Content);
            Content = ReplaceHtml("</?a[^>]*>", "", Content);
            Content = ReplaceHtml("</?img[^>]*>", "", Content);
            Content = ReplaceHtml("</?tbody[^>]*>", "", Content);
            Content = ReplaceHtml("</?li[^>]*>", "", Content);
            Content = ReplaceHtml("</?span[^>]*>", "", Content);
            Content = ReplaceHtml("</?div[^>]*>", "", Content);
            Content = ReplaceHtml("</?th[^>]*>", "", Content);
            Content = ReplaceHtml("</?td[^>]*>", "", Content);
            Content = ReplaceHtml("</?script[^>]*>", "", Content);
            Content = ReplaceHtml("(javascript|jscript|vbscript|vbs):", "", Content);
            Content = ReplaceHtml("on(mouse|exit|error|click|key)", "", Content);
            Content = ReplaceHtml("<\\?xml[^>]*>", "", Content);
            Content = ReplaceHtml("<\\/?[a-z]+:[^>]*>", "", Content);
            Content = ReplaceHtml("</?font[^>]*>", "", Content);
            Content = ReplaceHtml("</?b[^>]*>", "", Content);
            Content = ReplaceHtml("</?u[^>]*>", "", Content);
            Content = ReplaceHtml("</?i[^>]*>", "", Content);
            Content = ReplaceHtml("</?strong[^>]*>", "", Content);
            string clearHtml = Content;
            return clearHtml;
        }

        /// <summary>  
        /// 清除文本中的Html标签  
        /// </summary>  
        /// <param name="patrn">要替换的标签正则表达式</param>  
        /// <param name="strRep">替换为的内容</param>  
        /// <param name="content">要替换的内容</param>  
        /// <returns></returns>  
        private static string ReplaceHtml(string patrn, string strRep, string content)
        {
            if (string.IsNullOrEmpty(content)) {
                content = "";
            }
            Regex rgEx = new Regex(patrn, RegexOptions.IgnoreCase);
            string strTxt = rgEx.Replace(content, strRep);
            return strTxt;
        }
    }
}
