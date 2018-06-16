CREATE TABLE [dbo].[AppUser]
(
	[AppUser_Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Id_Role] INT NOT NULL,
	[Name] NVARCHAR(MAX) NOT NULL, 
	CONSTRAINT [FK_AppUser_RoleEntity] FOREIGN KEY ([Id_Role]) REFERENCES [RoleEntity]([RoleEntity_Id])
)
