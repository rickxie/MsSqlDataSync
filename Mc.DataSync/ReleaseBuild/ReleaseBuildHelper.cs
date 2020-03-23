using Dapper;
using MiniAbp.DataAccess;
using MiniAbp.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Mc.DataSync.ReleaseBuild
{
    /// <summary>
    /// 发布构建辅助类
    /// </summary>
    public class ReleaseBuildHelper
    { 
        #region 初始化参数

        /// <summary>
        /// 目标数据库连接字符串
        /// </summary>
        private string _dbTargetConnection = ConfigurationManager.ConnectionStrings["TargetConnection"]?.ConnectionString;

        #endregion

        #region 构造函数

        public ReleaseBuildHelper() {

        }

        #endregion

        #region 对外接口

        /// <summary>
        /// 根据配置文件目录,获取所有的tabs对象
        /// </summary>
        /// <param name="tabsDirPath">tabs配置文件目录,里面为xml文件</param>
        /// <returns></returns>
        public List<ReleaseBuildConfig> GetTabsByDir(string tabsDirPath)
        {
            //1.获取数据
            List<ReleaseBuildConfig> list = new List<ReleaseBuildConfig>();
            if (!Directory.Exists(tabsDirPath))
            {
               throw new Exception($"文件夹路径不存在: {tabsDirPath}");
            }
            DirectoryInfo di = new DirectoryInfo(tabsDirPath);
            var files = di.GetFiles("*.xml");
            StringBuilder sb = new StringBuilder();
            files.Foreach(fileInfo =>
            {
                list.Add(LoadReleaseConfigByPath(fileInfo.FullName));
            });

            //2.执行排序
            list.Sort((left, right) =>
            {
                if (left.TabConfig.No > right.TabConfig.No)
                    return 1;
                else if (left.TabConfig.No == right.TabConfig.No)
                    return 0;
                else
                    return -1;
            });

            return list;
        }

        /// <summary>
        /// 快速对比数据,只返回状态
        /// 无变化 = 0, 新增=1,修改=2
        /// </summary>
        /// <param name="comparisonConfig">对比参数对象</param>
        /// <param name="par">所需参数集合</param>
        /// <returns></returns>
        public int ComparisonDataForQuick(ComparisonConfig comparisonConfig,Dictionary<string,string> inputPar)
    {
        var tableList = comparisonConfig.ComparisonTables[0].ComparisonTableList;

        #region 1.快速校验主表是否属于新增

        var masterTable = tableList.Find(d => d.IsMasterTable == true);
        if (null == masterTable)
        {
            throw new Exception("配置文件中未指定对比的主表.请参照 ComparisonTable-->IsMasterTable 是否等于true.");
        }

        var ds = ComparisonDataByConfig(masterTable, inputPar);

        //如果主表只在源中有数据,则默认就是新增
        if (ds.Tables[TableName_OnlySourceData].Rows.Count > 0) {
            return 1;
        }

        #endregion

        #region 2.校验是否属于修改

        //2.1 校验主表是否属于修改,
        if (ds.Tables[TableName_DifferentData].Rows.Count > 0)
        {
            return 2;
        }

        //2.2 遍历校验其它表
        foreach (var comparisonTable in tableList)
        {
            //主表跳过,之前已经判断过了
            if(comparisonTable.TableName != masterTable.TableName)
            {
                //获取其它表对比数据
                var tempDs = ComparisonDataByConfig(comparisonTable, inputPar);
                //修改判断条件:1.当前表中比目标数据多数据. 2.当前表中的数据和目标表中的数据不一致
                if (tempDs.Tables[TableName_OnlySourceData].Rows.Count > 0 || tempDs.Tables[TableName_DifferentData].Rows.Count > 0)
                {
                    return 2;
                }
            } 
        }

        #endregion

        //否则就默认无变化
        return 0;
    }

        /// <summary>
        /// 获取所有对比表中变更记录
        /// </summary>
        /// <param name="comparisonConfig">对比参数对象</param>
        /// <param name="par">所需参数集合</param>
        /// <returns></returns>
        public DataTable GetComparisonViewTable(ComparisonConfig comparisonConfig, Dictionary<string, string> inputPar)
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("DifferentCount", typeof(string));
            dt.Columns.Add("OnlySourceCount", typeof(string));
            dt.Columns.Add("OnlyTargetCount", typeof(string));
            dt.Columns.Add("SameDataCount", typeof(string));
            dt.TableName = TableName_ViewTable;

            #region 遍历获取各个对比表中的对比数据

            //2.2 遍历校验其它表
            foreach (var comparisonTable in comparisonConfig.ComparisonTables[0].ComparisonTableList)
            {
                //获取其它表对比数据
                var ds = ComparisonDataByConfig(comparisonTable, inputPar);

                var newRow = dt.NewRow();
                newRow["Name"] = comparisonTable.TableName;
                newRow["DifferentCount"] = ds.Tables[TableName_DifferentData].Rows.Count;
                newRow["OnlySourceCount"] = ds.Tables[TableName_OnlySourceData].Rows.Count;
                newRow["OnlyTargetCount"] = ds.Tables[TableName_OnlyTargetData].Rows.Count;
                newRow["SameDataCount"] = ds.Tables[TableName_SameData].Rows.Count;
                dt.Rows.Add(newRow);
            }

            #endregion

            return dt;
        }

        /// <summary>
        /// 明细数据对比,返回需要显示的4张表
        /// DifferentData:不同数据.
        /// OnlySourceData:只在源中的数据.
        /// OnlyTargetData:只在目标中的数据.
        /// SameData:相同记录.
        /// </summary>
        /// <param name="comparisonConfig">对比参数对象</param>
        /// <param name="par">所需参数集合</param>
        /// <param name="tableName">需要获取的表名</param>
        /// <returns></returns>
        public DataSet ComparisonDataForDetail(ComparisonConfig comparisonConfig, Dictionary<string, string> inputPar,string tableName)
        {
 
            //遍历获取指定表对比数据
            foreach (var comparisonTable in comparisonConfig.ComparisonTables[0].ComparisonTableList)
            {
                //获取指定传入表的对比数据
                if (comparisonTable.TableName == tableName)
                {
                    //获取表对比数据
                    return ComparisonDataForDetail(comparisonTable, inputPar);
                }
            }

            //如果未找到指定表,则抛出异常
            throw new Exception("传入的表名不存在,请检查表名:"+ tableName); 
        }

        /// <summary>
        /// 明细数据对比,返回需要显示的4张表
        /// DifferentData:不同数据.
        /// OnlySourceData:只在源中的数据.
        /// OnlyTargetData:只在目标中的数据.
        /// SameData:相同记录.
        /// </summary>
        /// <param name="comparisonTable">对比配置表对象</param>
        /// <param name="par">所需参数集合</param>
        /// <returns></returns>
        public DataSet ComparisonDataForDetail(ComparisonTable comparisonTable, Dictionary<string, string> inputPar)
        {
            //获取表对比数据
            var ds = ComparisonDataByConfig(comparisonTable, inputPar);
            return ds;
        }


        /// <summary>
        /// 将指定xml配置文件转换为配置对象,无权限或者路径不存在则抛出异常
        /// </summary>
        /// <param name="configXmlPath">xml文件全路径</param>
        /// <returns></returns>
        public ReleaseBuildConfig LoadReleaseConfigByPath(string configXmlPath)
        {
            ReleaseBuildConfig releaseBuildConfig = null;
            try
            {
                //xml来源可能是外部文件，也可能是从其他系统获得
                FileStream file = new FileStream(configXmlPath, FileMode.Open, FileAccess.Read);
                XmlSerializer xmlSearializer = new XmlSerializer(typeof(ReleaseBuildConfig));
                releaseBuildConfig = (ReleaseBuildConfig)xmlSearializer.Deserialize(file);
                file.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("加载配置文件出错,请检查文件路径或者格式是否正确.错误信息:" + ex.Message);
            }
            return releaseBuildConfig;
        }

        #endregion


        #region 私有辅助方法

        /// <summary>
        /// 对比表数据Table表名
        /// </summary>
        public readonly string TableName_ViewTable = "ViewTable";
        /// <summary>
        /// 不同数据Table获取表名
        /// </summary>
        public readonly string TableName_DifferentData = "DifferentData";
        /// <summary>
        /// 只在源中的数据Table获取表名
        /// </summary>
        public readonly string TableName_OnlySourceData = "OnlySourceData";
        /// <summary>
        /// 只在目标中的数据Table获取表名
        /// </summary>
        public readonly string TableName_OnlyTargetData = "OnlyTargetData";
        /// <summary>
        /// 相同记录Table获取表名
        /// </summary>
        public readonly string TableName_SameData = "SameData";
        /// <summary>
        /// 表数据对比,返回差异数据
        /// DifferentData:不同数据.
        /// OnlySourceData:只在源中的数据.
        /// OnlyTargetData:只在目标中的数据.
        /// SameData:相同记录.
        /// </summary>
        /// <param name="comparisonConfig">对比参数对象</param>
        /// <param name="par">所需参数集合</param>
        /// <returns></returns>
        private DataSet ComparisonDataByConfig(ComparisonTable tableConfig, Dictionary<string, string> inputPar)
        {
            DataSet ds = new DataSet();

            #region 1.获取查询sql语句 sqlText

            string sqlText = GetSelectDataSql(tableConfig);

            #endregion

            #region 2.封装执行传入参数

            var sqlInputPar = ConvertToDynamicParameters(tableConfig.ComparisonCondition.ReplaceParams[0].ReplaceParam, inputPar);

            #endregion

            #region 3.获取数据进行对比

            //当前数据
            DataTable sourDt = DbDapper.RunDataTableSql(sqlText, sqlInputPar);
            //目标数据
            DataTable tarDt = DbTargetQuery(sqlText, sqlInputPar);


            //1.获取相同记录(Intersect 求两个集合的交集,两边同时存在)
            var sameDataDt = sourDt.Clone();//仅复制表结构
            var tempRows = sourDt.AsEnumerable().Intersect(tarDt.AsEnumerable(), DataRowComparer.Default);
            if(tempRows.Count() > 0)
            {
                sameDataDt = tempRows.CopyToDataTable();
            }
            sameDataDt.TableName = TableName_SameData;
            ds.Tables.Add(sameDataDt);


            //2.获取只在源中的数据(Except 差集,当前集合中存在,而在目标集合中不存在)
            var onlySourceDataRows = from r in sourDt.AsEnumerable()
                                where
                                    !(  from rr in tarDt.AsEnumerable() select rr.Field<string>(tableConfig.PrimaryKey) ).Contains( r.Field<string>(tableConfig.PrimaryKey))
                                select r;
            var onlySourceDataDt = sourDt.Clone();//仅复制表结构
            if (onlySourceDataRows.Count() > 0)
            {
                onlySourceDataDt = onlySourceDataRows.CopyToDataTable();
            }
            onlySourceDataDt.TableName = TableName_OnlySourceData;
            ds.Tables.Add(onlySourceDataDt);

            //3.获取只在目标中的数据(Except 差集,当前集合中存在,而在目标集合中不存在)
            var onlyTargetDataRows = from r in tarDt.AsEnumerable()
                                     where
                                         !(from rr in sourDt.AsEnumerable() select rr.Field<string>(tableConfig.PrimaryKey)).Contains(r.Field<string>(tableConfig.PrimaryKey))
                                     select r;
            var onlyTargetDataDt = tarDt.Clone();//仅复制表结构
            if (onlyTargetDataRows.Count() > 0)
            {
                onlyTargetDataDt = onlyTargetDataRows.CopyToDataTable();
            }
            onlyTargetDataDt.TableName = TableName_OnlyTargetData;
            ds.Tables.Add(onlyTargetDataDt);

            //4.不同数据(主键一致,部分数据内容中存在不一致的数据)
            var differentDataDt = sourDt.Clone();
            //4.1 先获取两边主键Id一致的数据
            var samePrimaryKeyRows = from r in sourDt.AsEnumerable()
                                     where
                                         (from rr in tarDt.AsEnumerable() select rr.Field<string>(tableConfig.PrimaryKey)).Contains(r.Field<string>(tableConfig.PrimaryKey))
                                       select r;
            //4.2 拿主键一致的数据去目标数据中做对比,拿出不一致的数据
            var differentDataRows = samePrimaryKeyRows.Except(tarDt.AsEnumerable(), DataRowComparer.Default);
            if (differentDataRows.Count() > 0) {
                differentDataDt = differentDataRows.CopyToDataTable();
            }

            var tempDt = MergeDataTable(differentDataDt, tarDt, tableConfig.PrimaryKey);
            tempDt.TableName = TableName_DifferentData;
            ds.Tables.Add(tempDt);


            #endregion

            return ds;
        }
        /// <summary>
        /// 源数据列前缀
        /// </summary>
        public readonly string SourColumnPrefix = "->";
        /// <summary>
        /// 目标数据列前缀
        /// </summary>
        public readonly string TargetColumnPrefix = "<-";
       
        /// <summary>
        /// 合并两个列表数据,只有主键一致的数据才会进行合并
        /// </summary>
        /// <param name="sourDt">源数据表</param>
        /// <param name="tarDt">目标数据表</param>
        /// <param name="KeyColName">主键字段</param>
        /// <returns></returns>
        private DataTable MergeDataTable(DataTable sourDt, DataTable tarDt, string KeyColName)
        {
          
            DataTable dt = new DataTable();
            foreach (DataColumn item in sourDt.Columns)
            {
                //增加源字段列
                var sourColumn = new DataColumn(SourColumnPrefix + item.ColumnName, item.DataType, item.Expression, item.ColumnMapping);
                dt.Columns.Add(sourColumn);

                //增加目标字段列
                var targetColumn = new DataColumn(TargetColumnPrefix + item.ColumnName, item.DataType, item.Expression, item.ColumnMapping);
                dt.Columns.Add(targetColumn);
            }
            
            //合并列
            DataTable sourDt1 = sourDt.Copy();
            DataTable tarDt2 = tarDt.Copy();
            sourDt1.PrimaryKey = new DataColumn[] { sourDt1.Columns[KeyColName] };
            tarDt2.PrimaryKey = new DataColumn[] { tarDt2.Columns[KeyColName] };
            //合并相同主键的数据
            foreach (DataRow sourRow in sourDt1.Rows)
            {
                //获取目标表中行
                DataRow targetRow = tarDt2.Rows.Find(sourRow[KeyColName]);
                if (targetRow != null)
                {
                    //增加新行,并赋值
                    var newRow = dt.NewRow();
                    foreach (DataColumn item in sourDt.Columns)
                    {
                        //增加源字段列
                        newRow[SourColumnPrefix + item.ColumnName] = sourRow[item.ColumnName];
                        //增加目标字段列
                        newRow[TargetColumnPrefix + item.ColumnName] = targetRow[item.ColumnName];
                    }
                    dt.Rows.Add(newRow);
                    //移除已经复制的目标数据
                    tarDt2.Rows.Remove(targetRow);
                }
            }
          
            return dt;
        }
 

        /// <summary>
        /// 将替换参数转换为sql传入参数对象
        /// </summary>
        /// <param name="replaceParams"></param>
        /// <param name="inputPar"></param>
        /// <returns></returns>
        private static DynamicParameters ConvertToDynamicParameters(List<ReplaceParam> replaceParams, Dictionary<string, string> inputPar)
        {
            var sqlInputPar = new Dapper.DynamicParameters();
            foreach (var item in replaceParams)
            {
                sqlInputPar.Add(item.ParamName, inputPar[item.InputColunmnName]);
            }

            return sqlInputPar;
        }

        /// <summary>
        /// 根据对比参数,获取对比请求数据sql语句
        /// </summary>
        /// <param name="tableConfig"></param>
        /// <returns></returns>
        private static string GetSelectDataSql(ComparisonTable tableConfig)
        {
            StringBuilder sqlText = new StringBuilder(" SELECT ");
            var comparisonColumnsList = tableConfig.ComparisonColumns[0].ComparisonColumnsList;
            for (int i = 0; i < comparisonColumnsList.Count; i++)
            {
                var item = comparisonColumnsList[i];
                if (i == 0)
                    sqlText.AppendLine(item.ComparisonShowColumn + " = " + item.ComparisonSelectColumn);
                else
                    sqlText.AppendLine("," + item.ComparisonShowColumn + " = " + item.ComparisonSelectColumn);
            }

            sqlText.AppendLine(" FROM " + tableConfig.TableName);
            sqlText.AppendLine(" WHERE " + tableConfig.ComparisonCondition.ConditionSql);

            return sqlText.ToString();
        }


        #region 自定义目标dbconnection查询

        /// <summary>
        /// 执行非主系统数据库数据方法
        /// </summary>
        /// <param name="_dbConnectionTag">webcnfig 的connectionStrings节点name</param>
        /// <param name="sqlText"></param>
        private void DbTargetExcute(string sqlText, object param = null)
        {
            using (var connection = DbDapper.NewDbConnection)
            {
                connection.ConnectionString = _dbTargetConnection;
                connection.Open();
                var transation = connection.BeginTransaction();
                try
                {
                    DbDapper.ExecuteNonQuery(sqlText, param, connection, transation);
                    transation.Commit();//提交事务
                }
                catch (Exception ex)
                {
                    transation.Rollback();//事务回滚
                }
            }
        }

        /// <summary>
        /// 查询非主系统数据库数据方法
        /// </summary>
        /// <param name="_dbConnectionTag">webcnfig 的connectionStrings节点name</param>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        private DataTable DbTargetQuery(string sqlText, object param = null)
        {
            using (var connection = DbDapper.NewDbConnection)
            {
                connection.ConnectionString = _dbTargetConnection;
                connection.Open();
                try
                {
                    var dt = DbDapper.RunDataTableSql(sqlText, param, connection, null);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #endregion

    }

    #region ReleaseBuildConfig 实体

    /// <summary>
    /// 配置对象
    /// </summary>
    public class ReleaseBuildConfig
    {
        TabConfig tabConfig = new TabConfig();
        ComparisonConfig comparisonConfig = new ComparisonConfig();

        /// <summary>
        /// tab显示参数
        /// </summary>
        [XmlElement(ElementName = "TabConfig")]
        public TabConfig TabConfig
        {
            get { return tabConfig; }
            set { tabConfig = value; }
        }

        /// <summary>
        /// 对比参数
        /// </summary>
        [XmlElement(ElementName = "ComparisonConfig")]
        public ComparisonConfig ComparisonConfig
        {
            get { return comparisonConfig; }
            set { comparisonConfig = value; }
        }
        public DataTable CheckTable { get; set; }


        public DataTable GetCheckTable()
        {
            if (this.CheckTable == null)
            {
                var dt = DbDapper.RunDataTableSql(TabConfig.GetSql);
                dt.Columns.Add("_Checked", typeof(bool));
                dt.Columns.Add("_Status", typeof(string));
                CheckTable = dt;
            }
            return CheckTable;
        }
    }

    /// <summary>
    /// 显示Tab参数配置实体
    /// </summary>
    public class TabConfig
    {
        /// <summary>
        /// Tab排列顺序号,例如:1
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// Tab显示名称,例如:流程
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 对应生成脚本的处理sql,例如:workflow.sql
        /// </summary>
        public string Handler { get; set; }
        /// <summary>
        /// 对应显示列表数据查询脚本(注:作为参数传递的列必须要查询出来)
        /// 例如:select * from workflow
        /// </summary>
        public string GetSql { get; set; }


        List<ReplaceParams> replaceParams = new List<ReplaceParams>();
        /// <summary>
        /// 传递参数配置
        /// </summary>
        [XmlElement(ElementName = "ReplaceParams")]
        public List<ReplaceParams> ReplaceParams
        {
            get { return replaceParams; }
            set { replaceParams = value; }
        }

    }

    #region ComparisonConfig 
    /// <summary>
    /// 对比表配置表
    /// </summary>
    public class ComparisonConfig
    {
        List<ComparisonTables> comparisonTables = new List<ComparisonTables>();
        /// <summary>
        /// 传递参数配置
        /// </summary>
        [XmlElement(ElementName = "ComparisonTables")]
        public List<ComparisonTables> ComparisonTables
        {
            get { return comparisonTables; }
            set { comparisonTables = value; }
        }
    }

    /// <summary>
    /// 对比表配置集合 
    /// </summary>
    public class ComparisonTables
    {
        List<ComparisonTable> comparisonTableList = new List<ComparisonTable>();

        [XmlElement(ElementName = "ComparisonTable")]
        public List<ComparisonTable> ComparisonTableList
        {
            get { return comparisonTableList; }
            set { comparisonTableList = value; }
        }
    }
    /// <summary>
    /// 对比表配置项参数
    /// </summary>
    public class ComparisonTable
    {
        /// <summary>
        /// 需要对比的表名,例如:WfdWorkflow
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 是否为校验主表,方便外层对比是否为新增,加快速度
        /// </summary>
        public bool IsMasterTable { get; set; }
        /// <summary>
        /// 主键,例如:Id
        /// </summary>
        public string PrimaryKey { get; set; }

        List<ComparisonColumns> comparisonColumnList = new List<ComparisonColumns>();
        /// <summary>
        /// 对比列集合
        /// </summary>
        [XmlElement(ElementName = "ComparisonColumns")]
        public List<ComparisonColumns> ComparisonColumns
        {
            get { return comparisonColumnList; }
            set { comparisonColumnList = value; }
        }

        ComparisonCondition comparisonCondition = new ComparisonCondition();
        /// <summary>
        /// 对比数据筛选条件
        /// </summary>
        [XmlElement(ElementName = "ComparisonCondition")]
        public ComparisonCondition ComparisonCondition
        {
            get { return comparisonCondition; }
            set { comparisonCondition = value; }
        }
    }

    /// <summary>
    /// 需要对比的列集合 
    /// </summary>
    public class ComparisonColumns
    {
        List<ComparisonColumn> comparisonColumnList = new List<ComparisonColumn>();

        [XmlElement(ElementName = "ComparisonColumn")]
        public List<ComparisonColumn> ComparisonColumnsList
        {
            get { return comparisonColumnList; }
            set { comparisonColumnList = value; }
        }
    }

    public class ComparisonColumn
    {

        string comparisonSelectColumn=string.Empty;
        /// <summary>
        /// 查询列字段或者函数
        /// </summary>
        [XmlElement("ComparisonSelectColumn")]
        public string ComparisonSelectColumn
        {
            get { return comparisonSelectColumn; }
            set { comparisonSelectColumn = value; }
        }

        string comparisonShowColumn = string.Empty;
        [XmlElement("ComparisonShowColumn")]
        /// <summary>
        /// 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 
        /// </summary>
        public string ComparisonShowColumn {
            get {
                if (string.IsNullOrEmpty(comparisonShowColumn))
                    return comparisonSelectColumn;
                else
                    return comparisonShowColumn;
            }
            set { comparisonShowColumn = value; }
        }

    }

    /// <summary>
    /// 对比数据筛选条件
    /// </summary>
    public class ComparisonCondition {
        /// <summary>
        /// 筛选数据语句,例如:[ActionLinkId] IN ( SELECT[ActionLinkId] FROM WfdWorkflowNode WHERE WfdWorkflowId = @WfdWorkflowId )
        /// </summary>
        public string ConditionSql { get; set; }

        List<ReplaceParams> replaceParams = new List<ReplaceParams>();
        /// <summary>
        /// 传递参数配置
        /// </summary>
        [XmlElement(ElementName = "ReplaceParams")]
        public List<ReplaceParams> ReplaceParams
        {
            get { return replaceParams; }
            set { replaceParams = value; }
        }
    }

    #endregion

    #region ReplaceParams 传递参数

    /// <summary>
    /// 传递参数集合 
    /// </summary>
    public class ReplaceParams
    {
        List<ReplaceParam> replaceParamList = new List<ReplaceParam>();

        [XmlElement(ElementName = "ReplaceParam")]
        public List<ReplaceParam> ReplaceParam
        {
            get { return replaceParamList; }
            set { replaceParamList = value; }
        }
    }
    /// <summary>
    /// 传递参数对象
    /// </summary>
    public class ReplaceParam
    {
        /// <summary>
        /// 执行sql参数化字段名称
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 传入参数化获取数据时传入的字段名称
        /// </summary>
        public string InputColunmnName { get; set; }
    }

    #endregion

    #endregion

    #region xml配置文件格式
    /*xml配置文件格式
     <?xml version='1.0' encoding='utf-8'?>
<ReleaseBuildConfig>
  <!-- Tab展示配置 -->
  <TabConfig>
    <!-- Tab排序号 -->
    <No>1</No>
    <!-- Tab显示名称 -->
    <Name>流程</Name>
    <!-- 对应生成脚本sql -->
    <Handler>workflow.sql</Handler>
    <!-- 对应显示数据脚本,必须要将作为参数的列查询出来 -->
    <GetSql>select Id AS WorkflowId,Code AS WorkflowCode,* from workflow</GetSql>
    <!-- 传递参数配置 -->
    <ReplaceParams>
      <ReplaceParam>
        <!-- 参数列字段  -->
        <Key>Id</Key>
        <!-- 参数替换变量名称 -->
        <ReplaceText>&lt;WorkflowId&gt;</ReplaceText>
      </ReplaceParam>
      <ReplaceParam>
        <!-- 参数列字段  -->
        <Key>Code</Key>
        <!-- 参数替换变量名称 -->
        <ReplaceText>&lt;WorkflowCode&gt;</ReplaceText>
      </ReplaceParam>
    </ReplaceParams>
  </TabConfig>
  <!-- 对比配置 -->
  <ComparisonConfig>
    <ComparisonTables>

      <!-- 对比表配置项 WfdWorkflow -->
      <ComparisonTable>
        <!-- 表名 -->
        <TableName>WfdWorkflow</TableName>
        <!-- 是否为校验主表,方便外层对比是否为新增,加快速度 -->
        <IsMasterTable>true</IsMasterTable>
        <!-- 主键 -->
        <PrimaryKey>Id</PrimaryKey>
        <!-- 需要对比的列 -->
        <ComparisonColumns>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Id</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn></ComparisonShowColumn>
          </ComparisonColumn>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Code</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn></ComparisonShowColumn>
          </ComparisonColumn>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Memo</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn></ComparisonShowColumn>
          </ComparisonColumn>
        </ComparisonColumns>
        <!-- 对比数据筛选条件 -->
        <ComparisonCondition>
          <!-- 筛选数据语句 -->
          <ConditionSql>
            Id = @WfdWorkflowId
          </ConditionSql>
          <!-- 传递参数配置 -->
          <ReplaceParams>
            <ReplaceParam>
              <!-- 执行sql参数化字段名称  -->
              <ParamName>WfdWorkflowId</ParamName>
              <!-- 传入参数化获取数据时传入的字段名称 -->
              <InputColunmnName>WorkflowId</InputColunmnName>
            </ReplaceParam>
          </ReplaceParams>
        </ComparisonCondition>
      </ComparisonTable>

      <!-- 对比表配置项 WfdWorkflowNode -->
      <ComparisonTable>
        <!-- 表名 -->
        <TableName>WfdWorkflowNode</TableName>
        <!-- 是否为校验主表,方便外层对比是否为新增,加快速度 -->
        <IsMasterTable>false</IsMasterTable>
        <!-- 主键 -->
        <PrimaryKey>Id</PrimaryKey>
        <!-- 需要对比的列 -->
        <ComparisonColumns>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Id</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn></ComparisonShowColumn>
          </ComparisonColumn>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Code</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn></ComparisonShowColumn>
          </ComparisonColumn>
        </ComparisonColumns>
        <!-- 对比数据筛选条件 -->
        <ComparisonCondition>
          <!-- 筛选数据语句 -->
          <ConditionSql>
            WfdWorkflowId = @WfdWorkflowId
          </ConditionSql>
          <!-- 传递参数配置 -->
          <ReplaceParams>
            <ReplaceParam>
              <!-- 执行sql参数化字段名称  -->
              <ParamName>WfdWorkflowId</ParamName>
              <!-- 传入参数化获取数据时传入的字段名称 -->
              <InputColunmnName>WorkflowId</InputColunmnName>
            </ReplaceParam>
          </ReplaceParams>
        </ComparisonCondition>
      </ComparisonTable>

      <!-- 对比表配置项 WfdWorkflowNodeAction -->
      <ComparisonTable>
        <!-- 表名 -->
        <TableName>WfdWorkflowNodeAction</TableName>
        <!-- 是否为校验主表,方便外层对比是否为新增,加快速度 -->
        <IsMasterTable>false</IsMasterTable>
        <!-- 主键 -->
        <PrimaryKey>Id</PrimaryKey>
        <!-- 需要对比的列 -->
        <ComparisonColumns>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>Id</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn>Id</ComparisonShowColumn>
          </ComparisonColumn>
          <ComparisonColumn>
            <!-- 查询列字段或者函数 -->
            <ComparisonSelectColumn>dbo.L(LangName,'zh-CN')</ComparisonSelectColumn>
            <!-- 显示字段名称,如果为空则默认使用 ComparisonSelectColumn中的值 -->
            <ComparisonShowColumn>NodeActioName</ComparisonShowColumn>
          </ComparisonColumn>
        </ComparisonColumns>
        <!-- 对比数据筛选条件 -->
        <ComparisonCondition>
          <!-- 筛选数据语句 -->
          <ConditionSql>
            [ActionLinkId] IN (
            SELECT [ActionLinkId] FROM WfdWorkflowNode WHERE WfdWorkflowId = @WfdWorkflowId
            )
          </ConditionSql>
          <!-- 传递参数配置 -->
          <ReplaceParams>
            <ReplaceParam>
              <!-- 执行sql参数化字段名称  -->
              <ParamName>WfdWorkflowId</ParamName>
              <!-- 传入参数化获取数据时传入的字段名称 -->
              <InputColunmnName>WorkflowId</InputColunmnName>
            </ReplaceParam>
          </ReplaceParams>
        </ComparisonCondition>
      </ComparisonTable>

    </ComparisonTables>
  </ComparisonConfig>
</ReleaseBuildConfig>

   
     */
    #endregion

}
