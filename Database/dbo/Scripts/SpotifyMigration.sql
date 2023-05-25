BEGIN
	-- Clear Associations
	DELETE FROM SpotifySongArtists;
	DELETE FROM SpotifyArtistAlbum;
	DELETE FROM SpotifySongAlbum;
	DELETE FROM SpotifyTrackPlay;

	-- Songs
	DELETE FROM SpotifySong;
	INSERT INTO SpotifySong
		SELECT
			s.id AS Id,
			s.title AS Title,
			s.duration AS Duration,
			s.url AS URL
		FROM OPENQUERY(SQLITE, 'SELECT * FROM Songs') s;

	-- Artists
	DELETE FROM SpotifyArtist;
	INSERT INTO SpotifyArtist
		SELECT
			a.id AS Id,
			a.name AS Name,
			a.url AS URL
		FROM OPENQUERY(SQLITE, 'SELECT * FROM Artists') a;

	-- Albums
	DELETE FROM SpotifyAlbum;
	INSERT INTO SpotifyAlbum
		SELECT
			a.id AS Id,
			a.title AS Title,
			a.url AS URL,
			a.artwork_url AS ArtworkURL
		FROM OPENQUERY(SQLITE, 'SELECT * FROM Albums') a;

	-- Add associations
	DBCC CHECKIDENT (SpotifySongArtists, RESEED, 0);
	INSERT INTO SpotifySongArtists
		SELECT
			sa.SongId,
			sa.ArtistId
			FROM OPENQUERY(SQLITE, 'SELECT * FROM SongArtists') sa;
	DBCC CHECKIDENT (SpotifySongAlbum, RESEED, 0);
	INSERT INTO SpotifySongAlbum
		SELECT * FROM OPENQUERY(SQLITE, 'SELECT * FROM SongAlbums');
	DBCC CHECKIDENT (SpotifyArtistAlbum, RESEED, 0);
	INSERT INTO SpotifyArtistAlbum
		SELECT * FROM OPENQUERY(SQLITE, 'SELECT * FROM ArtistAlbums');

	-- Sessions
	DELETE FROM SpotifySession;
	DBCC CHECKIDENT (SpotifySession, RESEED, 0);
	INSERT INTO SpotifySession
		SELECT
			s.start_time AS StartTime,
			s.end_time AS EndTime,
			s.song_count AS SongCount,
			s.time_listening AS TimeListening,
			(SELECT TOP 1 SpotifyAuthId FROM SpotifyAccount) AS AccountId
			FROM OPENQUERY(SQLITE, 'SELECT * FROM Sessions') s;

	-- Track Plays
	DBCC CHECKIDENT (SpotifyTrackPlay, RESEED, 0);
	INSERT INTO SpotifyTrackPlay
		SELECT
			tp.SongId,
			tp.SessionId,
			tp.time_played AS TimePlayed
			FROM OPENQUERY(SQLITE, 'SELECT * FROM TrackPlays') tp
		WHERE tp.SessionId IS NOT NULL;
END