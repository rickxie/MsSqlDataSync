using MiniAbp.Extension;
using MiniAbp.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.DataSync.DataSync
{
    /// <summary>
    /// 命令执行者
    /// </summary>
    public class Commander
    {
        /// <summary>
        /// 模板列表
        /// </summary>
        public static Dictionary<string, string> handlers = new Dictionary<string, string>();
        static Commander()
        {
            // 读取Handler
            DirectoryInfo di = new DirectoryInfo(AppPath.GetBinRoot());
            var handlerPath = Path.Combine(di.FullName, PathConfig.HANDLER_PATH);
            DirectoryInfo handlerFiles = new DirectoryInfo(handlerPath);
            var files = handlerFiles.GetFiles();
            files.Foreach(i =>
            {
                var allTxt = File.ReadAllText(i.FullName);
                handlers.Add(i.Name, allTxt);
            });
        }
        public string Name { get; set; }
        public List<Tuple<string, string>> Params { get; set; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns></returns>
        public string Execute()
        {
            if (!handlers.ContainsKey(Name))
            {
                throw new Exception($"Handler 不存在请确认, {Name}");
            }
            var handler = handlers[this.Name];
            // 替换所有参数
            Params.ForEach(i =>
            {
                handler = handler.Replace(i.Item1, i.Item2);
            });
            var handlerE = new HandlerExpert(handler);
            handlerE.Parse();
            return handlerE.Execute();
        }
    }
}
