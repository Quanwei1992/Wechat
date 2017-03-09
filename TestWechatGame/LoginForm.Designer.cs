namespace TestWechatGame
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_qr = new System.Windows.Forms.PictureBox();
            this.label_tips = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_qr
            // 
            this.pictureBox_qr.Location = new System.Drawing.Point(43, 52);
            this.pictureBox_qr.Name = "pictureBox_qr";
            this.pictureBox_qr.Size = new System.Drawing.Size(270, 270);
            this.pictureBox_qr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_qr.TabIndex = 0;
            this.pictureBox_qr.TabStop = false;
            // 
            // label_tips
            // 
            this.label_tips.Font = new System.Drawing.Font("黑体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_tips.Location = new System.Drawing.Point(43, 339);
            this.label_tips.Name = "label_tips";
            this.label_tips.Size = new System.Drawing.Size(270, 20);
            this.label_tips.TabIndex = 1;
            this.label_tips.Text = "使用手机微信扫码登陆";
            this.label_tips.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 501);
            this.Controls.Add(this.label_tips);
            this.Controls.Add(this.pictureBox_qr);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_qr)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_qr;
        private System.Windows.Forms.Label label_tips;
    }
}