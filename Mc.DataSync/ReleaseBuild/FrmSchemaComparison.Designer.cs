namespace Mc.DataSync.ReleaseBuild
{
    partial class FrmSchemaComparison
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
            this.btn_ComparisonAll = new System.Windows.Forms.Button();
            this.listV_ShowData = new System.Windows.Forms.ListView();
            this.text = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbox_Type = new System.Windows.Forms.ComboBox();
            this.lbl_Type = new System.Windows.Forms.Label();
            this.btn_search = new System.Windows.Forms.Button();
            this.lblSearchText = new System.Windows.Forms.Label();
            this.txt_SearchText = new System.Windows.Forms.TextBox();
            this.btnChose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.bgw_Generate = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_ComparisonAll
            // 
            this.btn_ComparisonAll.Location = new System.Drawing.Point(92, 37);
            this.btn_ComparisonAll.Name = "btn_ComparisonAll";
            this.btn_ComparisonAll.Size = new System.Drawing.Size(75, 23);
            this.btn_ComparisonAll.TabIndex = 4;
            this.btn_ComparisonAll.Text = "全局对比";
            this.btn_ComparisonAll.UseVisualStyleBackColor = true;
            this.btn_ComparisonAll.Click += new System.EventHandler(this.btn_ComparisonAll_Click);
            // 
            // listV_ShowData
            // 
            this.listV_ShowData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listV_ShowData.CheckBoxes = true;
            this.listV_ShowData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.text,
            this.Type});
            this.listV_ShowData.FullRowSelect = true;
            this.listV_ShowData.GridLines = true;
            this.listV_ShowData.HideSelection = false;
            this.listV_ShowData.Location = new System.Drawing.Point(8, 66);
            this.listV_ShowData.Name = "listV_ShowData";
            this.listV_ShowData.Size = new System.Drawing.Size(403, 497);
            this.listV_ShowData.TabIndex = 0;
            this.listV_ShowData.UseCompatibleStateImageBehavior = false;
            this.listV_ShowData.View = System.Windows.Forms.View.Details;
            this.listV_ShowData.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listV_ShowData_ItemChecked);
            this.listV_ShowData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listV_ShowData_MouseDoubleClick);
            // 
            // text
            // 
            this.text.Text = "名称";
            this.text.Width = 295;
            // 
            // Type
            // 
            this.Type.Tag = "Type";
            this.Type.Text = "类型";
            this.Type.Width = 101;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbox_Type);
            this.splitContainer1.Panel1.Controls.Add(this.lbl_Type);
            this.splitContainer1.Panel1.Controls.Add(this.btn_search);
            this.splitContainer1.Panel1.Controls.Add(this.lblSearchText);
            this.splitContainer1.Panel1.Controls.Add(this.txt_SearchText);
            this.splitContainer1.Panel1.Controls.Add(this.btnChose);
            this.splitContainer1.Panel1.Controls.Add(this.btn_ComparisonAll);
            this.splitContainer1.Panel1.Controls.Add(this.listV_ShowData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.btnGenerate);
            this.splitContainer1.Size = new System.Drawing.Size(975, 572);
            this.splitContainer1.SplitterDistance = 415;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // cbox_Type
            // 
            this.cbox_Type.FormattingEnabled = true;
            this.cbox_Type.Items.AddRange(new object[] {
            "全部",
            "表",
            "函数",
            "视图",
            "存储过程"});
            this.cbox_Type.Location = new System.Drawing.Point(233, 9);
            this.cbox_Type.Name = "cbox_Type";
            this.cbox_Type.Size = new System.Drawing.Size(117, 20);
            this.cbox_Type.TabIndex = 10;
            this.cbox_Type.SelectedIndexChanged += new System.EventHandler(this.cbox_Type_SelectedIndexChanged);
            // 
            // lbl_Type
            // 
            this.lbl_Type.AutoSize = true;
            this.lbl_Type.Location = new System.Drawing.Point(201, 12);
            this.lbl_Type.Name = "lbl_Type";
            this.lbl_Type.Size = new System.Drawing.Size(35, 12);
            this.lbl_Type.TabIndex = 8;
            this.lbl_Type.Text = "类型:";
            // 
            // btn_search
            // 
            this.btn_search.Location = new System.Drawing.Point(356, 8);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(49, 23);
            this.btn_search.TabIndex = 7;
            this.btn_search.Text = "搜索";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // lblSearchText
            // 
            this.lblSearchText.AutoSize = true;
            this.lblSearchText.Location = new System.Drawing.Point(11, 13);
            this.lblSearchText.Name = "lblSearchText";
            this.lblSearchText.Size = new System.Drawing.Size(35, 12);
            this.lblSearchText.TabIndex = 5;
            this.lblSearchText.Text = "名称:";
            // 
            // txt_SearchText
            // 
            this.txt_SearchText.Location = new System.Drawing.Point(47, 9);
            this.txt_SearchText.Name = "txt_SearchText";
            this.txt_SearchText.Size = new System.Drawing.Size(148, 21);
            this.txt_SearchText.TabIndex = 6;
            this.txt_SearchText.TextChanged += new System.EventHandler(this.txt_SearchText_TextChanged);
            // 
            // btnChose
            // 
            this.btnChose.Location = new System.Drawing.Point(8, 36);
            this.btnChose.Name = "btnChose";
            this.btnChose.Size = new System.Drawing.Size(78, 23);
            this.btnChose.TabIndex = 4;
            this.btnChose.Text = "全选/反选";
            this.btnChose.UseVisualStyleBackColor = true;
            this.btnChose.Click += new System.EventHandler(this.btnChose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tbResult);
            this.groupBox1.Location = new System.Drawing.Point(2, 47);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(561, 486);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择结果";
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(4, 19);
            this.tbResult.Margin = new System.Windows.Forms.Padding(2);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbResult.Size = new System.Drawing.Size(541, 454);
            this.tbResult.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(428, 534);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(126, 28);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "生成脚本";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // bgw_Generate
            // 
            this.bgw_Generate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Generate_DoWork);
            this.bgw_Generate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Generate_RunWorkerCompleted);
            // 
            // FrmSchemaComparison
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 572);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmSchemaComparison";
            this.Text = "发布配置工具";
            this.Load += new System.EventHandler(this.FrmReleaseBuild_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listV_ShowData;
        private System.Windows.Forms.Button btn_ComparisonAll;
        private System.Windows.Forms.ColumnHeader text;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Button btnChose;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker bgw_Generate;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.Label lblSearchText;
        private System.Windows.Forms.TextBox txt_SearchText;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.Label lbl_Type;
        private System.Windows.Forms.ComboBox cbox_Type;
    }
}