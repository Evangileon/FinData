namespace FinData
{
    partial class FxjConverter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FxjConverter));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.status3 = new System.Windows.Forms.TextBox();
            this.status2 = new System.Windows.Forms.Label();
            this.status = new System.Windows.Forms.Label();
            this.outputBox = new System.Windows.Forms.GroupBox();
            this.seperateChar = new System.Windows.Forms.ComboBox();
            this.browseFolder = new System.Windows.Forms.Button();
            this.descPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.url = new System.Windows.Forms.LinkLabel();
            this.ver = new System.Windows.Forms.Label();
            this.fxjPath = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.outputBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.outputBox);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(723, 436);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Blue;
            this.label6.Location = new System.Drawing.Point(8, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "(1)请选择要转换的数据：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.status3);
            this.groupBox4.Controls.Add(this.status2);
            this.groupBox4.Controls.Add(this.status);
            this.groupBox4.Location = new System.Drawing.Point(478, 175);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(225, 126);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "状态(转换速度慢，请耐心等待...)";
            // 
            // status3
            // 
            this.status3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.status3.Location = new System.Drawing.Point(6, 45);
            this.status3.Multiline = true;
            this.status3.Name = "status3";
            this.status3.ReadOnly = true;
            this.status3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.status3.Size = new System.Drawing.Size(213, 75);
            this.status3.TabIndex = 1;
            // 
            // status2
            // 
            this.status2.ForeColor = System.Drawing.Color.Fuchsia;
            this.status2.Location = new System.Drawing.Point(135, 20);
            this.status2.Name = "status2";
            this.status2.Size = new System.Drawing.Size(74, 22);
            this.status2.TabIndex = 0;
            this.status2.Text = "...";
            // 
            // status
            // 
            this.status.ForeColor = System.Drawing.Color.Fuchsia;
            this.status.Location = new System.Drawing.Point(12, 20);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(117, 22);
            this.status.TabIndex = 0;
            this.status.Text = "...";
            // 
            // outputBox
            // 
            this.outputBox.Controls.Add(this.seperateChar);
            this.outputBox.Controls.Add(this.browseFolder);
            this.outputBox.Controls.Add(this.descPath);
            this.outputBox.Controls.Add(this.label3);
            this.outputBox.Controls.Add(this.label2);
            this.outputBox.Location = new System.Drawing.Point(478, 14);
            this.outputBox.Name = "outputBox";
            this.outputBox.Size = new System.Drawing.Size(225, 116);
            this.outputBox.TabIndex = 7;
            this.outputBox.TabStop = false;
            this.outputBox.Text = "(2)输出设置";
            // 
            // seperateChar
            // 
            this.seperateChar.FormattingEnabled = true;
            this.seperateChar.Items.AddRange(new object[] {
            "    ",
            ",",
            "|",
            "/",
            "。",
            "*"});
            this.seperateChar.Location = new System.Drawing.Point(90, 84);
            this.seperateChar.Name = "seperateChar";
            this.seperateChar.Size = new System.Drawing.Size(67, 20);
            this.seperateChar.TabIndex = 13;
            this.seperateChar.Text = "    ";
            // 
            // browseFolder
            // 
            this.browseFolder.Location = new System.Drawing.Point(163, 41);
            this.browseFolder.Name = "browseFolder";
            this.browseFolder.Size = new System.Drawing.Size(56, 22);
            this.browseFolder.TabIndex = 12;
            this.browseFolder.Text = "浏览...";
            this.browseFolder.UseVisualStyleBackColor = true;
            this.browseFolder.Click += new System.EventHandler(this.browseFolder_Click);
            // 
            // descPath
            // 
            this.descPath.Location = new System.Drawing.Point(8, 42);
            this.descPath.Name = "descPath";
            this.descPath.Size = new System.Drawing.Size(151, 21);
            this.descPath.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "·字段分隔符：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "·目标文件夹：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.url);
            this.groupBox2.Controls.Add(this.ver);
            this.groupBox2.Controls.Add(this.fxjPath);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(478, 305);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 119);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "信息";
            // 
            // url
            // 
            this.url.ActiveLinkColor = System.Drawing.Color.Red;
            this.url.AutoSize = true;
            this.url.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.url.LinkColor = System.Drawing.Color.Blue;
            this.url.Location = new System.Drawing.Point(42, 96);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(173, 12);
            this.url.TabIndex = 8;
            this.url.TabStop = true;
            this.url.Text = "http://www.zhangwenzhang.com";
            this.url.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.url_LinkClicked);
            // 
            // ver
            // 
            this.ver.AutoSize = true;
            this.ver.Location = new System.Drawing.Point(94, 63);
            this.ver.Name = "ver";
            this.ver.Size = new System.Drawing.Size(11, 12);
            this.ver.TabIndex = 7;
            this.ver.Text = "V";
            // 
            // fxjPath
            // 
            this.fxjPath.AutoSize = true;
            this.fxjPath.Location = new System.Drawing.Point(23, 43);
            this.fxjPath.Name = "fxjPath";
            this.fxjPath.Size = new System.Drawing.Size(53, 12);
            this.fxjPath.TabIndex = 7;
            this.fxjPath.Text = "安装目录";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "·组件版本：V";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "·分析家安装位置:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(544, 136);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 33);
            this.button1.TabIndex = 2;
            this.button1.Text = "转换(&T)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // treeView1
            // 
            this.treeView1.CheckBoxes = true;
            this.treeView1.Indent = 25;
            this.treeView1.ItemHeight = 20;
            this.treeView1.Location = new System.Drawing.Point(8, 35);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(441, 389);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // FxjConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 454);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FxjConverter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "分析家数据转换器";
            this.Load += new System.EventHandler(this.FxjConverter_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.outputBox.ResumeLayout(false);
            this.outputBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label fxjPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox outputBox;
        private System.Windows.Forms.ComboBox seperateChar;
        private System.Windows.Forms.Button browseFolder;
        private System.Windows.Forms.TextBox descPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Label status2;
        private System.Windows.Forms.TextBox status3;
        private System.Windows.Forms.Label ver;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel url;
    }
}