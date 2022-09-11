CREATE TABLE [dbo].[Employee](
    [EmployeeID] [int] IDENTITY(1,1) NOT NULL,
    [FirstName] [nvarchar](255) NOT NULL,
    [LastName] [nvarchar](255) NOT NULL,
    [Email] [nvarchar](255) NOT NULL,
    [JobTitle] [nvarchar](255) NOT NULL,
CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
        (
    [EmployeeID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
GO
