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

        #region 1.架构比较接口

        #region Sql获取语句

        #region 1.获取所有表架构数据 CONST_SQL_GETALLTABLESCHEMA

        /// <summary>
        /// 获取所有表架构数据
        /// </summary>
        private const string CONST_SQL_GETALLTABLESCHEMA = @"
--DECLARE @Name NVARCHAR(200)
--SET @Name='t_depttemplat'
SELECT d.name                     TableName, --表名
       --a.colorder                 ColumnOrder,--序号
       a.name                     ColumnName,--列名
       (
           CASE 
                WHEN COLUMNPROPERTY(a.id ,a.name ,'IsIdentity')=1 THEN '1'
                ELSE '0'
           END
       )                          IsIdentity,--是否自增
       (
           CASE 
                WHEN (
                         SELECT COUNT(*)
                         FROM   sysobjects
                         WHERE  (
                                    NAME IN (SELECT NAME
                                             FROM   sysindexes
                                             WHERE  (id=a.id)  
                                                    AND (
                                                            indid IN (SELECT 
                                                                             indid
                                                                      FROM   
                                                                             sysindexkeys
                                                                      WHERE  (id = a.id)  
                                                                             AND (
                                                                                     colid IN (SELECT 
                                                                                                      colid
                                                                                               FROM   
                                                                                                      syscolumns
                                                                                               WHERE  (id = a.id)  
                                                                                                      AND (NAME = a.name))
                                                                                 ))
                                                        ))
                                )  
                                AND (xtype='PK')
                     )>0 THEN '1'
                ELSE '0'
           END
       )                          IsPrimaryKey,
       b.name [Type],
       COLUMNPROPERTY(a.id ,a.name ,'PRECISION') AS [Length],
       ISNULL(COLUMNPROPERTY(a.id ,a.name ,'Scale') ,0) AS Decimals,
       (CASE WHEN a.isnullable=1 THEN '1' ELSE '0' END) [IsNull],
       ISNULL(e.text ,'') [Default],
       ISNULL(g.[value] ,' ')  AS [Description]
FROM   syscolumns a
       LEFT JOIN systypes b ON  a.xtype = b.xusertype
       INNER JOIN sysobjects d ON  a.id = d.id AND  d.xtype = 'U' AND  d.name<>'dtproperties'
       LEFT JOIN syscomments e ON  a.cdefault = e.id
       LEFT JOIN sys.extended_properties g ON  a.id = g.major_id AND  a.colid = g.minor_id
       LEFT JOIN sys.extended_properties f ON  d.id = f.class AND  f.minor_id = 0
WHERE  b.name IS NOT NULL  
       AND (@Name='' OR d.name = @Name) --增加一个条件,方便获取数据
ORDER BY a.id,a.colorder

        ";

        #endregion

        #region 2.获取所有视图和存储过程数据 CONST_SQL_GETVIEWANDPROCEDURE

        /// <summary>
        /// 获取所有视图和存储过程数据  返回字段:name,type,definition
        /// </summary>
        private const string CONST_SQL_GETVIEWANDPROCEDURE = @"
            --获取数据表中所有的存储过程
            SELECT (
                       CASE 
                            WHEN a.[type]='P' THEN N'P/\'
                            WHEN a.[type]='V' THEN N'V/\'
                            ELSE 'unknown'
                       END
                   )+a.Name         AS Id,
                   a.Name,
                   a.[Type],
                   RTRIM(LTRIM( b.[Definition])) AS [Definition]
            FROM   sys.all_objects     a,
                   sys.sql_modules     b
            WHERE  a.is_ms_shipped = 0  
                   AND a.object_id = b.object_id  
                   AND a.[type]  IN ('P' ,'V')
            ORDER BY a.[name] ASC
        ";

        #endregion

        #region 3.获取所有的数据库函数 CONST_SQL_GETFUNCTION
        /// <summary>
        /// 获取所有的数据库函数  返回字段:name,type,definition
        /// </summary>
        private const string CONST_SQL_GETFUNCTION = @"
            --DECLARE @Name NVARCHAR(200)
            --SET @Name='func_CheckInternalPermission'
            --3.获取数据表中所有的函数
            SELECT 'FN/\'+a.name AS Id, 
                   a.Name,
                   --a.[type],
                   'FN' AS [Type],
                   RTRIM(LTRIM( b.[Definition])) AS [Definition]
            FROM   sys.all_objects     a,
                   sys.sql_modules     b
            WHERE  a.is_ms_shipped = 0  
                   AND a.object_id = b.object_id  
                   AND a.[type] IN ('AF' ,'FN' ,'TF' ,'FS' ,'FT' ,'IF')
                   AND (@Name='' OR a.name = @Name) --增加一个条件,方便获取数据
            ORDER BY a.[name] ASC

        ";

        #endregion

        #region 4.获取新增表语句 CONST_SQL_GETCREATETABLESCRIPT

        /// <summary>
        /// 获取建表语句 
        /// @TableName:表名
        /// </summary>
        public const string CONST_SQL_GETCREATETABLESCRIPT = @"
--获取指定表建表语句
--DECLARE @TableName     VARCHAR(100)
--SET @TableName = 'AppBusinessTable'    --表名


DECLARE @DbName        VARCHAR(40) --数据库名称
       ,@SQL           VARCHAR(MAX)--输出脚本
SET @DbName = (
        SELECT '['+NAME+']'
        FROM   MASTER..SysDataBases
        WHERE  DbId                = (
                   SELECT Dbid
                   FROM   MASTER..SysProcesses
                   WHERE  Spid     = @@spid
               )
    )

DECLARE @table_script NVARCHAR(MAX) --建表的脚本
DECLARE @index_script NVARCHAR(MAX) --索引的脚本
DECLARE @default_script NVARCHAR(MAX) --默认值的脚本
DECLARE @check_script NVARCHAR(MAX) --check约束的脚本
DECLARE @columnDescription_script NVARCHAR(MAX) --字段备注脚本
DECLARE @sql_cmd NVARCHAR(MAX)  --动态SQL命令
DECLARE @err_info VARCHAR(200)
--SET @tbname = UPPER(@tbname);
IF OBJECT_ID(@DbName+'.dbo.'+@TableName) IS NULL
BEGIN
    SET @err_info = '对象:'+@DbName+'.dbo.'+@TableName+'不存在!'
    RAISERROR(@err_info ,16 ,1)
    RETURN
END
----------------------生成创建表脚本----------------------------
--1.添加算定义字段
SET @table_script = 'CREATE TABLE '+@TableName+'
('+CHAR(13)+CHAR(10);
 
 
--添加表中的其它字段
SET @sql_cmd = N'
use '+@DbName+
    '
set @table_script='''' 
select @table_script=@table_script+
        '' [''+t.NAME+''] ''
        +(case when t.xusertype in (175,62,239,59,122,165,173) then ''[''+p.name+''] (''+convert(varchar(30),isnull(t.prec,''''))+'')''
              when t.xusertype in (231) and t.length=-1 then ''[ntext]''
              when t.xusertype in (231) and t.length<>-1 then ''[''+p.name+''] (''+convert(varchar(30),isnull(t.prec,''''))+'')''
             when t.xusertype in (167) and t.length=-1 then ''[text]''
              when t.xusertype in (167) and t.length<>-1 then ''[''+p.name+''] (''+convert(varchar(30),isnull(t.prec,''''))+'')''
              when t.xusertype in (106,108) then ''[''+p.name+''] (''+convert(varchar(30),isnull(t.prec,''''))+'',''+convert(varchar(30),isnull(t.scale,''''))+'')''
              else ''[''+p.name+'']''
         END)
         +(case when t.isnullable=1 then '' null'' else '' not null ''end)
         +(case when COLUMNPROPERTY(t.ID, t.NAME, ''ISIDENTITY'')=1 then '' identity'' else '''' end)
         +'',''+char(13)+char(10)
from syscolumns t join systypes p  on t.xusertype = p.xusertype
where t.ID=OBJECT_ID('''+@TableName+''')
ORDER BY  t.COLID; 
'

EXEC sp_executesql @sql_cmd
    ,N'@table_script varchar(max) output'
    ,@sql_cmd OUTPUT

SET @table_script = @table_script+@sql_cmd
IF LEN(@table_script)>0
    SET @table_script = SUBSTRING(@table_script ,1 ,LEN(@table_script)-3)+CHAR(13)
       +CHAR(10)
       +')'+CHAR(13)+CHAR(10)
       +' '+CHAR(13)+CHAR(10)+CHAR(13)+CHAR(10)
    
--------------------生成索引脚本---------------------------------------
SET @index_script = ''
SET @sql_cmd = N'
use '+@DbName+
    '
declare @ct int
declare @indid int      --当前索引ID
declare @p_indid int    --前一个索引ID
select @indid=-1, @p_indid=0,@ct=0    --初始化，以后用@indid和@p_indid判断是否索引ID发生变化
set @index_script=''''
select @indid=INDID
    ,@index_script=@index_script
    +(case when @indid<>@p_indid and @ct>0 then '')''+char(13)+char(10)+'' ''+char(13)+char(10) else '''' end)
    +(case when @indid<>@p_indid and UNIQ=''PRIMARY KEY'' 
          then ''ALTER TABLE ''+TABNAME+'' ADD CONSTRAINT ''+name+'' PRIMARY KEY ''+cluster+char(13)+char(10)
                +''(''+char(13)+char(10)
                +''    ''+COLNAME+char(13)+char(10)
          when @indid<>@p_indid and UNIQ=''UNIQUE'' 
          then ''ALTER TABLE ''+TABNAME+'' ADD CONSTRAINT ''+name+'' UNIQUE ''+cluster+char(13)+char(10)
                +''(''+char(13)+char(10)
                +''    ''+COLNAME+char(13)+char(10)
          when @indid<>@p_indid and UNIQ=''INDEX''     
          then ''CREATE ''+cluster+'' INDEX ''+name+'' ON ''+TABNAME+char(13)+char(10)
                +''(''+char(13)+char(10)
                +''    ''+COLNAME+char(13)+char(10)
          when @indid=@p_indid
          then  ''    ,''+COLNAME+char(13)+char(10)
     END) 
    ,@ct=@ct+1
    ,@p_indid=@indid
from 
(
    SELECT A.INDID,B.KEYNO
        ,REPLACE(NAME,''dbo.'','''') AS [Name],(SELECT NAME FROM SYSOBJECTS WHERE ID=A.ID) AS TABNAME,
        (SELECT NAME FROM SYSCOLUMNS WHERE ID=B.ID AND COLID=B.COLID) AS COLNAME,
        (CASE WHEN EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME=A.NAME AND XTYPE=''UQ'') THEN ''UNIQUE'' 
              WHEN EXISTS(SELECT 1 FROM SYSOBJECTS WHERE NAME=A.NAME AND XTYPE=''PK'') THEN ''PRIMARY KEY''
              ELSE ''INDEX'' END)  AS UNIQ,
        (CASE WHEN A.INDID=1 THEN ''CLUSTERED'' WHEN A.INDID>1 THEN ''NONCLUSTERED'' END) AS CLUSTER
    FROM SYSINDEXES A INNER JOIN SYSINDEXKEYS B ON A.INDID=B.INDID AND A.ID=B.ID
    WHERE A.ID=OBJECT_ID('''+@TableName+
    ''') and a.indid<>0
) t
ORDER BY INDID,KEYNO'

EXEC sp_executesql @sql_cmd
    ,N'@index_script varchar(max) output'
    ,@sql_cmd OUTPUT

SET @index_script = @sql_cmd
IF LEN(@index_script)>0
    SET @index_script = @index_script+')'+CHAR(13)+CHAR(10)+' '+CHAR(13)+CHAR(10)
       +CHAR(13)+CHAR(10)
    
--生成默认值约束
SET @sql_cmd = '
use '+@DbName+
    '
set @default_script=''''
SELECT @default_script=@default_script
        +''ALTER TABLE ''+OBJECT_NAME(O.PARENT_OBJ)
        +'' ADD CONSTRAINT ''+O.NAME+'' default ''+t.text+'' for ''+C.NAME+char(13)+char(10)
        +'' ''+char(13)+char(10)
FROM SYSOBJECTS O INNER JOIN SYSCOMMENTS T ON O.ID=T.ID
    INNER JOIN SYSCOLUMNS C ON O.PARENT_OBJ=C.ID AND C.CDEFAULT=T.ID
WHERE O.XTYPE=''D'' AND O.PARENT_OBJ=OBJECT_ID('''+@TableName+''')'

EXEC sp_executesql @sql_cmd
    ,N'@default_script varchar(max) output'
    ,@sql_cmd OUTPUT

SET @default_script = @sql_cmd+CHAR(13)+CHAR(10)


----------------------生成字段备注脚本 @columnDescription_script----------------------------

SET @columnDescription_script = ''
 --查询某一个表的结构
SELECT @columnDescription_script = @columnDescription_script+(
           '
IF EXISTS(SELECT 1
FROM   syscolumns a
       INNER JOIN sysobjects d ON  a.id = d.id AND  d.xtype = ''U'' AND  d.name<>''dtproperties''
       LEFT JOIN sys.extended_properties g ON  a.id = g.major_id AND  a.colid = 
            g.minor_id
WHERE  g.[value] IS NULL
	   AND d.name = '''+d.name+'''   AND a.name='''+a.name+
           '''
)
BEGIN
       exec sp_addextendedproperty ''MS_Description'', '''+CONVERT(NVARCHAR(100) ,g.[value]) 
          +''', ''user'', ''dbo'', ''table'', '''+CONVERT(NVARCHAR(100) ,d.name)
          +''', ''column'', '''+CONVERT(NVARCHAR(100) ,a.name)+
           '''
END       
ELSE
BEGIN	
	   exec sp_updateextendedproperty ''MS_Description'', '''+CONVERT(NVARCHAR(100) ,g.[value]) 
          +''', ''user'', ''dbo'', ''table'', '''+CONVERT(NVARCHAR(100) ,d.name)
          +''', ''column'', '''+CONVERT(NVARCHAR(100) ,a.name)+
           '''
END	   	   
'
       )
FROM   syscolumns a
       INNER JOIN sysobjects d ON  a.id = d.id AND  d.xtype = 'U' AND  d.name<>
            'dtproperties'
       LEFT JOIN sys.extended_properties g ON  a.id = g.major_id AND  a.colid = 
            g.minor_id
WHERE  g.[value] IS NOT NULL --获取有字段备注的列  
       AND d.name = @TableName
ORDER BY a.name
 
----------------------最终拼接输出----------------------------
--拼接创建表结构部分
SET @SQL = 
    ' 
--创建表结构
IF NOT EXISTS (
       SELECT 1
       FROM   sysobjects
       WHERE  id           = OBJECT_ID('''+@TableName+
    ''')  
              AND TYPE     = ''U''
   )
BEGIN
'+@table_script+@index_script+@default_script+'
END
GO
'
--拼接备注部分
SET @SQL = @SQL+
    '
--添加备注信息
 IF EXISTS (
       SELECT 1
       FROM   sysobjects
       WHERE  id           = OBJECT_ID('''+@TableName+
    ''')  
              AND TYPE     = ''U''
   )
BEGIN
PRINT '''+@TableName+'''
'+@columnDescription_script+'
END
GO
'

DECLARE @len     INT
       ,@n       INT

SET @len = LEN(@SQL)
SET @n = 0
WHILE (@len>0)
BEGIN
    --PRINT(substring(@SQL,@n*4000+1,4000));
    SET @n = @n+1
    SET @len = @len-4000;
END

SELECT @SQL AS [SQL]

        ";

        #endregion

        #endregion

        #region table DataColumn 定义

        /// <summary>
        /// 表字段默认主键,Id
        /// </summary>
        public const string TABLE_DATACOLUMN_ID = "Id";
        /// <summary>
        /// 显示名称
        /// </summary>
        public const string TABLE_DATACOLUMN_NAME = "Name";
        /// <summary>
        /// 类型定义,T=表,P=存储过程,V=视图,FN=函数
        /// </summary>
        public const string TABLE_DATACOLUMN_TYPE = "Type";
        /// <summary>
        /// 定义内容
        /// </summary>
        public const string TABLE_DATACOLUMN_DEFINITION = "Definition";
        /// <summary>
        /// 状态,0正常,1新增,2修改
        /// </summary>
        public const string TABLE_DATACOLUMN_STATUS = "_Status";
        /// <summary>
        /// 是否选中
        /// </summary>
        public const string TABLE_DATACOLUMN_CHECKED = "_Checked";

        #endregion


        #region 对外开放接口

        /// <summary>
        /// 获取两边数据库中数据表架构的明细行
        /// 表1:源数据表
        /// 表2:目标数据表
        /// </summary>
        /// <param name="name">特定表名,如果不传则获取所有表</param>
        /// <returns></returns>
        public DataSet GetTableSchemaDetailRows(string name = "")
        {
            var p = new Dapper.DynamicParameters();
            p.Add("Name", name);
            return GetSourAndTargetData(CONST_SQL_GETALLTABLESCHEMA, p);
        }

        /// <summary>
        /// 获取源数据和目标数据
        /// </summary>
        /// <param name="sql">执行语句</param>
        /// <returns></returns>
        public DataSet GetSourAndTargetData(string sql, DynamicParameters param = null)
        {
            var ds = new DataSet();

            //当前数据
            DataTable sourDt = DbDapper.RunDataTableSql(sql, param);
            sourDt.TableName = Guid.NewGuid().ToString();
            ds.Tables.Add(sourDt);

            //目标数据
            DataTable tarDt = DbTargetQuery(sql, param);
            tarDt.TableName = Guid.NewGuid().ToString();
            ds.Tables.Add(tarDt);

            return ds;
        }

        /// <summary>
        /// 获取指定类型名称对应的脚本内容
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DataSet GetDiffText(string name, string type)
        {
            var p = new Dapper.DynamicParameters();
            p.Add("Name", name);
            if ("T" == type) return GetSourAndTargetData(CONST_SQL_GETALLTABLESCHEMA, p);
            //1.1.1 获取函数 对比结果
            else if ("FN" == type) return GetSourAndTargetData(CONST_SQL_GETFUNCTION, p);
            //1.1.2 获取视图和存储过程 对比结果
            else return GetSourAndTargetData(CONST_SQL_GETVIEWANDPROCEDURE, p);
        }

        /// <summary>
        /// 根据表名,根据差异点生成可自行数据库脚本
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="status">新增=1,修改=2</param>
        /// <returns></returns>
        public string GenerateTableSchemaScript(string name, int status)
        {

            //新增
            if (status == 1)
            {
                var dt = DbDapper.RunDataTableSql(CONST_SQL_GETCREATETABLESCRIPT, new { TableName = name });
                return Convert.ToString(dt.Rows[0][0]);
            }
            //修改
            else if (status == 2)
            {
                var ds = GetTableSchemaDetailRows(name);

                //当前数据
                DataTable sourDt = ds.Tables[0];
                //目标数据
                DataTable tarDt = ds.Tables[1];

                var sb = new StringBuilder();
                var noLengthColumnTypeList = new List<string>() { "bigint", "bit", "date", "datetime", "float", "geography", "geometry", "hierarchyid", "image", "int", "money", "ntext", "real", "smalldatetime", "smallint", "smallmoney", "sql_variant", "text", "timestamp", "tinyint", "uniqueidentifier", "xml" };

                string tableName, columnName, isIdentity, type, isNull, defaultValue, description;
                int length, decimals, isPrimaryKey;
                foreach (DataRow sourRow in sourDt.Rows)
                {
                    tableName = sourRow["TableName"].ToString();
                    columnName = sourRow["ColumnName"].ToString();
                    isIdentity = sourRow["ColumnName"].ToString();
                    isPrimaryKey = Convert.ToInt32(sourRow["IsIdentity"]);
                    length = Convert.ToInt32(sourRow["Length"]);
                    decimals = Convert.ToInt32(sourRow["Decimals"]);
                    var _type = sourRow["Type"].ToString();
                    if (noLengthColumnTypeList.Contains(_type))
                    {
                        type = _type;
                    }
                    else
                    {
                        //decimal(18, 0)  numeric(18, 0)
                        if (_type == "decimal" || _type == "numeric")
                            type = _type + "(" + length + ", " + decimals + ")";
                        else if (length == -1)
                            type = _type + "(MAX)";
                        else
                            type = _type + "(" + length + ")";
                    }

                    isNull = Convert.ToInt32(sourRow["IsNull"]) == 1 ? " NULL " : " NOT NULL ";
                    if (!string.IsNullOrEmpty(sourRow["Default"].ToString()))
                    {
                        if (sourRow["Default"].ToString().ToLower() == "getdate()")
                            defaultValue = " DEFAULT(GETDATE()) ";
                        else
                            defaultValue = " DEFAULT('" + sourRow["Default"].ToString() + "') ";
                    }
                    else
                    {
                        defaultValue = "";
                    }
                    description = sourRow["Description"].ToString();

                    var tarRows = tarDt.Select("ColumnName = '" + columnName + "'");
                    //1.判断是否为新增列
                    if (tarRows.Count() == 0)
                    {
                        sb.AppendLine(@"ALTER TABLE " + tableName + " ADD " + columnName + " " + type + " " + defaultValue + " " + isNull + "");
                        sb.AppendLine("GO");
                    }
                    //2.否则就是修改项
                    else
                    {
                        var tarRow = tarRows[0];

                        //如果内容一致,则不需要生成
                        if (sourRow.ItemArray.SerializeJson() == tarRow.ItemArray.SerializeJson()) continue;

                        //2.1 判断是否为字段类型,长度修改
                        sb.AppendLine("ALTER TABLE " + tableName + " ALTER COLUMN " + columnName + " " + type + " " + defaultValue + " " + isNull + "");
                        sb.AppendLine("GO");

                        //2.2.判断是否为备注修改
                        if (description != tarRow["Description"].ToString())
                        {
                            sb.AppendLine(@" 
IF EXISTS(SELECT 1
FROM   syscolumns a
       INNER JOIN sysobjects d ON  a.id = d.id AND  d.xtype = 'U' AND  d.name<>'dtproperties'
       LEFT JOIN sys.extended_properties g ON  a.id = g.major_id AND  a.colid =
            g.minor_id
WHERE  g.[value] IS NULL
          AND d.name = '" + tableName + @"' 
       AND a.name = 'IsDeleted1' )
       exec sp_addextendedproperty 'MS_Description', '" + description + @"', 'user', 'dbo', 'table', '" + tableName + @"', 'column', '" + columnName + @"'
ELSE
           exec sp_updateextendedproperty 'MS_Description', '" + description + @"', 'user', 'dbo', 'table', '" + tableName + @"', 'column', '" + columnName + @"'
GO;
                            ");



                        }

                        //2.3.判断是否为主键不一致
                        if (isPrimaryKey != Convert.ToInt32(tarRow["IsPrimaryKey"]))
                        {

                            //如果不一致,先移除主键
                            sb.AppendLine(@" 
DECLARE @Pk VARCHAR(200);
SELECT @Pk = NAME
FROM   sysobjects
WHERE  parent_obj     = OBJECT_ID('" + tableName + @"')  
       AND xtype      = 'PK';
IF @Pk IS NOT NULL
BEGIN
    EXEC ('ALTER TABLE " + tableName + @" DROP '+@Pk)
END
GO
                            ");

                            //新增主键
                            if (1 == isPrimaryKey)
                            {
                                sb.AppendLine(@"ALTER TABLE " + tableName + @" ADD CONSTRAINT PK_" + tableName + @" PRIMARY KEY(" + columnName + @") ");
                                sb.AppendLine("GO");
                            }

                        }

                    }




                    //4.判断默认值是否不一致 XX

                    //5.判断自增是否不一致 XX

                    //6.索引是否一致 XX
                }

                return sb.ToString();

            }
            else
            {
                return string.Format(" --[TODO]传入的类型暂不处理 name:{0} status:{1}", name, status);
            }


        }

        /// <summary>
        /// 追加其它校验脚本
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public string AppendOtherSchemaCheckScript(string name, string type)
        {

            if ("P" == type)
            {
                return @"
                    IF EXISTS (
                           SELECT *
                           FROM   dbo.sysobjects
                           WHERE  id = OBJECT_ID(N'[dbo].[" + name + @"]')  
                                  AND OBJECTPROPERTY(id ,N'IsProcedure') = 1
                       )
                        -- 删除存储过程 
                        DROP PROCEDURE [dbo].[" + name + @"] 
                    GO 
                ";

            }
            else if ("V" == type)
            {
                return @"
                    -- 判断要创建的视图名是否存在 
                    IF EXISTS (
                           SELECT *
                           FROM   dbo.sysobjects
                           WHERE  id = OBJECT_ID(N'[dbo].[" + name + @"]')  
                                  AND OBJECTPROPERTY(id ,N'IsView') = 1
                       )
                        -- 删除视图 
                        DROP VIEW [dbo].[" + name + @"] 
                    GO
                ";

            }
            else if ("FN" == type)
            {
                return @"
                    IF EXISTS (
                           SELECT *
                           FROM   dbo.sysobjects
                           WHERE  id = OBJECT_ID(N'[dbo].[" + name + @"]')  
                                  AND xtype IN ('AF' ,'FN' ,'TF' ,'FS' ,'FT' ,'IF')
                       )
                        -- 删除函数 
                        DROP FUNCTION [dbo].[" + name + @"] 
                    GO 
                ";

            }
            return "";
        }

        /// <summary>
        /// 获取所有对比架构数据列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllSchemaData()
        {
            // 0.创建返回表数据结构
            DataTable returnDt = new DataTable();
            //增加字段列
            returnDt.Columns.AddRange(new DataColumn[] {
                new DataColumn(TABLE_DATACOLUMN_ID, typeof(string)),//唯一标识
                new DataColumn(TABLE_DATACOLUMN_NAME, typeof(string)),//显示名称
                new DataColumn(TABLE_DATACOLUMN_TYPE, typeof(string)),//类型定义,T=表,P=存储过程,V=视图,FN=函数
                new DataColumn(TABLE_DATACOLUMN_DEFINITION, typeof(string)), //定义内容
                new DataColumn(TABLE_DATACOLUMN_STATUS, typeof(string)), //状态,0正常,1新增,2修改 
                new DataColumn(TABLE_DATACOLUMN_CHECKED, typeof(bool)) //是否选中
            });

            //1.1.0 获取表
            #region 获取表对比结果

            var ds = GetTableSchemaDetailRows();

            //转换源数据
            var tempSourDt = TableSchemaConvert(ds.Tables[0]);

            //转换目标数据
            var tempTarDt = TableSchemaConvert(ds.Tables[1]);

            //1.1.1 获取表 对比结果
            AppendSchemaComparisonToTable(tempSourDt, tempTarDt, returnDt);

            #endregion

            //1.1.1 获取函数 对比结果
            var p = new Dapper.DynamicParameters();
            p.Add("Name", "");
            var funDs = GetSourAndTargetData(CONST_SQL_GETFUNCTION, p);
            AppendSchemaComparisonToTable(funDs.Tables[0], funDs.Tables[1], returnDt);

            //1.1.2 获取视图和存储过程 对比结果
            var viewAndProcDs = GetSourAndTargetData(CONST_SQL_GETVIEWANDPROCEDURE, p);
            AppendSchemaComparisonToTable(viewAndProcDs.Tables[0], viewAndProcDs.Tables[1], returnDt);

            //1.3 返回最终对比结果数据
            return returnDt;
        }

        /// <summary>
        /// 获取指定脚本的架构对比数据,并且追加到集合中
        /// </summary>
        /// <param name="sourDt">源数据</param>
        /// <param name="tarDt">目标数据</param>
        /// <param name="returnDt">追加返回对象</param>
        private void AppendSchemaComparisonToTable(DataTable sourDt, DataTable tarDt, DataTable returnDt)
        {
            //获取对比结果
            var functionDs = SchemaComparison(sourDt, tarDt, TABLE_DATACOLUMN_ID);

            //新增的项
            foreach (DataRow item in functionDs.Tables[TableName_OnlySourceData].Rows)
            {
                var newRow = returnDt.NewRow();
                newRow[TABLE_DATACOLUMN_ID] = item[TABLE_DATACOLUMN_ID];
                newRow[TABLE_DATACOLUMN_NAME] = item[TABLE_DATACOLUMN_NAME];
                newRow[TABLE_DATACOLUMN_TYPE] = item[TABLE_DATACOLUMN_TYPE];
                newRow[TABLE_DATACOLUMN_DEFINITION] = item[TABLE_DATACOLUMN_DEFINITION];
                newRow[TABLE_DATACOLUMN_STATUS] = "1";
                returnDt.Rows.Add(newRow);
            }

            //修改的项
            foreach (DataRow item in functionDs.Tables[TableName_DifferentData].Rows)
            {
                var newRow = returnDt.NewRow();
                newRow[TABLE_DATACOLUMN_ID] = item[SourColumnPrefix + TABLE_DATACOLUMN_ID];
                newRow[TABLE_DATACOLUMN_NAME] = item[SourColumnPrefix + TABLE_DATACOLUMN_NAME];
                newRow[TABLE_DATACOLUMN_TYPE] = item[SourColumnPrefix + TABLE_DATACOLUMN_TYPE];
                newRow[TABLE_DATACOLUMN_DEFINITION] = item[SourColumnPrefix + TABLE_DATACOLUMN_DEFINITION];
                newRow[TABLE_DATACOLUMN_STATUS] = "2";
                returnDt.Rows.Add(newRow);
            }

            ////一致的项
            //foreach (DataRow item in functionDs.Tables[TableName_SameData].Rows)
            //{
            //    var newRow = returnDt.NewRow();
            //    newRow[TABLE_DATACOLUMN_ID] = item[TABLE_DATACOLUMN_ID];
            //    newRow[TABLE_DATACOLUMN_NAME] = item[TABLE_DATACOLUMN_NAME];
            //    newRow[TABLE_DATACOLUMN_TYPE] = item[TABLE_DATACOLUMN_TYPE];
            //    newRow[TABLE_DATACOLUMN_DEFINITION] = item[TABLE_DATACOLUMN_DEFINITION];
            //    newRow[TABLE_DATACOLUMN_STATUS] = "0";
            //    returnDt.Rows.Add(newRow);
            //}

            ////删除的项
            //foreach (DataRow item in functionDs.Tables[TableName_OnlyTargetData].Rows)
            //{
            //    var newRow = returnDt.NewRow();
            //    newRow[TABLE_DATACOLUMN_ID] = item[TABLE_DATACOLUMN_ID];
            //    newRow[TABLE_DATACOLUMN_NAME] = item[TABLE_DATACOLUMN_NAME];
            //    newRow[TABLE_DATACOLUMN_TYPE] = item[TABLE_DATACOLUMN_TYPE];
            //    newRow[TABLE_DATACOLUMN_DEFINITION] = item[TABLE_DATACOLUMN_DEFINITION];
            //    newRow[TABLE_DATACOLUMN_STATUS] = "3";
            //    returnDt.Rows.Add(newRow);
            //}
        }

        /// <summary>
        /// 将表结构转换为一行表的对比数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable TableSchemaConvert(DataTable dt)
        {

            // 0.创建返回表数据结构
            DataTable returnDt = new DataTable();
            //增加字段列
            returnDt.Columns.AddRange(new DataColumn[] {
                new DataColumn(TABLE_DATACOLUMN_ID, typeof(string)),//唯一标识
                new DataColumn(TABLE_DATACOLUMN_NAME, typeof(string)),//显示名称
                new DataColumn(TABLE_DATACOLUMN_TYPE, typeof(string)),//类型定义,T=表,P=存储过程,V=视图,FN=函数
                new DataColumn(TABLE_DATACOLUMN_DEFINITION, typeof(string)) //定义内容
            });

            var tempTableKey = new List<string>();
            string tableName = string.Empty;
            //转换源数据
            foreach (DataRow item in dt.Rows)
            {
                tableName = Convert.ToString(item["TableName"]);
                if (tempTableKey.Contains(tableName)) continue;

                tempTableKey.Add(tableName);
                var newRow = returnDt.NewRow();
                newRow[TABLE_DATACOLUMN_ID] = @"T/\" + item["TableName"];
                newRow[TABLE_DATACOLUMN_NAME] = item["TableName"];
                newRow[TABLE_DATACOLUMN_TYPE] = "T";
                newRow[TABLE_DATACOLUMN_DEFINITION] = (dt.Select(" TableName = '" + tableName + "'").CopyToDataTable().SerializeJson());
                returnDt.Rows.Add(newRow);
            }

            return returnDt;
        }


        /// <summary>
        /// 对比两个结构相同的表,返回对比结果
        /// DifferentData:不同数据.
        /// OnlySourceData:只在源中的数据.
        /// OnlyTargetData:只在目标中的数据.
        /// SameData:相同记录.
        /// </summary>
        /// <param name="sourDt">当前数据</param>
        /// <param name="tarDt">目标数据</param>
        /// <param name="primaryKey">主键字段</param>
        /// <returns></returns>
        private DataSet SchemaComparison(DataTable sourDt, DataTable tarDt, string primaryKey)
        {
            DataSet ds = new DataSet();
            #region 3.获取数据进行对比

            //1.获取相同记录(Intersect 求两个集合的交集,两边同时存在)
            var sameDataDt = sourDt.Clone();//仅复制表结构
            var tempRows = sourDt.AsEnumerable().Intersect(tarDt.AsEnumerable(), DataRowComparer.Default);
            if (tempRows.Count() > 0)
            {
                sameDataDt = tempRows.CopyToDataTable();
            }
            sameDataDt.TableName = TableName_SameData;
            ds.Tables.Add(sameDataDt);


            //2.获取只在源中的数据(Except 差集,当前集合中存在,而在目标集合中不存在)
            var onlySourceDataRows = from r in sourDt.AsEnumerable()
                                     where
                                         !(from rr in tarDt.AsEnumerable() select rr.Field<string>(primaryKey)).Contains(r.Field<string>(primaryKey))
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
                                         !(from rr in sourDt.AsEnumerable() select rr.Field<string>(primaryKey)).Contains(r.Field<string>(primaryKey))
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
                                         (from rr in tarDt.AsEnumerable() select rr.Field<string>(primaryKey)).Contains(r.Field<string>(primaryKey))
                                     select r;
            //4.2 拿主键一致的数据去目标数据中做对比,拿出不一致的数据
            var differentDataRows = samePrimaryKeyRows.Except(tarDt.AsEnumerable(), DataRowComparer.Default);
            if (differentDataRows.Count() > 0)
            {
                differentDataDt = differentDataRows.CopyToDataTable();
            }

            var tempDt = MergeDataTable(differentDataDt, tarDt, primaryKey);
            tempDt.TableName = TableName_DifferentData;
            ds.Tables.Add(tempDt);


            #endregion

            return ds;

        }


        #endregion


        #endregion

        #region 2.数据对比

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
        public void DbTargetExcute(string sqlText, object param = null)
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
        public DataTable DbTargetQuery(string sqlText, object param = null)
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
