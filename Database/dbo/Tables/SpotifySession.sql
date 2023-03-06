CREATE TABLE [dbo].[SpotifySession]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[StartTime] DATETIME NOT NULL,
	[EndTime] DATETIME NOT NULL,
	[SongCount] INT NOT NULL DEFAULT 0,
	[TimeListening] INT NOT NULL DEFAULT 0,
	[AccountId] INT NULL,

	CONSTRAINT FK_Session_SpotifyAccount FOREIGN KEY (AccountId) REFERENCES [SpotifyAccount]([SpotifyAuthId])
)
