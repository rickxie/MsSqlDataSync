namespace Mc.DataSync.ReleaseBuild
{
    partial class FrmComparisonDetail
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
            this.dgv_ViewTable = new System.Windows.Forms.DataGridView();
            this.Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DifferentCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OnlySourceCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OnlyTargetCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SameDataCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tblControl = new System.Windows.Forms.TabControl();
            this.tabp_DifferentData = new System.Windows.Forms.TabPage();
            this.dgv_DifferentData = new System.Windows.Forms.DataGridView();
            this.tabp_OnlySourceData = new System.Windows.Forms.TabPage();
            this.dgv_OnlySourceData = new System.Windows.Forms.DataGridView();
            this.tabp_OnlyTargetData = new System.Windows.Forms.TabPage();
            this.dgv_OnlyTargetData = new System.Windows.Forms.DataGridView();
            this.tabp_SameData = new System.Windows.Forms.TabPage();
            this.dgv_SameData = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ViewTable)).BeginInit();
            this.tblControl.SuspendLayout();
            this.tabp_DifferentData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DifferentData)).BeginInit();
            this.tabp_OnlySourceData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OnlySourceData)).BeginInit();
            this.tabp_OnlyTargetData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OnlyTargetData)).BeginInit();
            this.tabp_SameData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SameData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv_ViewTable
            // 
            this.dgv_ViewTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_ViewTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ViewTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Name,
            this.DifferentCount,
            this.OnlySourceCount,
            this.OnlyTargetCount,
            this.SameDataCount});
            this.dgv_ViewTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgv_ViewTable.Location = new System.Drawing.Point(0, 0);
            this.dgv_ViewTable.Name = "dgv_ViewTable";
            this.dgv_ViewTable.RowTemplate.Height = 23;
            this.dgv_ViewTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_ViewTable.Size = new System.Drawing.Size(1218, 218);
            this.dgv_ViewTable.TabIndex = 0;
            this.dgv_ViewTable.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_ViewTable_CellClick);
            // 
            // Name
            // 
            this.Name.DataPropertyName = "Name";
            this.Name.HeaderText = "表名";
            this.Name.Name = "Name";
            this.Name.ReadOnly = true;
            // 
            // DifferentCount
            // 
            this.DifferentCount.DataPropertyName = "DifferentCount";
            this.DifferentCount.HeaderText = "不同记录";
            this.DifferentCount.Name = "DifferentCount";
            this.DifferentCount.ReadOnly = true;
            // 
            // OnlySourceCount
            // 
            this.OnlySourceCount.DataPropertyName = "OnlySourceCount";
            this.OnlySourceCount.HeaderText = "只在源中";
            this.OnlySourceCount.Name = "OnlySourceCount";
            this.OnlySourceCount.ReadOnly = true;
            // 
            // OnlyTargetCount
            // 
            this.OnlyTargetCount.DataPropertyName = "OnlyTargetCount";
            this.OnlyTargetCount.HeaderText = "只在目标";
            this.OnlyTargetCount.Name = "OnlyTargetCount";
            this.OnlyTargetCount.ReadOnly = true;
            // 
            // SameDataCount
            // 
            this.SameDataCount.DataPropertyName = "SameDataCount";
            this.SameDataCount.HeaderText = "相同记录";
            this.SameDataCount.Name = "SameDataCount";
            this.SameDataCount.ReadOnly = true;
            // 
            // tblControl
            // 
            this.tblControl.Controls.Add(this.tabp_DifferentData);
            this.tblControl.Controls.Add(this.tabp_OnlySourceData);
            this.tblControl.Controls.Add(this.tabp_OnlyTargetData);
            this.tblControl.Controls.Add(this.tabp_SameData);
            this.tblControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblControl.Location = new System.Drawing.Point(0, 218);
            this.tblControl.Name = "tblControl";
            this.tblControl.SelectedIndex = 0;
            this.tblControl.Size = new System.Drawing.Size(1218, 385);
            this.tblControl.TabIndex = 1;
            // 
            // tabp_DifferentData
            // 
            this.tabp_DifferentData.Controls.Add(this.dgv_DifferentData);
            this.tabp_DifferentData.Location = new System.Drawing.Point(4, 22);
            this.tabp_DifferentData.Name = "tabp_DifferentData";
            this.tabp_DifferentData.Padding = new System.Windows.Forms.Padding(3);
            this.tabp_DifferentData.Size = new System.Drawing.Size(1210, 359);
            this.tabp_DifferentData.TabIndex = 0;
            this.tabp_DifferentData.Text = "不同记录";
            this.tabp_DifferentData.UseVisualStyleBackColor = true;
            // 
            // dgv_DifferentData
            // 
            this.dgv_DifferentData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_DifferentData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_DifferentData.Location = new System.Drawing.Point(3, 3);
            this.dgv_DifferentData.Name = "dgv_DifferentData";
            this.dgv_DifferentData.RowTemplate.Height = 23;
            this.dgv_DifferentData.Size = new System.Drawing.Size(1204, 353);
            this.dgv_DifferentData.TabIndex = 0;
            this.dgv_DifferentData.DataSourceChanged += new System.EventHandler(this.dgv_DifferentData_DataSourceChanged);
            // 
            // tabp_OnlySourceData
            // 
            this.tabp_OnlySourceData.Controls.Add(this.dgv_OnlySourceData);
            this.tabp_OnlySourceData.Location = new System.Drawing.Point(4, 22);
            this.tabp_OnlySourceData.Name = "tabp_OnlySourceData";
            this.tabp_OnlySourceData.Padding = new System.Windows.Forms.Padding(3);
            this.tabp_OnlySourceData.Size = new System.Drawing.Size(1123, 339);
            this.tabp_OnlySourceData.TabIndex = 1;
            this.tabp_OnlySourceData.Text = "只在源中";
            this.tabp_OnlySourceData.UseVisualStyleBackColor = true;
            // 
            // dgv_OnlySourceData
            // 
            this.dgv_OnlySourceData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_OnlySourceData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_OnlySourceData.Location = new System.Drawing.Point(3, 3);
            this.dgv_OnlySourceData.Name = "dgv_OnlySourceData";
            this.dgv_OnlySourceData.RowTemplate.Height = 23;
            this.dgv_OnlySourceData.Size = new System.Drawing.Size(1117, 333);
            this.dgv_OnlySourceData.TabIndex = 0;
            // 
            // tabp_OnlyTargetData
            // 
            this.tabp_OnlyTargetData.Controls.Add(this.dgv_OnlyTargetData);
            this.tabp_OnlyTargetData.Location = new System.Drawing.Point(4, 22);
            this.tabp_OnlyTargetData.Name = "tabp_OnlyTargetData";
            this.tabp_OnlyTargetData.Size = new System.Drawing.Size(1123, 339);
            this.tabp_OnlyTargetData.TabIndex = 2;
            this.tabp_OnlyTargetData.Text = "只在目标中";
            this.tabp_OnlyTargetData.UseVisualStyleBackColor = true;
            // 
            // dgv_OnlyTargetData
            // 
            this.dgv_OnlyTargetData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_OnlyTargetData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_OnlyTargetData.Location = new System.Drawing.Point(0, 0);
            this.dgv_OnlyTargetData.Name = "dgv_OnlyTargetData";
            this.dgv_OnlyTargetData.RowTemplate.Height = 23;
            this.dgv_OnlyTargetData.Size = new System.Drawing.Size(1123, 339);
            this.dgv_OnlyTargetData.TabIndex = 0;
            // 
            // tabp_SameData
            // 
            this.tabp_SameData.Controls.Add(this.dgv_SameData);
            this.tabp_SameData.Location = new System.Drawing.Point(4, 22);
            this.tabp_SameData.Name = "tabp_SameData";
            this.tabp_SameData.Size = new System.Drawing.Size(1123, 339);
            this.tabp_SameData.TabIndex = 3;
            this.tabp_SameData.Text = "相同的记录";
            this.tabp_SameData.UseVisualStyleBackColor = true;
            // 
            // dgv_SameData
            // 
            this.dgv_SameData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_SameData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_SameData.Location = new System.Drawing.Point(0, 0);
            this.dgv_SameData.Name = "dgv_SameData";
            this.dgv_SameData.RowTemplate.Height = 23;
            this.dgv_SameData.Size = new System.Drawing.Size(1123, 339);
            this.dgv_SameData.TabIndex = 0;
            // 
            // FrmComparisonDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 603);
            this.Controls.Add(this.tblControl);
            this.Controls.Add(this.dgv_ViewTable);
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "明细对比";
            this.Load += new System.EventHandler(this.FrmComparisonDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ViewTable)).EndInit();
            this.tblControl.ResumeLayout(false);
            this.tabp_DifferentData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_DifferentData)).EndInit();
            this.tabp_OnlySourceData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OnlySourceData)).EndInit();
            this.tabp_OnlyTargetData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_OnlyTargetData)).EndInit();
            this.tabp_SameData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_SameData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_ViewTable;
        private System.Windows.Forms.TabControl tblControl;
        private System.Windows.Forms.TabPage tabp_DifferentData;
        private System.Windows.Forms.TabPage tabp_OnlySourceData;
        private System.Windows.Forms.TabPage tabp_OnlyTargetData;
        private System.Windows.Forms.TabPage tabp_SameData;
        private System.Windows.Forms.DataGridView dgv_DifferentData;
        private System.Windows.Forms.DataGridView dgv_OnlySourceData;
        private System.Windows.Forms.DataGridView dgv_OnlyTargetData;
        private System.Windows.Forms.DataGridView dgv_SameData;
        private System.Windows.Forms.DataGridViewTextBoxColumn Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn DifferentCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn OnlySourceCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn OnlyTargetCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SameDataCount;
    }
}