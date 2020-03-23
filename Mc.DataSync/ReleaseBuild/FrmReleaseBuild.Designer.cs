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
            this.SuspendLayout();
            // 
            // tabReleaseList
            // 
            this.tabReleaseList.Location = new System.Drawing.Point(31, 31);
            this.tabReleaseList.Name = "tabReleaseList";
            this.tabReleaseList.SelectedIndex = 0;
            this.tabReleaseList.Size = new System.Drawing.Size(393, 23);
            this.tabReleaseList.TabIndex = 0;
            this.tabReleaseList.SelectedIndexChanged += new System.EventHandler(this.tabReleaseList_SelectedIndexChanged);
            // 
            // btn_ComparisonAll
            // 
            this.btn_ComparisonAll.Location = new System.Drawing.Point(387, 71);
            this.btn_ComparisonAll.Name = "btn_ComparisonAll";
            this.btn_ComparisonAll.Size = new System.Drawing.Size(75, 23);
            this.btn_ComparisonAll.TabIndex = 4;
            this.btn_ComparisonAll.Text = "全局对比";
            this.btn_ComparisonAll.UseVisualStyleBackColor = true;
            this.btn_ComparisonAll.Click += new System.EventHandler(this.btn_ComparisonAll_Click);
            // 
            // btn_search
            // 
            this.btn_search.Location = new System.Drawing.Point(295, 71);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(75, 23);
            this.btn_search.TabIndex = 3;
            this.btn_search.Text = "搜索";
            this.btn_search.UseVisualStyleBackColor = true;
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // txt_SearchText
            // 
            this.txt_SearchText.Location = new System.Drawing.Point(64, 74);
            this.txt_SearchText.Name = "txt_SearchText";
            this.txt_SearchText.Size = new System.Drawing.Size(224, 21);
            this.txt_SearchText.TabIndex = 2;
            // 
            // lblSearchText
            // 
            this.lblSearchText.AutoSize = true;
            this.lblSearchText.Location = new System.Drawing.Point(32, 77);
            this.lblSearchText.Name = "lblSearchText";
            this.lblSearchText.Size = new System.Drawing.Size(35, 12);
            this.lblSearchText.TabIndex = 1;
            this.lblSearchText.Text = "条件:";
            // 
            // listV_ShowData
            // 
            this.listV_ShowData.CheckBoxes = true;
            this.listV_ShowData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.text});
            this.listV_ShowData.FullRowSelect = true;
            this.listV_ShowData.GridLines = true;
            this.listV_ShowData.HideSelection = false;
            this.listV_ShowData.Location = new System.Drawing.Point(32, 126);
            this.listV_ShowData.Name = "listV_ShowData";
            this.listV_ShowData.Size = new System.Drawing.Size(430, 386);
            this.listV_ShowData.TabIndex = 0;
            this.listV_ShowData.UseCompatibleStateImageBehavior = false;
            this.listV_ShowData.View = System.Windows.Forms.View.Details;
            this.listV_ShowData.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listV_ShowData_MouseDoubleClick);
            // 
            // text
            // 
            this.text.Text = "名称";
            this.text.Width = 423;
            // 
            // FrmReleaseBuild
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 563);
            this.Controls.Add(this.btn_ComparisonAll);
            this.Controls.Add(this.tabReleaseList);
            this.Controls.Add(this.btn_search);
            this.Controls.Add(this.listV_ShowData);
            this.Controls.Add(this.txt_SearchText);
            this.Controls.Add(this.lblSearchText);
            this.Name = "FrmReleaseBuild";
            this.Text = "发布配置工具";
            this.Load += new System.EventHandler(this.FrmReleaseBuild_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabReleaseList;
        private System.Windows.Forms.TextBox txt_SearchText;
        private System.Windows.Forms.Label lblSearchText;
        private System.Windows.Forms.ListView listV_ShowData;
        private System.Windows.Forms.Button btn_ComparisonAll;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.ColumnHeader text;
    }
}