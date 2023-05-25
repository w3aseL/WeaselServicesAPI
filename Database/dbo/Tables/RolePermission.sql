CREATE TABLE [dbo].[RolePermission]
(
	[PermissionId] INT NOT NULL,
	[RoleId] INT NOT NULL,

	CONSTRAINT [PK_RolePermission] PRIMARY KEY ([PermissionId], [RoleId]),
	CONSTRAINT [FK_RolePermission_Role] FOREIGN KEY ([RoleId]) REFERENCES [Role]([RoleId]),
	CONSTRAINT [FK_RolePermission_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [Permission]([PermissionId])
)