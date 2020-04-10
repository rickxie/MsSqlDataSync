namespace Mc.DataSync.ReleaseBuild
{
    partial class FrmReleaseBuild
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
            this.tabReleaseList = new System.Windows.Forms.TabControl();
            this.btn_ComparisonAll = new System.Windows.Forms.Button();
            this.btn_search = new System.Windows.Forms.Button();
            this.txt_SearchText = new System.Windows.Forms.TextBox();
            this.lblSearchText = new System.Windows.Forms.Label();
            this.listV_ShowData = new System.Windows.Forms.ListView();
            this.text = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
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
            // tabReleaseList
            // 
            this.tabReleaseList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabReleaseList.Location = new System.Drawing.Point(8, 22);
            this.tabReleaseList.Name = "tabReleaseList";
            this.tabReleaseList.SelectedIndex = 0;
            this.tabReleaseList.Size = new System.Drawing.Size(366, 23);
            this.tabReleaseList.TabIndex = 0;
            this.tabReleaseList.SelectedIndexChanged += new System.EventHandler(this.tabReleaseList_SelectedIndexChanged);
            // 
            // btn_ComparisonAll
            // 
            this.btn_ComparisonAll.Location = new System.Drawing.Point(328, 57);
            this.btn_ComparisonAll.Name = "btn_ComparisonAll";
            this.btn_ComparisonAll.Size = new System.Drawing.Size(75, 23);
            this.btn_ComparisonAll.TabIndex = 4;
            this.btn_ComparisonAll.Text = "全局对比";
            this.btn_ComparisonAll.UseVisualStyleBackColor = true;
            this.btn_ComparisonAll.Click += new System.EventHandler(this.btn_ComparisonAll_Click);
            // 
            // btn_search
            // 
            this.btn_search.Location = new System.Drawing.Point(274, 57);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(49, 23);
            this.btn_search.TabIndex = 3;
            this.btn_search.Text = "搜索";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // txt_SearchText
            // 
            this.txt_SearchText.Location = new System.Drawing.Point(46, 58);
            this.txt_SearchText.Name = "txt_SearchText";
            this.txt_SearchText.Size = new System.Drawing.Size(224, 21);
            this.txt_SearchText.TabIndex = 2;
            this.txt_SearchText.TextChanged += new System.EventHandler(this.txt_SearchText_TextChanged);
            // 
            // lblSearchText
            // 
            this.lblSearchText.AutoSize = true;
            this.lblSearchText.Location = new System.Drawing.Point(10, 61);
            this.lblSearchText.Name = "lblSearchText";
            this.lblSearchText.Size = new System.Drawing.Size(35, 12);
            this.lblSearchText.TabIndex = 1;
            this.lblSearchText.Text = "条件:";
            // 
            // listV_ShowData
            // 
            this.listV_ShowData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listV_ShowData.CheckBoxes = true;
            this.listV_ShowData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.text});
            this.listV_ShowData.FullRowSelect = true;
            this.listV_ShowData.GridLines = true;
            this.listV_ShowData.HideSelection = false;
            this.listV_ShowData.Location = new System.Drawing.Point(8, 117);
            this.listV_ShowData.Name = "listV_ShowData";
            this.listV_ShowData.Size = new System.Drawing.Size(402, 446);
            this.listV_ShowData.TabIndex = 0;
            this.listV_ShowData.UseCompatibleStateImageBehavior = false;
            this.listV_ShowData.View = System.Windows.Forms.View.Details;
            this.listV_ShowData.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listV_ShowData_ItemChecked);
            this.listV_ShowData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listV_ShowData_MouseDoubleClick);
            // 
            // text
            // 
            this.text.Text = "名称";
            this.text.Width = 323;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listV_ShowData);
            this.splitContainer1.Panel1.Controls.Add(this.btnChose);
            this.splitContainer1.Panel1.Controls.Add(this.btn_ComparisonAll);
            this.splitContainer1.Panel1.Controls.Add(this.lblSearchText);
            this.splitContainer1.Panel1.Controls.Add(this.tabReleaseList);
            this.splitContainer1.Panel1.Controls.Add(this.txt_SearchText);
            this.splitContainer1.Panel1.Controls.Add(this.btn_search);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.btnGenerate);
            this.splitContainer1.Size = new System.Drawing.Size(975, 572);
            this.splitContainer1.SplitterDistance = 414;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 5;
            // 
            // btnChose
            // 
            this.btnChose.Location = new System.Drawing.Point(10, 87);
            this.btnChose.Name = "btnChose";
            this.btnChose.Size = new System.Drawing.Size(75, 23);
            this.btnChose.TabIndex = 4;
            this.btnChose.Text = "全选差异";
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
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(557, 486);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择结果";
            // 
            // tbResult
            // 
            this.tbResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbResult.Location = new System.Drawing.Point(4, 19);
            this.tbResult.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(542, 454);
            this.tbResult.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerate.Location = new System.Drawing.Point(424, 534);
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
            // FrmReleaseBuild
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 572);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmReleaseBuild";
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

        private System.Windows.Forms.TabControl tabReleaseList;
        private System.Windows.Forms.TextBox txt_SearchText;
        private System.Windows.Forms.Label lblSearchText;
        private System.Windows.Forms.ListView listV_ShowData;
        private System.Windows.Forms.Button btn_ComparisonAll;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.ColumnHeader text;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.Button btnChose;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker bgw_Generate;
    }
}