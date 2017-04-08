namespace Shoy.FileCompare
{
    partial class PublisherHelper
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.LastDate = new System.Windows.Forms.DateTimePicker();
            this.btnAction = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SourceDir = new System.Windows.Forms.TextBox();
            this.TargetDir = new System.Windows.Forms.TextBox();
            this.FileExts = new System.Windows.Forms.TextBox();
            this.ReplaceRules = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LastDate
            // 
            this.LastDate.Location = new System.Drawing.Point(475, 72);
            this.LastDate.Name = "LastDate";
            this.LastDate.Size = new System.Drawing.Size(200, 21);
            this.LastDate.TabIndex = 5;
            // 
            // btnAction
            // 
            this.btnAction.Location = new System.Drawing.Point(576, 123);
            this.btnAction.Name = "btnAction";
            this.btnAction.Size = new System.Drawing.Size(80, 57);
            this.btnAction.TabIndex = 10;
            this.btnAction.Text = "Action";
            this.btnAction.UseVisualStyleBackColor = true;
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(5, 210);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.Size = new System.Drawing.Size(670, 156);
            this.rtbLog.TabIndex = 11;
            this.rtbLog.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "源文件夹：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "目标文件夹：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "需替换的扩展名：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 15;
            this.label5.Text = "替换规则：";
            // 
            // SourceDir
            // 
            this.SourceDir.Location = new System.Drawing.Point(110, 7);
            this.SourceDir.Name = "SourceDir";
            this.SourceDir.Size = new System.Drawing.Size(565, 21);
            this.SourceDir.TabIndex = 16;
            // 
            // TargetDir
            // 
            this.TargetDir.Location = new System.Drawing.Point(110, 40);
            this.TargetDir.Name = "TargetDir";
            this.TargetDir.Size = new System.Drawing.Size(565, 21);
            this.TargetDir.TabIndex = 17;
            // 
            // FileExts
            // 
            this.FileExts.Location = new System.Drawing.Point(110, 72);
            this.FileExts.Name = "FileExts";
            this.FileExts.Size = new System.Drawing.Size(359, 21);
            this.FileExts.TabIndex = 18;
            // 
            // ReplaceRules
            // 
            this.ReplaceRules.Location = new System.Drawing.Point(93, 111);
            this.ReplaceRules.Name = "ReplaceRules";
            this.ReplaceRules.Size = new System.Drawing.Size(452, 87);
            this.ReplaceRules.TabIndex = 19;
            this.ReplaceRules.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 20;
            this.label3.Text = "日志信息";
            // 
            // PublisherHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 378);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ReplaceRules);
            this.Controls.Add(this.FileExts);
            this.Controls.Add(this.TargetDir);
            this.Controls.Add(this.SourceDir);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.LastDate);
            this.Name = "PublisherHelper";
            this.Text = "文件比较器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DateTimePicker LastDate;
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SourceDir;
        private System.Windows.Forms.TextBox TargetDir;
        private System.Windows.Forms.TextBox FileExts;
        private System.Windows.Forms.RichTextBox ReplaceRules;
        private System.Windows.Forms.Label label3;
    }
}

