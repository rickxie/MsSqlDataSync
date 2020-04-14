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

            this.backgroundWorker1.WorkerReportsProgress = true;  //设置能报告进度更新
            this.backgroundWorker1.WorkerSupportsCancellation = true;  //设置支持异步取消
            this.bgw_Generate.WorkerReportsProgress = true;  //设置能报告进度更新
            this.bgw_Generate.WorkerSupportsCancellation = true;  //设置支持异步取消
            Control.CheckForIllegalCrossThreadCalls = false;//取消线程间的安全检查
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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("加载失败,请检查配置文件中的获取语句是否可以正确执行!\n异常信息:" + ex.Message);
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
            this.backgroundWorker1.RunWorkerAsync();  //运行backgroundWorker组件
            FrmProgressForm form = new FrmProgressForm(this.backgroundWorker1);  //显示进度条窗体
            form.ShowDialog(this);
            form.Close();
            
        }

        #region 全局对比进度条处理

        //全局对比 ,在另一个线程上开始运行(处理进度条)
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                var comparisonConfig = curConfig.ComparisonConfig;
                var table = curConfig.GetCheckTable();
                var sumCount = listV_ShowData.Items.Count;
                for (int i = 0; i < sumCount; i++)
                {
                    #region 1.执行处理逻辑

                    ListViewItem item = listV_ShowData.Items[i];
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
                    else
                    {
                        item.ForeColor = Color.Black;
                    }

                    var id = item.Name;
                    var row = table.Select($"id = '{id}'");
                    row[0]["_Status"] = status;


                    #endregion

                    #region 2.计算完成度

                    var percentum = Convert.ToInt32((i + 1) / Convert.ToDouble(sumCount) * 100);
                    worker.ReportProgress(percentum);
                    if (worker.CancellationPending) //获取程序是否已请求取消后台操作
                    {
                        e.Cancel = true;
                        break;
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("对比失败,请检查两边对比表结构是否一致!\n异常信息:" + ex.Message);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 全局对比结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                ShowChecked();
            }
            else
            {
                ShowChecked();
            }
        }


        #endregion



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

        /// <summary>
        /// 执行脚本生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //清空数据
            GenerateAppendText.Clear();

            this.bgw_Generate.RunWorkerAsync();  //运行backgroundWorker组件
            FrmProgressForm form = new FrmProgressForm(this.bgw_Generate);  //显示进度条窗体
            form.ShowDialog(this);
            form.Close();
        }

        #region 生成脚本处理进度条

        /// <summary>
        /// 临时生成脚本文本对象
        /// </summary>
        private StringBuilder GenerateAppendText = new StringBuilder();
        private void bgw_Generate_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var commands = GetBatTxt();
            var sumCount = commands.Count;
            for (int i = 0; i < sumCount; i++)
            {
                #region 1.执行处理逻辑

                var executor = new ExecutorExpert(commands[i]);
                executor.Parse();
                GenerateAppendText.Append(executor.Execute());

                #endregion

                #region 2.计算完成度

                var percentum = Convert.ToInt32((i + 1) / Convert.ToDouble(sumCount) * 100);
                worker.ReportProgress(percentum);
                if (worker.CancellationPending) //获取程序是否已请求取消后台操作
                {
                    e.Cancel = true;
                    break;
                }

                #endregion
            }

        }

        /// <summary>
        /// 进度条完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgw_Generate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
               //不做任何操作
            }
            else
            {
                ExpertGenerateSql();
            }
        }

        /// <summary>
        /// 导出生成脚本
        /// </summary>
        private void ExpertGenerateSql() {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "数据库脚本（*.sql）|*.sql";
            dialog.DefaultExt = ".sql";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dialog.FileName, GenerateAppendText.ToString());
            }
        }

        #endregion

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

            ShowChecked();
        }

   


    }
}
