﻿CREATE TABLE [dbo].[UserData]
(
	[UserData_Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Key] NVARCHAR(350) NOT NULL,
	[Value] NVARCHAR(MAX) NULL,
	[Id_AppUser] INT NOT NULL, 
	CONSTRAINT [FK_UserData_AppUser] FOREIGN KEY ([Id_AppUser]) REFERENCES [AppUser]([AppUser_Id]),
)
