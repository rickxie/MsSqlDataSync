using MiniAbp.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mc.DataSync.ReleaseBuild
{
    public partial class FrmBuildMain : Form
    {
        public FrmBuildMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 打开架构对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SchemaCompare_Click(object sender, EventArgs e)
        {
            var form = new FrmSchemaComparison();  
            form.ShowDialog(this);
            form.Close();
        }
        /// <summary>
        /// 打开数据对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DataCompare_Click(object sender, EventArgs e)
        {
            var form = new FrmReleaseBuild(); 
            form.ShowDialog(this);
            form.Close();
        }
        /// <summary>
        /// 链接字符串测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_connect_Click(object sender, EventArgs e)
        {
            btn_connect.Enabled = false;
            ReleaseBuildHelper rbh = new ReleaseBuildHelper();

            var sb = new StringBuilder();
            // 当前数据
            try
            {
                DbDapper.RunDataTableSql("select 1");
                sb.AppendLine("1.当前数据库连接成功!");
            }
            catch  
            {

                sb.AppendLine("1.当前数据库连接失败!");
            }

            //目标数据
            try
            {
                rbh.DbTargetQuery("select 1");
                sb.AppendLine("2.目标数据库连接成功!");
            }
            catch
            {

                sb.AppendLine("2.目标数据库连接失败!");
            }
            btn_connect.Enabled = true;
            MessageBox.Show(sb.ToString());
        }


    }
}
