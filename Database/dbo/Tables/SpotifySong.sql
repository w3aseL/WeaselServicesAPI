﻿CREATE TABLE [dbo].[SpotifySong]
(
	[Id] VARCHAR(255) NOT NULL PRIMARY KEY,
	[Title] VARCHAR(255) NOT NULL,
	[Duration] INT NOT NULL DEFAULT 0,
	[URL] VARCHAR(511) NULL
)
