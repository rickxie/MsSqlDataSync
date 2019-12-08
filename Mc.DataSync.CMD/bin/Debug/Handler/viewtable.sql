--表
SELECT * FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>'
--字段表
SELECT * FROM BpmViewTableColumn i WHERE i.ViewTableId = (SELECT Id FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>')					 
--表多语言
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT LangName FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>')
--表字段多语言
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT langName FROM BpmViewTableColumn i WHERE i.ViewTableId = (SELECT Id FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>'))