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
using System.IO;
namespace WechatTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var api = new WechatAPIService(new HttpClient());
            var QRSessionID = api.GetNewQRLoginSessionID();
            pictureBox_qr.ImageLocation = api.GetQRCodeUrl(QRSessionID);
            ((Action)(delegate() {

                while (true) {
                    var result = api.Login(QRSessionID);
                    if (result.code == 201) {
                        byte[] base64_image_bytes = Convert.FromBase64String(result.UserAvatar);
                        MemoryStream memoryStream = new MemoryStream(base64_image_bytes, 0, base64_image_bytes.Length);
                        memoryStream.Write(base64_image_bytes, 0, base64_image_bytes.Length);
                        //转成图片
                        var image = Image.FromStream(memoryStream);
                        this.BeginInvoke((Action)(delegate() {
                            pictureBox_qr.Image = image;
                        }));

                    } else if (result.code == 200) {
                        BeginInvoke((Action)(delegate () {
                            var redirectResult = api.LoginRedirect(result.redirect_uri);
                            var initRep = api.Init(redirectResult.pass_ticket, redirectResult.wxuin,redirectResult.wxsid,redirectResult.skey);
                            List<string> userNames = new List<string>();
                            if (initRep.ContactList != null) {
                                foreach (var contact in initRep.ContactList) {
                                    userNames.Add(contact.UserName);
                                    System.Diagnostics.Debug.WriteLine(contact.NickName);
                                }
                            }

                            var chatsetUsers = initRep.ChatSet.Split(',');
                            List<User> users = new List<User>();
                            foreach (var u in chatsetUsers) {
                                if (!u.Contains("@")) continue;
                                User user = new User();
                                user.UserName = u;
                                users.Add(user);
                            }

                            var batchRep = api.BatchGetContact(users.ToArray(), redirectResult.pass_ticket, redirectResult.wxuin, redirectResult.wxsid, redirectResult.skey);
                            if (batchRep.ContactList != null) {
                                foreach (var user in batchRep.ContactList) {
                                    System.Diagnostics.Debug.WriteLine(user.NickName);
                                }
                            }

                            var synccheckRep = api.SyncCheck(initRep.SyncKey.List,redirectResult.wxuin,redirectResult.wxsid,redirectResult.skey);


                        }));
                        return;
                    }
                }


            })).BeginInvoke(null,null);
        }
    }
}
