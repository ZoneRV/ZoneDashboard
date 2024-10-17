CREATE PROCEDURE [dbo].[spVanId_Delete]
    @VanName varchar(7)
AS
    begin 
        DELETE
        FROM [dbo].[VanId]
        WHERE VanName = @VanName;
    end