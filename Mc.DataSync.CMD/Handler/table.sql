--��
SELECT * FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>'
--�ֶα�
SELECT * FROM AppBusinessTableColumn i WHERE i.BusinessTableId = (SELECT  TOP 1 i.Id FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>')					 
--�ֶα����¼
SELECT * FROM AppBusinessTableColumnChangeHistory i WHERE i.BusinessTableId = (SELECT  TOP 1 i.Id FROM AppBusinessTable i WHERE i.AliasName = '<%tableAlias%>')		
--�������
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT LangName FROM AppBusinessTable  WHERE AliasName = '<%tableAlias%>')
--���ֶζ�����
SELECT * FROM AppLanguage i WHERE i.[Key] In (SELECT langName FROM AppBusinessTableColumn a WHERE a.BusinessTableId = (SELECT  TOP 1 b.Id FROM AppBusinessTable b WHERE b.AliasName = '<%tableAlias%>'))