--��
SELECT * FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>'
--�ֶα�
SELECT * FROM BpmViewTableColumn i WHERE i.ViewTableId = (SELECT Id FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>')					 
--�������
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT LangName FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>')
--���ֶζ�����
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT langName FROM BpmViewTableColumn i WHERE i.ViewTableId = (SELECT Id FROM BpmViewTable  i WHERE i.SchemaName = '<%viewName%>'))