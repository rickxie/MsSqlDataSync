using MiniAbp.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniAbp.Extension;

namespace Mc.DataSync.DataSync
{
    public class ExecutorExpert
    {
        /// <summary>
        /// 命令数据
        /// </summary>
        private string commandTxt;
        /// <summary>
        /// 命令列表
        /// </summary> 
        public List<Commander> commanders = new List<Commander>();
        /// <summary>
        /// 处理器
        /// </summary>
        public ExecutorExpert(string command)
        {
            this.commandTxt = command;
        }
        /// <summary>
        /// 解析
        /// </summary>
        public void Parse()
        {
            // 读取CommandStr
            var commandStrs = new List<string>();
            using (StringReader sr = new StringReader(commandTxt))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var trimedLine = line.Trim();
                    if (!trimedLine.StartsWith("//"))
                    {
                        commandStrs.Add(trimedLine);
                    }
                }
            }
            // 循环解析命令行
            foreach (var item in commandStrs)
            {
                var commandStr = item.Split(':');
                if (commandStr.Length != 2)
                {
                    throw new Exception($"每个命令行仅支持一个符号':'与多个';'分隔，前者用于分隔Handler命令，后者用于分隔参数,{item}");
                }
                var name = commandStr[0].Trim();
                var parmsStr = commandStr[1].Trim().Split(';');
                // 存放参数
                List<Tuple<string, string>> Params = new List<Tuple<string, string>>();
                // 参数列表 <%sss%> = 11 s
                for (int i = 0; i < parmsStr.Length; i++)
                {
                    var curParam = parmsStr[i].Trim().Split('=');
                    if (curParam.Length == 1 && string.IsNullOrWhiteSpace(curParam[0]))
                    {
                        continue;
                    }
                    if (curParam.Length != 2)
                    {
                        throw new Exception($"参数解析错误,每个参数支持一个符号'=',{item}");
                    }
                    // 名称
                    var pName = curParam[0].Trim();
                    // 值
                    var pValue = curParam[1].Trim();
                    Params.Add(new Tuple<string, string>(pName, pValue));
                }
                // 设置Command
                commanders.Add(new Commander() { Name = name, Params = Params });
            }
        }
        
        /// <summary>
        /// 执行生成语句
        /// </summary>
        public string Execute()
        {
            var sb = new StringBuilder();
            commanders.ForEach(i =>
            {
                var sql = i.Execute();
                sb.Append(sql);
            });
            return sb.ToString();
        }
    }


}
