CREATE TABLE [dbo].[SpotifySongAlbum]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[SongId] VARCHAR(255) NOT NULL,
	[AlbumId] VARCHAR(255) NOT NULL,

	CONSTRAINT FK_SongAlbum_Song FOREIGN KEY (SongId) REFERENCES [SpotifySong]([Id]),
	CONSTRAINT FK_SongAlbum_Album FOREIGN KEY (AlbumId) REFERENCES [SpotifyAlbum]([Id])
)
GO

CREATE INDEX [IDX_SongAlbum_Song] ON [SpotifySongAlbum] ([SongId])
GO

CREATE INDEX [IDX_SongAlbum_Album] ON [SpotifySongAlbum] ([AlbumId])
GO