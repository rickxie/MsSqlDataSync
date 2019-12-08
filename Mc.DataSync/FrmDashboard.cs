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
        public HandlerExpert executor = null;
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (ofdOpenFile.ShowDialog() == DialogResult.OK)
            {
                var linesStr = File.ReadAllText(ofdOpenFile.FileName, Encoding.Default);
                executor = new HandlerExpert(linesStr);
                executor.Parse();
                dgvResult.DataSource = executor.nsList;
            }
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            FrmDebug main = new FrmDebug();
            main.Show();
        }



        private void btnBuild_Click(object sender, EventArgs e)
        { 
            var result = fbdSaveDirectory.ShowDialog();
            if (result == DialogResult.OK)
            {
                var sql = executor.Execute();
                File.AppendAllText(fbdSaveDirectory.SelectedPath + "\\{0}.sync.sql".Fill("migration_"+DateTime.Now.ToString("yyyyMMddHHmmss")), sql.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmHelp help = new FrmHelp();
            help.ShowDialog();
        }
    }

    
}
