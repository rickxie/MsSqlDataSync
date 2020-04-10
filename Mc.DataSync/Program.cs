using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Mc.DataSync.ReleaseBuild;
using MiniAbp;
using MiniAbp.Runtime;

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
            MiniAbp.MiniAbp.StartWithSqlServer(GetConnectionString());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmBuildMain());
        }
        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T AppSettings<T>(string key)
        {
            try
            {
                var config = ConfigurationManager.AppSettings[key];
                var value = System.Convert.ChangeType(config, typeof(T));
                return (T)value;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        private static string GetConnectionString()
        {
            if (AppSettings<bool>("IsDebug"))
            {
                var defaultConn = ConfigurationManager.ConnectionStrings[AppSettings<string>("ConnectionStringName")]?.ConnectionString;
                return defaultConn;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(AppPath.RootPath);
                var rootPath = di.Parent.Parent.FullName + "\\";
                var webConfig = Path.Combine(rootPath, "Web.Config");
                //将XML文件加载进来
                XDocument document = XDocument.Load(webConfig);
                //获取到XML的根元素进行操作
                XElement root = document.Root;
                XElement ele = root.Element("connectionStrings");
                var adds = ele.Elements("add");
                foreach (var item in adds)
                {
                    if (item.Attribute("name").Value == AppSettings<string>("ConnectionStringName"))
                    {
                        return item.Attribute("connectionString").Value;
                    }
                }
            }
            MessageBox.Show("找不到链接字符串或链接字符串错误，请检查config>ConnectionStringName");
            return null;
        }

    }
}
