﻿CREATE TABLE [dbo].[Device]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UUID] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[DeviceId] VARCHAR(511) NULL,
	[DeviceName] VARCHAR(511) NULL,
	[Manufacturer] VARCHAR(511) NULL,
	[DeviceIPAddress] VARCHAR(255) NULL
)