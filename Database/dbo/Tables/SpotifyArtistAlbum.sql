CREATE TABLE [dbo].[SpotifyArtistAlbum]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[ArtistId] VARCHAR(255) NOT NULL,
	[AlbumId] VARCHAR(255) NOT NULL,

	CONSTRAINT FK_ArtistAlbum_Artist FOREIGN KEY (ArtistId) REFERENCES [SpotifyArtist]([Id]),
	CONSTRAINT FK_ArtistAlbum_Album FOREIGN KEY (AlbumId) REFERENCES [SpotifyAlbum]([Id])
)
