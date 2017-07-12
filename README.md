# MsSqlDataSync
Step1. 在Mc.DataSync.exe.config 修改需要生成脚本的ConnectionString  
Step2. 点开Mc.DataSync.exe 
### 如果要多个表同步，请书写同步文件并点击'打开'，格式如下 
```sql
--用户表同步
SELECT * FROM APPUSER
--企业表同步
SELECT * FROM APPENTERPRISE WHERE Id IN ( SELECT Id FROM XXX)
```
### 如果只需要更新某几列则如下
```sql
--用户表同步		   
SELECT UserName FROM APPUSER   
```
### 点击'生成' 会根据书写SQL语句生成对应的迁移脚本
### 调试器：单个语句迁移脚本生成
### 生成同步脚本：选中择会生成脚本
### 保存到文本文件：选择一个路径存成文件，否则会显示在最下面。
