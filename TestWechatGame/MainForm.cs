using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestWechatGame.PKTEN;
using Wechat;

namespace TestWechatGame
{
    public partial class MainForm : Form
    {

        WechatClient client = new WechatClient();
        

        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            client.OnInitComplate += () => {
                RunInMainthread(()=> {
                    UpdateGroups();
                });

            };
            LoginForm loginForm = new LoginForm(client);
            loginForm.ShowDialog(this);

        }

        DataTable mGroupsTable = new DataTable();
        void UpdateGroups()
        {
            mGroupsTable.Clear();
            mGroupsTable.Columns.Add("UserName");
            mGroupsTable.Columns.Add("DisplayName");

            var contactList = client.ContactList;
            foreach (var contactUserName in contactList) {
                if (contactUserName.StartsWith("@@")) {
                    var user = client.GetContact(contactUserName);
                    DataRow dr = mGroupsTable.NewRow();
                    dr["UserName"] = user.UserName;
                    dr["DisplayName"] = Utils.ClearHtml(user.NickName);
                    mGroupsTable.Rows.Add(dr);
                }
            }

            comboBox_group.DataSource = mGroupsTable;//设置数据源
            comboBox_group.DisplayMember = "DisplayName";//设置显示列
            comboBox_group.ValueMember = "UserName";//设置实际值
            comboBox_group.DropDownStyle = ComboBoxStyle.DropDownList;
        }


        void RunAsync(Action action)
        {
            ((Action)(delegate () {
                action?.Invoke();
            })).BeginInvoke(null, null);
        }

        void RunInMainthread(Action action)
        {
            this.BeginInvoke((Action)(delegate () {
                action?.Invoke();
            }));
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            if (button_run.Text == "启动") {
                button_run.Text = "停止";
                PKTENGame.Instance.OnNewAward += OnNewAward;
                PKTENGame.Instance.RunGame();
                comboBox_group.Enabled = false;
            } else{
                button_run.Text = "启动";
                PKTENGame.Instance.OnNewAward -= OnNewAward;
                PKTENGame.Instance.StopGame();
                comboBox_group.Enabled = true;
            }

        }


        private void OnNewAward(PKTENAward award)
        {
            
            RunInMainthread(() => {
                string userName = comboBox_group.SelectedValue as string;
                client.SendMsg(userName, award.ToString());
            });

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Logout();
        }
    }
}
