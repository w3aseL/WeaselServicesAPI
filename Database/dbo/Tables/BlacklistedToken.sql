﻿CREATE TABLE [dbo].[BlacklistedToken]
(
	[TokenId] INT NOT NULL PRIMARY KEY IDENTITY (1,1),
	[UserId] INT NOT NULL,
	[TokenType] VARCHAR(255) NOT NULL,
	[TokenData] VARCHAR(511) NOT NULL,

	CONSTRAINT Token_FK_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId])
)