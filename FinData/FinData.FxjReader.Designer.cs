namespace FinData
{
    partial class FxjReader
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FxjReader));
            this.button1 = new System.Windows.Forms.Button();
            this.newFileName = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.code = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dataType = new System.Windows.Forms.ComboBox();
            this.newFileNameDesc = new System.Windows.Forms.Label();
            this.codeDesc = new System.Windows.Forms.Label();
            this.dataTypeDesc = new System.Windows.Forms.Label();
            this.文件FToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.说明DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.关于AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.methodText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.数海淘金网ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.readFieldNames = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.url = new System.Windows.Forms.LinkLabel();
            this.exportData = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cboRecordCount = new System.Windows.Forms.ComboBox();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.btnCurrentDay = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(173, 123);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "读取数据(&D)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // newFileName
            // 
            this.newFileName.Location = new System.Drawing.Point(72, 57);
            this.newFileName.Name = "newFileName";
            this.newFileName.Size = new System.Drawing.Size(110, 21);
            this.newFileName.TabIndex = 2;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(12, 161);
            this.dataGridView.Name = "dataGridView";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.RowHeadersWidth = 70;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(856, 335);
            this.dataGridView.StandardTab = true;
            this.dataGridView.TabIndex = 3;
            this.dataGridView.VirtualMode = true;
            // 
            // code
            // 
            this.code.Location = new System.Drawing.Point(72, 33);
            this.code.Name = "code";
            this.code.Size = new System.Drawing.Size(110, 21);
            this.code.TabIndex = 2;
            this.code.Text = "SH600000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "数据类型";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "代　　码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "新文件名";
            // 
            // dataType
            // 
            this.dataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dataType.FormattingEnabled = true;
            this.dataType.Location = new System.Drawing.Point(72, 10);
            this.dataType.Name = "dataType";
            this.dataType.Size = new System.Drawing.Size(190, 20);
            this.dataType.TabIndex = 5;
            this.dataType.SelectedIndexChanged += new System.EventHandler(this.dataType_SelectedIndexChanged);
            // 
            // newFileNameDesc
            // 
            this.newFileNameDesc.AutoSize = true;
            this.newFileNameDesc.Location = new System.Drawing.Point(188, 60);
            this.newFileNameDesc.Name = "newFileNameDesc";
            this.newFileNameDesc.Size = new System.Drawing.Size(245, 12);
            this.newFileNameDesc.TabIndex = 4;
            this.newFileNameDesc.Text = "（一般为空，仅用于读取历史分笔成交数据）";
            // 
            // codeDesc
            // 
            this.codeDesc.AutoSize = true;
            this.codeDesc.Location = new System.Drawing.Point(188, 36);
            this.codeDesc.Name = "codeDesc";
            this.codeDesc.Size = new System.Drawing.Size(383, 12);
            this.codeDesc.TabIndex = 4;
            this.codeDesc.Text = "（必填，由市场+证券代码组成，如SZ000001、SH000001、SH600000等）";
            // 
            // dataTypeDesc
            // 
            this.dataTypeDesc.AutoSize = true;
            this.dataTypeDesc.Location = new System.Drawing.Point(268, 13);
            this.dataTypeDesc.Name = "dataTypeDesc";
            this.dataTypeDesc.Size = new System.Drawing.Size(53, 12);
            this.dataTypeDesc.TabIndex = 4;
            this.dataTypeDesc.Text = "（必选）";
            // 
            // 文件FToolStripMenuItem
            // 
            this.文件FToolStripMenuItem.Name = "文件FToolStripMenuItem";
            this.文件FToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.文件FToolStripMenuItem.Text = "文件(&F)";
            // 
            // 退出XToolStripMenuItem
            // 
            this.退出XToolStripMenuItem.Name = "退出XToolStripMenuItem";
            this.退出XToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出XToolStripMenuItem.Text = "退出(&X)";
            // 
            // 帮助HToolStripMenuItem
            // 
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.帮助HToolStripMenuItem.Text = "帮助(&H)";
            // 
            // 说明DToolStripMenuItem
            // 
            this.说明DToolStripMenuItem.Name = "说明DToolStripMenuItem";
            this.说明DToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.说明DToolStripMenuItem.Text = "说明(&D)";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // 关于AToolStripMenuItem
            // 
            this.关于AToolStripMenuItem.Name = "关于AToolStripMenuItem";
            this.关于AToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            // 
            // methodText
            // 
            this.methodText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.methodText.Location = new System.Drawing.Point(72, 93);
            this.methodText.Name = "methodText";
            this.methodText.ReadOnly = true;
            this.methodText.Size = new System.Drawing.Size(620, 21);
            this.methodText.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "调用方法";
            // 
            // 数海淘金网ToolStripMenuItem
            // 
            this.数海淘金网ToolStripMenuItem.Name = "数海淘金网ToolStripMenuItem";
            this.数海淘金网ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.数海淘金网ToolStripMenuItem.Text = "数海淘金网";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // readFieldNames
            // 
            this.readFieldNames.Location = new System.Drawing.Point(72, 123);
            this.readFieldNames.Name = "readFieldNames";
            this.readFieldNames.Size = new System.Drawing.Size(95, 27);
            this.readFieldNames.TabIndex = 8;
            this.readFieldNames.Text = "读取字段名(&F)";
            this.readFieldNames.UseVisualStyleBackColor = true;
            this.readFieldNames.Click += new System.EventHandler(this.readFieldNames_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 525);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(880, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // url
            // 
            this.url.ActiveLinkColor = System.Drawing.Color.Red;
            this.url.AutoSize = true;
            this.url.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.url.LinkColor = System.Drawing.Color.Blue;
            this.url.Location = new System.Drawing.Point(535, 9);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(173, 12);
            this.url.TabIndex = 10;
            this.url.TabStop = true;
            this.url.Text = "http://www.zhangwenzhang.com";
            this.url.Visible = false;
            this.url.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.url_LinkClicked);
            // 
            // exportData
            // 
            this.exportData.Location = new System.Drawing.Point(573, 123);
            this.exportData.Name = "exportData";
            this.exportData.Size = new System.Drawing.Size(119, 27);
            this.exportData.TabIndex = 0;
            this.exportData.Text = "导出下表数据(&X)";
            this.exportData.UseVisualStyleBackColor = true;
            this.exportData.Click += new System.EventHandler(this.exportData_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(305, 123);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(174, 28);
            this.button2.TabIndex = 11;
            this.button2.Text = "输出到SQL数据库(&E)-上海的";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cboRecordCount
            // 
            this.cboRecordCount.FormattingEnabled = true;
            this.cboRecordCount.Items.AddRange(new object[] {
            "0",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "45",
            "60",
            "90",
            "120",
            "240",
            "360"});
            this.cboRecordCount.Location = new System.Drawing.Point(441, 10);
            this.cboRecordCount.Name = "cboRecordCount";
            this.cboRecordCount.Size = new System.Drawing.Size(68, 20);
            this.cboRecordCount.TabIndex = 12;
            this.cboRecordCount.Text = "0";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Location = new System.Drawing.Point(372, 13);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(65, 12);
            this.lblRecordCount.TabIndex = 13;
            this.lblRecordCount.Text = "记录个数：";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(484, 123);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(83, 27);
            this.button3.TabIndex = 14;
            this.button3.Text = "输出深圳的";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnCurrentDay
            // 
            this.btnCurrentDay.Location = new System.Drawing.Point(711, 123);
            this.btnCurrentDay.Name = "btnCurrentDay";
            this.btnCurrentDay.Size = new System.Drawing.Size(75, 23);
            this.btnCurrentDay.TabIndex = 15;
            this.btnCurrentDay.Text = "当日分钟";
            this.btnCurrentDay.UseVisualStyleBackColor = true;
            this.btnCurrentDay.Click += new System.EventHandler(this.btnCurrentDay_Click);
            // 
            // FxjReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(880, 547);
            this.Controls.Add(this.btnCurrentDay);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.cboRecordCount);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.url);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.readFieldNames);
            this.Controls.Add(this.methodText);
            this.Controls.Add(this.dataType);
            this.Controls.Add(this.dataTypeDesc);
            this.Controls.Add(this.codeDesc);
            this.Controls.Add(this.newFileNameDesc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.newFileName);
            this.Controls.Add(this.code);
            this.Controls.Add(this.exportData);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FxjReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FinDataTools-分析家数据读取工具";
            this.Load += new System.EventHandler(this.FxjReader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox newFileName;
        public System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox code;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox dataType;
        private System.Windows.Forms.Label newFileNameDesc;
        private System.Windows.Forms.Label codeDesc;
        private System.Windows.Forms.Label dataTypeDesc;
        private System.Windows.Forms.ToolStripMenuItem 文件FToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出XToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助HToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 说明DToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 关于AToolStripMenuItem;
        private System.Windows.Forms.TextBox methodText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem 数海淘金网ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.Button readFieldNames;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.LinkLabel url;
        private System.Windows.Forms.Button exportData;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cboRecordCount;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnCurrentDay;
    }
}

