CREATE TABLE [dbo].[ChatMessage]
(
	[ChatMessage_Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Id_User] INT NOT NULL,
	[Id_ChatGroup] INT NOT NULL,
	[DateSend] DATETIME2 NOT NULL,
	[Message] NVARCHAR(MAX) NOT NULL,
	[Color] NVARCHAR(25) NULL, 
    CONSTRAINT [FK_ChatMessage_ToTable] FOREIGN KEY ([Id_User]) REFERENCES [AppUser]([AppUser_Id]), 
    CONSTRAINT [FK_ChatMessage_ToTable_1] FOREIGN KEY ([Id_ChatGroup]) REFERENCES [ChatGroup]([ChatGroup_Id])
)
