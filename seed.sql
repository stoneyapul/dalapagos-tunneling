DECLARE @userId uniqueidentifier 
SET @userId = '39ff0520-a19b-4181-a4d5-010e216864eb'

DECLARE @adminGroupId uniqueidentifier 
SET @adminGroupId = 'ae122e64-0dce-4e73-b44e-94ef828f9b76'

DECLARE @userGroupId uniqueidentifier 
SET @userGroupId = '5cc6fbe2-60fb-4092-a94e-c45ca9284b99'

INSERT INTO [dbo].[Organization]
           ([OrganizationName]
           ,[OrganizationUuid])
     VALUES
           ('Dalapagos'
           ,NEWID())

INSERT INTO [dbo].[OrganizationUser]
           ([OrganizationId]
            ,[UserUuid]
            ,[SecurityGroupUuid])
     VALUES
           (1
           ,@userId
           ,@adminGroupId)

INSERT INTO [dbo].[OrganizationUser]
           ([OrganizationId]
            ,[UserUuid]
            ,[SecurityGroupUuid])
     VALUES
           (1
           ,@userId
           ,@userGroupId)