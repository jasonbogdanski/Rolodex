SET IDENTITY_INSERT [dbo].[Employee] ON 
GO
INSERT [dbo].[Employee] ([EmployeeID], [FirstName], [LastName], [Email], [JobTitle]) VALUES (1, N'John', N'Smith', N'john.smith@gmail.com', N'Advisor')
GO
INSERT [dbo].[Employee] ([EmployeeID], [FirstName], [LastName], [Email], [JobTitle]) VALUES (2, N'Mike', N'Miller', N'mike.miller@hotmail.com', N'Marketing Manager')
GO
INSERT [dbo].[Employee] ([EmployeeID], [FirstName], [LastName], [Email], [JobTitle]) VALUES (3, N'Tony', N'Shark', N'ts@hey.com', N'CEO')
GO
SET IDENTITY_INSERT [dbo].[Employee] OFF
GO
