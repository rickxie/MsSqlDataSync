using MiniAbp.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sl.Bpm.CodeTool.Common
{
    public class AppInfo
    {



        private static AppInfo appInfo = new AppInfo();

        public static string AppRootPath
        {
            get { return appInfo.RootPath; }
        }
        public string RootPath
        {
            get
            {
                if (AppSettings<bool>("IsDebug"))
                {
                    return AppPath.RootPath;
                }
                else
                {

                    DirectoryInfo di = new DirectoryInfo(AppPath.RootPath);
                    return di.Parent.Parent.FullName + "\\";
                }
            }
        }

        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T AppSettings<T>(string key)
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
    }
}
