--流程
SELECT * FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>'																   
--流程
SELECT * FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')		   
--流程连线
SELECT * FROM WfdWorkflowLink i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')			
--流程节点连线节点
SELECT * FROM WfdWorkflowLinkPoint  i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')																			  
--流程节点动作
SELECT * FROM WfdWorkflowNodeAction  i WHERE i.ActionLinkId IN (SELECT i.ActionLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)	
--流程节点动作
SELECT * FROM WfdWorkflowProcessor  i WHERE i.ProcessorLinkId IN (SELECT i.ProcessorLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)			  
--流程多语言
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT w.LangName FROM WfdWorkflow w  WHERE w.Code = '<%workflowCode%>')
--流程节点多语言
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT langName FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>'))
--流程节点动作多语言
SELECT * FROM AppLanguage  i WHERE i.[Key] IN (SELECT langName FROM WfdWorkflowNodeAction  i WHERE i.ActionLinkId IN (SELECT i.ActionLinkId FROM WfdWorkflowNode i WHERE i.WfdWorkflowId = (SELECT TOP 1 Id FROM WfdWorkflow w WHERE w.Code = '<%workflowCode%>')	)	)