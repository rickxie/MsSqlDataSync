using MiniAbp;
using MiniAbp.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mc.DataSync.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            MiniAbp.MiniAbp.StartWithSqlServer(GetConnectionString());

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
            DirectoryInfo di = new DirectoryInfo(AppPath.RootPath);
#if !DEBUG
            var rootPath = di.Parent.Parent.FullName + "\\";
#else
            var rootPath = AppPath.RootPath;
#endif
            var webConfig = Path.Combine(rootPath, "Web.Config");
            //将XML文件加载进来
            XDocument document = XDocument.Load(webConfig);
            //获取到XML的根元素进行操作
            XElement root = document.Root;
            XElement ele = root.Element("connectionStrings");
            var adds = ele.Elements("add");
            var conn = AppSettings<string>("ConnectionStringName") ?? "Default";
            foreach (var item in adds)
            {
                if (item.Attribute("name").Value == conn)
                {
                    return item.Attribute("connectionString").Value;
                }
            }
            Console.WriteLine("找不到链接字符串或链接字符串错误，请检查config>ConnectionStringName");
            return "";
        }
    }
}
