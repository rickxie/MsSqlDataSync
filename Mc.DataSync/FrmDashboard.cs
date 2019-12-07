using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Mc.DataSync.DataSync;
using MiniAbp.Extension;

namespace Mc.DataSync
{
    public partial class FrmDashboard : Form
    {
        public FrmDashboard()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (ofdOpenFile.ShowDialog() == DialogResult.OK)
            {
                var linesStr = File.ReadLines(ofdOpenFile.FileName);
                var result = ReadLines(linesStr);
                dgvResult.DataSource = result;
            }
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            FrmDebug main = new FrmDebug();
            main.Show();
        }

        public List<NameAndSql> nsList = new List<NameAndSql>();
        public List<NameAndSql> ReadLines(IEnumerable<string> strList)
        {
            int lineNo = 0;
            var ns = new NameAndSql();
            foreach (var str in strList)
            {
                if (lineNo == 1)
                {
                    lineNo = 0;
                    ns.Sql = str;
                    nsList.Add(ns);
                }
                if (str.StartsWith("--"))
                {
                    lineNo = 1;
                    ns = new NameAndSql();
                    ns.Name = str.Substring(2, str.Length - 2);
                }

            }
            return nsList;
        }


        private void btnBuild_Click(object sender, EventArgs e)
        {

            StringBuilder sb = new StringBuilder();
            foreach (var nameAndSql in nsList)
            {
                var data = new SyncDataManager(nameAndSql.Sql);
                data.Analyze(true);
                sb.AppendLine("--" + nameAndSql.Name);
                sb.AppendLine(data.SyncSql);
            }


            var result = fbdSaveDirectory.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.AppendAllText(fbdSaveDirectory.SelectedPath + "\\{0}.sync.sql".Fill("migration_"+DateTime.Now.ToString("yyyyMMddHHmmss")), sb.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmHelp help = new FrmHelp();
            help.ShowDialog();
        }
        public class NameAndSql
        {
            public string Name { get; set; }
            public string Sql { get; set; }
        }
    }

    
}
