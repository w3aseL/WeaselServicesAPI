CREATE TABLE [dbo].[BlogAuthor]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Name] VARCHAR(255) NOT NULL,
	[UserId] INT NOT NULL,

	CONSTRAINT [FK_Author_User] FOREIGN KEY ([UserId]) REFERENCES [User] ([UserId])
)
GO

CREATE INDEX IDX_Author_Account ON [BlogAuthor]([UserId])
GO