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
    public partial class FrmComparisonDetail : Form
    {
        #region 私有变量

        /// <summary>
        /// 配置对象
        /// </summary>
        private ReleaseBuildConfig _releaseBuildConfig = null;
        /// <summary>
        /// 对比传入参数
        /// </summary>
        Dictionary<string, string> _inputPar = null;

        /// <summary>
        /// 辅助处理对象
        /// </summary>
        private ReleaseBuildHelper rbh = new ReleaseBuildHelper();

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="releaseBuildConfig"></param>
        /// <param name="inputPar"></param>
        public FrmComparisonDetail(ReleaseBuildConfig releaseBuildConfig, Dictionary<string, string> inputPar)
        {
            InitializeComponent();

            //存储配置对象
            _releaseBuildConfig = releaseBuildConfig;
            _inputPar = inputPar;
        }

        /// <summary>
        /// 加载页面数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmComparisonDetail_Load(object sender, EventArgs e)
        {
            //1.获取所有可以对比的表
            var viewTable = rbh.GetComparisonViewTable(_releaseBuildConfig.ComparisonConfig, _inputPar);
            dgv_ViewTable.DataSource = viewTable;

            //2.获取默认表中对比详情
            if (dgv_ViewTable.SelectedRows.Count > 0) {
                ComparisonDataForDetail(dgv_ViewTable.SelectedRows[0].Cells["Name"].Value.ToString());
            }
        }

        /// <summary>
        /// 行切换时,获取对比数据结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_ViewTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ComparisonDataForDetail(dgv_ViewTable.SelectedRows[0].Cells["Name"].Value.ToString());
        }

        /// <summary>
        /// 获取选中表对比明细
        /// </summary>
        /// <param name="tableName"></param>
        private void ComparisonDataForDetail(string tableName =null) {
            if (tableName == null) return;

            var ds = rbh.ComparisonDataForDetail(_releaseBuildConfig.ComparisonConfig, _inputPar, tableName);
            dgv_DifferentData.DataSource = ds.Tables["DifferentData"];
            tabp_DifferentData.Text = "不同记录(" + ds.Tables["DifferentData"].Rows.Count+")";

            dgv_OnlySourceData.DataSource = ds.Tables["OnlySourceData"];
            tabp_OnlySourceData.Text = "只在源中(" + ds.Tables["OnlySourceData"].Rows.Count + ")";

            dgv_OnlyTargetData.DataSource = ds.Tables["OnlyTargetData"];
            tabp_OnlyTargetData.Text = "只在目标(" + ds.Tables["OnlyTargetData"].Rows.Count + ")";

            dgv_SameData.DataSource = ds.Tables["SameData"];
            tabp_SameData.Text = "相同记录(" + ds.Tables["SameData"].Rows.Count + ")";

        }

        /// <summary>
        /// 数据变化时,高亮显示差异项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_DifferentData_DataSourceChanged(object sender, EventArgs e)
        {
            //将差异列设置背景颜色
            for (int i = 0; i < dgv_DifferentData.Rows.Count; i++)
            {
                var row = dgv_DifferentData.Rows[i];

                //遍历对比有差异的数据
                for (int j = 0; j < row.Cells.Count; j = j + 2)
                {
                    if (Convert.ToString( row.Cells[j].Value) != Convert.ToString(row.Cells[j + 1].Value))
                    {
                        dgv_DifferentData.Rows[i].Cells[j].Style.BackColor = Color.LightSalmon;
                        dgv_DifferentData.Rows[i].Cells[j + 1].Style.BackColor = Color.LightSalmon;
                    }
                }

            }
        }


    }
}
