using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MiniAbp.DataAccess;
using MiniAbp.Extension;

namespace Mc.DataSync.DataSync
{
    public class SyncDataManager
    {
        public string Query { get; set; }
        public DataTable Table { get; set; }
        public List<string> Columns { get; set; }
        public string TableName { get; set; }
        private string FromStr { get; set; }
        private readonly string newColumnFormat = "CASE WHEN [{0}] IS NULL THEN 'NULL' WHEN LEN([{0}]) < 100 AND ISDATE([{0}]) = 1 THEN 'N''' + CONVERT(VARCHAR(100),[{0}],21) + '''' ELSE 'N'''+CONVERT(NVARCHAR(MAX), REPLACE( [{0}], '''', '''''' ))+'''' END";

        private readonly string newInsertFormat ="'INSERT INTO {0} ([{1}])VALUES ('+{2}+ ')'";
        private readonly string newUpdateFormat ="'UPDATE {0} SET {1} +' WHERE Id  = ''' + Id + ''''";
        private readonly string updateColumnFormat = "[{0}] =' +  CASE WHEN [{0}] IS NULL THEN 'NULL' WHEN LEN([{0}]) < 100 AND ISDATE([{0}]) = 1 THEN 'N''' + CONVERT(VARCHAR(100),[{0}],21) + '''' ELSE 'N'''+CONVERT(NVARCHAR(MAX), REPLACE( [{0}], '''', '''''' ))+'''' END ";
        private readonly string splitOfNewColumn = "+','+ ";
        private readonly string insertOrUpdateFormat =
            "'IF(EXISTS(SELECT 1 FROM {0} WHERE Id = ''' + Id +''')) BEGIN '+{1}+' END ELSE BEGIN '+{2}+' END'";
        public string SyncInsertQuery { get; set; }
        public string SyncUpdateQuery { get; set; }
        public string SyncInsertOrUpdateQuery { get; set; }
        public string SyncSql { get; set; }
        public SyncDataManager(string dt)
        {
            Query = dt;
        }

        public void Analyze(bool needSql = false)
        {
            Table = DbDapper.RunDataTableSql(Query);
            //初始化列
            InitializeColumns();
            //获取表名
            InitializeTableName();
            //初始化From后面的语句
            InitializeFromString();
            //获取查询语句
            BuildQuery();
            //获取同步脚本
            if(needSql)
            GetSyncScript();
        }

        private void GetSyncScript()
        {
            var result = DbDapper.Query<string>(SyncInsertOrUpdateQuery);
            var sb = new StringBuilder();
            foreach (var str in result)
            {
                sb.AppendLine(str);
            }
            SyncSql = sb.ToString();
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        private void InitializeTableName()
        {
            Regex rg = new Regex(@"FROM\s+([a-zA-Z0-9\._]+)\s*");
            var matchs = rg.Match(Query);
            TableName = matchs.Groups[1].Value;
        }
        /// <summary>
        /// 获取FROM后面的字符串
        /// </summary>
        private void InitializeFromString()
        {
            var indexOfFrom = Query.ToUpper().IndexOf("FROM", 0, StringComparison.Ordinal);
            FromStr = Query.Substring(indexOfFrom, Query.Length - indexOfFrom);
        }

        /// <summary>
        /// 获取所有列
        /// </summary>
        private void InitializeColumns()
        {
            Columns = new List<string>();
            foreach (DataColumn column in Table.Columns)
            {
                Columns.Add(column.ColumnName);
            }
        }
        /// <summary>
        /// 组件查询语句
        /// </summary>
        /// <returns></returns>
        private void BuildQuery()
        {
            List<string> newInsertColumns = Columns.Select(column => newColumnFormat.Fill(column)).ToList();
            var singleInsertQuery = newInsertFormat.Fill(TableName, string.Join("],[", Columns),
                string.Join(splitOfNewColumn, newInsertColumns));
            List<string> newUpdateColumns = Columns.Select(column => updateColumnFormat.Fill(column)).ToList();
            var singleUpdateQuery = newUpdateFormat.Fill(TableName, string.Join("+',", newUpdateColumns));

            var sb = new StringBuilder();
            sb.Append("SELECT [QueryText] = ");
            sb.Append(singleInsertQuery);
            sb.Append(FromStr);
            SyncInsertQuery = sb.ToString();

            sb = new StringBuilder();
            sb.Append("SELECT [QueryText] = ");
            sb.Append(singleUpdateQuery);
            sb.Append(FromStr);
            SyncUpdateQuery = sb.ToString();
            
            sb = new StringBuilder();
            sb.Append("SELECT [QueryText] = ");
            sb.Append(insertOrUpdateFormat.Fill(TableName, singleUpdateQuery, singleInsertQuery));
            sb.Append(FromStr);
            SyncInsertOrUpdateQuery = sb.ToString();
            
        }
    }
}
