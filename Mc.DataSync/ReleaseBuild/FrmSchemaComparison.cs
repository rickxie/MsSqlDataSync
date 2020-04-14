using Mc.DataSync.DataSync;
using MiniAbp.DataAccess;
using MiniAbp.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniAbp.Extension;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Mc.DataSync.ReleaseBuild
{
    public partial class FrmSchemaComparison : Form
    {
        public FrmSchemaComparison()
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
        /// 左边对比架构表对象
        /// </summary>
        private DataTable curLeftTable = new DataTable();

        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmReleaseBuild_Load(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync();  //运行backgroundWorker组件
            FrmProgressForm form = new FrmProgressForm(this.backgroundWorker1);  //显示进度条窗体
            form.ShowDialog(this);
            form.Close();

        }
 

        #region 辅助类方法

        /// <summary>
        /// 加载列表数据
        /// </summary>
        private void LoadData()
        {
            curLeftTable = null;
            //1.获取数据
            curLeftTable = rbh.GetAllSchemaData();

            //2.数据绑定
            dataTableToListview(curLeftTable);

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
                    lvi.SubItems[0].Text = dr["Name"].ToString();
                    lvi.SubItems[0].Name = dr["Id"].ToString();
                    lv.Items.Add(lvi);

                    ListViewItem.ListViewSubItem lvsiType = new ListViewItem.ListViewSubItem();
                    lvsiType.Name = "_TypeName";
                    lvsiType.Text = dr["TypeName"].ToString();
                    lvi.SubItems.Add(lvsiType);

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

        private string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// 双击进行行对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listV_ShowData_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            var row = curLeftTable.Select("Id = '" + listV_ShowData.FocusedItem.Name + "'")[0];
            var type = row["Type"].ToString();
            string fileExtension = ".html", fileName, oldValue, newValue;
            fileName = row["Name"].ToString();
            var ds = rbh.GetDiffText(fileName, type);

            if ("T" == type) {
                fileExtension = ".js";
                oldValue = ds.Tables[0].Rows.Count == 0 ? "": ConvertJsonString(ds.Tables[0].SerializeJson());
                newValue = ds.Tables[1].Rows.Count == 0 ? "" : ConvertJsonString(ds.Tables[1].SerializeJson());
            }
            else {
                oldValue = ds.Tables[0].Rows.Count == 0 ? "" : Convert.ToString(ds.Tables[0].Rows[0]["Definition"]);
                newValue = ds.Tables[1].Rows.Count == 0 ? "" : Convert.ToString(ds.Tables[1].Rows[0]["Definition"]);
            }

            var diffText = new Hashtable();
            diffText.Add("fileExtension", fileExtension);
            diffText.Add("fileName", fileName);
            diffText.Add("oldValue", oldValue);
            diffText.Add("newValue", newValue);

            WinCompare wf = new WinCompare(fileName, diffText);
            wf.ShowDialog();
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

            curLeftTable = null;

            var t = new Task(LoadData);
            t.Start();

            for (int i = 1; i <= 100; )
            {
                if (curLeftTable == null && i < 99) {
                    i++;
                }

                if (curLeftTable != null) {
                    worker.ReportProgress(100);
                    break;
                }
                    
                #region 2.计算完成度

                worker.ReportProgress(i);
                Thread.Sleep(100);
                if (worker.CancellationPending) //获取程序是否已请求取消后台操作
                {
                    e.Cancel = true;
                    break;
                }

                #endregion

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
                //ShowChecked();
            }
            else
            {
                ShowChecked();
            }
        }


        #endregion

        /// <summary>
        /// 选中事项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listV_ShowData_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var id = e.Item.Name;
            var row = curLeftTable.Select($"id = '{id}'");
            row[0]["_Checked"] = e.Item.Checked;
            e.Item.BackColor = e.Item.Checked ? Color.LightGray : Color.White;
            curLeftTable.AcceptChanges();
            ShowChecked();
        }

        /// <summary>
        /// 显示文本内容
        /// </summary>
        private void ShowChecked()
        {
            var sb = new StringBuilder();
            var rows = curLeftTable.Select("_Checked");
            for (int i = 0; i < rows.Length; i++)
            {
                var value = rows[i]["Name"].ToString();
                var typeName = rows[i]["TypeName"].ToString();
                sb.AppendLine($"【{typeName}】：{value}  ");
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
            //// 获取执行批处理
            //foreach (var tab in _tabConfigList.Values)
            //{
            //    if (tab.CheckTable != null)
            //    {
            //        var rows = tab.CheckTable.Select("_Checked");
            //        var command = tab.TabConfig.Handler;
                    
            //        for (int i = 0; i < rows.Length; i++)
            //        {
            //            var row = rows[i];
            //            // 构造参数
            //            var @params = new StringBuilder();
            //            for (int j = 0; j < tab.TabConfig.ReplaceParams.Count; j++)
            //            {
            //                var pConfig = tab.TabConfig.ReplaceParams[j];
            //                foreach (var p in pConfig.ReplaceParam)
            //                {
            //                    @params.Append($"<%{p.ParamName}%>={row[p.InputColunmnName]};");
            //                }
            //            }

            //            var text = row["Text"].ToString();
            //            sb.Add($"{command}:{@params}");
            //        }
            //    }
            //}
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

            //获取对比表数据集合
            //var ds = rbh.GetTableSchemaDetailRows();

            //获取所有选中行
            var rows = curLeftTable.Select("_Checked");
            var sumCount = rows.Length;
            string type, tableName, status,tempText;
            for (int i = 0; i < sumCount; i++)
            {
                #region 1.执行处理逻辑

                tableName = rows[i]["Name"].ToString();
                type = rows[i]["Type"].ToString();
                status = rows[i]["_Status"].ToString();
                if ("T" == type)
                {
                    tempText = rbh.GenerateTableSchemaScript(tableName, Convert.ToInt32(status));
                }
                else
                {
                    //其它生成逻辑
                    GenerateAppendText.AppendLine(rbh.AppendOtherSchemaCheckScript(tableName, type));
                    tempText = rows[i]["Definition"].ToString();
                }

                GenerateAppendText.AppendLine(tempText);
                GenerateAppendText.AppendLine("GO");
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

        /// <summary>
        /// 是否全部选中
        /// </summary>
        private bool IsCheckAll = false;
        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChose_Click(object sender, EventArgs e)
        {
            this.btnChose.Enabled = false;
            IsCheckAll = !IsCheckAll;
            if (null != curLeftTable)
            {
                for (int i = 0; i < curLeftTable.Rows.Count; i++)
                {
                    DataRow row = curLeftTable.Rows[i];
                    listV_ShowData.Items[i].Checked = IsCheckAll;
                    row["_Checked"] = IsCheckAll;
                }
                curLeftTable.AcceptChanges();
                ShowChecked();
            }
            this.btnChose.Enabled = true;
        }
        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_search_Click(object sender, EventArgs e)
        {
            SearchData();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        private void SearchData() {
            //1.获取数据
            var dt = curLeftTable;

            var selectText = "";

            //追加类型
            selectText += (string.IsNullOrEmpty(cbox_Type.Text) || cbox_Type.Text == "全部")?"": " TypeName = '" + cbox_Type.Text + "' ";

            //追加名称
            selectText += (string.IsNullOrEmpty(selectText) ?"": " And ") + " Name like '%" + txt_SearchText.Text + "%' " ;
 
            //2.执行本地筛选
            var sameRows = dt.Select(selectText);

            //3.绑定数据
            if (sameRows?.Count() == 0) {
                dataTableToListview(curLeftTable.Clone());
            }
            else
            {
                //3.数据绑定
                var newDt = sameRows.CopyToDataTable();
                newDt.DefaultView.Sort = "TypeName,Name";
                dataTableToListview(newDt.DefaultView.ToTable());
            }
        }

        /// <summary>
        /// 输入时进行搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_SearchText_TextChanged(object sender, EventArgs e)
        {
            SearchData();
        }
        /// <summary>
        /// 切换时进行搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbox_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchData();
        }
    }
}
