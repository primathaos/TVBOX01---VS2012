namespace TVBOX01
{
    partial class UpMessage
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
            if (disposing && (components != null))
            {
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
            this.Messageshow = new System.Windows.Forms.Label();
            this.UPNOW = new System.Windows.Forms.Button();
            this.UPLATE = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Messageshow
            // 
            this.Messageshow.AutoSize = true;
            this.Messageshow.Font = new System.Drawing.Font("微软雅黑", 16.2F);
            this.Messageshow.ForeColor = System.Drawing.Color.Red;
            this.Messageshow.Location = new System.Drawing.Point(37, 24);
            this.Messageshow.Name = "Messageshow";
            this.Messageshow.Size = new System.Drawing.Size(203, 36);
            this.Messageshow.TabIndex = 0;
            this.Messageshow.Text = "Messageshow";
            // 
            // UPNOW
            // 
            this.UPNOW.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UPNOW.ForeColor = System.Drawing.Color.Red;
            this.UPNOW.Location = new System.Drawing.Point(12, 155);
            this.UPNOW.Name = "UPNOW";
            this.UPNOW.Size = new System.Drawing.Size(288, 50);
            this.UPNOW.TabIndex = 1;
            this.UPNOW.Text = "现在升级";
            this.UPNOW.UseVisualStyleBackColor = true;
            this.UPNOW.Click += new System.EventHandler(this.UPNOW_Click);
            // 
            // UPLATE
            // 
            this.UPLATE.Font = new System.Drawing.Font("微软雅黑 Light", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UPLATE.ForeColor = System.Drawing.Color.Black;
            this.UPLATE.Location = new System.Drawing.Point(309, 155);
            this.UPLATE.Name = "UPLATE";
            this.UPLATE.Size = new System.Drawing.Size(288, 50);
            this.UPLATE.TabIndex = 2;
            this.UPLATE.Text = "稍后升级";
            this.UPLATE.UseVisualStyleBackColor = true;
            this.UPLATE.Click += new System.EventHandler(this.UPLATE_Click);
            // 
            // UpMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 217);
            this.Controls.Add(this.UPLATE);
            this.Controls.Add(this.UPNOW);
            this.Controls.Add(this.Messageshow);
            this.Name = "UpMessage";
            this.Text = "软件升级通知";
            this.Load += new System.EventHandler(this.UpMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Messageshow;
        private System.Windows.Forms.Button UPNOW;
        private System.Windows.Forms.Button UPLATE;
    }
}