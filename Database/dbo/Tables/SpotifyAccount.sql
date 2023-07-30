CREATE TABLE [dbo].[SpotifyAccount]
(
	[SpotifyAuthId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] INT NOT NULL,
	[AccessToken] VARCHAR(511) NOT NULL,
	[AccessGeneratedDate] DATETIME NOT NULL,
	[RefreshToken] VARCHAR(511) NOT NULL,
	[RefreshGeneratedDate] DATETIME NOT NULL,
	[ExpiresIn] INT NOT NULL,

	CONSTRAINT SpotifyAccount_FK_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId])
)
GO

CREATE INDEX [IDX_SpotifyAccount_User] ON [SpotifyAccount] ([UserId])
GO