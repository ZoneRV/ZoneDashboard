Create Table [dbo].[VanId]
(
    [Id]      INTEGER      NOT NULL PRIMARY KEY IDENTITY,
    [VanName] varchar(7) NOT NULL,
    [VanId] varchar(24) NOT NULL DEFAULT '', 
    [Blocked] BIT NOT NULL DEFAULT 0
)