using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mc.DataSync.DataSync;
using MiniAbp.DataAccess;
using System.IO;
using MiniAbp.Extension;

namespace Mc.DataSync
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void tbQueryScript_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 19)
                Run();
        }

        private void Run()
        {
            var sqlScript = tbQueryScript.Text;
            var data = new SyncDataManager(sqlScript);
            data.Analyze(cbNeedQuery.Checked);
            dfvResult.DataSource = data.Table;
            txtSyncQuery.Text = data.SyncInsertOrUpdateQuery;
            if (cbNeedQuery.Checked)
            {
                if (!cbSaveToFile.Checked)
                    txtSyncSql.Text = data.SyncSql;
                else
                {
                    var result = fbdSaveDirectory.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        File.AppendAllText(fbdSaveDirectory.SelectedPath + "\\{0}.sync.sql".Fill(data.TableName), data.SyncSql);
                    }
                }
            }
        }

        private void tbQueryScript_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void txtSyncQuery_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void txtSyncSql_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }
    }
}
