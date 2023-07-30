CREATE TABLE [dbo].[SpotifySongArtists]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[SongId] VARCHAR(255) NOT NULL,
	[ArtistId] VARCHAR(255) NOT NULL,

	CONSTRAINT FK_SongArtist_Song FOREIGN KEY (SongId) REFERENCES [SpotifySong]([Id]),
	CONSTRAINT FK_SongArtist_Artist FOREIGN KEY (ArtistId) REFERENCES [SpotifyArtist]([Id])
)
GO

CREATE INDEX [IDX_SongArtist_Song] ON [SpotifySongArtists] ([SongId])
GO

CREATE INDEX [IDX_SongArtist_Artist] ON [SpotifySongArtists] ([ArtistId])
GO