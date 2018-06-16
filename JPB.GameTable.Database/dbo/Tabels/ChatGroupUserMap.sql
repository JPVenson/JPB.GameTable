CREATE TABLE [dbo].[ChatGroupUserMap]
(
	[ChatGroupUserMap_Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Id_AppUser] INT NOT NULL,
	[Id_ChatGroup] INT NOT NULL, 
    CONSTRAINT [FK_ChatGroupUserMap_ChatGroup] FOREIGN KEY ([Id_ChatGroup]) REFERENCES [ChatGroup]([ChatGroup_Id]), 
    CONSTRAINT [FK_ChatGroupUserMap_AppUser] FOREIGN KEY ([Id_AppUser]) REFERENCES [AppUser]([AppUser_Id]),
)
