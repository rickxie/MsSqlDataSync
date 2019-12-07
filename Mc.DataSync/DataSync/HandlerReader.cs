using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.DataSync.DataSync
{
    /// <summary>
    /// 执行一个单元命令
    /// </summary>
    public class HandlerExpert
    {
        private string handler { get; set; }
        public HandlerExpert(string handler)
        {
            this.handler = handler;
            
        }
        public List<NameAndSql> nsList = new List<NameAndSql>();
        /// <summary>
        /// 逐行读取字符串
        /// </summary>
        public void ReadSql()
        {
            var strList = new List<string>();
            using (StringReader sr = new StringReader(handler))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    strList.Add(line);
                }
            }
            ReadLines(strList);
        }
        /// <summary>
        /// 设置备注与SQL行
        /// </summary>
        /// <param name="strList"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 执行
        /// </summary>
        public string GetSql()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var nameAndSql in nsList)
            {
                var data = new SyncDataManager(nameAndSql.Sql);
                data.Analyze(true);
                sb.AppendLine("--" + nameAndSql.Name);
                sb.AppendLine(data.SyncSql);
            }
            return sb.ToString();
        }


    }
    public class NameAndSql
    {
        public string Name { get; set; }
        public string Sql { get; set; }
    }
}
