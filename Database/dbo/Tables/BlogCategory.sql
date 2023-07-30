CREATE TABLE [dbo].[BlogCategory]
(
	[CategoryId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[CategoryName] VARCHAR(511) NOT NULL,
	[IsHidden] BIT NOT NULL DEFAULT (0)
)
