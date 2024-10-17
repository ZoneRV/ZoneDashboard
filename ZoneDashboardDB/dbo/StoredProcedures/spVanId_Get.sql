CREATE PROCEDURE [dbo].[spVanId_Get]
    @VanName varchar(7)
AS
    begin 
        SELECT VanId, VanName, Blocked
        FROM [dbo].[VanId]
        WHERE VanName = @VanName;
    end