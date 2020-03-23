using MiniAbp.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mc.DataSync.ReleaseBuild
{
    public partial class FrmReleaseBuild : Form
    {
        public FrmReleaseBuild()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 辅助类
        /// </summary>
        private ReleaseBuildHelper rbh = new ReleaseBuildHelper();
        /// <summary>
        /// 存放选择对象
        /// </summary>
        private Dictionary<string, ReleaseBuildConfig> _tabConfigList = new Dictionary<string, ReleaseBuildConfig>(); 

        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmReleaseBuild_Load(object sender, EventArgs e)
        {
            //1.读取配置文件
            string xmlDirPath = @"D:\Shalu\Code\Base\MsSqlDataSync\MsSqlDataSync\Mc.DataSync\ReleaseBuild\";
            var listConfigs = rbh.GetTabsByDir(xmlDirPath);
            if (listConfigs.Count > 0)
            {
                //动态生成Tab项
                foreach (var item in listConfigs)
                {
                    var tempTabConfig = item.TabConfig;
                    _tabConfigList.Add(tempTabConfig.Name, item);
                    TabPage page = new TabPage
                    {
                        Text = tempTabConfig.Name,
                        Name = tempTabConfig.Name,
                    };

                    tabReleaseList.TabPages.Add(page);
                }

                //刷新默认选中的tab数据
                LoadTabData();

                //var hs = new Dictionary<string, string>();
                //hs.Add("WorkflowId", "0e823380-a49a-40d5-8d39-f95a91877a66");
                //ReleaseBuild.ComparisonConfig comparisonConfig = listConfigs[0].ComparisonConfig;
                //var t = rbh.ComparisonDataForQuick(comparisonConfig, hs);

                //var viewTable = rbh.GetComparisonViewTable(comparisonConfig, hs);

                //var ds = rbh.ComparisonDataForDetail(comparisonConfig, hs, "WfdWorkflowNode");
            }

        }

        /// <summary>
        /// tab切换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReleaseList_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTabData(tabReleaseList.SelectedTab.Name);
            //MessageBox.Show(tabReleaseList.SelectedTab.Name);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            //1.获取数据
            var tabConfig = _tabConfigList[tabReleaseList.SelectedTab.Name];
            var dt = DbDapper.RunDataTableSql(tabConfig.TabConfig.GetSql);

            //2.执行本地筛选
            var sameRows = dt.Select("text like '"+ txt_SearchText.Text + "%' ");
            
            //3.绑定数据
            listV_ShowData.Items.Clear();
            if (sameRows?.Count() > 0) {
                //3.数据绑定
                dataTableToListview(listV_ShowData, sameRows.CopyToDataTable());
            }

        }

        #region 辅助类方法

        /// <summary>
        /// 根据TabName进行数据加载
        /// </summary>
        /// <param name="tabName"></param>
        private void LoadTabData(string tabName = null)
        {

            if (tabName == null) tabName = tabReleaseList.SelectedTab.Name;

            //1.判断传入参数是否正确
            if (!_tabConfigList.ContainsKey(tabName))
            {
                MessageBox.Show("传入的tab参数不正确!");
                return;
            }

            //2.获取数据
            var tabConfig = _tabConfigList[tabName];
            var dt = DbDapper.RunDataTableSql(tabConfig.TabConfig.GetSql);

            //3.数据绑定
            dataTableToListview(listV_ShowData, dt);

        }

        #region table和listView之间转换

        public void dataTableToListview(ListView lv, DataTable dt)
        {
            if (dt != null)
            {
                lv.Items.Clear();
                //lv.Columns.Clear();
                //for (int i = 0; i < dt.Columns.Count; i++)
                //{
                //    lv.Columns.Add(dt.Columns[i].Caption.ToString());
                //}
                foreach (DataRow dr in dt.Rows)
                {
                    //绑定显示列
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems[0].Text = dr["text"].ToString();
                    lvi.SubItems[0].Name = "$_text";
                    lv.Items.Add(lvi);

                    //绑定其它列数据
                    foreach (DataColumn column in dt.Columns)
                    {
                        ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Name = column.ColumnName;
                        lvsi.Text = dr[column.ColumnName].ToString();
                        lvi.SubItems.Add(lvsi);
                    }
                    //for (int i = 1; i < dt.Columns.Count; i++)
                    //{
                    //    lvi.SubItems.Add(dr[i].ToString());
                    //}


                    //lv.Items.Add(lvi);
                }
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }
        public void listViewToDataTable(ListView lv, DataTable dt)
        {
            int i, j;
            DataRow dr;
            dt.Clear();
            dt.Columns.Clear();
            //生成DataTable列头 
            for (i = 0; i < lv.Columns.Count; i++)
            {
                dt.Columns.Add(lv.Columns[i].Text.Trim(), typeof(String));
            }
            //每行内容 
            for (i = 0; i < lv.Items.Count; i++)
            {
                dr = dt.NewRow();
                for (j = 0; j < lv.Columns.Count; j++)
                {
                    dr[j] = lv.Items[i].SubItems[j].Text.Trim();
                }
                dt.Rows.Add(dr);
            }
        }


        #endregion

        #endregion

        /// <summary>
        /// 双击进行行对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listV_ShowData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //封装输入参数
            Dictionary<string, string> inputPar = new Dictionary<string, string>();
            foreach (ListViewItem.ListViewSubItem subItem in listV_ShowData.FocusedItem.SubItems)
            {
                inputPar.Add(subItem.Name, subItem.Text);
            }

            //打开新对比窗口
            FrmComparisonDetail frmComparisonDetail = new FrmComparisonDetail(_tabConfigList[tabReleaseList.SelectedTab.Name],inputPar);
            frmComparisonDetail.ShowDialog();
        }

        /// <summary>
        /// 全局对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ComparisonAll_Click(object sender, EventArgs e)
        {
            var comparisonConfig = _tabConfigList[tabReleaseList.SelectedTab.Name].ComparisonConfig;
            //遍历所有的行
            foreach (ListViewItem item in listV_ShowData.Items)
            {
                //封装输入参数
                Dictionary<string, string> inputPar = new Dictionary<string, string>();
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    inputPar.Add(subItem.Name, subItem.Text);
                }

                //执行对比,并设置背景颜色
                var status = rbh.ComparisonDataForQuick(comparisonConfig, inputPar);

                if (status == 1) //新增
                {
                    item.BackColor = Color.Chartreuse;
                }
                else if (status == 2)//修改
                {
                    item.BackColor = Color.OrangeRed;
                }
                else {
                    item.BackColor = Color.MintCream;
                }
                
            }


        }


    }
}
