--表
SELECT * FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>'
--字段表
SELECT * FROM AppBusinessTableColumn i WHERE i.BusinessTableId = (SELECT  TOP 1 i.Id FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>')					 
--字段变更记录
SELECT * FROM AppBusinessTableColumnChangeHistory i WHERE i.BusinessTableId = (SELECT  TOP 1 i.Id FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>')		
--表多语言
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT LangName FROM AppBusinessTable  WHERE AliasName = '<%tableAlias%>')
--表字段多语言
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT langName FROM AppBusinessTableColumn a WHERE a.BusinessTableId = (SELECT  TOP 1 b.Id FROM AppBusinessTable b WHERE b.AliasName = '<%tableAlias%>'))