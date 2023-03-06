CREATE TABLE [dbo].[SpotifyAccountRequest]
(
	[SpotifyAccountRequestId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] INT NOT NULL,
	[AuthorizationCode] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[ExpirationDate] DATETIME NOT NULL,
	[OriginalUrl] VARCHAR(255) NULL,

	CONSTRAINT SpotifyAccountRequest_FK_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId])
)
