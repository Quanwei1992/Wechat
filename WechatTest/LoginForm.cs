using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wechat.API.Http;
using Wechat.API;
using Wechat;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WechatTest
{
    public partial class LoginForm : Form
    {

        WechatClient wc;

        public LoginForm()
        {
            InitializeComponent();
        }


        /// 清除文本中Html的标签  
        /// </summary>  
        /// <param name="Content"></param>  
        /// <returns></returns>  
        protected string ClearHtml(string Content)
        {
            Content = Zxj_ReplaceHtml("&#[^>]*;", "", Content);
            Content = Zxj_ReplaceHtml("</?marquee[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?object[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?param[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?embed[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?table[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml(" ", "", Content);
            Content = Zxj_ReplaceHtml("</?tr[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?th[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?p[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?a[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?img[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?tbody[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?li[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?span[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?div[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?th[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?td[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?script[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("(javascript|jscript|vbscript|vbs):", "", Content);
            Content = Zxj_ReplaceHtml("on(mouse|exit|error|click|key)", "", Content);
            Content = Zxj_ReplaceHtml("<\\?xml[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("<\\/?[a-z]+:[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?font[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?b[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?u[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?i[^>]*>", "", Content);
            Content = Zxj_ReplaceHtml("</?strong[^>]*>", "", Content);
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
        private string Zxj_ReplaceHtml(string patrn, string strRep, string content)
        {
            if (string.IsNullOrEmpty(content)) {
                content = "";
            }
            Regex rgEx = new Regex(patrn, RegexOptions.IgnoreCase);
            string strTxt = rgEx.Replace(content, strRep);
            return strTxt;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_users.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox_users.AutoCompleteSource = AutoCompleteSource.ListItems;

            wc = new WechatClient();
            wc.OnGetQRCodeImage = (image) => {
                RunInMainthread(()=> {
                    pictureBox_qr.Image = image;
                });
            };

            wc.OnUserScanQRCode = (image) => {
                RunInMainthread(() => {
                    pictureBox_qr.Image = image;
                    label_status.Text = "扫描成功\n请在手机上确认登陆";
                });
            };

            wc.OnLoginSucess = () => {
                RunInMainthread(()=> {
                    label_status.Text = "已确认,正在登陆....";
                });
            };

            wc.OnInitComplate = () => {
                RunInMainthread(() => {
                    label_status.Text = wc.CurrentUser.NickName+"("+wc.CurrentUser.Alias+")";
                });
            };

            wc.OnRecvMsg = (msg) => {
                if (msg.ToUserName == wc.CurrentUser.UserName && !msg.FromUserName.StartsWith("@@")) {
                    System.Diagnostics.Debug.WriteLine("RecvMsg:" + msg.Content + " from " + msg.FromUserName);
                    string rep = "";
                    if (msg.Content == "1") {
                        rep = "你";
                    }
                    if (msg.Content == "2") {
                        rep = "你好";
                    }
                    if (msg.Content == "3") {
                        rep = "你好吗";
                    }
                    //var rep = Simsimi.say(msg.Content);
                    wc.SendMsg(msg.FromUserName, rep);
                    System.Diagnostics.Debug.WriteLine("SendMsg:" + rep + " to " + msg.FromUserName);
                }
            };

            wc.OnAddUser = (user) => {
                RunInMainthread(() => {
                    string nickName = ClearHtml(user.NickName);
                    comboBox_users.Items.Add(nickName + "|" + user.UserName);
                });
            };


            RunAsync(()=>{
                wc.Run();
            });
        }



        void RunAsync(Action action) {
            ((Action)(delegate () {
                action?.Invoke();
            })).BeginInvoke(null, null);
        }

        void RunInMainthread(Action action) {
            this.BeginInvoke((Action)(delegate () {
                action?.Invoke();
            }));
        }

        private void button_send_msg_txt_Click(object sender, EventArgs e)
        {
            string str = comboBox_users.Text;
            var args = str.Split('|');
            if (args.Length == 2) {
                string userName = args[1];
                wc.SendMsg(userName,textBox_msg_text.Text);
            }
        }

        private void button_send_msg_image_Click(object sender, EventArgs e)
        {
            string str = comboBox_users.Text;
            var args = str.Split('|');
            if (args.Length == 2) {
                string userName = args[1];
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK) {
                    if (File.Exists(ofd.FileName)) {
                        var img = Image.FromFile(ofd.FileName);
                        if (wc.SendMsg(userName, img)) {
                            Debug.WriteLine("图片消息发送成功!");
                        } else {
                            Debug.WriteLine("图片消息发送失败!");
                        }
                    }
                }
                
            }
        }

        private void button_refreshGroupMemberInfo_Click(object sender, EventArgs e)
        {
            string str = comboBox_users.Text;
            var args = str.Split('|');
            if (args.Length == 2) {
                string userName = args[1];
                if (userName.StartsWith("@@")) {
                    wc.RefreshGroupMemberInfo(userName);
                }
            }
        }
    }
}
