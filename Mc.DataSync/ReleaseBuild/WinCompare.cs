using MiniAbp.DataAccess;
using MiniAbp.Runtime;
using Sl.Bpm.CodeTool.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniAbp.Extension;

namespace Mc.DataSync.ReleaseBuild
{
    [ComVisible(true)]
    public partial class WinCompare : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">窗口显示的title</param>
        /// <param name="diffText">对比内容对象
        /// var jar = new Hashtable();
        /// jar.Add("fileExtension", ".html");
        /// jar.Add("fileName", "test.html");
        /// jar.Add("oldValue", "xxxxx1 <br/> 1111");
        /// jar.Add("newValue", "xxxxx1 <br/> 1111");
        /// </param>
        public WinCompare(string title, Hashtable diffText)
        {
            FormTitle = title;
            DiffText = diffText;
            InitializeComponent();
        }
        /// <summary>
        /// 窗口显示的title
        /// </summary>
        private string FormTitle;
        /// <summary>
        /// 对比内容对象
        /// </summary>
        private Hashtable DiffText;

        private void WinCompare_Load(object sender, EventArgs e)
        {

            this.Text = $"内容对比  "+ FormTitle;
            var path = AppPath.RuntimePath;
            var url = $"file:///{path.Replace("\\", "/")}UI/compare.html";

            this.webBrowser1.ObjectForScripting = this;
            this.webBrowser1.Navigate(url);

        }

        /// <summary>
        /// 获取差异化内容
        /// </summary>
        /// <returns></returns>
        public string GetDiff()
        {
            //var jar = new Hashtable();
            //jar.Add("fileExtension", ".html");
            //jar.Add("fileName", "test.html");
            //jar.Add("oldValue", "xxxxx1 <br/> 1111");
            //jar.Add("newValue", "xxxxx1 <br/> 1111");
            return DiffText.SerializeJson();
        }

        private void WinCompare_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (this.savedContent != this.content)
            //{
            //    MessageBox.Show("等待文件保存中,当前无法关闭窗口,请稍候再试");
            //    e.Cancel = true;
            //}
        }
    }
}
