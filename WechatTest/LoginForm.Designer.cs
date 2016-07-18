namespace WechatTest
{
    partial class LoginForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_qr = new System.Windows.Forms.PictureBox();
            this.comboBox_users = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_qr
            // 
            this.pictureBox_qr.Location = new System.Drawing.Point(30, 35);
            this.pictureBox_qr.Name = "pictureBox_qr";
            this.pictureBox_qr.Size = new System.Drawing.Size(228, 202);
            this.pictureBox_qr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_qr.TabIndex = 0;
            this.pictureBox_qr.TabStop = false;
            // 
            // comboBox_users
            // 
            this.comboBox_users.FormattingEnabled = true;
            this.comboBox_users.Location = new System.Drawing.Point(30, 255);
            this.comboBox_users.Name = "comboBox_users";
            this.comboBox_users.Size = new System.Drawing.Size(228, 20);
            this.comboBox_users.TabIndex = 1;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 366);
            this.Controls.Add(this.comboBox_users);
            this.Controls.Add(this.pictureBox_qr);
            this.Name = "LoginForm";
            this.Text = "WeChat";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_qr;
        private System.Windows.Forms.ComboBox comboBox_users;
    }
}

