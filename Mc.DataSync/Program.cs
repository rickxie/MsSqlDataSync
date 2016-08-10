using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniAbp;

namespace Mc.DataSync
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Initiliaze();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmDashboard());
        }

        static readonly YBootstrapper Bootstrapper = new YBootstrapper();
        private static void Initiliaze()
        {
            Bootstrapper.Initialize();
        }
    }
}
