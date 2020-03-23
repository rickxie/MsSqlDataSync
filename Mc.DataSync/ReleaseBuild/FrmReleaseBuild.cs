using Mc.DataSync.DataSync;
using MiniAbp.DataAccess;
using MiniAbp.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private ReleaseBuildConfig curConfig => _tabConfigList[tabReleaseList.SelectedTab.Name];
        private TabConfig curTabConfig => _tabConfigList[tabReleaseList.SelectedTab.Name].TabConfig;

        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmReleaseBuild_Load(object sender, EventArgs e)
        {
            //1.读取配置文件
            string xmlDirPath = AppPath.GetRootRelative("Configs//Tabs");
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
            var dt = curConfig.GetCheckTable();

            //2.执行本地筛选
            var sameRows = dt.Select("text like '%"+ txt_SearchText.Text + "%' ");
            
            //3.绑定数据
            if (sameRows?.Count() > 0) {
                //3.数据绑定
                dataTableToListview(sameRows.CopyToDataTable());
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
            var dt = curConfig.GetCheckTable();

            //3.数据绑定
            dataTableToListview(dt);

        }

        #region table和listView之间转换

        public void dataTableToListview(DataTable dt)
        {
            ListView lv = listV_ShowData;

            if (dt != null)
            {
                lv.ItemChecked -= listV_ShowData_ItemChecked;
                lv.BeginUpdate();
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
                    lvi.SubItems[0].Name = dr["id"].ToString();
                    lv.Items.Add(lvi);
                    //绑定其它列数据
                    foreach (DataColumn column in dt.Columns)
                    {
                        ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                        lvsi.Name = column.ColumnName;
                        lvsi.Text = dr[column.ColumnName].ToString();
                        lvi.Checked = Convert.ToBoolean(string.IsNullOrWhiteSpace(dr["_Checked"].ToString())? false : dr["_Checked"]);
                        lvi.BackColor = lvi.Checked ? Color.LightGray : Color.White;
                        var status = string.IsNullOrWhiteSpace(dr["_Status"].ToString()) ? "0" : dr["_Status"].ToString();
                        if (status == "1") //新增
                        {
                            lvi.ForeColor = Color.LightGreen;
                        }
                        else if (status == "2")//修改
                        {
                            lvi.ForeColor = Color.LightCoral;
                        }
                        else
                        {
                            lvi.ForeColor = Color.Black;
                        }

                        lvi.SubItems.Add(lvsi);
                    }
                }
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                lv.EndUpdate();
                lv.ItemChecked += listV_ShowData_ItemChecked;
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
            var comparisonConfig =  curConfig.ComparisonConfig;
            var table = curConfig.GetCheckTable();
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
                    item.ForeColor = Color.LightGreen;
                }
                else if (status == 2)//修改
                {
                    item.ForeColor = Color.LightCoral;
                }
                else {
                    item.ForeColor = Color.Black;
                }

                var id = item.Name;
                var row = table.Select($"id = '{id}'");
                row[0]["_Status"] = status;
            }
            ShowChecked();
        }

        private void txt_SearchText_TextChanged(object sender, EventArgs e)
        {
            btn_search_Click(sender, e);
        }

        private void listV_ShowData_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var id = e.Item.Name;
            var table = curConfig.GetCheckTable();
            var row = table.Select($"id = '{id}'");
            row[0]["_Checked"] = e.Item.Checked;
            e.Item.BackColor = e.Item.Checked ? Color.LightGray : Color.White;
            table.AcceptChanges();
            ShowChecked();
        }

        private void ShowChecked()
        {
            var sb = new StringBuilder();
            foreach (var item in _tabConfigList.Values)
            {
                if (item.CheckTable != null)
                {
                    sb.AppendLine($"{item.TabConfig.Name}:");
                    var rows = item.CheckTable.Select("_Checked");
                    for (int i = 0; i < rows.Length; i++)
                    {
                        var value = rows[i]["Text"].ToString();
                        sb.AppendLine($"  {value}");
                    }
                }
            }
            tbResult.Text = sb.ToString();
        }

        /// <summary>
        /// 获取执行指令
        /// </summary>
        /// <returns></returns>
        private List<string> GetBatTxt()
        {
            var sb = new List<string>();
            // 获取执行批处理
            foreach (var tab in _tabConfigList.Values)
            {
                if (tab.CheckTable != null)
                {
                    var rows = tab.CheckTable.Select("_Checked");
                    var command = tab.TabConfig.Handler;
                    
                    for (int i = 0; i < rows.Length; i++)
                    {
                        var row = rows[i];
                        // 构造参数
                        var @params = new StringBuilder();
                        for (int j = 0; j < tab.TabConfig.ReplaceParams.Count; j++)
                        {
                            var pConfig = tab.TabConfig.ReplaceParams[j];
                            foreach (var p in pConfig.ReplaceParam)
                            {
                                @params.Append($"<%{p.ParamName}%>={row[p.InputColunmnName]};");
                            }
                        }

                        var text = row["Text"].ToString();
                        sb.Add($"{command}:{@params}");
                    }
                }
            }
            return sb;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            var commands = GetBatTxt();
            foreach (var cmd in commands)
            {
                var executor = new ExecutorExpert(cmd);
                executor.Parse();
                sb.Append(executor.Execute());
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "数据库脚本（*.sql）|*.sql";
            dialog.DefaultExt = ".sql";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, sb.ToString());
            }
        }

        private void btnChose_Click(object sender, EventArgs e)
        {
            var table = curConfig.GetCheckTable();
            var rows = table.Select($"_Status = 1 or _Status = 2");
            if (rows.Length > 0)
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    rows[i]["_Checked"] = true;
                }
            }

            //2.获取数据
            var dt = curConfig.GetCheckTable();
            //3.数据绑定
            dataTableToListview(dt);
        }
    }
}
