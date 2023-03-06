CREATE TABLE [dbo].[SpotifyTrackPlay]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[SongId] VARCHAR(255) NOT NULL,
	[SessionId] INT NOT NULL,
	[TimePlayed] INT NOT NULL DEFAULT 0,

	CONSTRAINT FK_TrackPlay_Song FOREIGN KEY (SongId) REFERENCES [SpotifySong]([Id]),
	CONSTRAINT FK_TrackPlay_Session FOREIGN KEY (SessionId) REFERENCES [SpotifySession]([Id])
) 
