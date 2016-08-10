namespace Mc.DataSync
{
    partial class FrmMain
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
            this.tbQueryScript = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbSaveToFile = new System.Windows.Forms.CheckBox();
            this.cbNeedQuery = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dfvResult = new System.Windows.Forms.DataGridView();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.txtSyncQuery = new System.Windows.Forms.TextBox();
            this.txtSyncSql = new System.Windows.Forms.TextBox();
            this.fbdSaveDirectory = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dfvResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbQueryScript
            // 
            this.tbQueryScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbQueryScript.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbQueryScript.Location = new System.Drawing.Point(3, 35);
            this.tbQueryScript.Multiline = true;
            this.tbQueryScript.Name = "tbQueryScript";
            this.tbQueryScript.Size = new System.Drawing.Size(757, 60);
            this.tbQueryScript.TabIndex = 0;
            this.tbQueryScript.Text = "SELECT iD FROM AppUser WHERE iD = \'007c4387-0c17-480b-b858-5f2ecd758fee\'";
            this.tbQueryScript.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbQueryScript_KeyPress);
            this.tbQueryScript.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbQueryScript_KeyUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbSaveToFile);
            this.splitContainer1.Panel1.Controls.Add(this.cbNeedQuery);
            this.splitContainer1.Panel1.Controls.Add(this.tbQueryScript);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(757, 600);
            this.splitContainer1.SplitterDistance = 97;
            this.splitContainer1.TabIndex = 1;
            // 
            // cbSaveToFile
            // 
            this.cbSaveToFile.AutoSize = true;
            this.cbSaveToFile.CheckAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cbSaveToFile.Checked = true;
            this.cbSaveToFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSaveToFile.Location = new System.Drawing.Point(115, 12);
            this.cbSaveToFile.Name = "cbSaveToFile";
            this.cbSaveToFile.Size = new System.Drawing.Size(84, 16);
            this.cbSaveToFile.TabIndex = 2;
            this.cbSaveToFile.Text = "保存到文件";
            this.cbSaveToFile.UseVisualStyleBackColor = true;
            // 
            // cbNeedQuery
            // 
            this.cbNeedQuery.AutoSize = true;
            this.cbNeedQuery.Checked = true;
            this.cbNeedQuery.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbNeedQuery.Location = new System.Drawing.Point(13, 13);
            this.cbNeedQuery.Name = "cbNeedQuery";
            this.cbNeedQuery.Size = new System.Drawing.Size(96, 16);
            this.cbNeedQuery.TabIndex = 1;
            this.cbNeedQuery.Text = "生成同步脚本";
            this.cbNeedQuery.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dfvResult);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(757, 499);
            this.splitContainer2.SplitterDistance = 249;
            this.splitContainer2.TabIndex = 1;
            // 
            // dfvResult
            // 
            this.dfvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dfvResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dfvResult.Location = new System.Drawing.Point(0, 0);
            this.dfvResult.Name = "dfvResult";
            this.dfvResult.RowTemplate.Height = 23;
            this.dfvResult.Size = new System.Drawing.Size(757, 249);
            this.dfvResult.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.txtSyncQuery);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.txtSyncSql);
            this.splitContainer3.Size = new System.Drawing.Size(757, 246);
            this.splitContainer3.SplitterDistance = 77;
            this.splitContainer3.TabIndex = 1;
            // 
            // txtSyncQuery
            // 
            this.txtSyncQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSyncQuery.Location = new System.Drawing.Point(0, 0);
            this.txtSyncQuery.Multiline = true;
            this.txtSyncQuery.Name = "txtSyncQuery";
            this.txtSyncQuery.Size = new System.Drawing.Size(757, 77);
            this.txtSyncQuery.TabIndex = 0;
            this.txtSyncQuery.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSyncQuery_KeyUp);
            // 
            // txtSyncSql
            // 
            this.txtSyncSql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSyncSql.Location = new System.Drawing.Point(0, 0);
            this.txtSyncSql.Multiline = true;
            this.txtSyncSql.Name = "txtSyncSql";
            this.txtSyncSql.Size = new System.Drawing.Size(757, 165);
            this.txtSyncSql.TabIndex = 0;
            this.txtSyncSql.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSyncSql_KeyUp);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 600);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmMain";
            this.Text = "调试台";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dfvResult)).EndInit();
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tbQueryScript;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dfvResult;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox txtSyncQuery;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtSyncSql;
        private System.Windows.Forms.CheckBox cbNeedQuery;
        private System.Windows.Forms.CheckBox cbSaveToFile;
        private System.Windows.Forms.FolderBrowserDialog fbdSaveDirectory;
    }
}

