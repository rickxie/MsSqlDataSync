--����
SELECT * FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>'																   
--����
SELECT * FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')		   
--��������
SELECT * FROM WfdWorkflowLink i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')			
--���̽ڵ����߽ڵ�
SELECT * FROM WfdWorkflowLinkPoint  i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')																			  
--���̽ڵ㶯��
SELECT * FROM WfdWorkflowNodeAction  i WHERE i.ActionLinkId IN (SELECT i.ActionLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)	
--���̽ڵ㶯��
SELECT * FROM WfdWorkflowProcessor  i WHERE i.ProcessorLinkId IN (SELECT i.ProcessorLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)			  
--���̶�����
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT w.LangName FROM WfdWorkflow w  WHERE w.Code = '<%workflowCode%>')
--���̽ڵ������
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT langName FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>'))
--���̽ڵ㶯��������
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT langName FROM WfdWorkflowNodeAction  i WHERE i.ActionLinkId IN (SELECT i.ActionLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)	)