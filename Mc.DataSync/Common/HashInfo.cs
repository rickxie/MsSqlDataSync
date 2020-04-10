using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sl.Bpm.CodeTool.Common
{
    public class HashInfo
    {
        /// <summary>
        /// 哈希代码
        /// </summary>
        public int HashCode { get; set; }

        /// <summary>
        /// 代码编号
        /// </summary>
        public string CodeItemId { get; set; }

        /// <summary>
        /// 变更集编号
        /// </summary>
        public string ChangeSetId { get; set; }

        /// <summary>
        /// 文件是否丢失
        /// </summary>
        public bool IsDelete { get; set; } = false;
    }
}
