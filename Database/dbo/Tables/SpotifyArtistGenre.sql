CREATE TABLE [dbo].[SpotifyArtistGenre]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ArtistId] VARCHAR(255) NOT NULL,
	[GenreId] INT NOT NULL,

	CONSTRAINT [FK_SpotifyArtistGenre_SpotifySong] FOREIGN KEY ([ArtistId]) REFERENCES [dbo].[SpotifyArtist] ([Id]),
	CONSTRAINT [FK_SpotifyArtistGenre_SpotifyGenre] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[SpotifyGenre] ([Id])
)
GO

CREATE INDEX [IDX_SpotifyArtistGenre_SpotifySong] ON [SpotifyArtistGenre] ([GenreId])
GO