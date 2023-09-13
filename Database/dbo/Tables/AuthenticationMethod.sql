CREATE TABLE [dbo].[AuthenticationMethod]
(
	[AuthenticationMethodId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UserId] INT NOT NULL,
	[SecretKey] VARCHAR(500) NULL,
	[PhoneNumber] VARCHAR(50) NULL,
	[AuthenticationTypeId] INT NOT NULL DEFAULT (1),
	[PriorityOrder] INT NULL,
	
	CONSTRAINT FK_AuthenticationMethod_User FOREIGN KEY ([UserId]) REFERENCES [User] ([UserId])
)
GO

CREATE INDEX IDX_AuthenticationMethod_UserId ON [AuthenticationMethod] ([UserId])
GO
