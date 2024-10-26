CREATE PROCEDURE [dbo].[spVanId_Get]
    @VanName varchar(7)
AS
    begin 
        SELECT VanId, VanName, Url, Blocked
        FROM [dbo].[VanId]
        WHERE VanName = @VanName;
    end