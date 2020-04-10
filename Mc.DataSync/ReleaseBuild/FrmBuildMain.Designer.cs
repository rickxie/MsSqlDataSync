namespace Mc.DataSync.ReleaseBuild
{
    partial class FrmBuildMain
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
            this.btn_SchemaCompare = new System.Windows.Forms.Button();
            this.btn_DataCompare = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.btn_connect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_SchemaCompare
            // 
            this.btn_SchemaCompare.Location = new System.Drawing.Point(48, 74);
            this.btn_SchemaCompare.Name = "btn_SchemaCompare";
            this.btn_SchemaCompare.Size = new System.Drawing.Size(199, 36);
            this.btn_SchemaCompare.TabIndex = 5;
            this.btn_SchemaCompare.Text = "1.架构对比";
            this.btn_SchemaCompare.UseVisualStyleBackColor = true;
            this.btn_SchemaCompare.Click += new System.EventHandler(this.btn_SchemaCompare_Click);
            // 
            // btn_DataCompare
            // 
            this.btn_DataCompare.Location = new System.Drawing.Point(48, 140);
            this.btn_DataCompare.Name = "btn_DataCompare";
            this.btn_DataCompare.Size = new System.Drawing.Size(199, 36);
            this.btn_DataCompare.TabIndex = 10;
            this.btn_DataCompare.Text = "2.表数据对比";
            this.btn_DataCompare.UseVisualStyleBackColor = true;
            this.btn_DataCompare.Click += new System.EventHandler(this.btn_DataCompare_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(48, 207);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(199, 36);
            this.btn_close.TabIndex = 15;
            this.btn_close.Text = "关闭";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(48, 12);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(199, 36);
            this.btn_connect.TabIndex = 1;
            this.btn_connect.Text = "0.数据库链接测试";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // FrmBuildMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 279);
            this.ControlBox = false;
            this.Controls.Add(this.btn_connect);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.btn_DataCompare);
            this.Controls.Add(this.btn_SchemaCompare);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmBuildMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "发布工具";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_SchemaCompare;
        private System.Windows.Forms.Button btn_DataCompare;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.Button btn_connect;
    }
}