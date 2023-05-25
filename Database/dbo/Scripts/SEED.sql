-- Roles
DBCC CHECKIDENT ([Role], RESEED, 0);
INSERT INTO [Role] (RoleName) VALUES
	('Administrator');

-- Permissions
DBCC CHECKIDENT (Permission, RESEED, 0);
INSERT INTO [Permission] (Name) VALUES
	('Do Everything');

-- Role-Permission Pairs
INSERT INTO [RolePermission] (RoleId, PermissionId) VALUES
	(1, 1);		-- Admin, Do Everything