CREATE TABLE [dbo].[UserAccountRequest]
(
	[UserAccountRequestId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] INT NOT NULL,
	[RequestCode] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
	[IsRegistrationRequest] BIT NOT NULL DEFAULT (0),
	[GeneratedDate] DATETIME NOT NULL DEFAULT GETDATE(),

	CONSTRAINT UserAccountRequest_FK_User FOREIGN KEY ([UserId]) REFERENCES [User]([UserId])
)
