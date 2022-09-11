SET IDENTITY_INSERT [dbo].[CompanyBranch] ON 
GO
INSERT [dbo].[CompanyBranch] ([CompanyBranchID], [Name], [City], [State]) VALUES (1, N'Buckeye', N'Columbus', N'Ohio')
GO
INSERT [dbo].[CompanyBranch] ([CompanyBranchID], [Name], [City], [State]) VALUES (2, N'Big Apple', N'New York', N'New York')
GO
INSERT [dbo].[CompanyBranch] ([CompanyBranchID], [Name], [City], [State]) VALUES (3, N'Windy City', N'Chicago', N'Illinois')
GO
SET IDENTITY_INSERT [dbo].[CompanyBranch] OFF
GO

UPDATE e
SET e.CompanyBranchId = 1
FROM dbo.Employee e
GO

ALTER TABLE dbo.Employee ALTER COLUMN CompanyBranchId INTEGER NOT NULL
GO